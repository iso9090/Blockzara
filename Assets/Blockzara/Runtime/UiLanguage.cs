using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Blockzara.Runtime
{
    public static class UiLanguage
    {
        static readonly Dictionary<string, string[]> Text = new Dictionary<string, string[]>();
        static readonly Dictionary<char, LetterForm> Glyphs = new Dictionary<char, LetterForm>();
        static Font arabicFont;
        static Font latinFont;

        static UiLanguage()
        {
            Add("play", "PLAY", "\u0627\u0644\u0639\u0628", "\u0634\u0631\u0648\u0639");
            Add("pause", "PAUSE", "\u0625\u064A\u0642\u0627\u0641", "\u062A\u0648\u0642\u0641");
            Add("resume", "RESUME", "\u0645\u062A\u0627\u0628\u0639\u0629", "\u062C\u0627\u0631\u06CC");
            Add("restart", "RESTART", "\u0625\u0639\u0627\u062F\u0629", "\u062F\u0648\u0628\u0627\u0631\u06C1");
            Add("score", "SCORE", "\u0627\u0644\u0646\u0642\u0627\u0637", "\u0627\u0633\u06A9\u0648\u0631");
            Add("best", "BEST", "\u0627\u0644\u0623\u0641\u0636\u0644", "\u0628\u06C1\u062A\u0631\u06CC\u0646");
            Add("lines", "LINES", "\u0627\u0644\u0623\u0633\u0637\u0631", "\u0644\u0627\u0626\u0646\u06CC\u06BA");
            Add("level", "LEVEL", "\u0627\u0644\u0645\u0633\u062A\u0648\u0649", "\u0633\u0637\u062D");
            Add("next", "NEXT", "\u0627\u0644\u062A\u0627\u0644\u064A\u0629", "\u0627\u06AF\u0644\u0627");
            Add("stage", "STAGE", "\u0627\u0644\u0645\u0631\u062D\u0644\u0629", "\u0645\u0631\u062D\u0644\u06C1");
            Add("pieces", "PIECES", "\u0642\u0637\u0639", "\u0679\u06A9\u0691\u06D2");
            Add("stageClear", "STAGE CLEAR", "\u0623\u0646\u0647\u064A\u062A \u0627\u0644\u0645\u0631\u062D\u0644\u0629", "\u0645\u0631\u062D\u0644\u06C1 \u0645\u06A9\u0645\u0644");
            Add("left", "LEFT", "\u064A\u0633\u0627\u0631", "\u0628\u0627\u0626\u06CC\u06BA");
            Add("rotate", "ROTATE", "\u062F\u0648\u0631\u0627\u0646", "\u06AF\u06BE\u0645\u0627\u0624");
            Add("right", "RIGHT", "\u064A\u0645\u064A\u0646", "\u062F\u0627\u0626\u06CC\u06BA");
            Add("soft", "SOFT DROP", "\u0625\u0646\u0632\u0627\u0644", "\u0622\u06C1\u0633\u062A\u06C1");
            Add("hard", "HARD DROP", "\u0625\u0633\u0642\u0627\u0637", "\u0641\u0648\u0631\u06CC");
            Add("over", "GAME OVER", "\u0627\u0646\u062A\u0647\u062A", "\u062E\u062A\u0645");
            Add("continueAd", "CONTINUE", "\u0645\u062A\u0627\u0628\u0639\u0629", "\u062C\u0627\u0631\u06CC");
            Add("settings", "Settings", "\u0627\u0644\u0625\u0639\u062F\u0627\u062F\u0627\u062A", "\u062A\u0631\u062A\u06CC\u0628\u0627\u062A");
            Add("stats", "Stats", "\u0627\u0644\u0625\u062D\u0635\u0627\u0621\u0627\u062A", "\u0627\u0639\u062F\u0627\u062F");
            Add("achievements", "Achievements", "\u0627\u0644\u0625\u0646\u062C\u0627\u0632\u0627\u062A", "\u06A9\u0627\u0645\u06CC\u0627\u0628\u06CC\u0627\u06BA");
            Add("themes", "Themes", "\u0627\u0644\u062B\u064A\u0645\u0627\u062A", "\u062A\u06BE\u06CC\u0645\u0632");
            Add("title.settings", "SETTINGS", "\u0627\u0644\u0625\u0639\u062F\u0627\u062F\u0627\u062A", "\u062A\u0631\u062A\u06CC\u0628\u0627\u062A");
            Add("title.stats", "STATS", "\u0627\u0644\u0625\u062D\u0635\u0627\u0621\u0627\u062A", "\u0627\u0639\u062F\u0627\u062F");
            Add("title.achievements", "ACHIEVEMENTS", "\u0627\u0644\u0625\u0646\u062C\u0627\u0632\u0627\u062A", "\u06A9\u0627\u0645\u06CC\u0627\u0628\u06CC\u0627\u06BA");
            Add("title.themes", "THEMES", "\u0627\u0644\u062B\u064A\u0645\u0627\u062A", "\u062A\u06BE\u06CC\u0645\u0632");
            Add("menu", "MAIN MENU", "\u0627\u0644\u0642\u0627\u0626\u0645\u0629", "\u0645\u06CC\u0646\u0648");
            Add("music", "Music", "\u0627\u0644\u0645\u0648\u0633\u064A\u0642\u0649", "\u0645\u0648\u0633\u06CC\u0642\u06CC");
            Add("sfx", "Sound effects", "\u0627\u0644\u0623\u0635\u0648\u0627\u062A", "\u0622\u0648\u0627\u0632");
            Add("vibration", "Vibration", "\u0627\u0644\u0627\u0647\u062A\u0632\u0627\u0632", "\u0644\u0631\u0632\u0634");
            Add("reduced", "Reduced effects", "\u0645\u0624\u062B\u0631\u0627\u062A \u0623\u0642\u0644", "\u06A9\u0645 \u0627\u062B\u0631\u0627\u062A");
            Add("shadow", "Shadow", "\u0627\u0644\u0638\u0644", "\u0633\u0627\u06CC\u06C1");
            Add("language", "Language", "\u0627\u0644\u0644\u063A\u0629", "\u0632\u0628\u0627\u0646");
            Add("locked", "LOCKED", "\u0645\u0642\u0641\u0644", "\u0645\u0642\u0641\u0644");
            Add("unlocked", "UNLOCKED", "\u0645\u0641\u062A\u0648\u062D", "\u06A9\u06BE\u0644\u0627");
            Add("yes", "YES", "\u0646\u0639\u0645", "\u06C1\u0627\u06BA");
            Add("no", "NO", "\u0644\u0627", "\u0646\u06C1\u06CC\u06BA");
            Add("back", "BACK", "\u0631\u062C\u0648\u0639", "\u0648\u0627\u067E\u0633");
            Add("tagline", "PLACE \u2022 CLEAR \u2022 EXPLORE", "\u0636\u0639  \u0627\u0645\u0633\u062D  \u0627\u0633\u062A\u0643\u0634\u0641", "\u0631\u06A9\u06BE\u0648  \u0645\u0679\u0627\u0624  \u062F\u06CC\u06A9\u06BE\u0648");
            Add("world", "WORLD\nMAP", "\u062E\u0631\u064A\u0637\u0629\n\u0627\u0644\u0639\u0627\u0644\u0645", "\u062F\u0646\u06CC\u0627\n\u0646\u0642\u0634\u06C1");
            Add("daily", "DAILY\nCHALLENGE", "\u062A\u062D\u062F\u064A\n\u0627\u0644\u064A\u0648\u0645", "\u0622\u062C \u06A9\u0627\n\u0686\u06CC\u0644\u0646\u062C");
            Add("skins", "SKINS", "\u0627\u0644\u0623\u0634\u0643\u0627\u0644", "\u0634\u06A9\u0644");
            Add("paused", "GAME PAUSED", "\u0627\u0644\u0644\u0639\u0628\u0629 \u0645\u062A\u0648\u0642\u0641\u0629", "\u06A9\u06BE\u06CC\u0644 \u0631\u06A9\u0627");
            Add("restartAsk", "RESTART?", "\u0625\u0639\u0627\u062F\u0629\u061F", "\u062F\u0648\u0628\u0627\u0631\u06C1\u061F");
            Add("menuAsk", "MAIN MENU?", "\u0627\u0644\u0642\u0627\u0626\u0645\u0629\u061F", "\u0645\u06CC\u0646\u0648\u061F");
            Add("restartBody", "Restart this game?", "\u0625\u0639\u0627\u062F\u0629 \u0647\u0630\u0647 \u0627\u0644\u0644\u0639\u0628\u0629\u061F", "\u06CC\u06C1 \u06A9\u06BE\u06CC\u0644 \u062F\u0648\u0628\u0627\u0631\u06C1\u061F");
            Add("leaveBody", "Leave this game and return to Main Menu?", "\u0627\u0644\u062E\u0631\u0648\u062C \u0625\u0644\u0649 \u0627\u0644\u0642\u0627\u0626\u0645\u0629\u061F", "\u0645\u06CC\u0646\u0648 \u067E\u0631 \u0648\u0627\u067E\u0633\u061F");
            Add("settingsNote", "Saved on this device. Extra themes are coming soon and are not for sale.", "\u0645\u062D\u0641\u0648\u0638 \u0639\u0644\u0649 \u0647\u0630\u0627 \u0627\u0644\u062C\u0647\u0627\u0632\u06D4 \u0627\u0644\u062B\u064A\u0645\u0627\u062A \u0627\u0644\u0625\u0636\u0627\u0641\u064A\u0629 \u0642\u0627\u062F\u0645\u0629 \u0648\u0644\u064A\u0633\u062A \u0644\u0644\u0628\u064A\u0639\u06D4", "\u0627\u0633 \u0688\u06CC\u0648\u0627\u0626\u0633 \u067E\u0631 \u0645\u062D\u0641\u0648\u0638\u06D4 \u0645\u0632\u06CC\u062F \u062A\u06BE\u06CC\u0645\u0632 \u062C\u0644\u062F \u0622\u0626\u06CC\u06BA \u06AF\u06CC \u0627\u0648\u0631 \u0641\u0631\u0648\u062E\u062A \u0646\u06C1\u06CC\u06BA\u06D4");
            Add("newBest", "NEW BEST", "\u0631\u0642\u0645 \u062C\u062F\u064A\u062F", "\u0646\u06CC\u0627 \u0628\u06C1\u062A\u0631\u06CC\u0646");
            Add("call.clear", "CLEAR!", "\u0645\u0633\u062D", "\u06A9\u0644\u06CC\u0626\u0631");
            Add("call.double", "DOUBLE!", "\u0645\u0632\u062F\u0648\u062C", "\u062F\u06AF\u0646\u0627");
            Add("call.amazing", "AMAZING!", "\u0645\u0630\u0647\u0644", "\u0634\u0627\u0646\u062F\u0627\u0631");
            Add("call.tetris", "TETRIS!", "\u062A\u062A\u0631\u0633", "\u0679\u06CC\u0679\u0631\u0633");
            Add("call.combo", "COMBO x", "\u0633\u0644\u0633\u0644\u0629", "\u06A9\u0648\u0645\u0628\u0648");
            Add("stat.bill", "ADS", "\u0627\u0644\u0625\u0639\u0644\u0627\u0646\u0627\u062A", "\u0627\u0634\u062A\u06C1\u0627\u0631\u0627\u062A");
            Add("stat.best", "Best score", "\u0623\u0641\u0636\u0644 \u0646\u062A\u064A\u062C\u0629", "\u0628\u06C1\u062A\u0631\u06CC\u0646 \u0627\u0633\u06A9\u0648\u0631");
            Add("stat.games", "Games", "\u0627\u0644\u0623\u0644\u0639\u0627\u0628", "\u06A9\u06BE\u06CC\u0644");
            Add("stat.lines", "Lines cleared", "\u0627\u0644\u0623\u0633\u0637\u0631 \u0627\u0644\u0645\u0645\u0633\u0648\u062D\u0629", "\u0645\u0679\u06CC \u0644\u0627\u0626\u0646\u06CC\u06BA");
            Add("stat.level", "Highest level", "\u0623\u0639\u0644\u0649 \u0645\u0633\u062A\u0648\u0649", "\u0627\u0639\u0644\u06CC \u0633\u0637\u062D");
            Add("stat.tetris", "Four-line clears", "\u0645\u0633\u062D \u0623\u0631\u0628\u0639\u0629", "\u0686\u0627\u0631 \u0644\u0627\u0626\u0646\u06CC\u06BA");
            Add("stat.combo", "Longest combo", "\u0623\u0637\u0648\u0644 \u0633\u0644\u0633\u0644\u0629", "\u0644\u0645\u0628\u06CC \u06A9\u0648\u0645\u0628\u0648");
            Add("stat.time", "Playtime", "\u0648\u0642\u062A \u0627\u0644\u0644\u0639\u0628", "\u0648\u0642\u062A");
            Add("min", "min", "\u062F", "\u0645\u0646\u0679");
            Add("theme.current", "CURRENT THEME  COASTAL", "\u0627\u0644\u062B\u064A\u0645 \u0627\u0644\u062D\u0627\u0644\u064A \u0633\u0627\u062D\u0644\u064A", "\u0645\u0648\u062C\u0648\u062F\u06C1 \u062A\u06BE\u06CC\u0645 \u0633\u0627\u062D\u0644");
            Add("theme.selected", "SELECTED", "\u0645\u062E\u062A\u0627\u0631", "\u0645\u0646\u062A\u062E\u0628");
            Add("theme.soon", "LOCKED / COMING SOON", "\u0645\u0642\u0641\u0644\u060C \u0642\u0631\u064A\u0628\u0627", "\u0645\u0642\u0641\u0644\u060C \u062C\u0644\u062F");
            Add("theme.harbor", "Harbor", "\u0627\u0644\u0645\u064A\u0646\u0627\u0621", "\u0628\u0646\u062F\u0631\u06AF\u0627\u06C1");
            Add("theme.night", "Night", "\u0627\u0644\u0644\u064A\u0644", "\u0631\u0627\u062A");
            Add("theme.note", "More themes are locked. No purchases on this build.", "\u0627\u0644\u062B\u064A\u0645\u0627\u062A \u0627\u0644\u0623\u062E\u0631\u0649 \u0645\u0642\u0641\u0644\u0629\u06D4 \u0644\u0627 \u0634\u0631\u0627\u0621 \u0641\u064A \u0647\u0630\u0647 \u0627\u0644\u0646\u0633\u062E\u0629\u06D4", "\u0628\u0627\u0642\u06CC \u062A\u06BE\u06CC\u0645\u0632 \u0645\u0642\u0641\u0644 \u06C1\u06CC\u06BA\u06D4 \u0627\u0633 \u0648\u0631\u0698\u0646 \u0645\u06CC\u06BA \u062E\u0631\u06CC\u062F \u0646\u06C1\u06CC\u06BA\u06D4");
            Add("ach.First game", "First game", "\u0623\u0648\u0644 \u0644\u0639\u0628\u0629", "\u067E\u06C1\u0644\u0627 \u06A9\u06BE\u06CC\u0644");
            Add("ach.Ten games", "Ten games", "\u0639\u0634\u0631 \u0644\u0639\u0628\u0627\u062A", "\u062F\u0633 \u06A9\u06BE\u06CC\u0644");
            Add("ach.Line clearer", "Line clearer", "\u0645\u0627\u0633\u062D \u0627\u0644\u0623\u0633\u0637\u0631", "\u0644\u0627\u0626\u0646 \u0635\u0627\u0641");
            Add("ach.Century", "Century", "\u0645\u0626\u0629 \u0633\u0637\u0631", "\u0633\u0648 \u0644\u0627\u0626\u0646\u06CC\u06BA");
            Add("ach.Tetris", "Tetris", "\u062A\u062A\u0631\u0633", "\u0679\u06CC\u0679\u0631\u0633");
            Add("ach.Combo", "Combo", "\u0633\u0644\u0633\u0644\u0629", "\u06A9\u0648\u0645\u0628\u0648");
            Add("ach.Level 5", "Level 5", "\u0627\u0644\u0645\u0633\u062A\u0648\u0649 5", "\u0633\u0637\u062D 5");
            Add("ach.High score", "High score", "\u0646\u062A\u064A\u062C\u0629 \u0639\u0627\u0644\u064A\u0629", "\u0628\u0691\u0627 \u0627\u0633\u06A9\u0648\u0631");
            Add("achd.First game", "Start a run", "\u0627\u0628\u062F\u0623 \u062C\u0648\u0644\u0629", "\u0627\u06CC\u06A9 \u0631\u0646");
            Add("achd.Ten games", "Start 10 runs", "\u0627\u0628\u062F\u0623 10", "\u062F\u0633 \u0631\u0646");
            Add("achd.Line clearer", "Clear 10 lines", "\u0627\u0645\u0633\u062D 10", "\u062F\u0633 \u0644\u0627\u0626\u0646\u06CC\u06BA");
            Add("achd.Century", "Clear 100 lines", "\u0627\u0645\u0633\u062D 100", "\u0633\u0648 \u0644\u0627\u0626\u0646\u06CC\u06BA");
            Add("achd.Tetris", "Clear four lines at once", "\u0623\u0631\u0628\u0639\u0629 \u0645\u0639\u0627", "\u0686\u0627\u0631 \u0627\u06A9\u0679\u06BE\u06CC");
            Add("achd.Combo", "Reach a x3 combo", "\u0633\u0644\u0633\u0644\u0629 3", "\u06A9\u0648\u0645\u0628\u0648 3");
            Add("achd.Level 5", "Reach displayed level 5", "\u0628\u0644\u063A 5", "\u0633\u0637\u062D 5");
            Add("achd.High score", "Reach a best of 1000", "\u0628\u0644\u063A 1000", "\u0627\u0633\u06A9\u0648\u0631 1000");
            Add("theme.coastal", "Coastal", "\u0627\u0644\u0633\u0627\u062D\u0644\u064A", "\u0633\u0627\u062D\u0644");
            Add("theme.free", "Three free themes. No purchases.", "\u062B\u0644\u0627\u062B\u0629 \u062B\u064A\u0645\u0627\u062A \u0645\u062C\u0627\u0646\u064A\u0629\u06D4 \u0644\u0627 \u0634\u0631\u0627\u0621\u06D4", "\u062A\u06CC\u0646 \u0645\u0641\u062A \u062A\u06BE\u06CC\u0645\u0632\u06D4 \u062E\u0631\u06CC\u062F \u0646\u06C1\u06CC\u06BA\u06D4");
            Add("title.daily", "DAILY", "\u062A\u062D\u062F\u064A \u0627\u0644\u064A\u0648\u0645", "\u0622\u062C \u06A9\u0627 \u0686\u06CC\u0644\u0646\u062C");
            Add("title.skins", "SKINS", "\u0627\u0644\u0623\u0634\u0643\u0627\u0644", "\u0634\u06A9\u0644");
            Add("daily.goal", "Clear 20 lines today", "\u0627\u0645\u0633\u062D 20 \u0633\u0637\u0631\u0627 \u0627\u0644\u064A\u0648\u0645", "\u0622\u062C 20 \u0644\u0627\u0626\u0646\u06CC\u06BA \u0645\u0679\u0627\u0624");
            Add("daily.streak", "Streak", "\u0627\u0644\u0633\u0644\u0633\u0644\u0629", "\u0633\u0644\u0633\u0644\u06C1");
            Add("daily.today", "Today", "\u0627\u0644\u064A\u0648\u0645", "\u0622\u062C");
            Add("daily.done", "Done for today", "\u0627\u0643\u062A\u0645\u0644 \u0627\u0644\u064A\u0648\u0645", "\u0622\u062C \u0645\u06A9\u0645\u0644");
            Add("daily.ready", "Not finished yet", "\u0644\u0645 \u064A\u0643\u062A\u0645\u0644 \u0628\u0639\u062F", "\u0627\u0628\u06BE\u06CC \u0628\u0627\u0642\u06CC");
            Add("dailyDone", "DAILY CLEAR", "\u0627\u0643\u062A\u0645\u0644 \u062A\u062D\u062F\u064A \u0627\u0644\u064A\u0648\u0645", "\u0622\u062C \u06A9\u0627 \u0686\u06CC\u0644\u0646\u062C \u0645\u06A9\u0645\u0644");
            Add("skin.solid", "Solid", "\u0635\u0644\u0628", "\u0679\u06BE\u0648\u0633");
            Add("skin.soft", "Soft", "\u0646\u0627\u0639\u0645", "\u0646\u0631\u0645");
            Add("skin.frame", "Framed", "\u0625\u0637\u0627\u0631", "\u0641\u0631\u06CC\u0645");
            Add("skin.note", "Three free styles. Same pieces and scores.", "\u062B\u0644\u0627\u062B\u0629 \u0623\u0634\u0643\u0627\u0644 \u0645\u062C\u0627\u0646\u064A\u0629\u06D4 \u0646\u0641\u0633 \u0627\u0644\u0642\u0637\u0639 \u0648\u0627\u0644\u0646\u0642\u0627\u0637\u06D4", "\u062A\u06CC\u0646 \u0645\u0641\u062A \u0627\u0646\u062F\u0627\u0632\u06D4 \u0648\u06C1\u06CC \u0645\u06C1\u0631\u06D2 \u0627\u0648\u0631 \u0627\u0633\u06A9\u0648\u0631\u06D4");

            Glyph('\u0621', '\uFE80', '\uFE80', '\uFE80', '\uFE80', false);
            Glyph('\u0622', '\uFE81', '\uFE82', '\uFE81', '\uFE82', false);
            Glyph('\u0623', '\uFE83', '\uFE84', '\uFE83', '\uFE84', false);
            Glyph('\u0624', '\uFE85', '\uFE86', '\uFE85', '\uFE86', false);
            Glyph('\u0625', '\uFE87', '\uFE88', '\uFE87', '\uFE88', false);
            Glyph('\u0626', '\uFE89', '\uFE8A', '\uFE8B', '\uFE8C', true);
            Glyph('\u0627', '\uFE8D', '\uFE8E', '\uFE8D', '\uFE8E', false);
            Glyph('\u0628', '\uFE8F', '\uFE90', '\uFE91', '\uFE92', true);
            Glyph('\u0629', '\uFE93', '\uFE94', '\uFE93', '\uFE94', false);
            Glyph('\u062A', '\uFE95', '\uFE96', '\uFE97', '\uFE98', true);
            Glyph('\u062B', '\uFE99', '\uFE9A', '\uFE9B', '\uFE9C', true);
            Glyph('\u062C', '\uFE9D', '\uFE9E', '\uFE9F', '\uFEA0', true);
            Glyph('\u062D', '\uFEA1', '\uFEA2', '\uFEA3', '\uFEA4', true);
            Glyph('\u062E', '\uFEA5', '\uFEA6', '\uFEA7', '\uFEA8', true);
            Glyph('\u062F', '\uFEA9', '\uFEAA', '\uFEA9', '\uFEAA', false);
            Glyph('\u0630', '\uFEAB', '\uFEAC', '\uFEAB', '\uFEAC', false);
            Glyph('\u0631', '\uFEAD', '\uFEAE', '\uFEAD', '\uFEAE', false);
            Glyph('\u0632', '\uFEAF', '\uFEB0', '\uFEAF', '\uFEB0', false);
            Glyph('\u0633', '\uFEB1', '\uFEB2', '\uFEB3', '\uFEB4', true);
            Glyph('\u0634', '\uFEB5', '\uFEB6', '\uFEB7', '\uFEB8', true);
            Glyph('\u0635', '\uFEB9', '\uFEBA', '\uFEBB', '\uFEBC', true);
            Glyph('\u0636', '\uFEBD', '\uFEBE', '\uFEBF', '\uFEC0', true);
            Glyph('\u0637', '\uFEC1', '\uFEC2', '\uFEC3', '\uFEC4', true);
            Glyph('\u0638', '\uFEC5', '\uFEC6', '\uFEC7', '\uFEC8', true);
            Glyph('\u0639', '\uFEC9', '\uFECA', '\uFECB', '\uFECC', true);
            Glyph('\u063A', '\uFECD', '\uFECE', '\uFECF', '\uFED0', true);
            Glyph('\u0641', '\uFED1', '\uFED2', '\uFED3', '\uFED4', true);
            Glyph('\u0642', '\uFED5', '\uFED6', '\uFED7', '\uFED8', true);
            Glyph('\u0643', '\uFED9', '\uFEDA', '\uFEDB', '\uFEDC', true);
            Glyph('\u0644', '\uFEDD', '\uFEDE', '\uFEDF', '\uFEE0', true);
            Glyph('\u0645', '\uFEE1', '\uFEE2', '\uFEE3', '\uFEE4', true);
            Glyph('\u0646', '\uFEE5', '\uFEE6', '\uFEE7', '\uFEE8', true);
            Glyph('\u0647', '\uFEE9', '\uFEEA', '\uFEEB', '\uFEEC', true);
            Glyph('\u0648', '\uFEED', '\uFEEE', '\uFEED', '\uFEEE', false);
            Glyph('\u0649', '\uFEEF', '\uFEF0', '\uFEEF', '\uFEF0', false);
            Glyph('\u064A', '\uFEF1', '\uFEF2', '\uFEF3', '\uFEF4', true);
            Glyph('\u0679', '\uFB66', '\uFB67', '\uFB68', '\uFB69', true);
            Glyph('\u067E', '\uFB56', '\uFB57', '\uFB58', '\uFB59', true);
            Glyph('\u0686', '\uFB7A', '\uFB7B', '\uFB7C', '\uFB7D', true);
            Glyph('\u0688', '\uFB88', '\uFB89', '\uFB88', '\uFB89', false);
            Glyph('\u0691', '\uFB8C', '\uFB8D', '\uFB8C', '\uFB8D', false);
            Glyph('\u0698', '\uFB8A', '\uFB8B', '\uFB8A', '\uFB8B', false);
            Glyph('\u06A9', '\uFB8E', '\uFB8F', '\uFB90', '\uFB91', true);
            Glyph('\u06AF', '\uFB92', '\uFB93', '\uFB94', '\uFB95', true);
            Glyph('\u06BA', '\uFB9E', '\uFB9F', '\uFB9E', '\uFB9F', false);
            Glyph('\u06BE', '\uFBAA', '\uFBAB', '\uFBAC', '\uFBAD', true);
            Glyph('\u06C1', '\uFBA6', '\uFBA7', '\uFBA8', '\uFBA9', true);
            Glyph('\u06CC', '\uFBFC', '\uFBFD', '\uFBFE', '\uFBFF', true);
            Glyph('\u06D2', '\uFBAE', '\uFBAF', '\uFBAE', '\uFBAF', false);
        }

        public static bool RightToLeft => LocalProgressService.Language != "en";

        public static Font Font => RightToLeft ? ArabicFont : LatinFont;

        public static Font ArabicFont
        {
            get
            {
                if (arabicFont == null) arabicFont = Resources.Load<Font>("Fonts/NotoNaskhArabic-Regular");
                return arabicFont != null ? arabicFont : LatinFont;
            }
        }

        public static Font LatinFont
        {
            get
            {
                if (latinFont == null) latinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (latinFont == null) latinFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                return latinFont;
            }
        }

        public static string Lookup(string key)
        {
            if (key != null && Text.TryGetValue(key, out var values)) return values[Column];
            return key ?? "";
        }

        public static string Line(string key) => Shape(Lookup(key));

        public static string Pair(string key, string value) => Shape(Lookup(key) + "  " + value);

        public static string Digits(string value) => MapDigits(value);

        public static string Achievement(AchievementState item)
        {
            var state = Lookup(item.Unlocked ? "unlocked" : "locked");
            return Shape(state + "  " + Lookup("ach." + item.Title) + "  " + Lookup("achd." + item.Title));
        }

        public static string ThemeLine(string nameKey, bool locked)
        {
            return Shape(Lookup(nameKey) + "    " + Lookup(locked ? "theme.soon" : "theme.selected"));
        }

        public static string Display(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return "";
            if (raw == "NEW BEST") return Line("newBest");
            if (raw.StartsWith("LEVEL ")) return Shape(Lookup("level") + " " + raw.Substring(6));
            var comboAt = raw.IndexOf("  COMBO x", System.StringComparison.Ordinal);
            var word = comboAt >= 0 ? raw.Substring(0, comboAt) : raw;
            var key = word == "TETRIS!" ? "call.tetris" : word == "AMAZING!" ? "call.amazing" : word == "DOUBLE!" ? "call.double" : word == "CLEAR!" ? "call.clear" : null;
            if (key == null) return raw;
            var logical = Lookup(key);
            if (comboAt >= 0) logical += "  " + Lookup("call.combo") + " " + raw.Substring(comboAt + "  COMBO x".Length);
            return Shape(logical);
        }

        public static bool HasArabic(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c >= '\u0600' && c <= '\u06FF') return true;
            }
            return false;
        }

        public static string Shape(string value)
        {
            if (string.IsNullOrEmpty(value) || !HasArabic(value)) return value;
            var lines = value.Split('\n');
            for (var i = 0; i < lines.Length; i++) lines[i] = ShapeLine(MapDigits(lines[i]));
            return string.Join("\n", lines);
        }

        public static void Refresh()
        {
            var texts = Object.FindObjectsByType<TranslatedText>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < texts.Length; i++) texts[i].Apply();
            var choices = Object.FindObjectsByType<LanguageChoice>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < choices.Length; i++) choices[i].Apply();
        }

        static int Column => LocalProgressService.Language == "en" ? 0 : LocalProgressService.Language == "ur" ? 2 : 1;

        static void Add(string key, string english, string arabic, string urdu)
        {
            Text[key] = new[] { english, arabic, urdu };
        }

        static string MapDigits(string value)
        {
            if (!RightToLeft || string.IsNullOrEmpty(value)) return value;
            var zero = LocalProgressService.Language == "ur" ? '\u06F0' : '\u0660';
            var chars = value.ToCharArray();
            var changed = false;
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] < '0' || chars[i] > '9') continue;
                chars[i] = (char)(zero + (chars[i] - '0'));
                changed = true;
            }
            return changed ? new string(chars) : value;
        }

        static string ShapeLine(string line)
        {
            var visual = new StringBuilder(line.Length);
            for (var i = 0; i < line.Length; i++)
            {
                var current = line[i];
                if (current == '\u0644' && i + 1 < line.Length && IsAlef(line[i + 1]))
                {
                    var linked = i > 0 && JoinsForward(line[i - 1]);
                    visual.Append(LamAlef(line[i + 1], linked));
                    i++;
                    continue;
                }
                if (!Glyphs.TryGetValue(current, out var glyph))
                {
                    visual.Append(current);
                    continue;
                }
                var before = i > 0 && JoinsForward(line[i - 1]);
                var after = glyph.Dual && i + 1 < line.Length && Glyphs.ContainsKey(line[i + 1]);
                visual.Append(before && after ? glyph.Medial : before ? glyph.Final : after ? glyph.Initial : glyph.Isolated);
            }
            return ReverseVisual(visual.ToString());
        }

        static string ReverseVisual(string shaped)
        {
            var chars = shaped.ToCharArray();
            System.Array.Reverse(chars);
            var i = 0;
            while (i < chars.Length)
            {
                if (!IsNumber(chars[i]))
                {
                    i++;
                    continue;
                }
                var end = i;
                while (end < chars.Length && IsNumber(chars[end])) end++;
                System.Array.Reverse(chars, i, end - i);
                i = end;
            }
            return new string(chars);
        }

        static bool IsNumber(char c)
        {
            return (c >= '0' && c <= '9') || (c >= '\u0660' && c <= '\u0669') || (c >= '\u06F0' && c <= '\u06F9');
        }

        static bool IsAlef(char c) => c == '\u0622' || c == '\u0623' || c == '\u0625' || c == '\u0627';

        static bool JoinsForward(char c) => Glyphs.TryGetValue(c, out var glyph) && glyph.Dual;

        static char LamAlef(char alef, bool linked)
        {
            if (alef == '\u0622') return linked ? '\uFEF6' : '\uFEF5';
            if (alef == '\u0623') return linked ? '\uFEF8' : '\uFEF7';
            if (alef == '\u0625') return linked ? '\uFEFA' : '\uFEF9';
            return linked ? '\uFEFC' : '\uFEFB';
        }

        static void Glyph(char letter, char isolated, char final, char initial, char medial, bool dual)
        {
            Glyphs[letter] = new LetterForm { Isolated = isolated, Final = final, Initial = initial, Medial = medial, Dual = dual };
        }

        struct LetterForm
        {
            public char Isolated;
            public char Final;
            public char Initial;
            public char Medial;
            public bool Dual;
        }
    }

    public sealed class TranslatedText : MonoBehaviour
    {
        public string Key;
        public int Align;
        Text label;

        public void Bind(string key, int align = 0)
        {
            Key = key;
            Align = align;
            Apply();
        }

        public void Apply()
        {
            if (label == null) label = GetComponent<Text>();
            if (label == null || string.IsNullOrEmpty(Key)) return;
            label.font = UiLanguage.Font;
            label.text = UiLanguage.Line(Key);
            if (Align == 1) label.alignment = UiLanguage.RightToLeft ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
            else if (Align == 2) label.alignment = UiLanguage.RightToLeft ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
        }
    }

    public sealed class LanguageChoice : MonoBehaviour
    {
        public string Code;
        public string Native;
        Text label;

        public void Bind(string code, string native, Text text)
        {
            Code = code;
            Native = native;
            label = text;
            Apply();
        }

        public void Apply()
        {
            if (label == null) label = GetComponentInChildren<Text>();
            if (label == null) return;
            var arabic = UiLanguage.HasArabic(Native);
            label.font = arabic ? UiLanguage.ArabicFont : UiLanguage.LatinFont;
            label.text = arabic ? UiLanguage.Shape(Native) : Native;
            var image = GetComponent<Image>();
            if (image == null) return;
            image.color = LocalProgressService.Language == Code
                ? new Color(0.12f, 0.46f, 0.3f, 1f)
                : new Color(0.05f, 0.16f, 0.32f, 1f);
        }
    }
}
