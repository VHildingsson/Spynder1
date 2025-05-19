using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float transitionTime = 1f;

    [Header("Countdown Settings")]
    [SerializeField] private Animator countdownAnimator;
    [SerializeField] private float countdownStartDelay = 0.5f; // Halfway through uncover
    [SerializeField] private string countdownAnimationName = "Countdown";

    private bool isGameFrozen = false;

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
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            FreezeGame();
            PlayUncover();
        }
    }

    public void FreezeGame()
    {
        isGameFrozen = true;
        Time.timeScale = 0f;
    }

    public void UnfreezeGame()
    {
        isGameFrozen = false;
        Time.timeScale = 1f;
    }

    public void LoadGameScene()
    {
        StartCoroutine(StartTransition("GameScene"));
    }

    public IEnumerator TransitionToMenu()
    {
        yield return StartCoroutine(StartTransition("MenuScene"));
    }

    private IEnumerator StartTransition(string sceneName)
    {
        transitionAnimator.SetTrigger("Cover");
        yield return new WaitForSecondsRealtime(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    public void PlayUncover()
    {
        StartCoroutine(UncoverRoutine());
    }

    private IEnumerator UncoverRoutine()
    {
        // Start uncover animation
        transitionAnimator.SetTrigger("Uncover");

        // Wait for halfway point
        yield return new WaitForSecondsRealtime(countdownStartDelay);

        // Start countdown animation
        if (countdownAnimator != null)
        {
            countdownAnimator.SetTrigger("Start");

            // Wait for countdown animation to finish
            yield return new WaitForSecondsRealtime(GetAnimationLength(countdownAnimator, countdownAnimationName));
        }
        else
        {
            // If no countdown animator, just wait remaining transition time
            yield return new WaitForSecondsRealtime(transitionTime - countdownStartDelay);
        }

        // Unfreeze game
        UnfreezeGame();
    }

    private float GetAnimationLength(Animator animator, string animationName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 0f;
    }

    public void LoadMenuScene()
    {
        StartCoroutine(StartTransition("MenuScene"));
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameRoutine());
    }

    private IEnumerator RestartGameRoutine()
    {
        // Play cover animation
        transitionAnimator.SetTrigger("Cover");

        // Wait for transition
        yield return new WaitForSecondsRealtime(transitionTime);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Note: The uncover animation will automatically play in Start()
    }
}