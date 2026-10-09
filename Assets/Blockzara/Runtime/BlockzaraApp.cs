using System;
using System.Collections.Generic;
using Blockzara.Core;
using UnityEngine;

namespace Blockzara.Runtime
{
    public sealed class BlockzaraApp : MonoBehaviour
    {
        public const float GhostAlpha = 0.35f;

        static readonly Color Field = new Color(0.02f, 0.08f, 0.20f);
        static readonly Color Empty = new Color(0.05f, 0.16f, 0.34f);
        static readonly Color[] Colors =
        {
            new Color(0.96f, 0.78f, 0.18f),
            Color.yellow,
            new Color(0.62f, 0.28f, 0.86f),
            Color.cyan,
            Color.green,
            new Color(0.90f, 0.18f, 0.22f),
            new Color(0.35f, 0.55f, 1f)
        };

        readonly HoldScheduler leftRepeat = new HoldScheduler();
        readonly HoldScheduler rightRepeat = new HoldScheduler();
        readonly HoldScheduler softRepeat = new HoldScheduler();

        FallingGame game;
        bool menu = true;
        int savedBest;
        int runSeed;
        bool dailyRun;
        bool dailyCelebrated;
        float dailyMessageUntil;
        int stage = 1;
        int stagePlaced;
        int stageCleared;
        float stageClearUntil;
        string blockSkin = "solid";
        float fallTimer;
        float calloutUntil;
        int seenCallout;
        bool rotateWas;
        bool hardWas;
        bool pauseWas;
        bool restartWas;
        bool continueWas;
        bool adBusy;
        float adBusySince;
        float adReturnedAt = -1f;
        readonly RoundAdPolicy roundAds = new RoundAdPolicy();
        Rect pauseRect;
        Rect restartRect;
        Rect overlayRestart;
        Rect overlayContinue;
        Rect leftRect;
        Rect rightRect;
        Rect rotateRect;
        Rect softRect;
        Rect hardRect;
        float boardLeft;
        float boardTop;
        float cell;
        float insetLeft;
        float insetRight;
        float insetTop;
        float insetBottom;
        GUIStyle scoreStyle;
        GUIStyle buttonStyle;
        GUIStyle calloutStyle;
        GUIStyle controlLabelStyle;
        GUIStyle hudCaptionStyle;
        GUIStyle hudValueStyle;
        Texture2D scenicBackground;
        BlockzaraMenuV5 menuView;
        BlockzaraPauseOverlay pauseOverlay;
        readonly PauseFlow pauseFlow = new PauseFlow();
        readonly GameplayFx fx = new GameplayFx();
        float playtimePending;

        public FallingGame Game => game;
        public bool InMenu => menu;

        void Awake()
        {
            LocalProgressService.Migrate();
            savedBest = LocalProgressService.LoadBestScore();
            scenicBackground = Resources.Load<Texture2D>("scenic-background-v1");
            GameAudio.Ensure(gameObject);
            GameAudio.PlayBed(true);
            menuView = gameObject.AddComponent<BlockzaraMenuV5>();
            menuView.Build(this);
            pauseOverlay = gameObject.AddComponent<BlockzaraPauseOverlay>();
            pauseOverlay.Build(this);
            BannerAdManager.Ensure();
            BannerAdManager.Show(BannerSurface.Home);
        }

        public void StartFromMenu()
        {
            dailyRun = false;
            StartRun(Environment.TickCount, LocalProgressService.LoadBestScore());
        }

        public void StartDaily()
        {
            dailyRun = true;
            StartRun(LocalProgressService.DailySeed(), LocalProgressService.LoadBestScore());
        }

        public void StartRun(int seed, int best = 0)
        {
            runSeed = seed;
            dailyCelebrated = false;
            dailyMessageUntil = 0f;
            stage = 1;
            stagePlaced = 0;
            stageCleared = 0;
            stageClearUntil = 0f;
            savedBest = best;
            roundAds.BeginRound();
            game = new FallingGame(new SeededRandom(seed), best);
            menu = false;
            fallTimer = 0f;
            seenCallout = game.CalloutId;
            calloutUntil = 0f;
            playtimePending = 0f;
            fx.Reset(game);
            pauseFlow.Reset();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
            ReleaseHolds();
            LocalProgressService.RecordGameStart();
            GameAudio.PlayBed(false);
            BannerAdManager.Show(BannerSurface.Gameplay);
            if (menuView != null) menuView.Hide();
            Debug.Log("BZ falling start best=" + game.Best);
        }

        public int SimulateHoldRight(float seconds) => SimulateHold(seconds, true);
        public int SimulateHoldSoft(float seconds) => SimulateHold(seconds, false);

        public int PressRotate() => game == null ? 0 : game.RotateClockwise();

        public int PressHardDrop()
        {
            if (game == null) return 0;
            var bestBefore = game.Best;
            var levelBefore = DisplayedLevel;
            var fromY = game.Y;
            var ghost = game.GhostY;
            var droppedType = game.Active;
            var droppedRotation = game.ActiveRotation;
            var droppedX = game.X;
            var lockedColor = Colors[(int)droppedType];
            IReadOnlyList<BoardCell> cells = game.HasPiece ? game.AbsoluteCells(game.X, ghost) : null;
            var rows = game.HardDrop();
            if (rows > 0) fx.OnHard(droppedType, droppedRotation, droppedX, fromY, ghost);
            PresentLock(cells, bestBefore, levelBefore, lockedColor);
            fallTimer = 0f;
            return rows;
        }

        int DisplayedLevel => game == null ? 1 : game.LinesCleared / 10 + 1;

