using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Blockzara.Runtime
{
    public static class BannerAdManager
    {
        public const string TestAppId = "ca-app-pub-3940256099942544~3347511713";
        public const string TestBannerId = "ca-app-pub-3940256099942544/9214589741";
        public const string TestInterstitialId = "ca-app-pub-3940256099942544/1033173712";
        public const string TestRewardedId = "ca-app-pub-3940256099942544/5224354917";
        public const float ReservedHeight = 110f;

        static readonly BannerPlacement placement = new BannerPlacement();
        static BannerView banner;
        static InterstitialAd interstitial;
        static RewardedAd rewarded;
        static bool initStarted;
        static bool ready;
        static bool interstitialFinished;
        static bool rewardEarned;
        static bool rewardFinished;
        static Action interstitialNext;
        static Action rewardEarnedAction;
        static Action rewardSkippedAction;
        static bool waitingInterstitial;
        static bool waitingRewarded;
        static bool fullscreenOpened;
        static bool rewardClosed;
        static float rewardClosedAt = -1f;
        static readonly List<Action> pending = new List<Action>();
        static readonly object pendingGate = new object();

        public static bool FullscreenOpened => fullscreenOpened;

        public static void Pump()
        {
            Action[] batch = null;
            lock (pendingGate)
            {
                if (pending.Count > 0)
                {
                    batch = pending.ToArray();
                    pending.Clear();
                }
            }
            if (batch != null)
            {
                foreach (var action in batch)
                {
                    try { action(); }
                    catch (Exception error) { Debug.Log("BZ ad pump " + error.Message); }
                }
            }
            if (!waitingRewarded || rewardFinished || !rewardClosed) return;
            if (rewardEarned || Time.unscaledTime - rewardClosedAt >= 0.75f) FinishRewarded();
        }

        static void Post(Action action)
        {
            lock (pendingGate) pending.Add(action);
        }
        static BannerSurface queued = BannerSurface.None;

        public static BannerSurface Surface => placement.Surface;
        public static bool Failed => placement.Failed;
        public static string LastError => placement.LastError;
        public static int Loads => placement.Loads;

        public static void Ensure()
        {
            if (initStarted) return;
            initStarted = true;
            if (!UsesSdk()) return;
            MobileAds.DisableSDKCrashReporting();
            MobileAds.Initialize(_ =>
            {
                ready = true;
                if (queued != BannerSurface.None) Load(queued);
                LoadInterstitial();
                LoadRewarded();
            });
        }

        public static void ShowInterstitialThen(Action next)
        {
            if (!UsesSdk() || interstitial == null || !interstitial.CanShowAd())
            {
                if (UsesSdk()) LoadInterstitial();
                next?.Invoke();
                return;
            }

            interstitialFinished = false;
            interstitialNext = next;
            waitingInterstitial = true;
            fullscreenOpened = false;
            HideBanner();
            try
            {
                interstitial.Show();
            }
            catch (Exception error)
            {
                Debug.Log("BZ interstitial show failed " + error.Message);
                FinishInterstitial();
            }
        }

        public static bool AbandonFullscreen()
        {
            var interstitialPending = waitingInterstitial && interstitialNext != null;
            if (waitingRewarded && !rewardFinished)
            {
                var closedLongEnough = rewardClosed && Time.unscaledTime - rewardClosedAt >= 0.75f;
                var neverOpened = !fullscreenOpened && !rewardClosed;
                if (rewardEarned || closedLongEnough || neverOpened) FinishRewarded();
            }
            if (waitingInterstitial && !interstitialFinished) FinishInterstitial();
            return interstitialPending;
        }

        public static void ShowRewarded(Action onEarned, Action onSkipped)
        {
            if (!UsesSdk() || rewarded == null || !rewarded.CanShowAd())
            {
                if (UsesSdk()) LoadRewarded();
                onSkipped?.Invoke();
                return;
            }

            rewardEarned = false;
            rewardFinished = false;
            rewardClosed = false;
            rewardClosedAt = -1f;
            rewardEarnedAction = onEarned;
            rewardSkippedAction = onSkipped;
            waitingRewarded = true;
            fullscreenOpened = false;
            HideBanner();
            try
            {
                rewarded.Show(_ => Post(() => rewardEarned = true));
            }
            catch (Exception error)
            {
                Debug.Log("BZ rewarded show failed " + error.Message);
                FinishRewarded();
            }
        }

        public static void Show(BannerSurface surface)
        {
            if (surface == placement.Surface && banner != null && !placement.Failed) return;
            placement.Show(surface);
            queued = surface;
            if (surface == BannerSurface.None)
            {
                DestroyBanner();
                return;
            }
            if (!UsesSdk() || !ready) return;
            Load(surface);
        }

        public static void OnAppPause(bool paused)
        {
            if (!UsesSdk() || banner == null) return;
            if (paused) banner.Hide();
            else if (placement.Surface != BannerSurface.None && !placement.Failed) banner.Show();
        }

        static void Load(BannerSurface surface)
        {
            DestroyBanner();
            if (surface == BannerSurface.None) return;
            var size = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            banner = new BannerView(TestBannerId, size, AdPosition.Bottom);
            banner.OnBannerAdLoaded += () => Post(() =>
            {
                placement.Loaded();
                LocalProgressService.RecordAdWatch();
            });
            banner.OnBannerAdLoadFailed += error =>
            {
                var message = error == null ? "Banner load failed." : error.GetMessage();
                Post(() =>
                {
                    placement.Fail(message);
                    DestroyBanner();
                });
            };
            banner.LoadAd(new AdRequest());
        }

        static void DestroyBanner()
        {
            if (banner == null) return;
            banner.Destroy();
            banner = null;
        }

        static void HideBanner()
        {
            if (banner != null) banner.Hide();
        }

        static void RestoreBanner()
        {
            if (banner != null && placement.Surface != BannerSurface.None && !placement.Failed) banner.Show();
        }

        static void LoadInterstitial()
        {
            if (!UsesSdk()) return;
            InterstitialAd.Load(TestInterstitialId, new AdRequest(), (ad, error) =>
            {
                if (error != null || ad == null) return;
                interstitial?.Destroy();
                interstitial = ad;
                var counted = false;
                ad.OnAdImpressionRecorded += () => Post(() =>
                {
                    if (counted) return;
                    counted = true;
                    LocalProgressService.RecordAdWatch();
                });
                ad.OnAdFullScreenContentOpened += () => Post(() => fullscreenOpened = true);
                ad.OnAdFullScreenContentClosed += () => Post(FinishInterstitial);
                ad.OnAdFullScreenContentFailed += _ => Post(FinishInterstitial);
            });
        }

        static void FinishInterstitial()
        {
            if (interstitialFinished) return;
            interstitialFinished = true;
            waitingInterstitial = false;
            interstitial?.Destroy();
            interstitial = null;
            RestoreBanner();
            var next = interstitialNext;
            interstitialNext = null;
            next?.Invoke();
            LoadInterstitial();
        }

        static void LoadRewarded()
        {
            if (!UsesSdk()) return;
            RewardedAd.Load(TestRewardedId, new AdRequest(), (ad, error) =>
            {
                if (error != null || ad == null) return;
                rewarded?.Destroy();
                rewarded = ad;
                ad.OnAdFullScreenContentOpened += () => Post(() => fullscreenOpened = true);
                ad.OnAdFullScreenContentClosed += () => Post(() =>
                {
                    rewardClosed = true;
                    rewardClosedAt = Time.unscaledTime;
                });
                ad.OnAdFullScreenContentFailed += _ => Post(() =>
                {
                    rewardEarned = false;
                    rewardClosed = true;
                    rewardClosedAt = Time.unscaledTime - 1f;
                });
            });
        }

        static void FinishRewarded()
        {
            if (rewardFinished) return;
            rewardFinished = true;
            waitingRewarded = false;
            rewarded?.Destroy();
            rewarded = null;
            RestoreBanner();
            var earned = rewardEarnedAction;
            var skipped = rewardSkippedAction;
            rewardEarnedAction = null;
            rewardSkippedAction = null;
            if (rewardEarned)
            {
                LocalProgressService.RecordAdWatch();
                Debug.Log("BZ continue earned");
                earned?.Invoke();
            }
            else
            {
                Debug.Log("BZ continue skipped");
                skipped?.Invoke();
            }
            LoadRewarded();
        }

        static bool UsesSdk()
        {
            return Application.platform == RuntimePlatform.Android && !Application.isEditor;
        }
    }
}
