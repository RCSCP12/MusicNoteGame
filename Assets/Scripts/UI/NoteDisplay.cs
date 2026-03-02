using UnityEngine;
using UnityEngine.UI;
using MusicNoteGame.Gameplay;
using System.Collections;

namespace MusicNoteGame.UI
{
    public class NoteDisplay : MonoBehaviour
    {
        [SerializeField] private RectTransform noteTransform;
        [SerializeField] private Image noteImage;
        [SerializeField] private StaffDisplay staffDisplay;
        [SerializeField] private float appearDuration = 0.2f;

        private NoteData currentNote;

        private void Awake()
        {
            if (noteTransform == null) noteTransform = GetComponent<RectTransform>();
            if (noteImage == null) noteImage = GetComponent<Image>();
        }

        public void DisplayNote(NoteData note, ClefType clef)
        {
            currentNote = note;
            int staffPosition = note.GetStaffPosition(clef);
            float yPosition = staffDisplay.GetYPositionForStaffPosition(staffPosition);
            noteTransform.anchoredPosition = new Vector2(noteTransform.anchoredPosition.x, yPosition);

            if (note.NeedsLedgerLines(clef))
                staffDisplay.ShowLedgerLines(staffPosition, noteTransform.anchoredPosition.x);
            else
                staffDisplay.ClearLedgerLines();

            StartCoroutine(AppearAnimation());
        }

        private IEnumerator AppearAnimation()
        {
            noteImage.enabled = true;
            noteTransform.localScale = Vector3.zero;
            float elapsed = 0f;

            while (elapsed < appearDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / appearDuration;
                noteTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }
            noteTransform.localScale = Vector3.one;
        }

        public void ShowCorrectFeedback() => StartCoroutine(FlashColor(Color.green));
        public void ShowIncorrectFeedback() => StartCoroutine(FlashColor(Color.red));

        private IEnumerator FlashColor(Color flashColor)
        {
            Color original = noteImage.color;
            noteImage.color = flashColor;
            yield return new WaitForSeconds(0.2f);
            noteImage.color = original;
        }

        public NoteData GetCurrentNote() => currentNote;
    }
}