        void Update()
        {
            if (Debug.developerConsoleVisible) Debug.developerConsoleVisible = false;
            if (Input.GetKeyDown(KeyCode.Escape)) HandleBack();
            BannerAdManager.Pump();
            RecoverStuckAd();
            if (menu || game == null) return;
            if (game.Best != savedBest)
            {
                savedBest = game.Best;
                LocalProgressService.SaveBestScoreIfHigher(savedBest);
            }
            Layout();
            ReadPointer(out var down, out var gui);
            var pause = down && pauseRect.Contains(gui);
            var continueTap = down && game.GameOver && roundAds.ContinueAvailable && overlayContinue.Contains(gui);
            var restart = down && (restartRect.Contains(gui) || (game.GameOver && overlayRestart.Contains(gui)));
            if (!adBusy && continueTap && !continueWas)
            {
                GameAudio.Play("click");
                OfferContinue();
            }
            if (!adBusy && !continueTap && pause && !pauseWas && !game.GameOver && pauseFlow.Screen == PauseScreen.Hidden)
            {
                GameAudio.Play("click");
                OpenPause();
            }
            if (restart && !restartWas && pauseFlow.Screen == PauseScreen.Hidden && !continueTap)
            {
                if (game.GameOver)
                {
                    GameAudio.Play("click");
                    var restartedByAd = adBusy && BannerAdManager.AbandonFullscreen();
                    adBusy = false;
                    if (!restartedByAd) EndRoundThen(() => StartRun(dailyRun ? runSeed : Environment.TickCount, game.Best));
                    Debug.Log("BZ gameover restart busyWas=" + restartedByAd);
                }
                else if (!adBusy)
                {
                    GameAudio.Play("click");
                    OpenPause();
                    AskRestart();
                }
            }
            pauseWas = pause;
            restartWas = restart;
            continueWas = continueTap;
            fx.Tick(game, Time.unscaledDeltaTime, LocalProgressService.ReducedEffects);
            if (!game.Paused && !game.GameOver) playtimePending += Time.deltaTime;
            if (playtimePending >= 15f) FlushPlaytime();
            if (game.Paused || game.GameOver)
            {
                ReleaseHolds();
                return;
            }

            var left = down && leftRect.Contains(gui);
            var right = down && rightRect.Contains(gui);
            var soft = down && softRect.Contains(gui);
            var rotate = down && rotateRect.Contains(gui);
            var hard = down && hardRect.Contains(gui);
            Repeat(leftRepeat, left && !right, () => PlayMove(game.MoveLeft()));
            Repeat(rightRepeat, right && !left, () => PlayMove(game.MoveRight()));
            if (rotate && !rotateWas && game.RotateClockwise() > 0)
            {
                fx.OnRotate();
                GameAudio.Play("rotate");
            }
            rotateWas = rotate;
            if (hard && !hardWas)
            {
                var bestBefore = game.Best;
                var levelBefore = DisplayedLevel;
                var fromY = game.Y;
                var ghost = game.GhostY;
                var droppedType = game.Active;
                var droppedRotation = game.ActiveRotation;
                var droppedX = game.X;
                var lockedColor = Colors[(int)droppedType];
                var cells = game.HasPiece ? game.AbsoluteCells(game.X, ghost) : null;
                var rows = game.HardDrop();
                if (rows > 0)
                {
                    fx.OnHard(droppedType, droppedRotation, droppedX, fromY, ghost);
                    GameAudio.Play("hard");
                }
                PresentLock(cells, bestBefore, levelBefore, lockedColor);
                fallTimer = 0f;
            }
            hardWas = hard;

            if (soft)
            {
                var steps = softRepeat.Tick(true, Time.deltaTime);
                for (var i = 0; i < steps; i++)
                {
                    var bestBefore = game.Best;
                    var levelBefore = DisplayedLevel;
                    var lockedColor = Colors[(int)game.Active];
                    var cells = game.HasPiece ? game.AbsoluteCells(game.X, game.Y) : null;
                    var moved = game.SoftDrop();
                    if (moved)
                    {
                        fx.OnSoft();
                        GameAudio.Play("soft");
                    }
                    if (!game.LastLock) continue;
                    PresentLock(cells, bestBefore, levelBefore, lockedColor);
                    break;
                }
                fallTimer = 0f;
                return;
            }

            softRepeat.Tick(false, 0f);
            fallTimer += Time.deltaTime;
            var interval = (float)game.FallInterval;
            while (fallTimer >= interval)
            {
                fallTimer -= interval;
                var serial = game.PieceSerial;
                var bestBefore = game.Best;
                var levelBefore = DisplayedLevel;
                var lockedColor = Colors[(int)game.Active];
                var cells = game.HasPiece ? game.AbsoluteCells(game.X, game.Y) : null;
                game.StepGravity();
                PresentLock(cells, bestBefore, levelBefore, lockedColor);
                if (game.PieceSerial != serial || game.GameOver)
                {
                    fallTimer = 0f;
                    break;
                }
            }
        }

        public void OpenPause()
        {
            if (game == null || game.GameOver || menu) return;
            game.SetPaused(true);
            GameAudio.Play("pause");
            GameAudio.Duck(true);
            FlushPlaytime();
            pauseFlow.Open();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
        }

        public void ResumeFromPause()
        {
            if (game != null && !game.GameOver) game.SetPaused(false);
            GameAudio.Play("resume");
            GameAudio.Duck(false);
            pauseFlow.Resume();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
        }

        public void AskRestart()
        {
            if (pauseFlow.Screen == PauseScreen.Hidden) OpenPause();
            pauseFlow.AskRestart();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
        }

        public void AskLeave()
        {
            pauseFlow.AskLeave();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
        }

        public void CancelPauseDialog()
        {
            pauseFlow.CancelDialog();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
        }

