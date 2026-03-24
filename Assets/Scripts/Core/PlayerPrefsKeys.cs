namespace MusicNoteGame.Core
{
    public static class PlayerPrefsKeys
    {
        public const string SelectedClef       = "SelectedClef";
        public const string SelectedMode       = "SelectedMode";
        public const string SelectedDifficulty = "SelectedDifficulty";
        public const string ChordsEnabled      = "ChordsEnabled";

        public static string HighScore(Gameplay.ClefType clef, int difficulty)
            => $"HighScore_{clef}_{difficulty}";
    }
}
