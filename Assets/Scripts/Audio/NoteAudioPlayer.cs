using UnityEngine;
using MusicNoteGame.Gameplay;

namespace MusicNoteGame.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class NoteAudioPlayer : MonoBehaviour
    {
        private static NoteAudioPlayer instance;
        public static NoteAudioPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<NoteAudioPlayer>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("NoteAudioPlayer");
                        instance = go.AddComponent<NoteAudioPlayer>();
                    }
                }
                return instance;
            }
        }

        [SerializeField] private float volume = 0.5f;
        [SerializeField] private float duration = 0.5f;

        private AudioSource audioSource;
        private float frequency;
        private float phase;
        private bool isPlaying;
        private bool isError;
        private float playTime;
        private int sampleRate;

        // Note frequencies (Hz) for octave 4
        private static readonly float[] NoteFrequencies = new float[]
        {
            261.63f, // C
            293.66f, // D
            329.63f, // E
            349.23f, // F
            392.00f, // G
            440.00f, // A
            493.88f  // B
        };

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // Critical Setup for 2D Sound
            audioSource.spatialBlend = 0f; // 2D (hear everywhere)
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            
            // Create a dummy clip to allow the AudioSource to play
            // OnAudioFilterRead needs the source to be playing to function
            // Create a dummy clip to force playback state
            if (audioSource.clip == null)
            {
                audioSource.clip = AudioClip.Create("ProceduralTone", 44100, 1, 44100, false);
            }
            
            audioSource.Stop(); // Ensure clean state
            sampleRate = AudioSettings.outputSampleRate;
        }

        private void Start()
        {
            // We need the AudioSource to be "playing" for OnAudioFilterRead to be called
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        public void PlayNote(NoteData note)
        {
            frequency = GetFrequency(note.noteName, note.octave);
            phase = 0;
            playTime = 0;
            isPlaying = true;
            isError = false;
        }

        public void PlayNote(NoteName noteName, int octave = 4)
        {
            frequency = GetFrequency(noteName, octave);
            phase = 0;
            playTime = 0;
            isPlaying = true;
            isError = false;
        }

        public void PlayError()
        {
            frequency = 150f; // Low buzz
            phase = 0;
            playTime = 0;
            isPlaying = true;
            isError = true;
        }

        public void StopNote()
        {
            isPlaying = false;
        }

        private float GetFrequency(NoteName note, int octave)
        {
            float baseFreq = NoteFrequencies[(int)note];
            // Adjust for octave (octave 4 is the base)
            int octaveDiff = octave - 4;
            return baseFreq * Mathf.Pow(2, octaveDiff);
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (!isPlaying) return;

            float increment = frequency * 2f * Mathf.PI / sampleRate;

            for (int i = 0; i < data.Length; i += channels)
            {
                // Generate wave
                float sample;
                if (isError)
                {
                    // Square wave for error buzz
                    sample = Mathf.Sign(Mathf.Sin(phase)) * volume * 0.5f;
                }
                else
                {
                    // Sine wave for music notes
                    sample = Mathf.Sin(phase) * volume;
                }

                // Apply envelope (fade out)
                float envelope = 1f - (playTime / duration);
                if (envelope < 0) envelope = 0;
                sample *= envelope;

                // Apply to all channels
                for (int c = 0; c < channels; c++)
                {
                    data[i + c] = sample;
                }

                phase += increment;
                if (phase > 2 * Mathf.PI) phase -= 2 * Mathf.PI;

                playTime += 1f / sampleRate;
            }

            if (playTime >= duration)
            {
                isPlaying = false;
            }
        }
    }
}
