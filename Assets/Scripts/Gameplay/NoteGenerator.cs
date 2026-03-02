using System.Collections.Generic;
using UnityEngine;

namespace MusicNoteGame.Gameplay
{
    public class NoteGenerator : MonoBehaviour
    {
        private List<NoteData> availableNotes = new List<NoteData>();
        private NoteData lastGeneratedNote;
        private ClefType currentClef;

        public void Initialize(LevelConfig levelConfig, ClefType clef)
        {
            currentClef = clef;
            availableNotes = levelConfig.GetNotesForClef(clef);
            lastGeneratedNote = null;
        }

        public NoteData GenerateRandomNote()
        {
            if (availableNotes == null || availableNotes.Count == 0)
            {
                Debug.LogError("NoteGenerator: No available notes!");
                return null;
            }

            NoteData newNote;
            int attempts = 0;

            do
            {
                int randomIndex = Random.Range(0, availableNotes.Count);
                newNote = availableNotes[randomIndex];
                attempts++;
            }
            while (availableNotes.Count > 1 && lastGeneratedNote != null &&
                   newNote.Equals(lastGeneratedNote) && attempts < 10);

            lastGeneratedNote = newNote;
            return new NoteData(newNote.noteName, newNote.octave);
        }

        public ClefType GetCurrentClef() => currentClef;
    }
}
