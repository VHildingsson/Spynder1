using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

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
    public Button playButton;

    public AudioClip leavesSound; // Sound to play when spawning a bug
    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            StartCoroutine(GameEntrySequence());
        }
    }

    public void LoadGameScene()
    {
        // Disable the play button when pressed
        if (playButton != null)
        {
            playButton.interactable = false;
        }

        StartCoroutine(TransitionToGame());
    }

    IEnumerator TransitionToGame()
    {
        leafAnimator.SetTrigger("Cover");
        if (leavesSound != null)
        {
            audioSource.PlayOneShot(leavesSound);
        }
        yield return new WaitForSeconds(coverDuration);

        SceneManager.LoadScene("GameScene");
    }

    IEnumerator GameEntrySequence()
    {
        // Start uncover animation
        leafAnimator.SetTrigger("Uncover");

        if (leavesSound != null)
        {
            audioSource.PlayOneShot(leavesSound);
        }

        // Very short delay before countdown starts
        yield return new WaitForSeconds(countdownStartDelay);

        // Only proceed with countdown if countdownAnimator exists
        if (countdownAnimator != null)
        {
            // Start countdown animation while uncover is still playing
            countdownAnimator.SetTrigger("Start");

            // Freeze game during transition
            Time.timeScale = 0f;
            leafAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            countdownAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

            // Wait for countdown to complete
            yield return new WaitForSecondsRealtime(
                countdownAnimator.GetCurrentAnimatorStateInfo(0).length
            );
        }

        // Wait for uncover animation to complete
        yield return new WaitForSecondsRealtime(uncoverDuration - countdownStartDelay);

        // Unfreeze game
        Time.timeScale = 1f;

        // Start game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
}
