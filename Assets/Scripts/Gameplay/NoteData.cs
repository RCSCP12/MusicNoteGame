using System;
using UnityEngine;

namespace MusicNoteGame.Gameplay
{
    public enum NoteName
    {
        C = 0,
        D = 1,
        E = 2,
        F = 3,
        G = 4,
        A = 5,
        B = 6
    }

    public enum ClefType
    {
        Treble,
        Bass
    }

    [Serializable]
    public class NoteData
    {
        public NoteName noteName;
        public int octave;

        public NoteData(NoteName name, int oct)
        {
            noteName = name;
            octave = oct;
        }

        public int GetStaffPosition(ClefType clef)
        {
            if (clef == ClefType.Treble)
            {
                int basePosition = GetAbsolutePosition();
                int e4Position = 4 * 7 + 2;
                return basePosition - e4Position;
            }
            else
            {
                int basePosition = GetAbsolutePosition();
                int g2Position = 2 * 7 + 4;
                return basePosition - g2Position;
            }
        }

        private int GetAbsolutePosition()
        {
            return octave * 7 + (int)noteName;
        }

        public bool NeedsLedgerLines(ClefType clef)
        {
            int pos = GetStaffPosition(clef);
            return pos < 0 || pos > 8;
        }

        public int GetLedgerLineCount(ClefType clef)
        {
            int pos = GetStaffPosition(clef);
            if (pos < 0)
                return (-pos + 1) / 2;
            else if (pos > 8)
                return (pos - 8 + 1) / 2;
            return 0;
        }

        public override string ToString()
        {
            return $"{noteName}{octave}";
        }

        public override bool Equals(object obj)
        {
            if (obj is NoteData other)
                return noteName == other.noteName && octave == other.octave;
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(noteName, octave);
        }

        public static NoteName KeyCodeToNoteName(KeyCode key)
        {
            return key switch
            {
                KeyCode.A => NoteName.A,
                KeyCode.B => NoteName.B,
                KeyCode.C => NoteName.C,
                KeyCode.D => NoteName.D,
                KeyCode.E => NoteName.E,
                KeyCode.F => NoteName.F,
                KeyCode.G => NoteName.G,
                _ => throw new ArgumentException($"Invalid key code: {key}")
            };
        }
    }
}