        public void ConfirmRestart()
        {
            var best = game == null ? savedBest : game.Best;
            var seed = dailyRun ? runSeed : Environment.TickCount;
            pauseFlow.ConfirmRestart();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
            EndRoundThen(() => StartRun(seed, best));
        }

        public void ConfirmLeave()
        {
            pauseFlow.ConfirmLeave();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
            EndRoundThen(ShowMenu);
        }

        void ShowMenu()
        {
            menu = true;
            dailyRun = false;
            game = null;
            GameAudio.Duck(false);
            GameAudio.PlayBed(true);
            if (menuView != null) menuView.Show();
            BannerAdManager.Show(BannerSurface.Home);
        }

        void BeginAdWait()
        {
            adBusy = true;
            adBusySince = Time.unscaledTime;
            adReturnedAt = -1f;
        }

        void RecoverStuckAd()
        {
            if (!adBusy) return;
            var now = Time.unscaledTime;
            var opened = BannerAdManager.FullscreenOpened;
            var abandoned = (!opened && now - adBusySince > 2.5f)
                || (opened && adReturnedAt > 0f && now - adReturnedAt > 1.2f);
            if (!abandoned) return;
            Debug.Log("BZ ad wait abandoned");
            BannerAdManager.AbandonFullscreen();
            adBusy = false;
        }

        void EndRoundThen(Action next)
        {
            if (adBusy) return;
            if (game == null || !roundAds.ClaimEndOfRound())
            {
                next();
                return;
            }

            BeginAdWait();
            BannerAdManager.ShowInterstitialThen(() =>
            {
                adBusy = false;
                next();
            });
        }

        void OfferContinue()
        {
            if (adBusy || game == null || !roundAds.ContinueAvailable) return;
            BeginAdWait();
            BannerAdManager.ShowRewarded(
                () =>
                {
                    adBusy = false;
                    var continued = game != null && roundAds.ClaimContinue() && game.ContinueOnce();
                    if (continued) fallTimer = 0f;
                    Debug.Log("BZ continue applied " + continued);
                },
                () =>
                {
                    adBusy = false;
                    roundAds.RefundContinue();
                });
        }

        public void OpenPauseSettings()
        {
            pauseFlow.OpenSettings();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
        }

        public void ClosePauseSettings()
        {
            pauseFlow.CloseSettings();
            if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
        }

        void HandleBack()
        {
            if (menu)
            {
                if (menuView != null) menuView.TryClosePanel();
                return;
            }
            if (game == null || game.GameOver) return;
            if (pauseFlow.Screen == PauseScreen.Hidden)
            {
                OpenPause();
                return;
            }
            if (pauseFlow.Back() == PauseBackResult.Resume) ResumeFromPause();
            else if (pauseOverlay != null) pauseOverlay.Apply(pauseFlow);
        }

        void OnApplicationPause(bool paused)
        {
            if (paused) FlushPlaytime();
            GameAudio.OnAppPause(paused);
            BannerAdManager.OnAppPause(paused);
            if (!adBusy) return;
            if (!paused) adReturnedAt = Time.unscaledTime;
        }

        void OnApplicationFocus(bool focus)
        {
            if (!focus) FlushPlaytime();
            GameAudio.OnAppPause(!focus);
            if (!adBusy) return;
            if (focus) adReturnedAt = Time.unscaledTime;
        }

        void OnDestroy() => FlushPlaytime();

        void OnGUI()
        {
            if (Screen.width < 20 || Screen.height < 20) return;
            EnsureStyles();
            blockSkin = LocalProgressService.Skin;
            if (menu) return;
            if (pauseFlow.Screen != PauseScreen.Hidden) return;

            DrawBackdrop();
            GUI.color = Color.white;
            Layout();
            DrawHud();
            DrawWell();
            DrawNext();
            DrawButtons();
            if (Time.unscaledTime < stageClearUntil)
                DrawStageClear();
            else if (fx.Banner.Length > 0 && Time.unscaledTime < fx.BannerUntil)
                DrawBanner(fx.Banner, fx.BannerColor);
            else if (game.Callout.Length > 0 && Time.unscaledTime < calloutUntil)
                DrawBanner(game.Callout, Color.white);
            else if (dailyRun && Time.unscaledTime < dailyMessageUntil)
                DrawBanner(UiLanguage.Line("dailyDone"), new Color(0.7f, 1f, 0.75f));
            if (game.GameOver) fx.Overlay = 1f;
            if (game.GameOver) DrawOverlay(UiLanguage.Line("over"), true, fx.Overlay);
        }

        void DrawHud()
        {
            DrawButton(pauseRect, UiLanguage.Line(game.Paused ? "resume" : "pause"), new Color(0.13f, 0.22f, 0.42f));
            DrawButton(restartRect, UiLanguage.Line("restart"), new Color(0.13f, 0.22f, 0.42f));
            var gap = 6f;
            var width = (Screen.width - insetLeft - insetRight - 16f - gap * 3f) / 4f;
            var y = pauseRect.yMax + 8f;
            DrawStat(new Rect(insetLeft + 8f, y, width, 58f), UiLanguage.Line("score"), game.Score.ToString());
            DrawStat(new Rect(insetLeft + 8f + (width + gap), y, width, 58f), UiLanguage.Line("best"), game.Best.ToString());
            DrawStat(new Rect(insetLeft + 8f + (width + gap) * 2f, y, width, 58f), UiLanguage.Line("lines"), game.LinesCleared.ToString());
            DrawStat(new Rect(insetLeft + 8f + (width + gap) * 3f, y, width, 58f), UiLanguage.Line("level"), DisplayedLevel.ToString());
        }

