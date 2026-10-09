using System;
using System.Linq;
using NUnit.Framework;

public sealed class VisualV2CaptureUtilityTests
{
    [Test]
    public void CaptureUtility_IsAvailableFromTheEditorAssembly()
    {
        var type = Type.GetType("Blockzara.Editor.VisualV2CaptureUtility, Assembly-CSharp-Editor");
        Assert.That(type, Is.Not.Null, "The editor-only Visual V2 capture entry point is required for deterministic Unity-rendered review frames.");
    }

    [Test]
    public void CaptureUtility_ProvidesPortraitGameViewConfiguration()
    {
        var type = Type.GetType("Blockzara.Editor.VisualV2CaptureUtility, Assembly-CSharp-Editor");
        Assert.That(type.GetMethod("ConfigurePortraitGameView", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic), Is.Not.Null,
            "The capture utility must select a real 720x1520 Editor Game View before Unity captures a frame.");
    }

    [Test]
    public void MainMenuV5_IsAnIsolatedUnityUiRenderer()
    {
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType("Blockzara.Runtime.BlockzaraMenuV5"))
            .FirstOrDefault(candidate => candidate != null);
        Assert.That(type, Is.Not.Null, "Main Menu V5 must live outside the falling-blocks gameplay renderer.");
        Assert.That(type.GetMethod("Build"), Is.Not.Null);
        Assert.That(type.GetMethod("Hide"), Is.Not.Null);
    }

    [Test]
    public void MainMenuV5_UsesSeparateSpritesForButtonsAndIcons()
    {
        var source = System.IO.File.ReadAllText("Assets/Blockzara/Runtime/BlockzaraMenuV5.cs");
        Assert.That(source, Does.Contain("Resources.Load<Sprite>(\"MainMenuV5/"));
        Assert.That(source, Does.Contain("Image.Type.Sliced"));
        Assert.That(source, Does.Contain("onClick.AddListener(app.StartFromMenu)"));
        Assert.That(source, Does.Not.Contain("LoadScene"));
    }
}
