using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    public AudioSource musicSource;   // overworld/combat/victory themes
    public AudioSource sfxSource;     // all one-shot sound effects

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);      // avoid duplicates in scene loads
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Plays background music (loops by default)
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("Tried to play music but clip was null.");
            return;
        }

        musicSource.loop = loop;
        musicSource.clip = clip;
        musicSource.Play();
    }

    // Plays a single sound effect
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Tried to play SFX but clip was null.");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}
