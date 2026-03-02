using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicNoteGame.Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<LevelConfig> levels = new List<LevelConfig>();

        public int CurrentLevelIndex { get; private set; }
        public LevelConfig CurrentLevel => GetLevel(CurrentLevelIndex);
        public int TotalLevels => levels.Count;

        public event Action<int> OnLevelChanged;
        public event Action OnAllLevelsCompleted;

        public void Initialize(int startingLevelIndex = 0)
        {
            CurrentLevelIndex = Mathf.Clamp(startingLevelIndex, 0, levels.Count - 1);
        }

        public LevelConfig GetLevel(int index)
        {
            return (index >= 0 && index < levels.Count) ? levels[index] : null;
        }

        public bool AdvanceToNextLevel()
        {
            if (CurrentLevelIndex < levels.Count - 1)
            {
                CurrentLevelIndex++;
                OnLevelChanged?.Invoke(CurrentLevelIndex);
                return true;
            }
            OnAllLevelsCompleted?.Invoke();
            return false;
        }

        public void ResetToFirstLevel()
        {
            CurrentLevelIndex = 0;
            OnLevelChanged?.Invoke(CurrentLevelIndex);
        }

        public float GetCurrentTimeLimit() => CurrentLevel?.timePerNote ?? 10f;
        public int GetCurrentNotesToComplete() => CurrentLevel?.notesToComplete ?? 5;
    }
}
