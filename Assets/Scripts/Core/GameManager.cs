using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MusicNoteGame.Gameplay;
using MusicNoteGame.Input;
using MusicNoteGame.UI;
using MusicNoteGame.Audio;

namespace MusicNoteGame.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private NoteGenerator noteGenerator;
        [SerializeField] private NoteInputHandler inputHandler;
        [SerializeField] private TimerController timerController;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private StaffDisplay staffDisplay;
        [SerializeField] private NoteDisplay noteDisplay;
        [SerializeField] private HUDController hudController;
        [SerializeField] private GameOverPanel gameOverPanel;

        [Header("Settings")]
        [SerializeField] private float feedbackDelay = 0.5f;

        public GameState CurrentState { get; private set; }
        public GameMode CurrentMode { get; private set; }
        public ClefType CurrentClef { get; private set; }
        public int CurrentDifficulty { get; private set; }

        public event Action<GameState> OnStateChanged;

        private bool chordsEnabled;
        private System.Collections.Generic.List<NoteData> currentTargetNotes = new System.Collections.Generic.List<NoteData>();
        private System.Collections.Generic.List<NoteData> accumulatedInputs = new System.Collections.Generic.List<NoteData>();
        private int correctAnswers;
        private int notesToWin;
        private float timePerNote;
        private bool waitingForNextNote;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        private void Start()
        {
            ValidateDependencies();
            SetupEventListeners();
        }

        private bool ValidateDependencies()
        {
            bool valid = true;
            if (noteGenerator == null) { Debug.LogError("GameManager: noteGenerator is not assigned."); valid = false; }
            if (inputHandler  == null) { Debug.LogError("GameManager: inputHandler is not assigned.");  valid = false; }
            if (noteDisplay   == null) { Debug.LogError("GameManager: noteDisplay is not assigned.");   valid = false; }
            if (staffDisplay  == null) { Debug.LogError("GameManager: staffDisplay is not assigned.");  valid = false; }
            if (scoreManager  == null) { Debug.LogError("GameManager: scoreManager is not assigned.");  valid = false; }
            if (levelManager  == null) { Debug.LogError("GameManager: levelManager is not assigned.");  valid = false; }
            if (gameOverPanel == null) { Debug.LogError("GameManager: gameOverPanel is not assigned."); valid = false; }
            return valid;
        }

        private void SetupEventListeners()
        {
            if (inputHandler != null) inputHandler.OnNoteInput += HandleNoteInput;
            if (timerController != null) timerController.OnTimeExpired += HandleTimeExpired;
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged += (s) => hudController?.UpdateScore(s);
                scoreManager.OnStreakChanged += (s) => hudController?.UpdateStreak(s);
            }
        }

        private void OnDestroy()
        {
            if (inputHandler != null) inputHandler.OnNoteInput -= HandleNoteInput;
            if (timerController != null) timerController.OnTimeExpired -= HandleTimeExpired;
            CancelInvoke();
        }

        public void StartGame(ClefType clef, GameMode mode, int difficulty = 1)
        {
            CancelInvoke();
            CurrentClef = clef;
            CurrentMode = mode;
            CurrentDifficulty = difficulty;
            chordsEnabled = PlayerPrefs.GetInt(PlayerPrefsKeys.ChordsEnabled, 0) == 1;

            gameOverPanel?.Hide();
            noteDisplay?.gameObject.SetActive(true);
            correctAnswers = 0;
            // Initialize LevelManager with the selected difficulty/level index
            levelManager.Initialize(difficulty);
            
            // Set difficulty settings
            // Set difficulty from Level Manager which uses LevelConfig
            notesToWin = levelManager.GetCurrentNotesToComplete();
            timePerNote = levelManager.GetCurrentTimeLimit();
            
            if (chordsEnabled)
            {
                timePerNote *= 2.0f; // Give the user more time to input multiple notes in a chord
            }

            scoreManager.ResetScore();
            staffDisplay.SetClef(clef);

            // Use first level config for notes
            var levelConfig = levelManager.CurrentLevel;
            if (levelConfig == null)
            {
                Debug.LogError("GameManager: No level config available — cannot start game.");
                return;
            }
            noteGenerator.Initialize(levelConfig, CurrentClef);

            hudController?.UpdateLevel(CurrentDifficulty + 1, levelManager.TotalLevels);
            hudController?.UpdateProgress(correctAnswers, notesToWin);

            // Deselect any focused UI element so Space (flat modifier) doesn't
            // trigger EventSystem Submit on buttons during gameplay.
            UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);

            ChangeState(GameState.Playing);
            GenerateNewNote();
        }

        private void GenerateNewNote()
        {
            waitingForNextNote = false;
            accumulatedInputs.Clear();
            hudController?.UpdatePressedNotes(accumulatedInputs);
            
            currentTargetNotes = noteGenerator.GenerateNotes(chordsEnabled);
            noteDisplay.DisplayNotes(currentTargetNotes, CurrentClef);
            foreach(var note in currentTargetNotes)
                NoteAudioPlayer.Instance?.PlayNote(note);

            if (CurrentMode == GameMode.TimedChallenge)
                timerController.StartTimer(timePerNote);

            inputHandler.EnableInput();
        }

        private void HandleNoteInput(NoteName inputNote, bool inputSharp, bool inputFlat)
        {
            if (CurrentState != GameState.Playing || waitingForNextNote) return;

            string inputStr = inputNote.ToString();
            if (inputSharp) inputStr += "#";
            else if (inputFlat) inputStr += "b";
            
            Debug.Log($"[GameManager] User Input Received: {inputStr} (isSharp: {inputSharp}, isFlat: {inputFlat})");

            var match = currentTargetNotes.Find(n => n.noteName == inputNote && n.isSharp == inputSharp && n.isFlat == inputFlat);

            if (match != null && !accumulatedInputs.Contains(match))
            {
                // Correct input
                accumulatedInputs.Add(match);
                NoteAudioPlayer.Instance?.PlayNote(match);
                hudController?.UpdatePressedNotes(accumulatedInputs);
                
                if (accumulatedInputs.Count == currentTargetNotes.Count)
                {
                    inputHandler.DisableInput();
                    timerController.StopTimer();
                    HandleCorrectAnswer();
                }
            }
            else if (match == null)
            {
                // Incorrect input (only punish if the pressed note wasn't in the chord at all)
                inputHandler.DisableInput();
                timerController.StopTimer();
                HandleIncorrectAnswer(inputNote, inputSharp, inputFlat);
            }
        }

        private void HandleCorrectAnswer()
        {
            correctAnswers++;
            foreach(var n in currentTargetNotes) NoteAudioPlayer.Instance?.PlayNote(n);
            scoreManager.RecordCorrectAnswer(timerController.TimeRemaining, timerController.MaxTime);
            noteDisplay.ShowCorrectFeedback();
            
            string targetStr = string.Join(" ", currentTargetNotes.ConvertAll(n => n.ToString()));
            hudController?.ShowFeedback(true, targetStr);
            hudController?.UpdateProgress(correctAnswers, notesToWin);

            if (correctAnswers >= notesToWin)
            {
                // Game complete!
                ChangeState(GameState.Victory);
                Invoke(nameof(ShowVictory), feedbackDelay);
            }
            else
            {
                waitingForNextNote = true;
                Invoke(nameof(GenerateNewNote), feedbackDelay);
            }
        }

        private void ShowVictory()
        {
            ShowGameOver(true);
        }

        private void HandleIncorrectAnswer(NoteName inputNote, bool inputSharp, bool inputFlat)
        {
            scoreManager.RecordIncorrectAnswer();
            NoteAudioPlayer.Instance?.PlayError();
            noteDisplay.ShowIncorrectFeedback();
            
            string playerAnswerStr;
            if (inputSharp) playerAnswerStr = $"{inputNote}#";
            else if (inputFlat) playerAnswerStr = $"{inputNote}b";
            else playerAnswerStr = inputNote.ToString();
            
            string targetStr = string.Join(" ", currentTargetNotes.ConvertAll(n => n.ToString()));
            hudController?.ShowFeedback(false, targetStr, playerAnswerStr);
            waitingForNextNote = true;
            Invoke(nameof(GenerateNewNote), feedbackDelay * 2);
        }

        private void HandleTimeExpired()
        {
            if (CurrentState != GameState.Playing) return;
            inputHandler.DisableInput();
            scoreManager.RecordIncorrectAnswer();
            NoteAudioPlayer.Instance?.PlayError();
            
            string targetStr = string.Join(" ", currentTargetNotes.ConvertAll(n => n.ToString()));
            hudController?.ShowFeedback(false, targetStr, "Timeout");
            waitingForNextNote = true;
            Invoke(nameof(GenerateNewNote), feedbackDelay * 2);
        }

        private void ShowGameOver(bool isVictory)
        {
            // Clear gameplay UI elements before showing the overlay
            noteDisplay?.gameObject.SetActive(false);
            hudController?.UpdatePressedNotes(null);

            bool isNewHighScore = scoreManager.SaveHighScoreIfBetter(CurrentClef, CurrentDifficulty);
            gameOverPanel?.Show(isVictory, scoreManager.TotalScore, scoreManager.CorrectAnswers,
                scoreManager.IncorrectAnswers, scoreManager.GetAccuracy(), scoreManager.BestStreak, isNewHighScore);
        }

        public void RestartGame() => StartGame(CurrentClef, CurrentMode, CurrentDifficulty);
        
        public bool HasNextLevel()
        {
            return levelManager != null && CurrentDifficulty < levelManager.TotalLevels - 1;
        }

        public void NextLevel()
        {
            if (HasNextLevel())
            {
                StartGame(CurrentClef, CurrentMode, CurrentDifficulty + 1);
            }
        }

        public void ReturnToMainMenu() => SceneManager.LoadScene(0);

        private void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
