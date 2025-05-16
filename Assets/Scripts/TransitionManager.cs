using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("Transition Settings")]
    public Animator leafAnimator;
    public float coverDuration = 1f;
    public float uncoverDuration = 1f;

    [Header("Countdown Settings")]
    public Animator countdownAnimator;
    public float countdownStartDelay = 0.1f; // Very short delay after uncover starts

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            StartCoroutine(GameEntrySequence());
        }
    }

    public void LoadGameScene()
    {
        StartCoroutine(TransitionToGame());
    }

    IEnumerator TransitionToGame()
    {
        // Play cover animation and wait for it to complete
        leafAnimator.SetTrigger("Cover");
        yield return new WaitForSeconds(coverDuration);

        // Leaves will stay covered (animation holds on last frame)
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator GameEntrySequence()
    {
        // Start uncover animation
        leafAnimator.SetTrigger("Uncover");

        // Very short delay before countdown starts
        yield return new WaitForSeconds(countdownStartDelay);

        // Start countdown animation while uncover is still playing
        countdownAnimator.SetTrigger("Start");

        // Freeze game during transition
        Time.timeScale = 0f;
        leafAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        countdownAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        // Wait for uncover animation to complete
        yield return new WaitForSecondsRealtime(uncoverDuration - countdownStartDelay);

        // Wait for countdown to complete
        yield return new WaitForSecondsRealtime(
            countdownAnimator.GetCurrentAnimatorStateInfo(0).length
        );

        // Unfreeze game
        Time.timeScale = 1f;

        // Start game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}