        void DrawStat(Rect rect, string caption, string value)
        {
            GUI.color = new Color(0.03f, 0.08f, 0.18f, 0.92f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = new Color(0.82f, 0.68f, 0.32f, 0.9f);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 2f), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 0.9f, 0.62f);
            GUI.Label(new Rect(rect.x + 4f, rect.y + 2f, rect.width - 8f, 18f), caption, hudCaptionStyle);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 4f, rect.y + 20f, rect.width - 8f, 34f), value, hudValueStyle);
        }

        void DrawWell()
        {
            var outer = new Rect(boardLeft - 8f, boardTop - 8f, cell * 10f + 16f, cell * 20f + 16f);
            var moodBand = BoardMood.Band(DisplayedLevel);
            GUI.color = BoardMood.Frame(LocalProgressService.Theme, moodBand);
            GUI.DrawTexture(outer, Texture2D.whiteTexture);
            GUI.color = BoardMood.Well(LocalProgressService.Theme, moodBand);
            GUI.DrawTexture(new Rect(outer.x + 3f, outer.y + 3f, outer.width - 6f, outer.height - 6f), Texture2D.whiteTexture);
            var holding = fx.ClearHold > 0.02f && game.ClearHoldReady;
            for (var y = 0; y < BoardState.VisibleRows; y++)
            for (var x = 0; x < BoardState.Width; x++)
            {
                var kind = holding ? game.ClearHoldCell(x, y) : game.Board.Get(x, y);
                var rect = CellRect(x, y);
                if (kind == 0)
                {
                    GUI.color = Empty;
                    GUI.DrawTexture(rect, Texture2D.whiteTexture);
                    GUI.color = new Color(1f, 1f, 1f, 0.05f);
                    GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1f), Texture2D.whiteTexture);
                    GUI.DrawTexture(new Rect(rect.x, rect.y, 1f, rect.height), Texture2D.whiteTexture);
                    continue;
                }
                var pulse = LandPulse(x, y);
                DrawBlock(rect, Colors[kind - 1], 1f, pulse);
                if (blockSkin == "frame") DrawSkinRim(rect, Colors[kind - 1]);
            }
            for (var y = 0; y < BoardState.VisibleRows; y++)
            {
                var flash = fx.ClearAlpha(y);
                if (flash <= 0.01f) continue;
                var tint = fx.ClearColor(y);
                var row = new Rect(boardLeft, boardTop + y * cell, cell * 10f, cell);
                var cover = holding ? 0.4f : 0.92f;
                GUI.color = new Color(tint.r, tint.g, tint.b, flash * cover);
                GUI.DrawTexture(row, Texture2D.whiteTexture);
                GUI.color = new Color(1f, 1f, 1f, flash * 0.62f);
                GUI.DrawTexture(new Rect(row.x, row.y + row.height * 0.18f, row.width, row.height * 0.64f), Texture2D.whiteTexture);
            }
            DrawStackWarning(outer, holding);
            if (!game.HasPiece) return;
            if (fx.HardTrail > 0.02f) DrawTrail();
            if (LocalProgressService.GhostEnabled && !holding)
                DrawPiece(game.X, game.GhostY, Colors[(int)game.Active], GhostAlpha, 1f, 0f, false);
            DrawPiece(fx.VisualColumn, game.Y, Colors[(int)game.Active], 1f, fx.ActiveScale, fx.RotateDegrees, true);
        }

        float LandPulse(int x, int y)
        {
            if (fx.LandPulse <= 0f) return 0f;
            for (var i = 0; i < fx.LandCount; i++)
            {
                if (!fx.LandCell(i, out var lx, out var ly)) continue;
                if (lx == x && ly == y) return fx.LandPulse;
            }
            return 0f;
        }

        void DrawPiece(float originX, int originY, Color color, float alpha, float scale, float degrees, bool marked)
        {
            var parts = Tetrominoes.Cells(game.Active, game.ActiveRotation);
            var center = Vector2.zero;
            var visible = 0;
            foreach (var part in parts)
            {
                var y = originY + part.Y;
                if (y < 0 || y >= BoardState.VisibleRows) continue;
                var rect = CellAt(originX + part.X, y, 1f);
                center += rect.center;
                visible++;
            }
            if (visible > 0) center /= visible;
            foreach (var part in parts)
            {
                var y = originY + part.Y;
                if (y < 0 || y >= BoardState.VisibleRows) continue;
                var rect = CellAt(originX + part.X, y, scale);
                if (Mathf.Abs(degrees) > 0.1f) rect = Spin(rect, center, degrees);
                DrawBlock(rect, color, alpha, 0f);
                if (marked) DrawActiveRim(rect);
                if (fx.SoftFlash > 0.05f && alpha > 0.9f)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.18f * fx.SoftFlash);
                    GUI.DrawTexture(new Rect(rect.x, rect.yMax - 3f, rect.width, 3f), Texture2D.whiteTexture);
                }
            }
        }

        void DrawTrail()
        {
            var color = Colors[(int)fx.HardType];
            foreach (var part in Tetrominoes.Cells(fx.HardType, fx.HardRotation))
            {
                var column = fx.HardX + part.X;
                var from = Mathf.Min(fx.HardFromY, fx.HardToY) + part.Y;
                var to = Mathf.Max(fx.HardFromY, fx.HardToY) + part.Y;
                for (var y = from; y <= to; y++)
                {
                    if (y < 0 || y >= BoardState.VisibleRows) continue;
                    DrawBlock(CellRect(column, y), color, 0.22f * fx.HardTrail, 0f);
                }
            }
        }

        static Rect Spin(Rect rect, Vector2 center, float degrees)
        {
            var rad = degrees * Mathf.Deg2Rad;
            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);
            var d = rect.center - center;
            var spun = center + new Vector2(d.x * cos - d.y * sin, d.x * sin + d.y * cos);
            return new Rect(spun.x - rect.width * 0.5f, spun.y - rect.height * 0.5f, rect.width, rect.height);
        }

        void DrawBlock(Rect rect, Color color, float alpha, float pulse)
        {
            if (blockSkin == "soft")
            {
                var shrink = rect.width * 0.18f;
                rect = new Rect(rect.x + shrink, rect.y + shrink, Mathf.Max(2f, rect.width - shrink * 2f), Mathf.Max(2f, rect.height - shrink * 2f));
            }
            if (pulse > 0f)
            {
                var grow = pulse * 1.5f;
                rect = new Rect(rect.x - grow, rect.y - grow, rect.width + grow * 2f, rect.height + grow * 2f);
            }
            GUI.color = new Color(0f, 0f, 0f, 0.28f * alpha);
            GUI.DrawTexture(new Rect(rect.x + 1f, rect.y + 2f, rect.width, rect.height), Texture2D.whiteTexture);
            GUI.color = new Color(color.r * 0.45f, color.g * 0.45f, color.b * 0.45f, alpha);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = new Color(Mathf.Min(1f, color.r * 1.05f), Mathf.Min(1f, color.g * 1.05f), Mathf.Min(1f, color.b * 1.05f), alpha);
            GUI.DrawTexture(new Rect(rect.x + 1.5f, rect.y + 1.5f, rect.width - 3f, rect.height - 4f), Texture2D.whiteTexture);
            var inset = Mathf.Max(2f, rect.width * 0.16f);
            GUI.color = new Color(1f, 1f, 1f, 0.34f * alpha);
            GUI.DrawTexture(new Rect(rect.x + inset, rect.y + 2f, rect.width * 0.42f, Mathf.Max(2f, rect.height * 0.18f)), Texture2D.whiteTexture);
            GUI.color = new Color(0f, 0f, 0f, 0.2f * alpha);
            GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + rect.height * 0.78f, rect.width - 4f, Mathf.Max(2f, rect.height * 0.14f)), Texture2D.whiteTexture);
        }

        static void DrawSkinRim(Rect rect, Color color)
        {
            var thickness = Mathf.Clamp(rect.width * 0.1f, 2f, 3.5f);
            GUI.color = new Color(Mathf.Min(1f, color.r + 0.35f), Mathf.Min(1f, color.g + 0.35f), Mathf.Min(1f, color.b + 0.35f), 0.9f);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), Texture2D.whiteTexture);
        }

        static void DrawActiveRim(Rect rect)
        {
            var thickness = Mathf.Clamp(rect.width * 0.1f, 2.5f, 4f);
            GUI.color = new Color(1f, 0.98f, 0.82f, 0.96f);
            GUI.DrawTexture(new Rect(rect.x - 1f, rect.y - 1f, rect.width + 2f, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x - 1f, rect.yMax - thickness + 1f, rect.width + 2f, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x - 1f, rect.y, thickness, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - thickness + 1f, rect.y, thickness, rect.height), Texture2D.whiteTexture);
        }

        void DrawNext()
        {
            var panel = new Rect(boardLeft, boardTop - 96f, cell * 10f, 86f);
            GUI.color = new Color(0.03f, 0.08f, 0.18f, 0.92f);
            GUI.DrawTexture(panel, Texture2D.whiteTexture);
            GUI.color = new Color(0.82f, 0.68f, 0.32f);
            GUI.DrawTexture(new Rect(panel.x, panel.y, panel.width, 2f), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 0.9f, 0.62f);
            GUI.Label(new Rect(panel.x, panel.y + 2f, panel.width, 20f), UiLanguage.Line("next"), scoreStyle);
            if (dailyRun)
            {
                var progress = Mathf.Min(game.LinesCleared, LocalProgressService.DailyGoal) + " / " + LocalProgressService.DailyGoal;
                GUI.Label(new Rect(panel.xMax - 96f, panel.y + 2f, 92f, 20f), progress, hudValueStyle);
            }
            var cells = Tetrominoes.Cells(game.Next, Rotation.Zero);
            var minX = 4;
            var minY = 4;
            var maxX = 0;
            var maxY = 0;
            foreach (var part in cells)
            {
                if (part.X < minX) minX = part.X;
                if (part.Y < minY) minY = part.Y;
                if (part.X > maxX) maxX = part.X;
                if (part.Y > maxY) maxY = part.Y;
            }
            var spanX = maxX - minX + 1;
            var spanY = maxY - minY + 1;
            var mini = Mathf.Floor(Mathf.Min((panel.width - 24f) / spanX, (panel.height - 30f) / spanY));
            mini = Mathf.Clamp(mini, 22f, 40f);
            var originX = panel.center.x - spanX * mini * 0.5f - minX * mini;
            var originY = panel.y + 24f + (panel.height - 30f - spanY * mini) * 0.5f - minY * mini;
            var color = Colors[(int)game.Next];
            foreach (var part in cells)
                DrawBlock(new Rect(originX + part.X * mini, originY + part.Y * mini, mini - 2f, mini - 2f), color, 1f, 0f);
            DrawStageCounter(panel);
        }

        static int StageGoal(int index) => Mathf.Min(40, 10 + (index - 1) * 5);

        void DrawStageCounter(Rect panel)
        {
            var rtl = UiLanguage.RightToLeft;
            const float side = 100f;
            var stageBox = new Rect(rtl ? panel.xMax - side - 6f : panel.x + 6f, panel.y + 26f, side, 54f);
            var countBox = new Rect(rtl ? panel.x + 6f : panel.xMax - side - 6f, panel.y + 26f, side, 54f);
            GUI.color = new Color(1f, 0.9f, 0.62f);
            GUI.Label(new Rect(stageBox.x, stageBox.y, stageBox.width, 18f), UiLanguage.Line("stage"), hudCaptionStyle);
            GUI.color = Color.white;
            GUI.Label(new Rect(stageBox.x, stageBox.y + 16f, stageBox.width, 28f), stage.ToString(), hudValueStyle);
            GUI.color = new Color(1f, 0.93f, 0.72f);
            GUI.Label(new Rect(countBox.x, countBox.y, countBox.width, 32f), Mathf.Max(0, StageGoal(stage) - stagePlaced).ToString(), hudValueStyle);
            GUI.color = new Color(0.9f, 0.95f, 1f);
            GUI.Label(new Rect(countBox.x, countBox.y + 30f, countBox.width, 16f), UiLanguage.Line("pieces"), hudCaptionStyle);
            GUI.color = Color.white;
        }

        void DrawStageClear()
        {
            var rect = new Rect(boardLeft, boardTop + cell * 7.2f, cell * 10f, 64f);
            GUI.color = new Color(0.03f, 0.1f, 0.22f, 0.94f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = new Color(0.86f, 0.72f, 0.34f);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 3f), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 0.93f, 0.72f);
            GUI.Label(new Rect(rect.x, rect.y + 4f, rect.width, 28f), UiLanguage.Line("stageClear"), hudCaptionStyle);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x, rect.y + 28f, rect.width, 30f), stageCleared.ToString(), hudValueStyle);
        }

        void DrawBanner(string text, Color color)
        {
            var rect = new Rect(boardLeft, boardTop + cell * 8f, cell * 10f, 42f);
            GUI.color = new Color(0.02f, 0.05f, 0.12f, 0.72f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = color;
            GUI.Label(rect, UiLanguage.Display(text), calloutStyle);
        }

        void DrawButtons()
        {
            ReadPointer(out var down, out var gui);
            var blue = new Color(0.12f, 0.28f, 0.55f);
            var gold = new Color(0.72f, 0.55f, 0.12f);
            var green = new Color(0.12f, 0.45f, 0.28f);
            var red = new Color(0.55f, 0.14f, 0.18f);
            DrawControl(leftRect, down && leftRect.Contains(gui), blue, UiLanguage.Line("left"), icon => DrawLeftArrow(icon));
            DrawControl(rightRect, down && rightRect.Contains(gui), blue, UiLanguage.Line("right"), icon => DrawRightArrow(icon));
            DrawControl(rotateRect, down && rotateRect.Contains(gui), gold, UiLanguage.Line("rotate"), icon => DrawRotateArrow(icon));
            DrawControl(softRect, down && softRect.Contains(gui), green, UiLanguage.Line("soft"), icon => DrawDownArrow(icon, false));
            DrawControl(hardRect, down && hardRect.Contains(gui), red, UiLanguage.Line("hard"), icon => DrawDownArrow(icon, true));
        }

        void DrawControl(Rect rect, bool pressed, Color color, string label, Action<Rect> icon)
        {
            var fill = pressed ? new Color(color.r * 0.68f, color.g * 0.68f, color.b * 0.68f) : color;
            GUI.color = new Color(0f, 0f, 0f, 0.35f);
            GUI.DrawTexture(new Rect(rect.x + 2f, rect.y + 4f, rect.width, rect.height), Texture2D.whiteTexture);
            GUI.color = fill;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            if (!pressed)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.22f);
                GUI.DrawTexture(new Rect(rect.x + 3f, rect.y + 3f, rect.width - 6f, rect.height * 0.16f), Texture2D.whiteTexture);
            }
            GUI.color = Color.white;
            var iconSize = Mathf.Min(rect.width * 0.42f, rect.height * 0.48f);
            var iconRect = new Rect(rect.center.x - iconSize * 0.5f, rect.y + 8f, iconSize, iconSize * 0.72f);
            icon(iconRect);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 4f, rect.yMax - 24f, rect.width - 8f, 20f), label, controlLabelStyle);
        }

        static void DrawLeftArrow(Rect area)
        {
            const int steps = 12;
            var thickness = area.width / steps;
            for (var i = 0; i < steps; i++)
            {
                var height = Mathf.Lerp(3f, area.height, i / (float)(steps - 1));
                GUI.DrawTexture(new Rect(area.x + i * thickness, area.center.y - height * 0.5f, thickness + 1f, height), Texture2D.whiteTexture);
            }
        }

        static void DrawRightArrow(Rect area)
        {
            const int steps = 12;
            var thickness = area.width / steps;
            for (var i = 0; i < steps; i++)
            {
                var height = Mathf.Lerp(area.height, 3f, i / (float)(steps - 1));
                GUI.DrawTexture(new Rect(area.x + i * thickness, area.center.y - height * 0.5f, thickness + 1f, height), Texture2D.whiteTexture);
            }
        }

        static void DrawDownArrow(Rect area, bool doubled)
        {
            if (!doubled)
            {
                DrawDownTriangle(area);
                return;
            }
            var gap = 3f;
            var height = (area.height - gap) * 0.5f;
            DrawDownTriangle(new Rect(area.x, area.y, area.width, height));
            DrawDownTriangle(new Rect(area.x, area.y + height + gap, area.width, height));
        }

        static void DrawDownTriangle(Rect area)
        {
            const int steps = 10;
            var thickness = area.height / steps;
            for (var i = 0; i < steps; i++)
            {
                var width = Mathf.Lerp(area.width, 3f, i / (float)(steps - 1));
                GUI.DrawTexture(new Rect(area.center.x - width * 0.5f, area.y + i * thickness, width, thickness + 1f), Texture2D.whiteTexture);
            }
        }

        static void DrawRotateArrow(Rect area)
        {
            var center = area.center;
            var radius = Mathf.Min(area.width, area.height) * 0.46f;
            const int steps = 16;
            for (var i = 0; i < steps; i++)
            {
                var angle = (-40f + i * 18f) * Mathf.Deg2Rad;
                var point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                GUI.DrawTexture(new Rect(point.x - 2.5f, point.y - 2.5f, 5f, 5f), Texture2D.whiteTexture);
            }
            var head = center + new Vector2(Mathf.Cos(250f * Mathf.Deg2Rad), Mathf.Sin(250f * Mathf.Deg2Rad)) * radius;
            DrawDownTriangle(new Rect(head.x - 7f, head.y - 4f, 14f, 10f));
        }

        void DrawButton(Rect rect, string text, Color color)
        {
            GUI.color = new Color(0f,0f,0f,.35f);
            GUI.DrawTexture(new Rect(rect.x+2f,rect.y+4f,rect.width,rect.height), Texture2D.whiteTexture);
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = new Color(1f,1f,1f,.22f);
            GUI.DrawTexture(new Rect(rect.x+3f,rect.y+3f,rect.width-6f,rect.height*.18f),Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(rect, text, buttonStyle);
        }

        void DrawBackdrop()
        {
            GUI.color = Color.white;
            if (scenicBackground != null) GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), scenicBackground, ScaleMode.ScaleAndCrop);
            GUI.color = BoardMood.GameWash(LocalProgressService.Theme, game == null ? 0 : BoardMood.Band(DisplayedLevel));
            GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }


        void DrawOverlay(string heading, bool gameOver, float alpha)
        {
            GUI.color = new Color(0.01f, 0.04f, 0.1f, 0.82f * alpha);
            GUI.DrawTexture(new Rect(boardLeft, boardTop, cell * 10f, cell * 20f), Texture2D.whiteTexture);
            var plate = new Rect(boardLeft + cell, boardTop + cell * 6f, cell * 8f, gameOver ? cell * 5.2f : 72f);
            GUI.color = new Color(0.02f, 0.07f, 0.16f, 0.94f * alpha);
            GUI.DrawTexture(plate, Texture2D.whiteTexture);
            GUI.color = new Color(0.86f, 0.72f, 0.34f, alpha);
            GUI.DrawTexture(new Rect(plate.x, plate.y, plate.width, 3f), Texture2D.whiteTexture);
            GUI.color = new Color(1f, 0.96f, 0.82f, alpha);
            GUI.Label(new Rect(plate.x, plate.y + 8f, plate.width, 64f), heading, calloutStyle);
            if (!gameOver) return;
            GUI.Label(new Rect(plate.x, plate.y + 72f, plate.width, 32f), UiLanguage.Pair("score", game.Score.ToString()), scoreStyle);
            GUI.Label(new Rect(plate.x, plate.y + 104f, plate.width, 32f), UiLanguage.Pair("best", game.Best.ToString()), scoreStyle);
            if (roundAds.ContinueAvailable)
                DrawButton(overlayContinue, UiLanguage.Line("continueAd"), new Color(0.72f, 0.48f, 0.08f));
            DrawButton(overlayRestart, UiLanguage.Line("restart"), new Color(0.12f, 0.42f, 0.32f));
        }

        Rect CellRect(int x, int y) => CellAt(x, y, 1f);

        Rect CellAt(float x, float y, float scale)
        {
            var size = (cell - 3f) * scale;
            var pad = (cell - size) * 0.5f;
            return new Rect(boardLeft + x * cell + pad, boardTop + y * cell + pad, size, size);
        }

        void Layout()
        {
            var safe = Screen.safeArea;
            insetLeft = safe.x;
            insetRight = Screen.width - safe.xMax;
            insetTop = Screen.height - safe.yMax;
            insetBottom = safe.y + BannerAdManager.ReservedHeight;
            const float chrome = 410f;
            var usableWidth = Screen.width - insetLeft - insetRight - 20f;
            var usableHeight = Screen.height - insetTop - insetBottom - chrome;
            cell = Mathf.Floor(Mathf.Min(usableWidth / 10f, usableHeight / 20f));
            if (cell < 8f) cell = 8f;
            boardLeft = Mathf.Floor(insetLeft + (Screen.width - insetLeft - insetRight - cell * 10f) * 0.5f);
            boardTop = insetTop + 228f;
            pauseRect = new Rect(insetLeft + 8f, insetTop + 8f, 132f, 48f);
            restartRect = new Rect(Screen.width - insetRight - 140f, insetTop + 8f, 132f, 48f);
            var gap = 8f;
            var rowGap = 18f;
            var width = (Screen.width - insetLeft - insetRight - 16f - gap * 2f) / 3f;
            var row = boardTop + cell * 20f + 14f;
            var buttonHeight = 72f;
            var x0 = insetLeft + 8f;
            leftRect = new Rect(x0, row, width, buttonHeight);
            rightRect = new Rect(x0 + width + gap, row, width, buttonHeight);
            softRect = new Rect(x0 + (width + gap) * 2f, row, width, buttonHeight);
            var bottom = row + buttonHeight + rowGap;
            rotateRect = new Rect(leftRect.x, bottom, rightRect.xMax - leftRect.x, buttonHeight);
            hardRect = new Rect(softRect.x, bottom, softRect.width, buttonHeight);
            var continueShown = game != null && game.GameOver && roundAds.ContinueAvailable;
            overlayContinue = new Rect(boardLeft + cell, boardTop + cell * 11.2f, cell * 8f, 58f);
            overlayRestart = new Rect(boardLeft + cell * 2f, boardTop + cell * (continueShown ? 14.1f : 12f), cell * 6f, 58f);
        }

        static void ReadPointer(out bool down, out Vector2 gui)
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                down = touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled;
                gui = new Vector2(touch.position.x, Screen.height - touch.position.y);
                return;
            }
            down = Input.GetMouseButton(0);
            var mouse = Input.mousePosition;
            gui = new Vector2(mouse.x, Screen.height - mouse.y);
        }

        void Repeat(HoldScheduler scheduler, bool held, Func<bool> action)
        {
            var steps = scheduler.Tick(held, Time.deltaTime);
            for (var i = 0; i < steps; i++) action();
        }

        int SimulateHold(float seconds, bool right)
        {
            var hold = new HoldScheduler();
            var moves = 0;
            var elapsed = 0f;
            var first = true;
            const float step = 0.05f;
            while (true)
            {
                var count = hold.Tick(true, first ? 0f : step);
                first = false;
                for (var i = 0; i < count; i++)
                {
                    var moved = right ? game.MoveRight() : game.SoftDrop();
                    if (!moved) return moves;
                    moves++;
                }
                if (elapsed >= seconds) return moves;
                elapsed += step;
            }
        }

        void ReleaseHolds()
        {
            leftRepeat.Tick(false, 0f);
            rightRepeat.Tick(false, 0f);
            softRepeat.Tick(false, 0f);
            rotateWas = false;
            hardWas = false;
        }

        void PresentLock(IReadOnlyList<BoardCell> cells, int bestBefore, int levelBefore, Color pieceColor)
        {
            if (game == null || !game.LastLock) return;
            fx.OnLock(game, cells, bestBefore, levelBefore, pieceColor);
            if (game.LastClearCount > 0 && !LocalProgressService.ReducedEffects) fx.BeginClearHold();
            if (game.GameOver) fx.Overlay = 1f;
            NoteLock(bestBefore, levelBefore);
        }

        bool PlayMove(bool moved)
        {
            if (moved) GameAudio.Play("move");
            return moved;
        }

        void NoteStage()
        {
            stagePlaced++;
            if (stagePlaced < StageGoal(stage)) return;
            stageCleared = stage;
            stage++;
            stagePlaced = 0;
            stageClearUntil = Time.unscaledTime + 1.8f;
            GameAudio.Play("level");
        }

        void FlushPlaytime()
        {
            if (playtimePending <= 0f) return;
            LocalProgressService.AddPlaytime(playtimePending);
            playtimePending = 0f;
        }

        void NoteLock(int bestBefore, int levelBefore)
        {
            if (game == null || !game.LastLock) return;
            if (game.Callout.Length > 0) calloutUntil = Time.unscaledTime + 1.6f;
            seenCallout = game.CalloutId;
            LocalProgressService.RecordLock(game.LastClearCount, game.Combo, DisplayedLevel);
            if (game.GameOver) GameAudio.Play("over");
            else
            {
                NoteStage();
                if (game.LastClearCount >= 4) GameAudio.Play("tetris");
                else if (game.LastClearCount > 0) GameAudio.Play("clear");
                else GameAudio.Play("land");
            }
            if (game.Combo >= 2) GameAudio.Play("combo");
            if (dailyRun) LocalProgressService.RecordDaily(game.Score, game.LinesCleared);
            if (dailyRun && !dailyCelebrated && game.LinesCleared >= LocalProgressService.DailyGoal)
            {
                dailyCelebrated = true;
                dailyMessageUntil = Time.unscaledTime + 1.8f;
            }
            if (DisplayedLevel > levelBefore || game.Best > bestBefore) GameAudio.Play("level");
            GameAudio.Pulse(true);
            Debug.Log("BZ lock score=" + game.Score + " best=" + game.Best + " lines=" + game.LinesCleared + " combo=" + game.Combo + " callout=" + game.Callout + " over=" + game.GameOver);
        }

        void EnsureStyles()
        {
            if (scoreStyle == null)
            {
            scoreStyle = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
            buttonStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            calloutStyle = new GUIStyle(GUI.skin.label) { fontSize = 34, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            controlLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                clipping = TextClipping.Clip
            };
            hudCaptionStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                clipping = TextClipping.Clip
            };
            hudValueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 26,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                clipping = TextClipping.Clip
            };
            }
            var font = UiLanguage.Font;
            var rtl = UiLanguage.RightToLeft;
            if (font != null)
            {
                scoreStyle.font = font;
                buttonStyle.font = font;
                calloutStyle.font = font;
                controlLabelStyle.font = font;
                hudCaptionStyle.font = rtl ? UiLanguage.ArabicFont : font;
                hudValueStyle.font = UiLanguage.LatinFont;
            }
            calloutStyle.fontSize = rtl ? 30 : 34;
            controlLabelStyle.fontSize = rtl ? 14 : 13;
            hudCaptionStyle.fontSize = rtl ? 13 : 15;
        }

        void DrawStackWarning(Rect outer, bool holding)
        {
            var top = BoardState.VisibleRows;
            for (var y = 0; y < BoardState.VisibleRows && top == BoardState.VisibleRows; y++)
            for (var x = 0; x < BoardState.Width; x++)
            {
                var kind = holding ? game.ClearHoldCell(x, y) : game.Board.Get(x, y);
                if (kind == 0) continue;
                top = y;
                break;
            }
            if (top > 4) return;
            var closeness = Mathf.Clamp01((5 - top) / 5f);
            var alpha = 0.28f + closeness * 0.45f;
            if (!LocalProgressService.ReducedEffects)
                alpha *= 0.82f + 0.18f * Mathf.Sin(Time.unscaledTime * 2.2f);
            var thickness = 5f;
            GUI.color = new Color(0.96f, 0.62f, 0.28f, alpha);
            GUI.DrawTexture(new Rect(outer.x, outer.y, outer.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(outer.x, outer.yMax - thickness, outer.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(outer.x, outer.y, thickness, outer.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(outer.xMax - thickness, outer.y, thickness, outer.height), Texture2D.whiteTexture);
        }
    }
}
