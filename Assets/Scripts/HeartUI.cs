using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    public Animator animator;
    public Image heartImage;
    private bool isActive = false;

    public void PlayGainAnimation()
    {
        if (!isActive)
        {
            isActive = true;
            heartImage.enabled = true;
            animator.SetTrigger("Gain");
        }
    }

    public void PlayLoseAnimation()
    {
        if (isActive)
        {
            isActive = false;
            animator.SetTrigger("Lose");
        }
    }

    public void EnableHeart()
    {
        isActive = true;
        heartImage.enabled = true;
    }

    // Called by animation event at end of Lose animation
    public void DisableHeart()
    {
        heartImage.enabled = false;
    }
}
