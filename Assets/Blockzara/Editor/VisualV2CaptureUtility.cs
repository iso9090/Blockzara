using System;
using System.IO;
using System.Reflection;
using Blockzara.Core;
using Blockzara.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Blockzara.Editor
{
    [InitializeOnLoad]
    public static class VisualV2CaptureUtility
    {
        const string CommandLineFlag = "-visualV2Capture";
        const string SessionKey = "Blockzara.VisualV2Capture.Active";
        static readonly string[] FrameNames = { "01-main-menu", "02-gameplay", "03-stacked-blocks", "04-pause", "05-game-over" };

        static int frameIndex;
        static double nextCaptureAt;
        static bool exitWhenFinished;

        static VisualV2CaptureUtility()
        {
            var requested = HasCaptureFlag();
            Debug.Log("BZ Visual V2 capture initializer requested=" + requested);
            if (!requested) return;
            exitWhenFinished = true;
            SessionState.SetBool(SessionKey, true);
            EditorApplication.update += BeginRequestedCapture;
        }

        static void BeginRequestedCapture()
        {
            EditorApplication.update -= BeginRequestedCapture;
            if (EditorApplication.isPlaying)
                StartCapturingFrames();
            else
                Begin();
        }

        [MenuItem("Blockzara/Visual V2/Capture Review Frames")]
        public static void CaptureReviewFrames()
        {
            exitWhenFinished = false;
            Begin();
        }

        static void Begin()
        {
            Debug.Log("BZ Visual V2 capture begin playing=" + EditorApplication.isPlaying);
            if (SessionState.GetBool(SessionKey, false) && EditorApplication.isPlaying) return;

            SessionState.SetBool(SessionKey, true);
            Directory.CreateDirectory(OutputDirectory);
            ConfigurePortraitGameView();
            Screen.SetResolution(720, 1520, false);
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("BLOCKZARA Visual V2 Capture").AddComponent<BlockzaraApp>();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.isPlaying = true;
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            Debug.Log("BZ Visual V2 capture play-state=" + state);
            if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(SessionKey, false)) return;

            StartCapturingFrames();
        }

        static void StartCapturingFrames()
        {
            if (!SessionState.GetBool(SessionKey, false)) return;

            frameIndex = 0;
            nextCaptureAt = EditorApplication.timeSinceStartup + 1.0;
            EditorApplication.update -= CaptureNextFrame;
            EditorApplication.update += CaptureNextFrame;
        }

        static void CaptureNextFrame()
        {
            if (!EditorApplication.isPlaying || !SessionState.GetBool(SessionKey, false)) return;
            if (EditorApplication.timeSinceStartup < nextCaptureAt) return;

            var app = UnityEngine.Object.FindFirstObjectByType<BlockzaraApp>();
            if (app == null) return;
            if (Screen.width != 720 || Screen.height != 1520)
            {
                Debug.Log("BZ Visual V2 waiting for portrait render size=" + Screen.width + "x" + Screen.height);
                nextCaptureAt = EditorApplication.timeSinceStartup + 0.25;
                return;
            }

            if (frameIndex < FrameNames.Length)
            {
                ApplyState(app, frameIndex);
                var path = Path.Combine(OutputDirectory, FrameNames[frameIndex] + ".png");
                Debug.Log("BZ Visual V2 capture frame=" + FrameNames[frameIndex] + " path=" + path);
                ScreenCapture.CaptureScreenshot(path);
                frameIndex++;
                nextCaptureAt = EditorApplication.timeSinceStartup + 1.0;
                return;
            }

            EditorApplication.update -= CaptureNextFrame;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            SessionState.EraseBool(SessionKey);
            EditorApplication.isPlaying = false;
            if (exitWhenFinished) EditorApplication.delayCall += () => EditorApplication.Exit(0);
        }

        static void ApplyState(BlockzaraApp app, int state)
        {
            if (state == 0) return;

            app.StartRun(1024 + state);
            if (state == 1) return;

            if (state == 2)
            {
                for (var y = 15; y < BoardState.VisibleRows; y++)
                for (var x = 0; x < BoardState.Width; x++)
                    if ((x + y) % 3 != 0) app.Game.Board.Set(x, y, (x + y) % 7 + 1);
                app.Game.BeginPiece(Tetromino.T, Rotation.Zero, 3, 8);
                return;
            }

            if (state == 3)
            {
                app.Game.BeginPiece(Tetromino.L, Rotation.Right, 3, 7);
                app.Game.SetPaused(true);
                return;
            }

            for (var x = 0; x < BoardState.Width; x++)
                for (var y = -2; y < 3; y++)
                    app.Game.Board.Set(x, y, 1);
            app.Game.TrySpawnNext();
        }

        static bool HasCaptureFlag()
        {
            foreach (var argument in Environment.GetCommandLineArgs())
                if (string.Equals(argument, CommandLineFlag, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        static void ConfigurePortraitGameView()
        {
            var editorAssembly = typeof(EditorWindow).Assembly;
            var gameViewType = editorAssembly.GetType("UnityEditor.GameView");
            var gameViewSizesType = editorAssembly.GetType("UnityEditor.GameViewSizes");
            var sizeGroupType = editorAssembly.GetType("UnityEditor.GameViewSizeGroupType");
            var sizeType = editorAssembly.GetType("UnityEditor.GameViewSizeType");
            var sizeDefinitionType = editorAssembly.GetType("UnityEditor.GameViewSize");
            if (gameViewType == null || gameViewSizesType == null || sizeGroupType == null || sizeType == null || sizeDefinitionType == null)
                throw new InvalidOperationException("Unity Game View reflection types are unavailable.");

            var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(gameViewSizesType);
            var sizes = singletonType.GetProperty("instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null, null);
            if (sizes == null) throw new InvalidOperationException("Unity Game View size service is unavailable.");
            var android = Enum.Parse(sizeGroupType, "Android");
            var group = gameViewSizesType.GetMethod("GetGroup", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(sizes, new[] { android });
            var fixedResolution = Enum.Parse(sizeType, "FixedResolution");
            var customSize = Activator.CreateInstance(sizeDefinitionType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                new object[] { fixedResolution, 720, 1520, "Blockzara Visual V2 Review" }, null);
            group.GetType().GetMethod("AddCustomSize", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(group, new[] { customSize });
            var selectedIndex = (int)group.GetType().GetMethod("GetTotalCount", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(group, null) - 1;
            var gameView = EditorWindow.GetWindow(gameViewType, false, "Game", true);
            gameViewType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(gameView, selectedIndex, null);
            gameView.Repaint();
        }

        static string OutputDirectory => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "outputs", "visual-v2-review"));
    }
}
