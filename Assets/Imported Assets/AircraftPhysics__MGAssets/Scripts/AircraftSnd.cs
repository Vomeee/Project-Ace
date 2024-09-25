using UnityEngine;

namespace MGAssets
{
    namespace AircraftPhysics
    {
        public class AircraftSnd : MonoBehaviour
        {
            public static AircraftSnd current;
            public AudioSource audioSource;


            public bool mute = false;
            public float volume = 1;

            [Space]
            public AudioClip clickSND;


            ///////////// Inicialization
            void Awake()
            {
                if (audioSource == null) audioSource = GetComponent<AudioSource>();
            }
            void OnEnable() { current = this; }
            void OnDisable() { if (current == this) current = null; }
            /////////////




            /////////// Play Sounds
            public void playClip(AudioClip clip, float volume = 1f) { if (!mute) audioSource.PlayOneShot(clip, volume); }
            public void playClick() { if (!mute && clickSND) audioSource.PlayOneShot(clickSND, volume); }
            ///////////


            /////////  External Static Calls
            public static void Play(AudioClip clip, float volume = 1f) { if (current != null) AircraftSnd.current.playClip(clip, volume); }
            public static void PlayClick() { if (current != null) AircraftSnd.current.playClick(); }
            /////////
        }
    }
}