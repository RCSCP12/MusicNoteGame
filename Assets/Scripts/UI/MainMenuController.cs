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

        [Header("Chords Selection")]
        [SerializeField] private Button chordsButton;
        [SerializeField] private Image chordsHighlight;
        private bool chordsEnabled = false;

        [Header("Difficulty Selection")]
        [SerializeField] private Button easyButton;
        [SerializeField] private Button mediumButton;
        [SerializeField] private Button hardButton;
        [SerializeField] private Image easyHighlight;
        [SerializeField] private Image mediumHighlight;
        [SerializeField] private Image hardHighlight;
        [SerializeField] private TextMeshProUGUI difficultyDescText;

        [Header("Level Configs")]
        [SerializeField] private Gameplay.LevelConfig[] levelConfigs;

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
        private int selectedDifficulty = 0; // 0=Easy, 1=Medium, 2=Hard

        private void Start()
        {
            trebleClefButton?.onClick.AddListener(() => SelectClef(ClefType.Treble));
            bassClefButton?.onClick.AddListener(() => SelectClef(ClefType.Bass));
            practiceButton?.onClick.AddListener(() => SelectMode(GameMode.Practice));
            timedButton?.onClick.AddListener(() => SelectMode(GameMode.TimedChallenge));
            easyButton?.onClick.AddListener(() => SelectDifficulty(0));
            mediumButton?.onClick.AddListener(() => SelectDifficulty(1));
            hardButton?.onClick.AddListener(() => SelectDifficulty(2));
            chordsButton?.onClick.AddListener(ToggleChords);
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
        private void ToggleChords() { chordsEnabled = !chordsEnabled; UpdateVisuals(); }

        private void UpdateVisuals()
        {
            if (trebleHighlight) trebleHighlight.color = selectedClef == ClefType.Treble ? selectedColor : unselectedColor;
            if (bassHighlight) bassHighlight.color = selectedClef == ClefType.Bass ? selectedColor : unselectedColor;
            if (practiceHighlight) practiceHighlight.color = selectedMode == GameMode.Practice ? selectedColor : unselectedColor;
            if (timedHighlight) timedHighlight.color = selectedMode == GameMode.TimedChallenge ? selectedColor : unselectedColor;
            if (chordsHighlight) chordsHighlight.color = chordsEnabled ? selectedColor : unselectedColor;

            // Difficulty highlights
            if (easyHighlight) easyHighlight.color = selectedDifficulty == 0 ? selectedColor : unselectedColor;
            if (mediumHighlight) mediumHighlight.color = selectedDifficulty == 1 ? selectedColor : unselectedColor;
            if (hardHighlight) hardHighlight.color = selectedDifficulty == 2 ? selectedColor : unselectedColor;

            // Difficulty description
            if (difficultyDescText)
            {
                if (levelConfigs != null && selectedDifficulty < levelConfigs.Length && levelConfigs[selectedDifficulty] != null)
                {
                    var cfg = levelConfigs[selectedDifficulty];
                    difficultyDescText.text = $"{cfg.levelName}: {cfg.notesToComplete} notes, {cfg.timePerNote:0} sec timer";
                }
                else
                {
                    difficultyDescText.text = "";
                }
            }
        }

        private void LoadHighScores()
        {
            if (trebleHighScoreText) trebleHighScoreText.text = $"High Score: {PlayerPrefs.GetInt(PlayerPrefsKeys.HighScore(ClefType.Treble, selectedDifficulty), 0)}";
            if (bassHighScoreText) bassHighScoreText.text = $"High Score: {PlayerPrefs.GetInt(PlayerPrefsKeys.HighScore(ClefType.Bass, selectedDifficulty), 0)}";
        }

        public void StartGame()
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedClef, (int)selectedClef);
            PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedMode, (int)selectedMode);
            PlayerPrefs.SetInt(PlayerPrefsKeys.SelectedDifficulty, selectedDifficulty);
            PlayerPrefs.SetInt(PlayerPrefsKeys.ChordsEnabled, chordsEnabled ? 1 : 0);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
