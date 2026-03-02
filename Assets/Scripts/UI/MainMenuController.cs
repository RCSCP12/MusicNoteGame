using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MusicNoteGame.Core;
using MusicNoteGame.Gameplay;

namespace MusicNoteGame.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Clef Selection")]
        [SerializeField] private Button trebleClefButton;
        [SerializeField] private Button bassClefButton;
        [SerializeField] private Image trebleHighlight;
        [SerializeField] private Image bassHighlight;

        [Header("Mode Selection")]
        [SerializeField] private Button practiceButton;
        [SerializeField] private Button timedButton;
        [SerializeField] private Image practiceHighlight;
        [SerializeField] private Image timedHighlight;

        [Header("Difficulty Selection")]
        [SerializeField] private Button easyButton;
        [SerializeField] private Button mediumButton;
        [SerializeField] private Button hardButton;
        [SerializeField] private Image easyHighlight;
        [SerializeField] private Image mediumHighlight;
        [SerializeField] private Image hardHighlight;
        [SerializeField] private TextMeshProUGUI difficultyDescText;

        [Header("Start")]
        [SerializeField] private Button startButton;

        [Header("High Scores")]
        [SerializeField] private TextMeshProUGUI trebleHighScoreText;
        [SerializeField] private TextMeshProUGUI bassHighScoreText;

        [Header("Colors")]
        [SerializeField] private Color selectedColor = new Color(0.4f, 0.8f, 0.4f);
        [SerializeField] private Color unselectedColor = new Color(0.6f, 0.6f, 0.6f);

        private ClefType selectedClef = ClefType.Treble;
        private GameMode selectedMode = GameMode.TimedChallenge;
        private int selectedDifficulty = 1; // 0=Easy, 1=Medium, 2=Hard

        private void Start()
        {
            trebleClefButton?.onClick.AddListener(() => SelectClef(ClefType.Treble));
            bassClefButton?.onClick.AddListener(() => SelectClef(ClefType.Bass));
            practiceButton?.onClick.AddListener(() => SelectMode(GameMode.Practice));
            timedButton?.onClick.AddListener(() => SelectMode(GameMode.TimedChallenge));
            easyButton?.onClick.AddListener(() => SelectDifficulty(0));
            mediumButton?.onClick.AddListener(() => SelectDifficulty(1));
            hardButton?.onClick.AddListener(() => SelectDifficulty(2));
            startButton?.onClick.AddListener(StartGame);
            // Rename buttons to represent levels
            SetButtonText(easyButton, "Level 1");
            SetButtonText(mediumButton, "Level 2");
            SetButtonText(hardButton, "Level 3");
            
            UpdateVisuals();
            LoadHighScores();
        }

        private void SetButtonText(Button button, string text)
        {
            if (button == null) return;
            var tmp = button.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) { tmp.text = text; return; }
            
            var legacy = button.GetComponentInChildren<Text>();
            if (legacy != null) { legacy.text = text; }
        }

        private void SelectClef(ClefType clef) { selectedClef = clef; UpdateVisuals(); }
        private void SelectMode(GameMode mode) { selectedMode = mode; UpdateVisuals(); }
        private void SelectDifficulty(int diff) { selectedDifficulty = diff; UpdateVisuals(); LoadHighScores(); }

        private void UpdateVisuals()
        {
            if (trebleHighlight) trebleHighlight.color = selectedClef == ClefType.Treble ? selectedColor : unselectedColor;
            if (bassHighlight) bassHighlight.color = selectedClef == ClefType.Bass ? selectedColor : unselectedColor;
            if (practiceHighlight) practiceHighlight.color = selectedMode == GameMode.Practice ? selectedColor : unselectedColor;
            if (timedHighlight) timedHighlight.color = selectedMode == GameMode.TimedChallenge ? selectedColor : unselectedColor;

            // Difficulty highlights
            if (easyHighlight) easyHighlight.color = selectedDifficulty == 0 ? selectedColor : unselectedColor;
            if (mediumHighlight) mediumHighlight.color = selectedDifficulty == 1 ? selectedColor : unselectedColor;
            if (hardHighlight) hardHighlight.color = selectedDifficulty == 2 ? selectedColor : unselectedColor;

            // Difficulty description
            if (difficultyDescText)
            {
                difficultyDescText.text = selectedDifficulty switch
                {
                    0 => "Level 1: 5 notes, 15 sec timer",
                    1 => "Level 2: 10 notes, 10 sec timer",
                    2 => "Level 3: 15 notes, 5 sec timer",
                    _ => ""
                };
            }
        }

        private void LoadHighScores()
        {
            // Update keys to match ScoreManager: HighScore_{Clef}_{Difficulty}
            string trebleKey = $"HighScore_{ClefType.Treble}_{selectedDifficulty}";
            string bassKey = $"HighScore_{ClefType.Bass}_{selectedDifficulty}";
            
            if (trebleHighScoreText) trebleHighScoreText.text = $"High Score: {PlayerPrefs.GetInt(trebleKey, 0)}";
            if (bassHighScoreText) bassHighScoreText.text = $"High Score: {PlayerPrefs.GetInt(bassKey, 0)}";
        }

        public void StartGame()
        {
            PlayerPrefs.SetInt("SelectedClef", (int)selectedClef);
            PlayerPrefs.SetInt("SelectedMode", (int)selectedMode);
            PlayerPrefs.SetInt("SelectedDifficulty", selectedDifficulty);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
