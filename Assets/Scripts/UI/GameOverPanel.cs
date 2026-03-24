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
                finalScoreText.text = $"<color=#7CA0BC>FINAL SCORE</color>\n<size=+10><b>{score}</b></size>";
                finalScoreText.gameObject.SetActive(!isPractice);
            }

            if (correctText) correctText.text = $"<color=#7CA0BC>CORRECT</color>   <b><color=#5DEA80>{correct}</color></b>";
            if (incorrectText)
            {
                string incorrectColor = incorrect > 0 ? "#FF6B6B" : "#5DEA80";
                incorrectText.text = $"<color=#7CA0BC>INCORRECT</color>   <b><color={incorrectColor}>{incorrect}</color></b>";
            }

            if (accuracyText)
            {
                string accuracyColor = accuracy >= 100f ? "#5DEA80" : accuracy >= 80f ? "#FFD060" : "#FF6B6B";
                accuracyText.text = $"<color=#7CA0BC>ACCURACY</color>   <b><color={accuracyColor}>{accuracy:F1}%</color></b>";
                accuracyText.color = Color.white;
            }

            if (streakText)
            {
                streakText.text = $"<color=#7CA0BC>BEST STREAK</color>   <b>{streak}</b>";
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