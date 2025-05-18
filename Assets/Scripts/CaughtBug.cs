using UnityEngine;
using System.Collections;
public class CaughtBug : MonoBehaviour
{
    [Header("Timing Settings")]
    public float defaultDespawnTime = 5f;
    public float breakAnimationDuration = 0.5f;

    [Header("References")]
    public Animator animator;
    public ParticleSystem breakParticles;

    private bool isBreaking = false;

    private void Start()
    {
        Invoke("StartBreakingSequence", defaultDespawnTime);
    }

    private void OnMouseDown()
    {
        if (isBreaking) return;
        if (ToolManager.Instance?.currentTool != ToolManager.ToolMode.SpiderTool)
            return;

        var effects = GetComponent<CaughtBugEffects>();
        if (effects != null)
        {
            effects.ApplyEffects(); // Make sure this runs
            Debug.Log("Effects applied for: " + effects.bugType);
        }

        // Play sound and destroy
        if (ToolManager.Instance.spiderEatSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, ToolManager.Instance.spiderEatSounds.Length);
            ToolManager.Instance.audioSource.PlayOneShot(ToolManager.Instance.spiderEatSounds[randomIndex]);
        }

        Destroy(gameObject); // Destroy after applying effects
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void StartBreakingSequence()
    {
        if (isBreaking) return;
        isBreaking = true;

        animator?.SetTrigger("Break");
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        Destroy(gameObject, breakAnimationDuration);
    }
}