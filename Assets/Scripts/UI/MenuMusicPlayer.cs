using UnityEngine;

namespace MusicNoteGame.UI
{
    [RequireComponent(typeof(AudioSource))]
    public class MenuMusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] [Range(0f, 1f)] private float volume = 0.4f;

        private void Start()
        {
            if (clip == null)
            {
                Debug.LogWarning("[MenuMusicPlayer] No audio clip assigned.");
                return;
            }

            var source = GetComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.volume = volume;
            source.playOnAwake = false;
            source.Play();
        }
    }
}
