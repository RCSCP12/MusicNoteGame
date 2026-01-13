using System;
using UnityEngine;

namespace MusicNoteGame.Gameplay
{
    public class ScoreManager : MonoBehaviour
    {
        public int CorrectAnswers { get; private set; }
        public int IncorrectAnswers { get; private set; }
        public int CurrentStreak { get; private set; }
        public int BestStreak { get; private set; }
        public int TotalScore { get; private set; }

        public event Action<int> OnScoreChanged;
        public event Action<int> OnStreakChanged;

        private const int BASE_POINTS = 100;
        private const int STREAK_BONUS = 10;

        public void ResetScore()
        {
            CorrectAnswers = 0;
            IncorrectAnswers = 0;
            CurrentStreak = 0;
            BestStreak = 0;
            TotalScore = 0;
            OnScoreChanged?.Invoke(TotalScore);
            OnStreakChanged?.Invoke(CurrentStreak);
        }

        public void RecordCorrectAnswer(float timeRemaining = 0f, float maxTime = 1f)
        {
            CorrectAnswers++;
            CurrentStreak++;
            if (CurrentStreak > BestStreak) BestStreak = CurrentStreak;

            int streakBonus = CurrentStreak * STREAK_BONUS;
            int timeBonus = maxTime > 0 ? Mathf.RoundToInt((timeRemaining / maxTime) * 50) : 0;
            TotalScore += BASE_POINTS + streakBonus + timeBonus;

            OnScoreChanged?.Invoke(TotalScore);
            OnStreakChanged?.Invoke(CurrentStreak);
        }

        public void RecordIncorrectAnswer()
        {
            IncorrectAnswers++;
            CurrentStreak = 0;
            OnStreakChanged?.Invoke(CurrentStreak);
        }

        public float GetAccuracy()
        {
            int total = CorrectAnswers + IncorrectAnswers;
            return total == 0 ? 0f : (float)CorrectAnswers / total * 100f;
        }

        public int GetHighScore(ClefType clef, int difficulty)
        {
            string key = $"HighScore_{clef}_{difficulty}";
            return PlayerPrefs.GetInt(key, 0);
        }

        public bool SaveHighScoreIfBetter(ClefType clef, int difficulty)
        {
            string key = $"HighScore_{clef}_{difficulty}";
            if (TotalScore > PlayerPrefs.GetInt(key, 0))
            {
                PlayerPrefs.SetInt(key, TotalScore);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }
    }
}
