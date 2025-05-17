
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
