using Blockzara.Core;
using Blockzara.Runtime;
using NUnit.Framework;
using UnityEngine;

public sealed class LocalProgressEditTests
{
    int best;
    int games;
    int lines;
    int level;
    int tetrises;
    int combo;
    float playtime;
    int version;

    [SetUp]
    public void Remember()
    {
        best = PlayerPrefs.GetInt(LocalProgressService.BestScoreKey, 0);
        games = PlayerPrefs.GetInt(LocalProgressService.GamesKey, 0);
        lines = PlayerPrefs.GetInt(LocalProgressService.LinesKey, 0);
        level = PlayerPrefs.GetInt(LocalProgressService.HighestLevelKey, 0);
        tetrises = PlayerPrefs.GetInt(LocalProgressService.TetrisesKey, 0);
        combo = PlayerPrefs.GetInt(LocalProgressService.LongestComboKey, 0);
        playtime = PlayerPrefs.GetFloat(LocalProgressService.PlaytimeKey, 0f);
        version = PlayerPrefs.GetInt(LocalProgressService.SaveVersionKey, 0);
    }

    [TearDown]
    public void Restore()
    {
        PlayerPrefs.SetInt(LocalProgressService.BestScoreKey, best);
        PlayerPrefs.SetInt(LocalProgressService.GamesKey, games);
        PlayerPrefs.SetInt(LocalProgressService.LinesKey, lines);
        PlayerPrefs.SetInt(LocalProgressService.HighestLevelKey, level);
        PlayerPrefs.SetInt(LocalProgressService.TetrisesKey, tetrises);
        PlayerPrefs.SetInt(LocalProgressService.LongestComboKey, combo);
        PlayerPrefs.SetFloat(LocalProgressService.PlaytimeKey, playtime);
        PlayerPrefs.SetInt(LocalProgressService.SaveVersionKey, version);
        PlayerPrefs.Save();
    }

    [Test]
    public void Career_RecordsLinesAndKeepsTheExistingBest()
    {
        LocalProgressService.Migrate();
        LocalProgressService.RecordLock(4, 3, 5);
        var stats = LocalProgressService.LoadCareer();
        Assert.That(stats.Best, Is.EqualTo(best));
        Assert.That(stats.Lines, Is.EqualTo(lines + 4));
        Assert.That(stats.Tetrises, Is.EqualTo(tetrises + 1));
        Assert.That(stats.LongestCombo, Is.EqualTo(Mathf.Max(combo, 3)));
        Assert.That(stats.HighestLevel, Is.EqualTo(Mathf.Max(level, 5)));
        Assert.That(LocalProgressService.Achievements().Length, Is.EqualTo(8));
    }

    [Test]
    public void Presentation_DoesNotChangeTheFallingRules()
    {
        var game = new FallingGame(new SeededRandom(4), 12);
        var score = game.Score;
        var x = game.X;
        var y = game.Y;
        var fx = new GameplayFx();
        fx.Reset(game);
        fx.Tick(game, 0.05f, false);
        fx.OnRotate();
        fx.OnSoft();
        fx.Tick(game, 0.05f, true);
        Assert.That(game.Score, Is.EqualTo(score));
        Assert.That(game.X, Is.EqualTo(x));
        Assert.That(game.Y, Is.EqualTo(y));
        Assert.That(game.Best, Is.EqualTo(12));
        Assert.That(fx.VisualColumn, Is.EqualTo(x).Within(0.001f));
    }
}
