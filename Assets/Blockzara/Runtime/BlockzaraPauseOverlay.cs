using UnityEngine;
using UnityEngine.UI;

namespace Blockzara.Runtime
{
    public sealed class BlockzaraPauseOverlay : MonoBehaviour
    {
        Canvas canvas;
        GameObject pauseGroup;
        GameObject restartGroup;
        GameObject leaveGroup;
        GameObject settingsGroup;
        readonly System.Collections.Generic.List<Toggle> settingsToggles = new System.Collections.Generic.List<Toggle>();

        public void Build(BlockzaraApp app)
        {
            var root = new GameObject("Pause Overlay", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 40;
            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(720f, 1520f);
            scaler.matchWidthOrHeight = 0.5f;
            root.AddComponent<GraphicRaycaster>();
            Stretch(root.GetComponent<RectTransform>());
            var scenic = Resources.Load<Texture2D>("scenic-background-v1");
            if (scenic != null)
            {
                var background = new GameObject("Scenic", typeof(RectTransform));
                background.transform.SetParent(root.transform, false);
                Stretch(background.GetComponent<RectTransform>());
                var raw = background.AddComponent<RawImage>();
                raw.texture = scenic;
                raw.raycastTarget = true;
            }
            var dim = CreateImage(root.transform, "Dim", new Color(0.01f, 0.04f, 0.1f, 0.72f));
            Stretch(dim.rectTransform);
            pauseGroup = CreateCard(root.transform, "GAME PAUSED", "paused");
            AddAction(pauseGroup.transform, "RESUME", "resume", UiSprites.PlayBadge(), () => app.ResumeFromPause());
            AddAction(pauseGroup.transform, "RESTART", "restart", null, () => app.AskRestart());
            AddIconLabel(pauseGroup.transform.Find("RESTART"), "↻");
            AddAction(pauseGroup.transform, "SETTINGS", "title.settings", Resources.Load<Sprite>("MainMenuV5/icon-settings"), () => app.OpenPauseSettings());
            AddAction(pauseGroup.transform, "MAIN MENU", "menu", UiSprites.House(), () => app.AskLeave());
            restartGroup = CreateCard(root.transform, "RESTART?", "restartAsk");
            AddBody(restartGroup.transform, "Restart this game?", "restartBody");
            AddAction(restartGroup.transform, "YES", "yes", null, () => app.ConfirmRestart());
            AddAction(restartGroup.transform, "NO", "no", null, () => app.CancelPauseDialog());
            leaveGroup = CreateCard(root.transform, "MAIN MENU?", "menuAsk");
            AddBody(leaveGroup.transform, "Leave this game and return to Main Menu?", "leaveBody");
            AddAction(leaveGroup.transform, "YES", "yes", null, () => app.ConfirmLeave());
            AddAction(leaveGroup.transform, "NO", "no", null, () => app.CancelPauseDialog());
            settingsGroup = CreateCard(root.transform, "SETTINGS", "title.settings");
            AddToggle(settingsGroup.transform, "Music", "music", () => LocalProgressService.MusicEnabled, value =>
            {
                LocalProgressService.MusicEnabled = value;
                GameAudio.ApplySettings();
            });
            AddToggle(settingsGroup.transform, "Sound effects", "sfx", () => LocalProgressService.SfxEnabled, value => LocalProgressService.SfxEnabled = value);
            AddToggle(settingsGroup.transform, "Vibration", "vibration", () => LocalProgressService.VibrationEnabled, value => LocalProgressService.VibrationEnabled = value);
            AddToggle(settingsGroup.transform, "Shadow", "shadow", () => LocalProgressService.GhostEnabled, value => LocalProgressService.GhostEnabled = value);
            AddLanguage(settingsGroup.transform);
            AddAction(settingsGroup.transform, "BACK", "back", null, () => app.ClosePauseSettings());
            canvas.gameObject.SetActive(false);
        }

        public void Apply(PauseFlow flow)
        {
            if (canvas == null || flow == null) return;
            canvas.gameObject.SetActive(flow.Screen != PauseScreen.Hidden);
            if (flow.Screen == PauseScreen.Settings) RefreshSettings();
            pauseGroup.SetActive(flow.Screen == PauseScreen.Pause);
            restartGroup.SetActive(flow.Screen == PauseScreen.ConfirmRestart);
            leaveGroup.SetActive(flow.Screen == PauseScreen.ConfirmLeave);
            settingsGroup.SetActive(flow.Screen == PauseScreen.Settings);
        }

        static GameObject CreateCard(Transform parent, string title, string key)
        {
            var card = CreateImage(parent, title, new Color(0.03f, 0.08f, 0.16f, 0.98f));
            var rect = card.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(560f, 820f);
            rect.anchoredPosition = new Vector2(0f, 40f);
            var frame = card.gameObject.AddComponent<Outline>();
            frame.effectColor = new Color(0.86f, 0.7f, 0.32f, 1f);
            frame.effectDistance = new Vector2(3f, -3f);
            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(28, 28, 28, 28);
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            var label = AddText(card.transform, title, 34, FontStyle.Bold, new Color(1f, 0.93f, 0.72f), key);
            label.gameObject.AddComponent<LayoutElement>().preferredHeight = 56f;
            return card.gameObject;
        }

        static void AddBody(Transform parent, string value, string key)
        {
            var label = AddText(parent, value, 24, FontStyle.Bold, Color.white, key);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.gameObject.AddComponent<LayoutElement>().preferredHeight = 90f;
        }

        static void AddAction(Transform parent, string label, string key, Sprite icon, UnityEngine.Events.UnityAction action)
        {
            var image = CreateImage(parent, label, new Color(0.08f, 0.16f, 0.32f, 1f));
            image.gameObject.AddComponent<LayoutElement>().preferredHeight = 78f;
            image.gameObject.AddComponent<Outline>().effectColor = new Color(0.86f, 0.7f, 0.32f, 0.9f);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => GameAudio.Play("click"));
            button.onClick.AddListener(action);
            var row = image.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.padding = new RectOffset(24, 24, 8, 8);
            row.spacing = 12f;
            row.childAlignment = TextAnchor.MiddleCenter;
            row.childControlWidth = true;
            row.childControlHeight = true;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = true;
            if (icon != null)
            {
                var iconImage = CreateImage(image.transform, "Icon", Color.white);
                iconImage.sprite = icon;
                iconImage.preserveAspect = true;
                var iconLayout = iconImage.gameObject.AddComponent<LayoutElement>();
                iconLayout.preferredWidth = 42f;
                iconLayout.preferredHeight = 42f;
            }
            var caption = AddText(image.transform, label, 26, FontStyle.Bold, Color.white, key);
            caption.gameObject.AddComponent<LayoutElement>().preferredWidth = 240f;
        }

