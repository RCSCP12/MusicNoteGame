using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MusicNoteGame.Core;

namespace MusicNoteGame.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI correctText;
        [SerializeField] private TextMeshProUGUI incorrectText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI streakText;
        [SerializeField] private GameObject newHighScoreObj;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;

        private void Awake()
        {
            retryButton?.onClick.AddListener(() => { Hide(); GameManager.Instance?.RestartGame(); });
            menuButton?.onClick.AddListener(() => GameManager.Instance?.ReturnToMainMenu());
            Hide();
        }

        public void Show(bool isVictory, int score, int correct, int incorrect, float accuracy, int streak, bool newHighScore)
        {
            panelRoot?.SetActive(true);
            if (titleText) titleText.text = isVictory ? "Congratulations!" : "Game Over";
            if (finalScoreText) finalScoreText.text = $"Final Score: {score}";
            if (correctText) correctText.text = $"Correct: {correct}";
            if (incorrectText) incorrectText.text = $"Incorrect: {incorrect}";
            if (accuracyText) accuracyText.text = $"Accuracy: {accuracy:F1}%";
            if (streakText) streakText.text = $"Best Streak: {streak}";
            if (newHighScoreObj) newHighScoreObj.SetActive(newHighScore);
        }

        public void Hide() => panelRoot?.SetActive(false);
    }
}
