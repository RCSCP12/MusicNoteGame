using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MusicNoteGame.Gameplay;

namespace MusicNoteGame.UI
{
    public class ClefDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI clefText;

        private void Start()
        {
            // Create text component if not assigned
            if (clefText == null)
            {
                // Try to find or create text
                clefText = GetComponentInChildren<TextMeshProUGUI>();
                if (clefText == null)
                {
                    GameObject textObj = new GameObject("ClefText");
                    textObj.transform.SetParent(transform, false);
                    clefText = textObj.AddComponent<TextMeshProUGUI>();
                    var rt = textObj.GetComponent<RectTransform>();
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.sizeDelta = Vector2.zero;
                }
            }

            clefText.fontSize = 72;
            clefText.alignment = TextAlignmentOptions.Center;
            clefText.color = Color.black;
        }

        public void SetClef(ClefType clef)
        {
            if (clefText != null)
            {
                // Use musical symbols or letters
                // Treble clef (G clef) = 𝄞 or just "G"
                // Bass clef (F clef) = 𝄢 or just "F"
                clefText.text = clef == ClefType.Treble ? "𝄞" : "𝄢";
            }
        }
    }
}
