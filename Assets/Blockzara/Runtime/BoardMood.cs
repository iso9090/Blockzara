using UnityEngine;

namespace Blockzara.Runtime
{
    public static class BoardMood
    {
        public static int Band(int displayedLevel)
        {
            if (displayedLevel >= 15) return 4;
            if (displayedLevel >= 10) return 3;
            if (displayedLevel >= 5) return 2;
            if (displayedLevel >= 2) return 1;
            return 0;
        }

        public static Color MenuWash(string theme)
        {
            if (theme == "harbor") return new Color(0.62f, 0.34f, 0.1f, 0.28f);
            if (theme == "night") return new Color(0.01f, 0.04f, 0.12f, 0.55f);
            return new Color(0.02f, 0.08f, 0.2f, 0.08f);
        }

        public static Color GameWash(string theme, int band)
        {
            var wash = theme == "harbor"
                ? new Color(0.28f, 0.16f, 0.05f, 0.38f)
                : theme == "night"
                    ? new Color(0.01f, 0.03f, 0.1f, 0.62f)
                    : new Color(0.02f, 0.08f, 0.2f, 0.45f);
            if (band == 1) return Color.Lerp(wash, new Color(0.78f, 0.42f, 0.1f, 0.58f), 0.7f);
            if (band == 2) return Color.Lerp(wash, new Color(0.55f, 0.24f, 0.08f, 0.5f), 0.55f);
            if (band == 3) return Color.Lerp(wash, new Color(0.02f, 0.04f, 0.14f, 0.72f), 0.65f);
            if (band == 4) return Color.Lerp(wash, new Color(0.08f, 0.02f, 0.16f, 0.78f), 0.75f);
            return wash;
        }

        public static Color Well(string theme, int band)
        {
            var well = theme == "harbor"
                ? new Color(0.08f, 0.1f, 0.07f, 0.96f)
                : theme == "night"
                    ? new Color(0.02f, 0.03f, 0.08f, 0.96f)
                    : new Color(0.03f, 0.07f, 0.16f, 0.96f);
            if (band == 1) return Color.Lerp(well, new Color(0.22f, 0.12f, 0.04f, 0.96f), 0.5f);
            if (band == 2) return Color.Lerp(well, new Color(0.16f, 0.08f, 0.04f, 0.96f), 0.45f);
            if (band == 3) return Color.Lerp(well, new Color(0.02f, 0.03f, 0.08f, 0.96f), 0.55f);
            if (band == 4) return Color.Lerp(well, new Color(0.06f, 0.02f, 0.1f, 0.96f), 0.7f);
            return well;
        }

        public static Color Frame(string theme, int band)
        {
            var frame = theme == "harbor"
                ? new Color(0.86f, 0.62f, 0.24f, 0.95f)
                : theme == "night"
                    ? new Color(0.45f, 0.58f, 0.82f, 0.95f)
                    : new Color(0.72f, 0.58f, 0.28f, 0.95f);
            if (band == 1) return Color.Lerp(frame, new Color(0.95f, 0.62f, 0.2f, 0.95f), 0.7f);
            if (band == 2) return Color.Lerp(frame, new Color(0.92f, 0.48f, 0.18f, 0.95f), 0.55f);
            if (band == 4) return Color.Lerp(frame, new Color(0.62f, 0.38f, 0.78f, 0.95f), 0.45f);
            return frame;
        }
    }
}
