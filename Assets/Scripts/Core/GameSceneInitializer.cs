using UnityEngine;
using MusicNoteGame.Gameplay;

namespace MusicNoteGame.Core
{
    public class GameSceneInitializer : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private void Start()
        {
            int clefRaw = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedClef, 0);
            int modeRaw = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedMode, 1);
            int difficulty = PlayerPrefs.GetInt(PlayerPrefsKeys.SelectedDifficulty, 1);

            if (!System.Enum.IsDefined(typeof(ClefType), clefRaw))
            {
                Debug.LogWarning($"GameSceneInitializer: Invalid SelectedClef value {clefRaw}, defaulting to Treble.");
                clefRaw = 0;
            }
            if (!System.Enum.IsDefined(typeof(GameMode), modeRaw))
            {
                Debug.LogWarning($"GameSceneInitializer: Invalid SelectedMode value {modeRaw}, defaulting to Practice.");
                modeRaw = 0;
            }

            gameManager?.StartGame((ClefType)clefRaw, (GameMode)modeRaw, difficulty);
        }
    }
}
