
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource; // Your main AudioSource
    public AudioClip secondarySound; // A completely different sound

    // Plays the default sound (original functionality)
    public void PlayAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    // Plays the specific secondary sound
    public void PlaySecondarySound()
    {
        if (audioSource != null && secondarySound != null)
        {
            audioSource.PlayOneShot(secondarySound);
        }
    }
}
