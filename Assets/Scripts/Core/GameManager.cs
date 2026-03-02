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

        private NoteData currentNote;
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
            SetupEventListeners();
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

        public void StartGame(ClefType clef, GameMode mode, int difficulty = 1)
        {
            CurrentClef = clef;
            CurrentMode = mode;
            CurrentDifficulty = difficulty;

            correctAnswers = 0;
            // Initialize LevelManager with the selected difficulty/level index
            levelManager.Initialize(difficulty);
            
            // Set difficulty settings
            // Set difficulty from Level Manager which uses LevelConfig
            notesToWin = levelManager.GetCurrentNotesToComplete();
            timePerNote = levelManager.GetCurrentTimeLimit();
            scoreManager.ResetScore();
            staffDisplay.SetClef(clef);

            // Use first level config for notes
            var levelConfig = levelManager.CurrentLevel;
            noteGenerator.Initialize(levelConfig, CurrentClef);

            hudController?.UpdateLevel(1, 1); // Single round based on difficulty
            hudController?.UpdateProgress(correctAnswers, notesToWin);

            ChangeState(GameState.Playing);
            GenerateNewNote();
        }

        private void GenerateNewNote()
        {
            waitingForNextNote = false;
            currentNote = noteGenerator.GenerateRandomNote();
            noteDisplay.DisplayNote(currentNote, CurrentClef);
            NoteAudioPlayer.Instance?.PlayNote(currentNote);

            if (CurrentMode == GameMode.TimedChallenge)
                timerController.StartTimer(timePerNote);

            inputHandler.EnableInput();
        }

        private void HandleNoteInput(NoteName inputNote)
        {
            if (CurrentState != GameState.Playing || waitingForNextNote) return;

            inputHandler.DisableInput();
            timerController.StopTimer();

            if (inputNote == currentNote.noteName)
                HandleCorrectAnswer();
            else
                HandleIncorrectAnswer(inputNote);
        }

        private void HandleCorrectAnswer()
        {
            correctAnswers++;
            NoteAudioPlayer.Instance?.PlayNote(currentNote);
            scoreManager.RecordCorrectAnswer(timerController.TimeRemaining, timerController.MaxTime);
            noteDisplay.ShowCorrectFeedback();
            hudController?.ShowFeedback(true, currentNote.noteName.ToString());
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

        private void HandleIncorrectAnswer(NoteName inputNote)
        {
            scoreManager.RecordIncorrectAnswer();
            NoteAudioPlayer.Instance?.PlayError();
            noteDisplay.ShowIncorrectFeedback();
            hudController?.ShowFeedback(false, currentNote.noteName.ToString(), inputNote.ToString());
            waitingForNextNote = true;
            Invoke(nameof(GenerateNewNote), feedbackDelay * 2);
        }

        private void HandleTimeExpired()
        {
            if (CurrentState != GameState.Playing) return;
            inputHandler.DisableInput();
            scoreManager.RecordIncorrectAnswer();
            NoteAudioPlayer.Instance?.PlayError();
            hudController?.ShowFeedback(false, currentNote.noteName.ToString(), "Timeout");
            waitingForNextNote = true;
            Invoke(nameof(GenerateNewNote), feedbackDelay * 2);
        }

        private void ShowGameOver(bool isVictory)
        {
            bool isNewHighScore = scoreManager.SaveHighScoreIfBetter(CurrentClef, CurrentDifficulty);
            gameOverPanel?.Show(isVictory, scoreManager.TotalScore, scoreManager.CorrectAnswers,
                scoreManager.IncorrectAnswers, scoreManager.GetAccuracy(), scoreManager.BestStreak, isNewHighScore);
        }

        public void RestartGame() => StartGame(CurrentClef, CurrentMode, CurrentDifficulty);
        public void ReturnToMainMenu() => SceneManager.LoadScene(0);

        private void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