        static void AddIconLabel(Transform button, string icon)
        {
            if (button == null) return;
            var text = AddText(button, icon, 28, FontStyle.Bold, new Color(1f, 0.93f, 0.72f));
            text.gameObject.AddComponent<LayoutElement>().preferredWidth = 36f;
        }

        void AddLanguage(Transform parent)
        {
            var row = CreateImage(parent, "Language", new Color(0.06f, 0.12f, 0.24f, 1f));
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(12, 12, 8, 8);
            layout.spacing = 8f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            var caption = AddText(row.transform, "Language", 22, FontStyle.Bold, Color.white, "language", 1);
            caption.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1.1f;
            AddLanguageButton(row.transform, "ar", "\u0627\u0644\u0639\u0631\u0628\u064A\u0629");
            AddLanguageButton(row.transform, "en", "English");
            AddLanguageButton(row.transform, "ur", "\u0627\u0631\u062F\u0648");
        }

        static void AddLanguageButton(Transform parent, string code, string native)
        {
            var image = CreateImage(parent, code, new Color(0.08f, 0.16f, 0.32f, 1f));
            image.gameObject.AddComponent<LayoutElement>().preferredHeight = 48f;
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                GameAudio.Play("click");
                LocalProgressService.Language = code;
            });
            var text = AddText(image.transform, "Label", 18, FontStyle.Bold, Color.white);
            text.text = native;
            image.gameObject.AddComponent<LanguageChoice>().Bind(code, native, text);
        }

        void AddToggle(Transform parent, string label, string key, System.Func<bool> read, System.Action<bool> write)
        {
            var row = CreateImage(parent, label, new Color(0.06f, 0.12f, 0.24f, 1f));
            row.gameObject.AddComponent<LayoutElement>().preferredHeight = 72f;
            var layout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 8, 8);
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            var text = AddText(row.transform, label, 24, FontStyle.Bold, Color.white, key, 1);
            text.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;
            var toggleObject = new GameObject(label + " Toggle", typeof(RectTransform));
            toggleObject.transform.SetParent(row.transform, false);
            var toggleLayout = toggleObject.AddComponent<LayoutElement>();
            toggleLayout.preferredWidth = 84f;
            toggleLayout.preferredHeight = 42f;
            var toggleImage = toggleObject.AddComponent<Image>();
            var value = read();
            toggleImage.color = value ? new Color(0.15f, 0.55f, 0.32f) : new Color(0.25f, 0.28f, 0.34f);
            var toggle = toggleObject.AddComponent<Toggle>();
            toggle.targetGraphic = toggleImage;
            toggle.isOn = value;
            toggle.onValueChanged.AddListener(on =>
            {
                toggleImage.color = on ? new Color(0.15f, 0.55f, 0.32f) : new Color(0.25f, 0.28f, 0.34f);
                write(on);
            });
            settingsToggles.Add(toggle);
        }

        void RefreshSettings()
        {
            for (var i = 0; i < settingsToggles.Count; i++)
            {
                var toggle = settingsToggles[i];
                var current = i == 0 ? LocalProgressService.MusicEnabled : i == 1 ? LocalProgressService.SfxEnabled : i == 2 ? LocalProgressService.VibrationEnabled : LocalProgressService.GhostEnabled;
                toggle.SetIsOnWithoutNotify(current);
                if (toggle.targetGraphic != null) toggle.targetGraphic.color = current ? new Color(0.15f, 0.55f, 0.32f) : new Color(0.25f, 0.28f, 0.34f);
            }
        }

        static Text AddText(Transform parent, string value, int size, FontStyle style, Color color, string key = null, int align = 0)
        {
            var child = new GameObject(value, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            var text = child.AddComponent<Text>();
            text.text = value;
            text.font = Font();
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            if (!string.IsNullOrEmpty(key)) text.gameObject.AddComponent<TranslatedText>().Bind(key, align);
            return text;
        }

        static Image CreateImage(Transform parent, string name, Color color)
        {
            var child = new GameObject(name, typeof(RectTransform));
            child.transform.SetParent(parent, false);
            var image = child.AddComponent<Image>();
            image.color = color;
            return image;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static Font Font() => UiLanguage.Font;
    }
}
