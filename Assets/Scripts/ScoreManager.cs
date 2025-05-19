using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public HeartUI[] heartContainers;
    public GameObject gameOverPanel;
    public Animator gameOverPanelAnimator;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI statsText;

    [Header("Game Over Settings")]
    public float gameOverDelay = 1.5f;

    [Header("Sound Effects")]
    public AudioClip lifeGainedSound;
    public AudioClip lifeLostSound;
    public AudioClip gameOverSound;
    public AudioSource audioSource;

    // Game statistics
    public int normalBugsCaught { get; private set; }
    public int goldenBugsCaught { get; private set; }
    public int mothsCaught { get; private set; }
    public int menelausCaught { get; private set; }
    public int websPlaced { get; private set; }
    public float timePlayed { get; private set; }
    private int score = 0;
    public int currentLives = 3;
    private List<int> activeHeartIndices = new List<int>();
    private bool isGameOver = false;

    public event Action<int> OnScoreChanged;
    public static event Action OnGameOver;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Start()
    {
        InitializeHearts();
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!isGameOver)
        {
            timePlayed += Time.deltaTime;
        }
    }

    void InitializeHearts()
    {
        activeHeartIndices.Clear();
        for (int i = 0; i < currentLives; i++)
        {
            if (i < heartContainers.Length)
            {
                heartContainers[i].EnableHeart();
                heartContainers[i].PlayGainAnimation();
                activeHeartIndices.Add(i);
            }
        }
    }

    public void AddScore(int value, bool isGolden = false)
    {
        score += isGolden ? value * 3 : value;
        UpdateScoreUI();
        OnScoreChanged?.Invoke(score);
    }

    public void AddLife()
    {
        if (currentLives >= heartContainers.Length) return;

        currentLives++;
        if (lifeGainedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(lifeGainedSound);
        }

        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (!activeHeartIndices.Contains(i))
            {
                heartContainers[i].PlayGainAnimation();
                activeHeartIndices.Add(i);
                break;
            }
        }
    }

    public void LoseLife()
    {
        if (currentLives <= 0 || isGameOver) return;

        currentLives--;

        if (lifeLostSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(lifeLostSound);
        }

        if (activeHeartIndices.Count > 0)
        {
            int lastIndex = activeHeartIndices[activeHeartIndices.Count - 1];
            heartContainers[lastIndex].PlayLoseAnimation();
            activeHeartIndices.RemoveAt(activeHeartIndices.Count - 1);
        }

        if (currentLives <= 0)
        {
            StartCoroutine(GameOverSequence());
        }
    }

    public void BugEscaped()
    {
        LoseLife();
    }

    public void IncrementWebCount()
    {
        websPlaced++;
    }

    private IEnumerator GameOverSequence()
    {
        isGameOver = true;

        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        // Wait for the delay before showing panel
        yield return new WaitForSecondsRealtime(gameOverDelay); // Use WaitForSecondsRealtime

        // Pause the game (except for UI elements)
        Time.timeScale = 0f;

        // Show and animate the game over panel
        gameOverPanel.SetActive(true);
        gameOverPanelAnimator.SetTrigger("Show");
        gameOverPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        // Calculate and display stats
        ShowGameStats();

        OnGameOver?.Invoke();
    }

    private void ShowGameStats()
    {
        int webPenalty = Mathf.Max(0, websPlaced - (normalBugsCaught + goldenBugsCaught + mothsCaught + menelausCaught)) * 10;
        int finalScore = Mathf.Max(0, score - webPenalty);

        statsText.text =
            $"Normal Bugs: {normalBugsCaught}\n" +
            $"Golden Bugs: {goldenBugsCaught}\n" +
            $"Moths: {mothsCaught}\n" +
            $"Menelaus: {menelausCaught}\n" +
            $"Webs Placed: {websPlaced}\n" +
            $"Time Played: {FormatTime(timePlayed)}\n" +
            $"Web Penalty: -{webPenalty}";

        finalScoreText.text = $"FINAL SCORE: {finalScore}";
    }

    private string FormatTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }

    public void TrackBugCaught(CaughtBugEffects.BugType bugType)
    {
        switch (bugType)
        {
            case CaughtBugEffects.BugType.Normal:
                normalBugsCaught++;
                break;
            case CaughtBugEffects.BugType.Golden:
                goldenBugsCaught++;
                break;
            case CaughtBugEffects.BugType.Moth:
                mothsCaught++;
                break;
            case CaughtBugEffects.BugType.Menelaus:
                menelausCaught++;
                break;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateScoreUI()
    {
        scoreText.text = "SCORE: " + score;
    }
}




