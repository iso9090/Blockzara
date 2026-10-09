using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blockzara.Runtime
{
    public sealed class BlockzaraMenuV5 : MonoBehaviour
    {
        Canvas canvas;
        BlockzaraApp app;

        public void Build(BlockzaraApp host)
        {
            app = host;
            var root = new GameObject("Main Menu V5", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(720f, 1520f);
            scaler.matchWidthOrHeight = 0.5f;
            root.AddComponent<GraphicRaycaster>();
            Stretch(root.GetComponent<RectTransform>());

            var events = new GameObject("Menu EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            events.transform.SetParent(transform, false);

            var background = CreateChild(root.transform, "Scenic Background");
            Stretch(background);
            var photo = background.gameObject.AddComponent<RawImage>();
            photo.texture = Resources.Load<Texture2D>("scenic-background-v1");
            photo.raycastTarget = false;
            background.gameObject.AddComponent<ScenicCover>();
            var wash = CreateChild(root.transform, "Theme Wash");
            Stretch(wash);
            var washImage = wash.gameObject.AddComponent<Image>();
            washImage.raycastTarget = false;
            washImage.color = BoardMood.MenuWash(LocalProgressService.Theme);
            wash.gameObject.AddComponent<ThemeWash>();

            var column = CreateChild(root.transform, "Menu Column");
            var columnRect = column;
            columnRect.anchorMin = new Vector2(0.5f, 1f);
            columnRect.anchorMax = new Vector2(0.5f, 1f);
            columnRect.pivot = new Vector2(0.5f, 1f);
            columnRect.anchoredPosition = new Vector2(0f, -36f);
            columnRect.sizeDelta = new Vector2(648f, 820f);
            var stack = column.gameObject.AddComponent<VerticalLayoutGroup>();
            stack.spacing = 16f;
            stack.padding = new RectOffset(0, 0, 8, 0);
            stack.childAlignment = TextAnchor.UpperCenter;
            stack.childControlWidth = true;
            stack.childControlHeight = true;
            stack.childForceExpandWidth = true;
            stack.childForceExpandHeight = false;

            var logo = CreateSprite(column, "Logo", Load("logo"), Image.Type.Simple);
            logo.preserveAspect = true;
            logo.raycastTarget = false;
            Preferred(logo.rectTransform, 230f);

            var plaque = CreateSprite(column, "Tagline", Load("plaque"), Image.Type.Sliced);
            plaque.raycastTarget = false;
            Preferred(plaque.rectTransform, 78f);
            var tagline = AddLabel(plaque.transform, "PLACE • CLEAR • EXPLORE", 24, FontStyle.Bold, new Color(1f, 0.93f, 0.72f), "tagline");
            Stretch(tagline.rectTransform);

            var playBackground = Load("play-button");
            var playTriangle = Load("icon-play");
            var playSlot = CreateChild(column, "Play Slot");
            Preferred(playSlot, 200f);
            var slotRow = playSlot.gameObject.AddComponent<HorizontalLayoutGroup>();
            slotRow.childAlignment = TextAnchor.MiddleCenter;
            slotRow.childControlWidth = false;
            slotRow.childControlHeight = false;
            slotRow.childForceExpandWidth = false;
            slotRow.childForceExpandHeight = false;
            var play = CreateButton(playSlot, "PLAY", playBackground, playTriangle, false, 192f, 28);
            var playImage = play.GetComponent<Image>();
            playImage.type = Image.Type.Simple;
            playImage.preserveAspect = false;
            var playElement = play.GetComponent<LayoutElement>();
            playElement.ignoreLayout = true;
            playElement.preferredWidth = 192f;
            playElement.preferredHeight = 192f;
            playElement.minWidth = 192f;
            playElement.minHeight = 192f;
            playElement.flexibleWidth = 0f;
            var playRect = play.GetComponent<RectTransform>();
            playRect.anchorMin = new Vector2(0.5f, 0.5f);
            playRect.anchorMax = new Vector2(0.5f, 0.5f);
            playRect.pivot = new Vector2(0.5f, 0.5f);
            playRect.anchoredPosition = Vector2.zero;
            playRect.sizeDelta = new Vector2(192f, 192f);
            var playRow = play.GetComponent<HorizontalLayoutGroup>();
            if (playRow != null) DestroyImmediate(playRow);
            for (var childIndex = play.transform.childCount - 1; childIndex >= 0; childIndex--)
            {
                var child = play.transform.GetChild(childIndex);
                if (child.name == "Spacer") DestroyImmediate(child.gameObject);
            }
            var playColumn = play.gameObject.AddComponent<VerticalLayoutGroup>();
            playColumn.padding = new RectOffset(8, 8, 22, 14);
            playColumn.spacing = 2f;
            playColumn.childAlignment = TextAnchor.MiddleCenter;
            playColumn.childControlWidth = true;
            playColumn.childControlHeight = true;
            playColumn.childForceExpandWidth = true;
            playColumn.childForceExpandHeight = false;
            var playIcon = play.transform.Find("Icon").GetComponent<Image>();
            playIcon.sprite = playTriangle;
            playIcon.color = Color.white;
            playIcon.preserveAspect = true;
            var playIconLayout = playIcon.GetComponent<LayoutElement>();
            playIconLayout.preferredWidth = 64f;
            playIconLayout.preferredHeight = 64f;
            var playLabel = play.transform.Find("PLAY").GetComponent<Text>();
            playLabel.fontSize = 26;
            playLabel.alignment = TextAnchor.MiddleCenter;
            playLabel.gameObject.AddComponent<TranslatedText>().Bind("play");
            var playGlass = play.gameObject.AddComponent<CanvasGroup>();
            playGlass.alpha = 0.72f;
            playGlass.interactable = true;
            playGlass.blocksRaycasts = true;
            play.gameObject.AddComponent<PlayPressFeedback>();
            play.onClick.AddListener(() =>
            {
                GameAudio.Play("click");
                app.StartFromMenu();
            });
            Debug.Log("BZ menu play background=" + (playBackground != null ? playBackground.name : "MISSING")
                + " icon=" + (playTriangle != null ? playTriangle.name : "MISSING"));
            AddFeatureRow(column);

            var navigation = CreateChild(root.transform, "Bottom Navigation");
            navigation.anchorMin = new Vector2(0f, 0f);
            navigation.anchorMax = new Vector2(1f, 0f);
            navigation.pivot = new Vector2(0.5f, 0f);
            navigation.anchoredPosition = new Vector2(0f, 22f);
            navigation.sizeDelta = new Vector2(-36f, 168f);
            var row = navigation.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 12f;
            row.childAlignment = TextAnchor.MiddleCenter;
            row.childControlWidth = true;
            row.childControlHeight = true;
            row.childForceExpandWidth = true;
            row.childForceExpandHeight = true;
            AddNavTile(navigation, "Settings", Load("icon-settings"), () => ShowPanel(root.transform, "Settings"));
            AddNavTile(navigation, "Stats", Load("icon-stats"), () => ShowPanel(root.transform, "Stats"));
            AddNavTile(navigation, "Achievements", Load("icon-achievements"), () => ShowPanel(root.transform, "Achievements"));
            AddNavTile(navigation, "Themes", Load("icon-themes"), () => ShowPanel(root.transform, "Themes"));
            root.gameObject.AddComponent<MenuSafeArea>().Bind(column, navigation);
        }

        void ShowPanel(Transform parent, string kind)
        {
            GameAudio.Play("click");
            var existing = parent.Find("Info Panel");
            if (existing != null) Destroy(existing.gameObject);
            if (kind == "Stats")
            {
                ShowStats(parent);
                return;
            }
            if (kind == "Settings" || kind == "Achievements" || kind == "Themes")
            {
                ShowCoastPanel(parent, kind);
                return;
            }
            var panel = CreateChild(parent, "Info Panel");
            Stretch(panel);
            var shade = panel.gameObject.AddComponent<Image>();
            shade.color = new Color(0.02f, 0.06f, 0.14f, 0.96f);
            var border = shade.gameObject.AddComponent<Outline>();
            border.effectColor = new Color(0.86f, 0.7f, 0.32f, 1f);
            border.effectDistance = new Vector2(3f, -3f);
            var layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(28, 28, 48, 170);
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var title = AddLabel(panel, kind.ToUpperInvariant(), 32, FontStyle.Bold, new Color(1f, 0.92f, 0.7f), "title." + kind.ToLowerInvariant());
            title.gameObject.AddComponent<LayoutElement>().preferredHeight = 48f;
            if (kind == "Settings") FillSettings(panel);
            else if (kind == "Themes") FillThemes(panel);
            else if (kind == "Daily") FillDaily(panel);
            else if (kind == "Skins") FillSkins(panel);
            else FillAchievements(panel);
            var back = CreateButton(panel, "BACK", null, null, false, 72f, 28);
            var backLabel = back.transform.Find("BACK");
            if (backLabel != null) backLabel.gameObject.AddComponent<TranslatedText>().Bind("back");
            back.onClick.AddListener(() => Destroy(panel.gameObject));
        }

        static void FillSettings(Transform panel)
        {
            AddToggle(panel, "Music", "music", LocalProgressService.MusicEnabled, value =>
            {
                LocalProgressService.MusicEnabled = value;
                GameAudio.ApplySettings();
            });
            AddToggle(panel, "Sound effects", "sfx", LocalProgressService.SfxEnabled, value => LocalProgressService.SfxEnabled = value);
            AddToggle(panel, "Vibration", "vibration", LocalProgressService.VibrationEnabled, value => LocalProgressService.VibrationEnabled = value);
            AddToggle(panel, "Reduced effects", "reduced", LocalProgressService.ReducedEffects, value => LocalProgressService.ReducedEffects = value);
            AddToggle(panel, "Shadow", "shadow", LocalProgressService.GhostEnabled, value => LocalProgressService.GhostEnabled = value);
            AddLanguage(panel);
            var note = AddLabel(panel, "Saved on this device. Extra themes are coming soon and are not for sale.", 16, FontStyle.Normal, new Color(0.85f, 0.9f, 1f), "settingsNote", 2);
            note.horizontalOverflow = HorizontalWrapMode.Wrap;
            note.gameObject.AddComponent<LayoutElement>().preferredHeight = 88f;
        }

        void ShowCoastPanel(Transform parent, string kind)
        {
            var bottom = kind == "Settings" ? 0.12f : kind == "Themes" ? 0.42f : 0.16f;
            var top = kind == "Settings" ? 0.88f : 0.84f;
            var card = OpenCoastCard(parent, "title." + kind.ToLowerInvariant(), bottom, top);
            if (kind == "Settings") FillSettings(card);
            else if (kind == "Themes") FillThemes(card);
            else FillAchievements(card);
            AddCoastBack(card);
        }

        RectTransform OpenCoastCard(Transform parent, string titleKey, float bottom, float top)
        {
            var dim = CreateChild(parent, "Info Panel");
            Stretch(dim);
            dim.gameObject.AddComponent<Image>().color = new Color(0.02f, 0.06f, 0.12f, 0.42f);
            var card = CreateChild(dim, "Card");
            card.anchorMin = new Vector2(0.07f, bottom);
            card.anchorMax = new Vector2(0.93f, top);
            card.offsetMin = Vector2.zero;
            card.offsetMax = Vector2.zero;
            card.gameObject.AddComponent<Image>().color = new Color(0.03f, 0.1f, 0.22f, 0.94f);
            var outline = card.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.86f, 0.72f, 0.34f, 1f);
            outline.effectDistance = new Vector2(4f, -4f);
            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(22, 22, 22, 18);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var title = AddLabel(card, titleKey, 30, FontStyle.Bold, new Color(1f, 0.92f, 0.7f), titleKey);
            title.alignment = TextAnchor.MiddleCenter;
            title.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;
            AddGoldLine(card);
            return card;
        }

        void AddCoastBack(RectTransform card)
        {
            var back = CreateButton(card, "BACK", null, null, false, 58f, 24);
            var backImage = back.GetComponent<Image>();
            if (backImage != null) backImage.color = new Color(0.1f, 0.32f, 0.5f, 1f);
            var backLabel = back.transform.Find("BACK");
            if (backLabel != null) backLabel.gameObject.AddComponent<TranslatedText>().Bind("back");
            var dim = card.parent.gameObject;
            back.onClick.AddListener(() => Destroy(dim));
        }

        void ShowStats(Transform parent)
        {
            var card = OpenCoastCard(parent, "title.stats", 0.2f, 0.8f);
            AddAdBill(card);
            var stats = LocalProgressService.LoadCareer();
            var minutes = Mathf.FloorToInt(stats.PlaytimeSeconds / 60f);
            AddStatRow(card, "stat.best", stats.Best.ToString());
            AddStatRow(card, "stat.games", stats.Games.ToString());
            AddStatRow(card, "stat.lines", stats.Lines.ToString());
            AddStatRow(card, "stat.level", stats.HighestLevel.ToString());
            AddStatRow(card, "stat.tetris", stats.Tetrises.ToString());
            AddStatRow(card, "stat.combo", stats.LongestCombo.ToString());
            AddStatRow(card, "stat.time", minutes.ToString(), "min");
            AddCoastBack(card);
        }

        static void AddGoldLine(Transform parent)
        {
            var line = CreateChild(parent, "Gold");
            line.gameObject.AddComponent<LayoutElement>().preferredHeight = 3f;
            line.gameObject.AddComponent<Image>().color = new Color(0.86f, 0.72f, 0.34f, 1f);
        }

        static void AddStatRow(Transform parent, string key, string value, string unitKey = null)
        {
            var row = CreateChild(parent, key);
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 46f;
            row.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.16f, 0.32f, 0.96f);
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 4, 4);
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.reverseArrangement = UiLanguage.RightToLeft;
            var label = AddLabel(row, key, 18, FontStyle.Bold, new Color(0.9f, 0.95f, 1f), key, 1);
            label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var number = AddLabel(row, value, 20, FontStyle.Bold, new Color(1f, 0.93f, 0.72f));
            number.font = UiLanguage.LatinFont;
            number.text = value;
            number.alignment = TextAnchor.MiddleCenter;
            number.gameObject.AddComponent<LayoutElement>().preferredWidth = 72f;
            if (unitKey == null) return;
            var unit = AddLabel(row, unitKey, 16, FontStyle.Bold, new Color(0.9f, 0.95f, 1f), unitKey, 1);
            unit.gameObject.AddComponent<LayoutElement>().preferredWidth = 36f;
        }

        static void AddAdBill(Transform panel)
        {
            var row = CreateChild(panel, "Ad Bill");
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 6, 6);
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.reverseArrangement = UiLanguage.RightToLeft;
            var icon = CreateChild(row, "Bill");
            var iconLayout = icon.gameObject.AddComponent<LayoutElement>();
            iconLayout.preferredWidth = 92f;
            iconLayout.preferredHeight = 50f;
            var image = icon.gameObject.AddComponent<Image>();
            image.sprite = BillSprite();
            image.preserveAspect = true;
            var amount = AddLabel(row, LocalProgressService.FormatAdBill(LocalProgressService.AdsWatched), 34, FontStyle.Bold, new Color(0.62f, 0.95f, 0.68f));
            amount.font = UiLanguage.LatinFont;
            amount.text = LocalProgressService.FormatAdBill(LocalProgressService.AdsWatched);
            amount.alignment = TextAnchor.MiddleCenter;
            amount.gameObject.AddComponent<LayoutElement>().preferredWidth = 120f;
        }

        static Sprite billSprite;

        static Sprite BillSprite()
        {
            if (billSprite != null) return billSprite;
            const int width = 96;
            const int height = 52;
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var paper = new Color(0.16f, 0.72f, 0.34f, 1f);
            var edge = new Color(0.06f, 0.42f, 0.18f, 1f);
            var ink = new Color(0.86f, 0.96f, 0.78f, 1f);
            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var border = x < 4 || y < 4 || x >= width - 4 || y >= height - 4;
                var frame = x < 8 || y < 8 || x >= width - 8 || y >= height - 8;
                var dx = x - width / 2f;
                var dy = (y - height / 2f) * 1.7f;
                var seal = dx * dx + dy * dy < 150f;
                tex.SetPixel(x, y, border || (frame && !seal) ? edge : seal ? ink : paper);
            }
            tex.Apply();
            billSprite = Sprite.Create(tex, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
            return billSprite;
        }

        static void FillAchievements(Transform panel)
        {
            foreach (var item in LocalProgressService.Achievements())
            {
                var row = CreateChild(panel, item.Title);
                row.gameObject.AddComponent<LayoutElement>().preferredHeight = 46f;
                row.gameObject.AddComponent<Image>().color = item.Unlocked
                    ? new Color(0.1f, 0.28f, 0.22f, 0.96f)
                    : new Color(0.05f, 0.16f, 0.32f, 0.96f);
                var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
                layout.padding = new RectOffset(16, 16, 4, 4);
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.childControlWidth = true;
                layout.childControlHeight = true;
                layout.childForceExpandWidth = true;
                layout.childForceExpandHeight = true;
                var label = AddLabel(row, UiLanguage.Achievement(item), 16, FontStyle.Bold, item.Unlocked ? new Color(0.78f, 0.95f, 0.72f) : new Color(0.82f, 0.88f, 0.96f));
                label.alignment = UiLanguage.RightToLeft ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
                label.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            }
        }

        static void AddLanguage(Transform panel)
        {
            var row = CreateChild(panel, "Language");
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 8, 8);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            var caption = AddLabel(row, "Language", 20, FontStyle.Bold, Color.white, "language", 1);
            caption.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1.1f;
            AddLanguageButton(row, "ar", "\u0627\u0644\u0639\u0631\u0628\u064A\u0629");
            AddLanguageButton(row, "en", "English");
            AddLanguageButton(row, "ur", "\u0627\u0631\u062F\u0648");
        }

        static void AddLanguageButton(Transform parent, string code, string native)
        {
            var image = CreateSprite(parent, code, null, Image.Type.Simple);
            image.color = new Color(0.08f, 0.16f, 0.32f, 1f);
            image.gameObject.AddComponent<LayoutElement>().preferredHeight = 52f;
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                GameAudio.Play("click");
                LocalProgressService.Language = code;
            });
            var text = AddLabel(image.transform, "Label", 18, FontStyle.Bold, Color.white);
            text.text = native;
            image.gameObject.AddComponent<LanguageChoice>().Bind(code, native, text);
        }

        static void AddToggle(Transform parent, string label, string key, bool value, UnityEngine.Events.UnityAction<bool> changed)
        {
            var row = CreateChild(parent, label);
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 64f;
            var background = row.gameObject.AddComponent<Image>();
            background.color = new Color(0.05f, 0.16f, 0.32f, 0.96f);
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(16, 16, 8, 8);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            var text = AddLabel(row, label, 22, FontStyle.Bold, Color.white, key, 1);
            text.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var toggleObject = new GameObject(label + " Toggle", typeof(RectTransform));
            toggleObject.transform.SetParent(row, false);
            var toggleLayout = toggleObject.AddComponent<LayoutElement>();
            toggleLayout.preferredWidth = 72f;
            toggleLayout.preferredHeight = 40f;
            var toggleImage = toggleObject.AddComponent<Image>();
            toggleImage.color = value ? new Color(0.15f, 0.55f, 0.32f) : new Color(0.25f, 0.28f, 0.34f);
            var toggle = toggleObject.AddComponent<Toggle>();
            toggle.targetGraphic = toggleImage;
            toggle.isOn = value;
            toggle.onValueChanged.AddListener(on =>
            {
                toggleImage.color = on ? new Color(0.15f, 0.55f, 0.32f) : new Color(0.25f, 0.28f, 0.34f);
                changed(on);
            });
        }

        public void Hide()
        {
            if (canvas != null) canvas.gameObject.SetActive(false);
        }

        public void Show()
        {
            if (canvas != null) canvas.gameObject.SetActive(true);
        }

        public bool TryClosePanel()
        {
            if (canvas == null) return false;
            var existing = canvas.transform.Find("Info Panel");
            if (existing == null) return false;
            Destroy(existing.gameObject);
            return true;
        }

        void FillThemes(Transform panel)
        {
            var previewObject = new GameObject("Theme Preview", typeof(RectTransform));
            previewObject.transform.SetParent(panel, false);
            previewObject.AddComponent<LayoutElement>().preferredHeight = 148f;
            var preview = previewObject.AddComponent<RawImage>();
            preview.texture = Resources.Load<Texture2D>("scenic-background-v1");
            preview.uvRect = new Rect(0.2f, 0.2f, 0.6f, 0.45f);
            var theme = LocalProgressService.Theme;
            preview.color = theme == "night" ? new Color(0.55f, 0.68f, 0.95f, 1f) : theme == "harbor" ? new Color(1f, 0.78f, 0.48f, 1f) : Color.white;
            preview.raycastTarget = false;
            AddChoice(panel, "coastal", "theme.coastal", LocalProgressService.Theme, value => LocalProgressService.Theme = value, "Themes");
            AddChoice(panel, "harbor", "theme.harbor", LocalProgressService.Theme, value => LocalProgressService.Theme = value, "Themes");
            AddChoice(panel, "night", "theme.night", LocalProgressService.Theme, value => LocalProgressService.Theme = value, "Themes");
            var note = AddLabel(panel, "Three free themes. No purchases.", 16, FontStyle.Normal, new Color(0.85f, 0.9f, 1f), "theme.free", 2);
            note.horizontalOverflow = HorizontalWrapMode.Wrap;
            note.gameObject.AddComponent<LayoutElement>().preferredHeight = 48f;
        }

        void FillDaily(Transform panel)
        {
            var goal = AddLabel(panel, "Clear 20 lines today", 22, FontStyle.Bold, Color.white, "daily.goal", 2);
            goal.horizontalOverflow = HorizontalWrapMode.Wrap;
            goal.gameObject.AddComponent<LayoutElement>().preferredHeight = 64f;
            AddNumberLine(panel, "daily.streak", LocalProgressService.DailyStreak);
            AddNumberLine(panel, "daily.today", LocalProgressService.DailyBest);
            var state = AddLabel(panel, LocalProgressService.DailyDone ? "Done for today" : "Not finished yet", 20, FontStyle.Bold, new Color(0.75f, 1f, 0.7f), LocalProgressService.DailyDone ? "daily.done" : "daily.ready");
            state.gameObject.AddComponent<LayoutElement>().preferredHeight = 36f;
            var play = CreateButton((RectTransform)panel, "PLAY", null, null, false, 72f, 28);
            var playLabel = play.transform.Find("PLAY");
            if (playLabel != null) playLabel.gameObject.AddComponent<TranslatedText>().Bind("play");
            play.onClick.AddListener(() => app.StartDaily());
        }

        void AddNumberLine(Transform panel, string key, int value)
        {
            var row = CreateChild(panel, key);
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(28, 28, 0, 0);
            layout.spacing = 16f;
            layout.childAlignment = UiLanguage.RightToLeft ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.reverseArrangement = UiLanguage.RightToLeft;
            var label = AddLabel(row, key, 22, FontStyle.Bold, new Color(1f, 0.93f, 0.72f), key, 1);
            label.gameObject.AddComponent<LayoutElement>().preferredWidth = 220f;
            var number = AddLabel(row, value.ToString(), 22, FontStyle.Bold, Color.white);
            number.font = UiLanguage.LatinFont;
            number.text = value.ToString();
            number.gameObject.AddComponent<LayoutElement>().preferredWidth = 72f;
        }

        void FillSkins(Transform panel)
        {
            AddChoice(panel, "solid", "skin.solid", LocalProgressService.Skin, value => LocalProgressService.Skin = value, "Skins");
            AddChoice(panel, "soft", "skin.soft", LocalProgressService.Skin, value => LocalProgressService.Skin = value, "Skins");
            AddChoice(panel, "frame", "skin.frame", LocalProgressService.Skin, value => LocalProgressService.Skin = value, "Skins");
            var note = AddLabel(panel, "Three free styles. Same pieces and scores.", 16, FontStyle.Normal, new Color(0.85f, 0.9f, 1f), "skin.note", 2);
            note.horizontalOverflow = HorizontalWrapMode.Wrap;
            note.gameObject.AddComponent<LayoutElement>().preferredHeight = 64f;
        }

        static Transform PanelHost(Transform from)
        {
            var node = from;
            while (node != null && node.name != "Info Panel") node = node.parent;
            return node != null && node.parent != null ? node.parent : from;
        }

        void AddChoice(Transform panel, string code, string key, string current, System.Action<string> choose, string reopen)
        {
            var selected = current == code;
            var row = CreateChild(panel, code);
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 64f;
            var image = row.gameObject.AddComponent<Image>();
            image.color = selected ? new Color(0.12f, 0.46f, 0.3f, 1f) : new Color(0.05f, 0.16f, 0.32f, 0.96f);
            var button = row.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                GameAudio.Play("click");
                choose(code);
                ShowPanel(PanelHost(panel), reopen);
            });
            var label = AddLabel(row, code, 22, FontStyle.Bold, Color.white, key);
            Stretch(label.rectTransform);
        }

        Button CreateButton(RectTransform parent, string label, Sprite background, Sprite icon, bool locked, float height, int fontSize)
        {
            var image = CreateSprite(parent, label, background, background == null ? Image.Type.Simple : Image.Type.Sliced);
            if (background == null) image.color = locked ? new Color(0.15f, 0.28f, 0.62f) : new Color(0.08f, 0.65f, 0.24f);
            Preferred(image.rectTransform, height);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = locked ? Selectable.Transition.None : Selectable.Transition.ColorTint;
            button.interactable = !locked;
            var layout = image.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(46, 30, 10, 10);
            layout.spacing = 12f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            if (!locked) AddFlexible(image.transform);
            AddIcon(image.transform, icon, locked ? 72f : 78f);
            var text = AddLabel(image.transform, label, fontSize, FontStyle.Bold, Color.white);
            text.alignment = TextAnchor.MiddleCenter;
            if (locked) text.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            if (!locked) AddFlexible(image.transform);
            if (!locked) return button;

            var badge = CreateChild(image.transform, "Lock");
            badge.gameObject.AddComponent<LayoutElement>().preferredWidth = 92f;
            var badgeLayout = badge.gameObject.AddComponent<VerticalLayoutGroup>();
            badgeLayout.childAlignment = TextAnchor.MiddleCenter;
            badgeLayout.childControlWidth = true;
            badgeLayout.childControlHeight = true;
            badgeLayout.childForceExpandWidth = true;
            badgeLayout.childForceExpandHeight = false;
            badgeLayout.spacing = 2f;
            AddIcon(badge, Load("icon-lock"), 36f);
            var lockedLabel = AddLabel(badge, "LOCKED", 13, FontStyle.Bold, new Color(1f, 0.9f, 0.55f));
            lockedLabel.gameObject.AddComponent<LayoutElement>().preferredHeight = 18f;
            return button;
        }

        void AddFeatureRow(RectTransform column)
        {
            var row = CreateChild(column, "Feature Icons");
            var rowLayout = row.gameObject.AddComponent<LayoutElement>();
            rowLayout.preferredHeight = 184f;
            rowLayout.minHeight = 184f;
            rowLayout.flexibleHeight = 0f;
            var horizontal = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontal.padding = new RectOffset(12, 12, 0, 0);
            horizontal.spacing = 24f;
            horizontal.childAlignment = TextAnchor.MiddleCenter;
            horizontal.childControlWidth = true;
            horizontal.childControlHeight = false;
            horizontal.childForceExpandWidth = false;
            horizontal.childForceExpandHeight = false;
            AddFeatureTile(row, "WORLD\nMAP", Load("btn-world"), Load("icon-map"), "world", null);
            AddFeatureTile(row, "DAILY\nCHALLENGE", Load("btn-daily"), Load("icon-calendar"), "daily", () => ShowPanel(canvas.transform, "Daily"));
            AddFeatureTile(row, "SKINS", Load("btn-skins"), Load("icon-shirt"), "skins", () => ShowPanel(canvas.transform, "Skins"));
        }

        void AddFeatureTile(RectTransform parent, string label, Sprite background, Sprite icon, string key, UnityEngine.Events.UnityAction action)
        {
            var image = CreateSprite(parent, label, background, background == null ? Image.Type.Simple : Image.Type.Sliced);
            if (background == null) image.color = new Color(0.12f, 0.22f, 0.48f);
            image.rectTransform.sizeDelta = new Vector2(168f, 168f);
            var tileLayout = image.gameObject.AddComponent<LayoutElement>();
            tileLayout.preferredWidth = 168f;
            tileLayout.preferredHeight = 168f;
            tileLayout.minWidth = 168f;
            tileLayout.minHeight = 168f;
            tileLayout.flexibleHeight = 0f;
            tileLayout.flexibleWidth = 0f;
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            button.interactable = true;
            button.onClick.AddListener(() =>
            {
                GameAudio.Play("click");
                action?.Invoke();
            });
            var layout = image.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 6, 4);
            layout.spacing = 1f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            AddIcon(image.transform, icon, 88f);
            var text = AddLabel(image.transform, label, 15, FontStyle.Bold, Color.white, key);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.lineSpacing = 0.82f;
            text.gameObject.AddComponent<LayoutElement>().preferredHeight = 34f;
            if (action == null)
            {
                var lockRow = CreateChild(image.transform, "Lock");
                lockRow.gameObject.AddComponent<LayoutElement>().preferredHeight = 24f;
                var lockLayout = lockRow.gameObject.AddComponent<HorizontalLayoutGroup>();
                lockLayout.childAlignment = TextAnchor.MiddleCenter;
                lockLayout.spacing = 4f;
                lockLayout.childControlWidth = true;
                lockLayout.childControlHeight = true;
                lockLayout.childForceExpandWidth = false;
                lockLayout.childForceExpandHeight = true;
                AddIcon(lockRow, Load("icon-lock"), 20f);
                var locked = AddLabel(lockRow, "LOCKED", 11, FontStyle.Bold, new Color(1f, 0.9f, 0.55f), "locked");
                locked.gameObject.AddComponent<LayoutElement>().preferredWidth = 62f;
            }
            var glass = image.gameObject.AddComponent<CanvasGroup>();
            glass.alpha = action == null ? 0.58f : 0.92f;
            glass.interactable = true;
            glass.blocksRaycasts = true;
        }

        void AddNavTile(RectTransform parent, string label, Sprite icon, UnityEngine.Events.UnityAction action)
        {
            var tile = CreateSprite(parent, label, Load("nav-tile"), Image.Type.Sliced);
            var layout = tile.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(6, 6, 12, 8);
            layout.spacing = 2f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            AddIcon(tile.transform, icon, 70f);
            var text = AddLabel(tile.transform, label, 18, FontStyle.Bold, new Color(1f, 0.94f, 0.78f), label.ToLowerInvariant());
            text.gameObject.AddComponent<LayoutElement>().preferredHeight = 26f;
            if (action == null)
            {
                tile.raycastTarget = false;
                var locked = AddLabel(tile.transform, "LOCKED", 12, FontStyle.Bold, new Color(1f, 0.86f, 0.45f), "locked");
                locked.gameObject.AddComponent<LayoutElement>().preferredHeight = 16f;
                return;
            }
            var button = tile.gameObject.AddComponent<Button>();
            button.targetGraphic = tile;
            button.onClick.AddListener(() =>
            {
                GameAudio.Play("click");
                action();
            });
        }

        static void AddIcon(Transform parent, Sprite sprite, float size)
        {
            var image = CreateSprite(parent, "Icon", sprite, Image.Type.Simple);
            image.preserveAspect = true;
            image.raycastTarget = false;
            if (sprite == null) image.color = new Color(1f, 1f, 1f, 0f);
            var layout = image.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = size;
            layout.preferredHeight = size;
        }

        static void AddFlexible(Transform parent)
        {
            var spacer = CreateChild(parent, "Spacer");
            spacer.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
        }

        static Text AddLabel(Transform parent, string value, int size, FontStyle style, Color color, string key = null, int align = 0)
        {
            var rect = CreateChild(parent, value);
            var text = rect.gameObject.AddComponent<Text>();
            text.text = value;
            text.font = MenuFont;
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            if (!string.IsNullOrEmpty(key)) text.gameObject.AddComponent<TranslatedText>().Bind(key, align);
            var shadow = rect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            shadow.effectDistance = new Vector2(1f, -1f);
            return text;
        }

        static Image CreateSprite(Transform parent, string name, Sprite sprite, Image.Type type)
        {
            var rect = CreateChild(parent, name);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.type = sprite == null ? Image.Type.Simple : type;
            image.preserveAspect = false;
            return image;
        }

        static RectTransform CreateChild(Transform parent, string name)
        {
            var child = new GameObject(name, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            return child.GetComponent<RectTransform>();
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static void Preferred(RectTransform rect, float height)
        {
            var layout = rect.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = height;
            layout.minHeight = height;
            layout.flexibleHeight = 0f;
        }

        static Sprite Load(string name) => Resources.Load<Sprite>("MainMenuV5/" + name);

        static Font MenuFont => UiLanguage.Font;
    }

    sealed class ThemeWash : MonoBehaviour
    {
        Image image;

        void Awake() => image = GetComponent<Image>();

        void Update()
        {
            if (image == null) return;
            image.color = BoardMood.MenuWash(LocalProgressService.Theme);
        }
    }

    sealed class ScenicCover : MonoBehaviour
    {
        RawImage image;
        int width;
        int height;

        void Awake()
        {
            image = GetComponent<RawImage>();
            Apply();
        }

        void Update()
        {
            if (Screen.width == width && Screen.height == height) return;
            Apply();
        }

        void Apply()
        {
            width = Screen.width;
            height = Screen.height;
            var texture = image.texture;
            if (texture == null || width < 1 || height < 1) return;
            var view = (float)width / height;
            var aspect = (float)texture.width / texture.height;
            if (aspect > view)
            {
                var shown = view / aspect;
                image.uvRect = new Rect((1f - shown) * 0.5f, 0f, shown, 1f);
                return;
            }
            var shownHeight = aspect / view;
            image.uvRect = new Rect(0f, Mathf.Clamp01(1f - shownHeight), 1f, shownHeight);
        }
    }

    sealed class MenuSafeArea : MonoBehaviour
    {
        RectTransform column;
        RectTransform navigation;
        Rect last;
        Canvas canvas;

        public void Bind(RectTransform menuColumn, RectTransform menuNavigation)
        {
            column = menuColumn;
            navigation = menuNavigation;
            canvas = GetComponent<Canvas>();
        }

        void Update()
        {
            if (column == null || navigation == null || Screen.width < 1) return;
            var safe = Screen.safeArea;
            if (safe == last) return;
            last = safe;
            var scale = canvas != null && canvas.scaleFactor > 0.01f ? canvas.scaleFactor : 1f;
            var top = (Screen.height - safe.yMax) / scale;
            var bottom = safe.y / scale;
            column.anchoredPosition = new Vector2(0f, -(36f + top));
            var banner = BannerAdManager.ReservedHeight / scale;
            navigation.anchoredPosition = new Vector2(0f, 12f + bottom + banner);
        }
    }

    sealed class PlayPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        bool pressed;

        void Update()
        {
            var scale = pressed ? 0.96f : 1f;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * scale, Time.unscaledDeltaTime * 14f);
        }

        public void OnPointerDown(PointerEventData eventData) => pressed = true;

        public void OnPointerUp(PointerEventData eventData) => pressed = false;

        public void OnPointerExit(PointerEventData eventData) => pressed = false;
    }
}
