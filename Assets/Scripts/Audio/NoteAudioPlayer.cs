using UnityEngine;
using MusicNoteGame.Gameplay;
using System.Collections.Generic;

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
        private int sampleRate;

        private class SynthVoice
        {
            public float frequency;
            public float phase;
            public float playTime;
            public bool isPlaying;
            public bool isError;
        }

        private const int MAX_VOICES = 16;
        private SynthVoice[] voices = new SynthVoice[MAX_VOICES];

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
            
            audioSource.spatialBlend = 0f; // 2D (hear everywhere)
            audioSource.playOnAwake = false;
            audioSource.loop = true;
            
            if (audioSource.clip == null)
            {
                audioSource.clip = AudioClip.Create("ProceduralTone", 44100, 1, 44100, false);
            }
            
            audioSource.Stop(); 
            sampleRate = AudioSettings.outputSampleRate;

            for (int i = 0; i < MAX_VOICES; i++)
            {
                voices[i] = new SynthVoice();
            }
        }

        private void Start()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        private SynthVoice GetFreeVoice()
        {
            SynthVoice oldestVoice = voices[0];
            for (int i = 0; i < MAX_VOICES; i++)
            {
                if (!voices[i].isPlaying)
                    return voices[i];
                
                if (voices[i].playTime > oldestVoice.playTime)
                    oldestVoice = voices[i];
            }
            // If all voices are taken, overwrite the oldest one
            return oldestVoice;
        }

        public void PlayNote(NoteData note)
        {
            SynthVoice voice = GetFreeVoice();
            voice.frequency = GetFrequency(note);
            voice.phase = 0;
            voice.playTime = 0;
            voice.isError = false;
            voice.isPlaying = true;
        }

        public void PlayNote(NoteName noteName, int octave = 4)
        {
            SynthVoice voice = GetFreeVoice();
            float baseFreq = NoteFrequencies[(int)noteName];
            int octaveDiff = octave - 4;
            voice.frequency = baseFreq * Mathf.Pow(2, octaveDiff);
            voice.phase = 0;
            voice.playTime = 0;
            voice.isError = false;
            voice.isPlaying = true;
        }

        public void PlayError()
        {
            SynthVoice voice = GetFreeVoice();
            voice.frequency = 150f; // Low buzz
            voice.phase = 0;
            voice.playTime = 0;
            voice.isError = true;
            voice.isPlaying = true;
        }

        public void StopNote()
        {
            for (int i = 0; i < MAX_VOICES; i++)
            {
                voices[i].isPlaying = false;
            }
        }

        private float GetFrequency(NoteData note)
        {
            float baseFreq = NoteFrequencies[(int)note.noteName];
            
            // Adjust for sharp/flat by multiplying/dividing by the 12th root of 2
            if (note.isSharp) baseFreq *= 1.059463f;
            else if (note.isFlat) baseFreq *= 0.943874f;

            // Adjust for octave (octave 4 is the base)
            int octaveDiff = note.octave - 4;
            return baseFreq * Mathf.Pow(2, octaveDiff);
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                float mixedSample = 0f;
                int activeVoices = 0;

                for (int v = 0; v < MAX_VOICES; v++)
                {
                    SynthVoice voice = voices[v];
                    if (!voice.isPlaying) continue;

                    activeVoices++;
                    float increment = voice.frequency * 2f * Mathf.PI / sampleRate;

                    float sample;
                    if (voice.isError)
                    {
                        // Square wave for error buzz
                        sample = Mathf.Sign(Mathf.Sin(voice.phase)) * volume * 0.5f;
                    }
                    else
                    {
                        // Sine wave for music notes
                        sample = Mathf.Sin(voice.phase) * volume;
                    }

                    // Apply envelope (fade out)
                    float envelope = 1f - (voice.playTime / duration);
                    if (envelope < 0) envelope = 0;
                    sample *= envelope;

                    mixedSample += sample;

                    voice.phase += increment;
                    if (voice.phase > 2 * Mathf.PI) voice.phase -= 2 * Mathf.PI;

                    voice.playTime += 1f / sampleRate;

                    if (voice.playTime >= duration)
                    {
                        voice.isPlaying = false;
                    }
                }

                if (activeVoices > 0)
                {
                    // Soft clipping/normalizing to prevent distortion with multiple simultaneous notes
                    float normalization = Mathf.Max(1f, activeVoices * 0.4f);
                    mixedSample /= normalization;
                }

                // Apply to all channels
                for (int c = 0; c < channels; c++)
                {
                    data[i + c] = mixedSample;
                }
            }
        }
    }
}
