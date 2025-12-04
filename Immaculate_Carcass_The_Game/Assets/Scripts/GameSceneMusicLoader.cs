using UnityEngine;

public class GameSceneMusicLoader : MonoBehaviour
{
    public AudioClip overworldMusic;  // Assign in Inspector
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (overworldMusic != null)
        {
            audioSource.clip = overworldMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No overworld music assigned!");
        }
    }
}
