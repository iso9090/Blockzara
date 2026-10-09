using System.Globalization;
using UnityEngine;

namespace Blockzara.Runtime
{
    public struct CareerStats
    {
        public int Best;
        public int Games;
        public int Lines;
        public int HighestLevel;
        public int Tetrises;
        public int LongestCombo;
        public float PlaytimeSeconds;
    }

    public struct AchievementState
    {
        public string Title;
        public string Detail;
        public bool Unlocked;
    }

    public static class LocalProgressService
    {
        public const string BestScoreKey = "blockzara.best";
        public const string GamesKey = "blockzara.games";
        public const string LinesKey = "blockzara.lines";
        public const string HighestLevelKey = "blockzara.maxLevel";
        public const string TetrisesKey = "blockzara.tetrises";
        public const string LongestComboKey = "blockzara.longestCombo";
        public const string PlaytimeKey = "blockzara.playtime";
        public const string MusicKey = "blockzara.music";
        public const string SfxKey = "blockzara.sfx";
        public const string VibrationKey = "blockzara.vibration";
        public const string ReducedEffectsKey = "blockzara.reducedEffects";
        public const string GhostKey = "blockzara.ghost";
        public const string LanguageKey = "blockzara.language";
        public const string ThemeKey = "blockzara.theme";
        public const string SkinKey = "blockzara.skin";
        public const string DailyDateKey = "blockzara.daily.date";
        public const string DailyBestKey = "blockzara.daily.best";
        public const string DailyStreakKey = "blockzara.daily.streak";
        public const string DailyDoneKey = "blockzara.daily.done";
        public const string DailyLastKey = "blockzara.daily.last";
        public const int DailyGoal = 20;
        public const string AdsWatchedKey = "blockzara.ads.watched";
        public const string SaveVersionKey = "blockzara.save.version";
        public const int SaveVersion = 1;

        public static int LoadBestScore() => PlayerPrefs.GetInt(BestScoreKey, 0);

        public static int AdsWatched => PlayerPrefs.GetInt(AdsWatchedKey, 0);

        public static void RecordAdWatch()
        {
            PlayerPrefs.SetInt(AdsWatchedKey, AdsWatched + 1);
            PlayerPrefs.Save();
        }

        public static string FormatAdBill(int watched)
        {
            if (watched < 0) watched = 0;
            var cents = watched / 10;
            return (cents / 100).ToString(CultureInfo.InvariantCulture) + "." + (cents % 100).ToString("00", CultureInfo.InvariantCulture);
        }

        public static void SaveBestScoreIfHigher(int score)
        {
            if (score <= LoadBestScore()) return;
            PlayerPrefs.SetInt(BestScoreKey, score);
            PlayerPrefs.Save();
        }

        public static void Migrate()
        {
            if (PlayerPrefs.GetInt(SaveVersionKey, 0) >= SaveVersion) return;
            PlayerPrefs.SetInt(SaveVersionKey, SaveVersion);
            PlayerPrefs.Save();
        }

        public static CareerStats LoadCareer()
        {
            return new CareerStats
            {
                Best = LoadBestScore(),
                Games = PlayerPrefs.GetInt(GamesKey, 0),
                Lines = PlayerPrefs.GetInt(LinesKey, 0),
                HighestLevel = PlayerPrefs.GetInt(HighestLevelKey, 0),
                Tetrises = PlayerPrefs.GetInt(TetrisesKey, 0),
                LongestCombo = PlayerPrefs.GetInt(LongestComboKey, 0),
                PlaytimeSeconds = PlayerPrefs.GetFloat(PlaytimeKey, 0f)
            };
        }

        public static void RecordGameStart()
        {
            PlayerPrefs.SetInt(GamesKey, PlayerPrefs.GetInt(GamesKey, 0) + 1);
            PlayerPrefs.Save();
        }

        public static void RecordLock(int linesCleared, int combo, int displayedLevel)
        {
            if (linesCleared > 0)
                PlayerPrefs.SetInt(LinesKey, PlayerPrefs.GetInt(LinesKey, 0) + linesCleared);
            if (linesCleared >= 4)
                PlayerPrefs.SetInt(TetrisesKey, PlayerPrefs.GetInt(TetrisesKey, 0) + 1);
            if (combo > PlayerPrefs.GetInt(LongestComboKey, 0))
                PlayerPrefs.SetInt(LongestComboKey, combo);
            if (displayedLevel > PlayerPrefs.GetInt(HighestLevelKey, 0))
                PlayerPrefs.SetInt(HighestLevelKey, displayedLevel);
            PlayerPrefs.Save();
        }

        public static void AddPlaytime(float seconds)
        {
            if (seconds <= 0f) return;
            PlayerPrefs.SetFloat(PlaytimeKey, PlayerPrefs.GetFloat(PlaytimeKey, 0f) + seconds);
            PlayerPrefs.Save();
        }

