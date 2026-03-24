using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MusicNoteGame.Gameplay;

namespace MusicNoteGame.Input
{
    public class NoteInputHandler : MonoBehaviour
    {
        /// <summary>
        /// Fired when the player presses a note key (A–G).
        /// Event arguments: (NoteName note, bool isSharp, bool isFlat)
        /// </summary>
        public event Action<NoteName, bool, bool> OnNoteInput;
        public bool IsEnabled { get; private set; } = true;

        /// <summary>True while CapsLock is held down.</summary>
        public bool IsFlatActive => Keyboard.current != null && Keyboard.current.capsLockKey.isPressed;

        public bool IsSharpActive => Keyboard.current != null && (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

        public void EnableInput() => IsEnabled = true;
        public void DisableInput()
        {
            IsEnabled = false;
        }

        private void Update()
        {
            if (!IsEnabled) return;
            if (Keyboard.current == null) return;

            // Note keys A–G
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                FireNote(NoteName.A);
                return;
            }
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                FireNote(NoteName.B);
                return;
            }
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                FireNote(NoteName.C);
                return;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                FireNote(NoteName.D);
                return;
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                FireNote(NoteName.E);
                return;
            }
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                FireNote(NoteName.F);
                return;
            }
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                FireNote(NoteName.G);
                return;
            }
        }

        private void FireNote(NoteName note)
        {
            // Sharp is activated when holding Shift
            bool sharp = IsSharpActive;
            
            // Flat is activated when holding CapsLock
            bool flat = IsFlatActive;
            
            // Mutually exclusive: if holding shift to sharp, it overrides flat.
            if (sharp && flat) flat = false;

            OnNoteInput?.Invoke(note, sharp, flat);
        }
    }
}
