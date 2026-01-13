using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MusicNoteGame.Gameplay;

namespace MusicNoteGame.UI
{
    public class StaffDisplay : MonoBehaviour
    {
        [Header("Clef")]
        [SerializeField] private Image clefImage;
        [SerializeField] private TextMeshProUGUI clefText;
        [SerializeField] private Sprite trebleClefSprite;
        [SerializeField] private Sprite bassClefSprite;

        [Header("Staff Settings")]
        [SerializeField] private float lineSpacing = 20f;

        [Header("Ledger Lines")]
        [SerializeField] private GameObject ledgerLinePrefab;
        [SerializeField] private Transform ledgerLineContainer;

        private ClefType currentClef;
        private GameObject[] ledgerLines = new GameObject[4];

        public float LineSpacing => lineSpacing;

        public void SetClef(ClefType clef)
        {
            currentClef = clef;

            // Try load sprites if not assigned
            if (trebleClefSprite == null) trebleClefSprite = LoadSprite("Sprites/TrebleClef");
            if (bassClefSprite == null) bassClefSprite = LoadSprite("Sprites/BassClef");

            Sprite targetSprite = clef == ClefType.Treble ? trebleClefSprite : bassClefSprite;

            if (targetSprite != null && clefImage != null)
            {
                // Use Sprite
                clefImage.sprite = targetSprite;
                clefImage.gameObject.SetActive(true);
                if (clefText != null) clefText.gameObject.SetActive(false);
            }
            else if (clefText != null)
            {
                // Fallback to Text
                clefText.text = clef == ClefType.Treble ? "G" : "F";
                clefText.fontSize = 72;
                clefText.fontStyle = TMPro.FontStyles.Bold | TMPro.FontStyles.Italic;
                clefText.color = Color.black;
                clefText.gameObject.SetActive(true);
                if (clefImage != null) clefImage.gameObject.SetActive(false);
            }
        }

        private Sprite LoadSprite(string path)
        {
            var sprite = Resources.Load<Sprite>(path);
            if (sprite != null) return sprite;

            var texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            return null;
        }

        public void ClearLedgerLines()
        {
            foreach (var line in ledgerLines)
                if (line != null) line.SetActive(false);
        }

        public void ShowLedgerLines(int staffPosition, float noteXPosition)
        {
            ClearLedgerLines();
            if (ledgerLinePrefab == null || ledgerLineContainer == null) return;

            int lineIndex = 0;
            if (staffPosition < 0)
            {
                for (int pos = -2; pos >= staffPosition && lineIndex < ledgerLines.Length; pos -= 2)
                    ShowLedgerLineAt(lineIndex++, pos, noteXPosition);
            }
            else if (staffPosition > 8)
            {
                for (int pos = 10; pos <= staffPosition && lineIndex < ledgerLines.Length; pos += 2)
                    ShowLedgerLineAt(lineIndex++, pos, noteXPosition);
            }
        }

        private void ShowLedgerLineAt(int index, int position, float xPosition)
        {
            if (ledgerLines[index] == null)
                ledgerLines[index] = Instantiate(ledgerLinePrefab, ledgerLineContainer);

            var rt = ledgerLines[index].GetComponent<RectTransform>();
            if (rt != null)
                rt.anchoredPosition = new Vector2(xPosition, GetYPositionForStaffPosition(position));

            ledgerLines[index].SetActive(true);
        }

        public float GetYPositionForStaffPosition(int staffPosition)
        {
            float staffCenter = 4f;
            return (staffPosition - staffCenter) * (lineSpacing / 2f);
        }
    }
}
