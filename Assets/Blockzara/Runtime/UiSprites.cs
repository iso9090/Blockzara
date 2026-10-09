using UnityEngine;

namespace Blockzara.Runtime
{
    public static class UiSprites
    {
        static Sprite playBadge;
        static Sprite house;

        public static Sprite PlayBadge()
        {
            if (playBadge != null) return playBadge;
            const int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var center = (size - 1) * 0.5f;
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var d = Mathf.Sqrt(dx * dx + dy * dy);
                Color color;
                if (d > 58f) color = Color.clear;
                else if (d > 46f) color = new Color(0.86f, 0.68f, 0.28f, 1f);
                else color = new Color(0.05f, 0.48f, 0.28f, 1f);
                if (d < 46f && dy > 8f && Mathf.Abs(dx) < 28f) color = Color.Lerp(color, new Color(0.45f, 0.9f, 0.62f), 0.45f);
                if (PointInTriangle(x, y, 50f, 40f, 50f, 88f, 86f, 64f)) color = new Color(1f, 0.96f, 0.86f, 1f);
                tex.SetPixel(x, y, color);
            }
            tex.Apply();
            playBadge = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return playBadge;
        }

        public static Sprite House()
        {
            if (house != null) return house;
            const int size = 64;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                tex.SetPixel(x, y, Color.clear);
            for (var y = 8; y < 34; y++)
            for (var x = 16; x < 48; x++)
                tex.SetPixel(x, y, Color.white);
            for (var y = 34; y < 56; y++)
            {
                var inset = (y - 34) * 16 / 22;
                for (var x = 8 + inset; x < 56 - inset; x++)
                    tex.SetPixel(x, y, Color.white);
            }
            tex.Apply();
            house = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return house;
        }

        static bool PointInTriangle(float px, float py, float ax, float ay, float bx, float by, float cx, float cy)
        {
            var d1 = Sign(px, py, ax, ay, bx, by);
            var d2 = Sign(px, py, bx, by, cx, cy);
            var d3 = Sign(px, py, cx, cy, ax, ay);
            var hasNeg = d1 < 0f || d2 < 0f || d3 < 0f;
            var hasPos = d1 > 0f || d2 > 0f || d3 > 0f;
            return !(hasNeg && hasPos);
        }

        static float Sign(float px, float py, float ax, float ay, float bx, float by)
        {
            return (px - bx) * (ay - by) - (ax - bx) * (py - by);
        }
    }
}