        public static bool MusicEnabled
        {
            get => PlayerPrefs.GetInt(MusicKey, 1) == 1;
            set => SetFlag(MusicKey, value);
        }

        public static bool SfxEnabled
        {
            get => PlayerPrefs.GetInt(SfxKey, 1) == 1;
            set => SetFlag(SfxKey, value);
        }

        public static bool VibrationEnabled
        {
            get => PlayerPrefs.GetInt(VibrationKey, 0) == 1;
            set => SetFlag(VibrationKey, value);
        }

        public static bool ReducedEffects
        {
            get => PlayerPrefs.GetInt(ReducedEffectsKey, 0) == 1;
            set => SetFlag(ReducedEffectsKey, value);
        }

        public static bool GhostEnabled
        {
            get => PlayerPrefs.GetInt(GhostKey, 1) == 1;
            set => SetFlag(GhostKey, value);
        }

        public static string Language
        {
            get
            {
                var value = PlayerPrefs.GetString(LanguageKey, "ar");
                return value == "en" || value == "ur" ? value : "ar";
            }
            set
            {
                var code = value == "en" || value == "ur" ? value : "ar";
                PlayerPrefs.SetString(LanguageKey, code);
                PlayerPrefs.Save();
                UiLanguage.Refresh();
            }
        }

        public static string Theme
        {
            get
            {
                var value = PlayerPrefs.GetString(ThemeKey, "coastal");
                return value == "harbor" || value == "night" ? value : "coastal";
            }
            set
            {
                PlayerPrefs.SetString(ThemeKey, value == "harbor" || value == "night" ? value : "coastal");
                PlayerPrefs.Save();
            }
        }

        public static string Skin
        {
            get
            {
                var value = PlayerPrefs.GetString(SkinKey, "solid");
                return value == "soft" || value == "frame" ? value : "solid";
            }
            set
            {
                PlayerPrefs.SetString(SkinKey, value == "soft" || value == "frame" ? value : "solid");
                PlayerPrefs.Save();
            }
        }

        public static int DailySeed() => DailyStamp(System.DateTime.Now);

        public static int DailyBest
        {
            get
            {
                EnsureDailyToday();
                return PlayerPrefs.GetInt(DailyBestKey, 0);
            }
        }

        public static int DailyStreak => PlayerPrefs.GetInt(DailyStreakKey, 0);

        public static bool DailyDone
        {
            get
            {
                EnsureDailyToday();
                return PlayerPrefs.GetInt(DailyDoneKey, 0) == 1;
            }
        }

        public static void RecordDaily(int score, int lines)
        {
            EnsureDailyToday();
            if (score > PlayerPrefs.GetInt(DailyBestKey, 0))
                PlayerPrefs.SetInt(DailyBestKey, score);
            if (lines >= DailyGoal && PlayerPrefs.GetInt(DailyDoneKey, 0) == 0)
            {
                PlayerPrefs.SetInt(DailyDoneKey, 1);
                var today = DailyStamp(System.DateTime.Now);
                var yesterday = DailyStamp(System.DateTime.Now.Date.AddDays(-1));
                var last = PlayerPrefs.GetInt(DailyLastKey, 0);
                var streak = last == yesterday ? PlayerPrefs.GetInt(DailyStreakKey, 0) + 1 : 1;
                PlayerPrefs.SetInt(DailyStreakKey, streak);
                PlayerPrefs.SetInt(DailyLastKey, today);
            }
            PlayerPrefs.Save();
        }

        static int DailyStamp(System.DateTime day) => day.Year * 10000 + day.Month * 100 + day.Day;

        static void EnsureDailyToday()
        {
            var today = DailyStamp(System.DateTime.Now);
            if (PlayerPrefs.GetInt(DailyDateKey, 0) == today) return;
            PlayerPrefs.SetInt(DailyDateKey, today);
            PlayerPrefs.SetInt(DailyBestKey, 0);
            PlayerPrefs.SetInt(DailyDoneKey, 0);
            PlayerPrefs.Save();
        }

        public static AchievementState[] Achievements()
        {
            var stats = LoadCareer();
            return new[]
            {
                Gate("First game", "Start a run", stats.Games >= 1),
                Gate("Ten games", "Start 10 runs", stats.Games >= 10),
                Gate("Line clearer", "Clear 10 lines", stats.Lines >= 10),
                Gate("Century", "Clear 100 lines", stats.Lines >= 100),
                Gate("Tetris", "Clear four lines at once", stats.Tetrises >= 1),
                Gate("Combo", "Reach a x3 combo", stats.LongestCombo >= 3),
                Gate("Level 5", "Reach displayed level 5", stats.HighestLevel >= 5),
                Gate("High score", "Reach a best of 1000", stats.Best >= 1000)
            };
        }

        static AchievementState Gate(string title, string detail, bool unlocked)
        {
            return new AchievementState { Title = title, Detail = detail, Unlocked = unlocked };
        }

        static void SetFlag(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
