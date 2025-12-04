using UnityEngine;

public class CombatMusicLoader : MonoBehaviour
{
    public AudioClip combatMusic;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (combatMusic != null)
        {
            audioSource.clip = combatMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("CombatMusicLoader: No combat music assigned!");
        }
    }
}
