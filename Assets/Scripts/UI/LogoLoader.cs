using UnityEngine;
using UnityEngine.UI;

namespace MusicNoteGame.UI
{
    [RequireComponent(typeof(Image))]
    public class LogoLoader : MonoBehaviour
    {
        [SerializeField] private string resourcePath = "Sprites/RCSLogo";

        private void Awake()
        {
            var sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite != null)
                GetComponent<Image>().sprite = sprite;
            else
                Debug.LogWarning($"[LogoLoader] Could not load sprite at Resources/{resourcePath}");
        }
    }
}
