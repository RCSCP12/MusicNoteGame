using UnityEngine;

namespace MusicNoteGame.Utility
{
    public static class SpriteGenerator
    {
        public static Sprite CreateWhiteSquare(int size = 64)
        {
            Texture2D tex = new Texture2D(size, size);
            Color[] colors = new Color[size * size];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.white;
            tex.SetPixels(colors);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        public static Sprite CreateOval(int width = 40, int height = 30)
        {
            Texture2D tex = new Texture2D(width, height);
            Color[] colors = new Color[width * height];

            float cx = width / 2f;
            float cy = height / 2f;
            float rx = width / 2f;
            float ry = height / 2f;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    colors[y * width + x] = (dx * dx + dy * dy <= 1) ? Color.black : Color.clear;
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }

        public static Sprite CreateHollowOval(int width = 40, int height = 30, int thickness = 3)
        {
            Texture2D tex = new Texture2D(width, height);
            Color[] colors = new Color[width * height];

            float cx = width / 2f;
            float cy = height / 2f;
            float rx = width / 2f;
            float ry = height / 2f;
            float innerRx = rx - thickness;
            float innerRy = ry - thickness;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    float innerDx = (x - cx) / innerRx;
                    float innerDy = (y - cy) / innerRy;

                    bool inOuter = dx * dx + dy * dy <= 1;
                    bool inInner = innerDx * innerDx + innerDy * innerDy <= 1;

                    colors[y * width + x] = (inOuter && !inInner) ? Color.black : Color.clear;
                }
            }
            tex.SetPixels(colors);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }
    }
}
