using System.Collections.Generic;
using UnityEngine;

namespace MusicNoteGame.Gameplay
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "MusicNoteGame/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Level Info")]
        public int levelNumber = 1;
        public string levelName = "Level 1";

        [Header("Note Configuration")]
        public List<NoteDefinition> trebleClefNotes = new List<NoteDefinition>();
        public List<NoteDefinition> bassClefNotes = new List<NoteDefinition>();

        [Header("Timing")]
        public float timePerNote = 10f;

        [Header("Progression")]
        public int notesToComplete = 5;
        public bool usesLedgerLines = false;

        public List<NoteData> GetNotesForClef(ClefType clef)
        {
            var notes = new List<NoteData>();
            var definitions = clef == ClefType.Treble ? trebleClefNotes : bassClefNotes;

            foreach (var def in definitions)
                notes.Add(new NoteData(def.noteName, def.octave));

            return notes;
        }
    }

    [System.Serializable]
    public class NoteDefinition
    {
        public NoteName noteName;
        public int octave;
    }
}
