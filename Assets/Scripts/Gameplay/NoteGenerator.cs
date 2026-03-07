using System.Collections.Generic;
using UnityEngine;

namespace MusicNoteGame.Gameplay
{
    public class NoteGenerator : MonoBehaviour
    {
        private List<NoteData> availableNotes = new List<NoteData>();
        private NoteData lastGeneratedNote;
        private ClefType currentClef;
        private LevelConfig currentLevelConfig;

        public void Initialize(LevelConfig levelConfig, ClefType clef)
        {
            currentClef = clef;
            currentLevelConfig = levelConfig;
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

            // Note: if the level definition *forced* a sharp/flat, keep it.
            // Otherwise, roll probabilistically.
            bool isSharp = newNote.isSharp;
            bool isFlat = newNote.isFlat;

            if (!isSharp && !isFlat && currentLevelConfig != null)
            {
                float roll = Random.value;
                if (roll < currentLevelConfig.sharpProbability)
                {
                    isSharp = true;
                }
                else if (roll < currentLevelConfig.sharpProbability + currentLevelConfig.flatProbability)
                {
                    isFlat = true;
                }
            }

            lastGeneratedNote = newNote;
            NoteData generatedNoteData = new NoteData(newNote.noteName, newNote.octave, isSharp, isFlat);
            
            Debug.Log($"[NoteGenerator] Spawned Note: {generatedNoteData} (isSharp: {isSharp}, isFlat: {isFlat})");
            
            return generatedNoteData;
        }

        public List<NoteData> GenerateNotes(bool isChord)
        {
            List<NoteData> result = new List<NoteData>();
            
            NoteData baseNote = GenerateRandomNote();
            if (baseNote == null) return result;
            
            result.Add(baseNote);
            
            if (isChord)
            {
                // Define the possible chord formulas
                // Arrays represent (semitones from root, note name steps from root)
                var chordFormulas = new List<(int[] semitones, int[] nameSteps)>
                {
                    (new[] {4, 7}, new[] {2, 4}), // Major (4 semitones, 7 semitones)
                    (new[] {3, 7}, new[] {2, 4}), // Minor
                    (new[] {3, 6}, new[] {2, 4}), // Diminished
                    (new[] {4, 8}, new[] {2, 4}), // Augmented
                    (new[] {2, 7}, new[] {1, 4}), // Sus2
                    (new[] {5, 7}, new[] {3, 4})  // Sus4
                };

                var formula = chordFormulas[Random.Range(0, chordFormulas.Count)];
                
                int rootSemitone = GetAbsoluteSemitone(baseNote);
                int rootNameIndex = (int)baseNote.noteName;
                int rootOctave = baseNote.octave;

                for (int i = 0; i < formula.semitones.Length; i++)
                {
                    int targetSemitone = rootSemitone + formula.semitones[i];
                    int targetNameIndex = rootNameIndex + formula.nameSteps[i];
                    
                    int targetOctave = rootOctave + (targetNameIndex / 7);
                    NoteName targetName = (NoteName)(targetNameIndex % 7);
                    
                    int baseTargetSemitone = GetBaseSemitone(targetName, targetOctave);
                    int semitoneDiff = targetSemitone - baseTargetSemitone;
                    
                    bool isSharp = false;
                    bool isFlat = false;

                    // Handle standard accidentals (simulating double sharps/flats with valid spelling if necessary,
                    // but usually keeping it simple for standard 12 pitch classes).
                    if (semitoneDiff > 0)
                    {
                        // E.g., if semitoneDiff is 1 or 2. NoteData only has isSharp.
                        // We will map it to a sharp for gameplay purposes.
                        isSharp = true;
                    }
                    else if (semitoneDiff < 0)
                    {
                        isFlat = true;
                    }

                    // In cases of double sharps/flats (semitoneDiff = 2 or -2),
                    // this simple approach creates a sharp/flat. It's an approximation 
                    // that works well for visual gameplay in this context.
                    
                    NoteData chordNote = new NoteData(targetName, targetOctave, isSharp, isFlat);
                    result.Add(chordNote);
                }
            }
            return result;
        }

        private int GetAbsoluteSemitone(NoteData note)
        {
            int baseSemi = GetBaseSemitone(note.noteName, note.octave);
            if (note.isSharp) baseSemi += 1;
            if (note.isFlat) baseSemi -= 1;
            return baseSemi;
        }

        private int GetBaseSemitone(NoteName name, int octave)
        {
            int[] baseSemitones = { 0, 2, 4, 5, 7, 9, 11 }; // C, D, E, F, G, A, B
            return (octave * 12) + baseSemitones[(int)name];
        }

        public ClefType GetCurrentClef() => currentClef;
    }
}
