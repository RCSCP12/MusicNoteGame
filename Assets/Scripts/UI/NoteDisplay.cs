using UnityEngine;
using UnityEngine.UI;
using MusicNoteGame.Gameplay;
using System.Collections;
using System.Collections.Generic;

namespace MusicNoteGame.UI
{
    public class NoteDisplay : MonoBehaviour
    {
        [SerializeField] private RectTransform noteTransform;
        [SerializeField] private Image noteImage;
        [SerializeField] private TMPro.TextMeshProUGUI accidentalText;
        [SerializeField] private StaffDisplay staffDisplay;
        [SerializeField] private float appearDuration = 0.2f;

        private List<NoteData> currentNotes = new List<NoteData>();
        private List<GameObject> clonedNotes = new List<GameObject>();

        private void Awake()
        {
            if (noteTransform == null) noteTransform = GetComponent<RectTransform>();
            if (noteImage == null) noteImage = GetComponent<Image>();
        }

        public void DisplayNotes(List<NoteData> notes, ClefType clef)
        {
            currentNotes = notes;
            
            // cleanup old clones
            foreach(var clone in clonedNotes) if (clone != null) Destroy(clone);
            clonedNotes.Clear();
            
            staffDisplay.ClearLedgerLines();

            for(int i = 0; i < notes.Count; i++)
            {
                var note = notes[i];
                int staffPosition = note.GetStaffPosition(clef);
                float yPosition = staffDisplay.GetYPositionForStaffPosition(staffPosition);

                RectTransform targetTransform;
                Image targetImage;
                TMPro.TextMeshProUGUI targetAccidental;

                if (i == 0)
                {
                    targetTransform = noteTransform;
                    targetImage = noteImage;
                    targetAccidental = accidentalText;
                }
                else
                {
                    GameObject cloneObj = Instantiate(gameObject, transform.parent);
                    Destroy(cloneObj.GetComponent<NoteDisplay>());
                    clonedNotes.Add(cloneObj);
                    
                    targetTransform = cloneObj.GetComponent<RectTransform>();
                    targetImage = cloneObj.GetComponent<Image>();

                    // The accidental text is a child of the note gameobject in this structure
                    targetAccidental = cloneObj.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);

                    if (targetTransform == null)
                    {
                        Debug.LogError("NoteDisplay: Cloned note is missing RectTransform — skipping.");
                        continue;
                    }
                }

                targetTransform.anchoredPosition = new Vector2(noteTransform.anchoredPosition.x, yPosition);

                if (note.NeedsLedgerLines(clef))
                    staffDisplay.ShowLedgerLines(staffPosition, targetTransform.anchoredPosition.x);

                if (targetAccidental != null)
                {
                    if (note.isSharp || note.isFlat)
                    {
                        targetAccidental.text = note.isSharp ? "♯" : "♭";
                        targetAccidental.fontSize = 28;
                        targetAccidental.color = new Color(0.91f, 0.91f, 0.94f);
                        targetAccidental.alignment = TMPro.TextAlignmentOptions.Center;

                        // Position to the left of the note head (key-signature style)
                        var accRT = targetAccidental.rectTransform;
                        accRT.anchoredPosition = new Vector2(-35f, 0f);
                        accRT.sizeDelta = new Vector2(24f, 36f);

                        targetAccidental.gameObject.SetActive(true);
                    }
                    else
                    {
                        targetAccidental.gameObject.SetActive(false);
                    }
                }

                StartCoroutine(AppearAnimation(targetImage, targetTransform));
            }
        }

        public void DisplayNote(NoteData note, ClefType clef)
        {
            DisplayNotes(new List<NoteData> { note }, clef);
        }

        private IEnumerator AppearAnimation(Image img, RectTransform trans)
        {
            if (img != null) img.enabled = true;
            if (trans != null) trans.localScale = Vector3.zero;
            float elapsed = 0f;

            while (elapsed < appearDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / appearDuration;
                if (trans != null) trans.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }
            if (trans != null) trans.localScale = Vector3.one;
        }

        public void ShowCorrectFeedback() 
        {
            StartCoroutine(FlashColor(Color.green, noteImage));
            foreach(var clone in clonedNotes) if (clone != null) StartCoroutine(FlashColor(Color.green, clone.GetComponent<Image>()));
        }

        public void ShowIncorrectFeedback() 
        {
            StartCoroutine(FlashColor(Color.red, noteImage));
            foreach(var clone in clonedNotes) if (clone != null) StartCoroutine(FlashColor(Color.red, clone.GetComponent<Image>()));
        }

        private IEnumerator FlashColor(Color flashColor, Image img)
        {
            if (img == null) yield break;
            Color original = img.color;
            img.color = flashColor;
            yield return new WaitForSeconds(0.2f);
            if (img != null) img.color = original;
        }

        public List<NoteData> GetCurrentNotes() => currentNotes;
    }
}
