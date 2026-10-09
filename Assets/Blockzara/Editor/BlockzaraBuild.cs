using System.IO;
using Blockzara.Runtime;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BlockzaraBuild
{
    public static void CreateSceneAndBuildAndroidDebug() => BuildDebug("Builds/Blockzara-debug.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV3() => BuildDebug("Builds/Blockzara-debug-ui-v3.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV31() => BuildDebug("Builds/Blockzara-debug-ui-v3-1.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV32() => BuildDebug("Builds/Blockzara-debug-ui-v3-2.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV33() => BuildDebug("Builds/Blockzara-debug-ui-v3-3.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV34() => BuildDebug("Builds/Blockzara-debug-ui-v3-4.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV35() => BuildDebug("Builds/Blockzara-debug-ui-v3-5.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV36() => BuildDebug("Builds/Blockzara-debug-ui-v3-6.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV37() => BuildDebug("Builds/Blockzara-debug-ui-v3-7.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV38() => BuildDebug("Builds/Blockzara-debug-ui-v3-8.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV39() => BuildDebug("Builds/Blockzara-debug-ui-v3-9.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV310() => BuildDebug("Builds/Blockzara-debug-ui-v3-10.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV311() => BuildDebug("Builds/Blockzara-debug-ui-v3-11.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV312() => BuildDebug("Builds/Blockzara-debug-ui-v3-12.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV313() => BuildDebug("Builds/Blockzara-debug-ui-v3-13.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV314() => BuildDebug("Builds/Blockzara-debug-ui-v3-14.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV315() => BuildDebug("Builds/Blockzara-debug-ui-v3-15.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV316() => BuildDebug("Builds/Blockzara-debug-ui-v3-16.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV317() => BuildDebug("Builds/Blockzara-debug-ui-v3-17.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV318() => BuildDebug("Builds/Blockzara-debug-ui-v3-18.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV319() => BuildDebug("Builds/Blockzara-debug-ui-v3-19.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV320() => BuildDebug("Builds/Blockzara-debug-ui-v3-20.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV321() => BuildDebug("Builds/Blockzara-debug-ui-v3-21.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV322() => BuildDebug("Builds/Blockzara-debug-ui-v3-22.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV323() => BuildDebug("Builds/Blockzara-debug-ui-v3-23.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV324() => BuildDebug("Builds/Blockzara-debug-ui-v3-24.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV325() => BuildDebug("Builds/Blockzara-debug-ui-v3-25.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV326() => BuildDebug("Builds/Blockzara-debug-ui-v3-26.apk");

    public static void CreateSceneAndBuildAndroidDebugUiV3Arm64()
    {
        var previous = PlayerSettings.Android.targetArchitectures;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        try
        {
            BuildDebug("Builds/Blockzara-debug-ui-v3-arm64.apk");
        }
        finally
        {
            PlayerSettings.Android.targetArchitectures = previous;
        }
    }

    static void BuildDebug(string path)
    {
        Directory.CreateDirectory("Assets/Blockzara/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        new GameObject("BLOCKZARA").AddComponent<BlockzaraApp>();
        EditorSceneManager.SaveScene(scene, "Assets/Blockzara/Scenes/Bootstrap.unity");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.blockzara.game");
        ApplyIcon();
        ConfigureAudio();
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        Directory.CreateDirectory("Builds");
        BuildPipeline.BuildPlayer(new[] { "Assets/Blockzara/Scenes/Bootstrap.unity" }, path, BuildTarget.Android, BuildOptions.Development);
    }

    static void ApplyIcon()
    {
        const string path = "Assets/Blockzara/Icons/app-icon.png";
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            throw new FileNotFoundException(path);
        importer.textureType = TextureImporterType.Default;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.mipmapEnabled = false;
        importer.isReadable = true;
        importer.alphaIsTransparency = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 1024;
        importer.SaveAndReimport();

        var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        Assign(UnityEditor.Android.AndroidPlatformIconKind.Adaptive, icon);
        Debug.Log("BLOCKZARA icon assigned " + icon.width + "x" + icon.height);
    }

    static void Assign(PlatformIconKind kind, Texture2D icon)
    {
        var icons = PlayerSettings.GetPlatformIcons(NamedBuildTarget.Android, kind);
        foreach (var slot in icons)
        {
            var layers = new Texture2D[Mathf.Max(1, slot.maxLayerCount)];
            for (var layer = 0; layer < layers.Length; layer++)
                layers[layer] = icon;
            slot.SetTextures(layers);
        }

        PlayerSettings.SetPlatformIcons(NamedBuildTarget.Android, kind, icons);
    }

    static void ConfigureAudio()
    {
        ConfigureClip("Assets/Resources/Music/menu-acoustic.mp3", true);
        ConfigureClip("Assets/Resources/Music/play-focus.mp3", true);
        ConfigureClip("Assets/Resources/Sfx/whoosh.mp3", false);
        ConfigureClip("Assets/Resources/Sfx/click.mp3", false);
        ConfigureClip("Assets/Resources/Sfx/pop.mp3", false);
        ConfigureClip("Assets/Resources/Sfx/game-over.mp3", false);
    }

    static void ConfigureClip(string path, bool stream)
    {
        var importer = AssetImporter.GetAtPath(path) as AudioImporter;
        if (importer == null) throw new FileNotFoundException(path);
        var settings = importer.defaultSampleSettings;
        settings.loadType = stream ? AudioClipLoadType.Streaming : AudioClipLoadType.CompressedInMemory;
        settings.compressionFormat = AudioCompressionFormat.Vorbis;
        settings.quality = stream ? 0.6f : 0.8f;
        importer.defaultSampleSettings = settings;
        importer.loadInBackground = stream;
        importer.SaveAndReimport();
    }
}
