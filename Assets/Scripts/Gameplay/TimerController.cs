using System;
using UnityEngine;

namespace MusicNoteGame.Gameplay
{
    public class TimerController : MonoBehaviour
    {
        public float TimeRemaining { get; private set; }
        public float MaxTime { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPracticeMode { get; private set; }

        public event Action OnTimeExpired;
        public event Action<float, float> OnTimeUpdated;

        public void StartTimer(float duration)
        {
            if (duration <= 0)
            {
                IsPracticeMode = true;
                IsRunning = false;
                return;
            }

            IsPracticeMode = false;
            MaxTime = duration;
            TimeRemaining = duration;
            IsRunning = true;
            OnTimeUpdated?.Invoke(TimeRemaining, MaxTime);
        }

        public void StopTimer() => IsRunning = false;
        public void PauseTimer() => IsRunning = false;

        public void ResumeTimer()
        {
            if (!IsPracticeMode && TimeRemaining > 0) IsRunning = true;
        }

        private void Update()
        {
            if (!IsRunning || IsPracticeMode) return;

            TimeRemaining -= Time.deltaTime;
            OnTimeUpdated?.Invoke(TimeRemaining, MaxTime);

            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                IsRunning = false;
                OnTimeExpired?.Invoke();
            }
        }

        public float GetTimePercentage()
        {
            return MaxTime <= 0 ? 1f : Mathf.Clamp01(TimeRemaining / MaxTime);
        }
    }
}
