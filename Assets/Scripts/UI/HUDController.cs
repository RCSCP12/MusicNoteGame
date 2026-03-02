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

        [Header("Level")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressBar;

        [Header("Timer")]
        [SerializeField] private Slider timerBar;
        [SerializeField] private Image timerFill;
        [SerializeField] private Color normalColor = Color.green;
        [SerializeField] private Color warningColor = Color.red;

        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private Color correctColor = Color.green;
        [SerializeField] private Color incorrectColor = Color.red;

        private TimerController timerController;
        private Coroutine feedbackCoroutine;

        private void Start()
        {
            timerController = FindAnyObjectByType<TimerController>();
            if (timerController != null)
                timerController.OnTimeUpdated += UpdateTimer;
            if (feedbackText) feedbackText.gameObject.SetActive(false);
            
            if (backButton != null)
                backButton.onClick.AddListener(() => GameManager.Instance.ReturnToMainMenu());
        }

        private void OnDestroy()
        {
            if (timerController != null)
                timerController.OnTimeUpdated -= UpdateTimer;
        }

        public void UpdateScore(int score)
        {
            if (scoreText) scoreText.text = $"Score: {score}";
        }

        public void UpdateStreak(int streak)
        {
            if (streakText) streakText.text = streak > 0 ? $"Streak: {streak}" : "";
        }

        public void UpdateLevel(int current, int total)
        {
            if (levelText) levelText.text = $"Level {current}/{total}";
        }

        public void UpdateProgress(int current, int total)
        {
            if (progressText) progressText.text = $"{current}/{total}";
            if (progressBar) { progressBar.maxValue = total; progressBar.value = current; }
        }

        private void UpdateTimer(float remaining, float max)
        {
            if (timerBar) { timerBar.maxValue = max; timerBar.value = remaining; }
            if (timerFill)
            {
                float pct = max > 0 ? remaining / max : 1f;
                timerFill.color = pct <= 0.3f ? warningColor : normalColor;
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
                feedbackText.text = correct ? $"Correct! {answer}" :
                    (playerAnswer == "Timeout" ? $"Time's up! It was {answer}" : $"Wrong! It was {answer}");
                yield return new WaitForSeconds(1.5f);
                feedbackText.gameObject.SetActive(false);
            }
        }
    }
}
