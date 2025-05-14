using UnityEngine;

public class CaughtBug : MonoBehaviour
{
    public int scoreValue = 10; // Points per caught bug
    public float despawnTime = 5f; // Time before the breaking animation starts
    public Animator animator; // Reference to Animator component

    private bool isBreaking = false; // Track if the bug is breaking

    private void Start()
    {
        // Start despawn timer
        Invoke("StartBreakingAnimation", despawnTime);
    }

    private void OnMouseDown()
    {
        // Only allow interaction if it's not breaking and Pickup Tool is active
        if (isBreaking || ToolManager.Instance.currentTool != ToolManager.ToolMode.SpiderTool)
        {
            return;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        // Destroy immediately upon clicking
        Destroy(gameObject);
    }

    private void StartBreakingAnimation()
    {
        isBreaking = true; // Prevent further interaction

        if (animator != null)
        {
            animator.SetTrigger("Break"); // Trigger breaking animation
        }

        // Destroy bug after the animation plays (adjust time if needed)
        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        Invoke("Despawn", animationDuration);
    }

    private void Despawn()
    {
        Destroy(gameObject); // Remove bug after animation
    }
}




