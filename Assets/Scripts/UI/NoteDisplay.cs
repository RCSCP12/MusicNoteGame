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

            // Pre-calculate Y positions then resolve X offsets for overlapping notes.
            // When two notes are closer vertically than the note oval's height, the
            // upper note shifts right (standard notation convention).
            const float NoteOvalH  = 28f;
            const float NoteOvalW  = 40f;
            const float RightShift = NoteOvalW + 4f; // pixels to shift a displaced note

            float baseX = noteTransform.anchoredPosition.x;

            float[] yPos = new float[notes.Count];
            int[]   staffPos = new int[notes.Count];
            for (int i = 0; i < notes.Count; i++)
            {
                staffPos[i] = notes[i].GetStaffPosition(clef);
                yPos[i] = staffDisplay.GetYPositionForStaffPosition(staffPos[i]);
            }

            // Build bottom-to-top sorted order
            int[] order = new int[notes.Count];
            for (int i = 0; i < notes.Count; i++) order[i] = i;
            System.Array.Sort(order, (a, b) => yPos[a].CompareTo(yPos[b]));

            float[] xOff = new float[notes.Count]; // default = 0 (left column)
            for (int j = 1; j < order.Length; j++)
            {
                int prev = order[j - 1];
                int curr = order[j];
                if (Mathf.Abs(yPos[curr] - yPos[prev]) < NoteOvalH)
                    xOff[curr] = xOff[prev] == 0f ? RightShift : 0f; // alternate columns
            }

            for(int i = 0; i < notes.Count; i++)
            {
                var note = notes[i];

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

                targetTransform.anchoredPosition = new Vector2(baseX + xOff[i], yPos[i]);

                if (note.NeedsLedgerLines(clef))
                    staffDisplay.ShowLedgerLines(staffPos[i], targetTransform.anchoredPosition.x);

                if (targetAccidental != null)
                {
                    if (note.isSharp || note.isFlat)
                    {
                        // ♯/♭ are outside the default TMP atlas; use ASCII # and italic b instead.
                        targetAccidental.richText = true;
                        targetAccidental.text = note.isSharp ? "#" : "<i>b</i>";
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
