using UnityEngine;
using MusicNoteGame.Gameplay;

namespace MusicNoteGame.Core
{
    public class GameSceneInitializer : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        private void Start()
        {
            ClefType clef = (ClefType)PlayerPrefs.GetInt("SelectedClef", 0);
            GameMode mode = (GameMode)PlayerPrefs.GetInt("SelectedMode", 1);
            int difficulty = PlayerPrefs.GetInt("SelectedDifficulty", 1);
            gameManager?.StartGame(clef, mode, difficulty);
        }
    }
}
