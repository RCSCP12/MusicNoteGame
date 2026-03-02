using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MusicNoteGame.Gameplay;

namespace MusicNoteGame.Input
{
    public class NoteInputHandler : MonoBehaviour
    {
        public event Action<NoteName> OnNoteInput;
        public bool IsEnabled { get; private set; } = true;

        public void EnableInput() => IsEnabled = true;
        public void DisableInput() => IsEnabled = false;

        private void Update()
        {
            if (!IsEnabled) return;

            // Check each note key using the new Input System
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                OnNoteInput?.Invoke(NoteName.A);
                return;
            }
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                OnNoteInput?.Invoke(NoteName.B);
                return;
            }
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                OnNoteInput?.Invoke(NoteName.C);
                return;
            }
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                OnNoteInput?.Invoke(NoteName.D);
                return;
            }
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                OnNoteInput?.Invoke(NoteName.E);
                return;
            }
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                OnNoteInput?.Invoke(NoteName.F);
                return;
            }
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                OnNoteInput?.Invoke(NoteName.G);
                return;
            }
        }
    }
}
