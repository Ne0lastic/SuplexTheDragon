using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public AudioClip backgroundMusic;  // Drag your MP3 file here
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }
}
