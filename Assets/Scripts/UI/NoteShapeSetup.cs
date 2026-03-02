using UnityEngine;
using UnityEngine.UI;

namespace MusicNoteGame.UI
{
    [RequireComponent(typeof(Image))]
    public class NoteShapeSetup : MonoBehaviour
    {
        private void Awake()
        {
            var image = GetComponent<Image>();
            image.sprite = CreateOvalSprite(40, 28);
            image.color = Color.black;
        }

        private Sprite CreateOvalSprite(int width, int height)
        {
            Texture2D tex = new Texture2D(width, height);
            Color[] colors = new Color[width * height];

            float cx = width / 2f;
            float cy = height / 2f;
            float rx = width / 2f - 1;
            float ry = height / 2f - 1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = (x - cx) / rx;
                    float dy = (y - cy) / ry;
                    colors[y * width + x] = (dx * dx + dy * dy <= 1) ? Color.white : Color.clear;
                }
            }
            tex.SetPixels(colors);
            tex.filterMode = FilterMode.Bilinear;
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100);
        }
    }
}
