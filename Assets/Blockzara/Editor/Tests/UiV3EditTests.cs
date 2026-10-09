using Blockzara.Runtime;
using NUnit.Framework;

public sealed class UiV3EditTests
{
    [Test]
    public void Pause_BackFromSettings_ReturnsToPause()
    {
        var flow = new PauseFlow();
        flow.Open();
        flow.OpenSettings();
        Assert.That(flow.Back(), Is.EqualTo(PauseBackResult.Stay));
        Assert.That(flow.Screen, Is.EqualTo(PauseScreen.Pause));
    }

    [Test]
    public void Pause_RestartConfirm_DoesNotRestartUntilConfirmed()
    {
        var flow = new PauseFlow();
        flow.Open();
        flow.AskRestart();
        Assert.That(flow.Screen, Is.EqualTo(PauseScreen.ConfirmRestart));
        Assert.That(flow.Back(), Is.EqualTo(PauseBackResult.Stay));
        Assert.That(flow.Screen, Is.EqualTo(PauseScreen.Pause));
        flow.AskRestart();
        flow.ConfirmRestart();
        Assert.That(flow.Screen, Is.EqualTo(PauseScreen.Hidden));
    }

    [Test]
    public void Pause_LeaveConfirm_NoReturnsToPause_YesCloses()
    {
        var flow = new PauseFlow();
        flow.Open();
        flow.AskLeave();
        flow.CancelDialog();
        Assert.That(flow.Screen, Is.EqualTo(PauseScreen.Pause));
        flow.AskLeave();
        flow.ConfirmLeave();
        Assert.That(flow.Screen, Is.EqualTo(PauseScreen.Hidden));
    }

    [Test]
    public void Pause_BackOnThePanel_Resumes()
    {
        var flow = new PauseFlow();
        flow.Open();
        Assert.That(flow.Back(), Is.EqualTo(PauseBackResult.Resume));
        Assert.That(flow.Screen, Is.EqualTo(PauseScreen.Hidden));
    }

    [Test]
    public void Banners_SwitchSurfaces_WithoutDuplicates()
    {
        var placement = new BannerPlacement();
        placement.Show(BannerSurface.Home);
        placement.Show(BannerSurface.Home);
        Assert.That(placement.Loads, Is.EqualTo(1));
        placement.Show(BannerSurface.Gameplay);
        Assert.That(placement.Loads, Is.EqualTo(2));
        placement.Show(BannerSurface.Gameplay);
        Assert.That(placement.Surface, Is.EqualTo(BannerSurface.Gameplay));
        Assert.That(placement.Loads, Is.EqualTo(2));
    }

    [Test]
    public void Banners_LoadFailure_DoesNotChangeTheRequestedSurface()
    {
        var placement = new BannerPlacement();
        placement.Show(BannerSurface.Gameplay);
        placement.Fail("offline");
        Assert.That(placement.Failed, Is.True);
        Assert.That(placement.Surface, Is.EqualTo(BannerSurface.Gameplay));
        Assert.That(placement.LastError, Is.EqualTo("offline"));
        placement.Show(BannerSurface.Home);
        Assert.That(placement.Failed, Is.False);
        Assert.That(placement.Surface, Is.EqualTo(BannerSurface.Home));
    }

    [Test]
    public void RoundAds_ShowOncePerRound_AndContinueOnce()
    {
        var round = new RoundAdPolicy();
        round.BeginRound();
        Assert.That(round.ContinueAvailable, Is.True);
        Assert.That(round.ClaimContinue(), Is.True);
        Assert.That(round.ClaimContinue(), Is.False);
        round.RefundContinue();
        Assert.That(round.ClaimContinue(), Is.True);
        Assert.That(round.ClaimEndOfRound(), Is.True);
        Assert.That(round.ClaimEndOfRound(), Is.False);
        Assert.That(round.ContinueAvailable, Is.False);
        round.BeginRound();
        Assert.That(round.ClaimEndOfRound(), Is.True);
        Assert.That(round.ClaimContinue(), Is.False);
    }

    [Test]
    public void AdBill_ScalesOneUnitPerThousandAds()
    {
        Assert.That(LocalProgressService.FormatAdBill(0), Is.EqualTo("0.00"));
        Assert.That(LocalProgressService.FormatAdBill(10), Is.EqualTo("0.01"));
        Assert.That(LocalProgressService.FormatAdBill(500), Is.EqualTo("0.50"));
        Assert.That(LocalProgressService.FormatAdBill(1000), Is.EqualTo("1.00"));
        Assert.That(LocalProgressService.FormatAdBill(2500), Is.EqualTo("2.50"));
    }

    [Test]
    public void RoundAds_UseGoogleTestUnits()
    {
        Assert.That(BannerAdManager.TestInterstitialId, Is.EqualTo("ca-app-pub-3940256099942544/1033173712"));
        Assert.That(BannerAdManager.TestRewardedId, Is.EqualTo("ca-app-pub-3940256099942544/5224354917"));
    }
}
