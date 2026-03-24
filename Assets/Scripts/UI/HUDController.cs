using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MusicNoteGame.Gameplay;
using System.Collections;
using MusicNoteGame.Core;

namespace MusicNoteGame.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI streakText;
        [SerializeField] private Button backButton;

        [Header("Accidental Indicator")]
        [SerializeField] private TextMeshProUGUI accidentalIndicatorText;

        [Header("Level")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressBar;

        [Header("Timer")]
        [SerializeField] private Slider timerBar;
        [SerializeField] private Image timerFill;
        [SerializeField] private Color normalColor = new Color(0.3f, 0.85f, 0.45f);
        [SerializeField] private Color warningColor = new Color(1f, 0.45f, 0.1f);
        [SerializeField] [Range(0f, 1f)] private float warningThreshold = 0.3f;

        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Color correctColor = new Color(0.3f, 0.85f, 0.45f);
        [SerializeField] private Color incorrectColor = new Color(1f, 0.35f, 0.35f);
        
        [Header("Chords")]
        [SerializeField] private TextMeshProUGUI pressedNotesText;

        [Header("References")]
        [SerializeField] private TimerController timerController;
        [SerializeField] private MusicNoteGame.Input.NoteInputHandler inputHandler;

        private Coroutine feedbackCoroutine;

        private void Start()
        {
            if (timerController == null)
                timerController = FindAnyObjectByType<TimerController>();
            if (timerController != null)
                timerController.OnTimeUpdated += UpdateTimer;

            if (inputHandler == null)
                inputHandler = FindAnyObjectByType<MusicNoteGame.Input.NoteInputHandler>();

            if (feedbackText) feedbackText.gameObject.SetActive(false);
            if (accidentalIndicatorText) accidentalIndicatorText.text = "";
            
            if (backButton != null)
                backButton.onClick.AddListener(() => GameManager.Instance.ReturnToMainMenu());
        }

        private void Update()
        {
            if (accidentalIndicatorText != null && inputHandler != null && inputHandler.IsEnabled)
            {
                if (inputHandler.IsSharpActive)
                {
                    accidentalIndicatorText.text = "#";
                }
                else if (inputHandler.IsFlatActive)
                {
                    accidentalIndicatorText.text = "b";
                }
                else
                {
                    accidentalIndicatorText.text = "";
                }
            }
            else if (accidentalIndicatorText != null)
            {
                accidentalIndicatorText.text = "";
            }
        }

        private void OnDestroy()
        {
            if (timerController != null)
                timerController.OnTimeUpdated -= UpdateTimer;
        }

        public void UpdateScore(int score)
        {
            if (scoreText) scoreText.text = $"<color=#7CA0BC>SCORE</color>  <b>{score}</b>";
        }

        public void UpdateStreak(int streak)
        {
            if (streakText) streakText.text = streak > 0 ? $"<color=#FFD060>★  {streak} streak</color>" : "";
        }

        public void UpdateLevel(int current, int total)
        {
            if (levelText) levelText.text = $"<color=#7CA0BC>LEVEL</color>  <b>{current} / {total}</b>";
        }

        public void UpdateProgress(int current, int total)
        {
            if (progressText) progressText.text = $"<b>{current}</b><color=#888888> / {total}</color>";
            if (progressBar) { progressBar.maxValue = total; progressBar.value = current; }
        }

        private void UpdateTimer(float remaining, float max)
        {
            if (timerBar) { timerBar.maxValue = max; timerBar.value = remaining; }
            if (timerFill)
            {
                float pct = max > 0 ? remaining / max : 1f;
                timerFill.color = pct <= warningThreshold ? warningColor : normalColor;
            }
        }

        public void ShowFeedback(bool correct, string answer, string playerAnswer = null)
        {
            if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = StartCoroutine(ShowFeedbackRoutine(correct, answer, playerAnswer));
        }

        private IEnumerator ShowFeedbackRoutine(bool correct, string answer, string playerAnswer)
        {
            if (feedbackText)
            {
                feedbackText.gameObject.SetActive(true);
                feedbackText.color = correct ? correctColor : incorrectColor;
                feedbackText.text = correct ? $"<b>{answer}</b>" :
                    (playerAnswer == "Timeout" ? $"Time's up — <b>{answer}</b>" : $"It was <b>{answer}</b>");
                yield return new WaitForSeconds(1.5f);
                feedbackText.gameObject.SetActive(false);
            }
        }

        public void UpdatePressedNotes(System.Collections.Generic.List<NoteData> notes)
        {
            if (pressedNotesText != null)
            {
                if (notes == null || notes.Count == 0)
                {
                    pressedNotesText.text = "";
                }
                else
                {
                    string txt = "Pressed: ";
                    foreach (var n in notes) txt += n.ToString() + " ";
                    pressedNotesText.text = txt;
                }
            }
        }
    }
}
