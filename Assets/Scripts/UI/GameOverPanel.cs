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
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button menuButton;

        private bool initialized;

        private void Initialize()
        {
            if (initialized) return;
            initialized = true;
            retryButton?.onClick.AddListener(() => { Hide(); GameManager.Instance?.RestartGame(); });
            nextLevelButton?.onClick.AddListener(() => { Hide(); GameManager.Instance?.NextLevel(); });
            menuButton?.onClick.AddListener(() => GameManager.Instance?.ReturnToMainMenu());
        }

        private void Awake()
        {
            Initialize();
            // Don't hide here, as activating the GameObject synchronously calls Awake 
            // and this would instantly re-hide the panel during Show().
        }

        public void Show(bool isVictory, int score, int correct, int incorrect, float accuracy, int streak, bool newHighScore)
        {
            panelRoot?.SetActive(true);
            // Render on top of all other canvas children
            transform.SetAsLastSibling();
            
            bool isPractice = GameManager.Instance != null && GameManager.Instance.CurrentMode == GameMode.Practice;

            if (titleText) 
            {
                if (isPractice) titleText.text = "Practice Complete!";
                else titleText.text = isVictory ? "Congratulations!" : "Game Over";
            }
            
            if (finalScoreText) 
            {
                finalScoreText.text = $"Final Score: {score}";
                finalScoreText.gameObject.SetActive(!isPractice);
            }
            
            if (correctText) correctText.text = $"Correct: {correct}";
            if (incorrectText) incorrectText.text = $"Incorrect: {incorrect}";

            if (accuracyText)
            {
                accuracyText.text = $"Accuracy: {accuracy:F1}%";
                if (accuracy >= 100f)
                    accuracyText.color = Color.green;
                else if (accuracy >= 80f)
                    accuracyText.color = Color.yellow;
                else
                    accuracyText.color = Color.red;
            }

            if (streakText)
            {
                streakText.text = $"Best Streak: {streak}";
                streakText.gameObject.SetActive(!isPractice);
            }

            if (newHighScoreObj) newHighScoreObj.SetActive(newHighScore && !isPractice);

            if (nextLevelButton)
            {
                bool showNext = isVictory && GameManager.Instance != null && GameManager.Instance.HasNextLevel();
                nextLevelButton.gameObject.SetActive(showNext);
            }
        }

        public void Hide() => panelRoot?.SetActive(false);
    }
}