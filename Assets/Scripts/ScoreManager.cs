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

    private const string HIGH_SCORE_KEY = "HighScore";

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public HeartUI[] heartContainers;
    public GameObject gameOverPanel;
    public Animator gameOverPanelAnimator;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI statsText;

    [Header("Game Over Settings")]
    public float gameOverDelay = 0.5f;

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
    public int HighScore { get; private set; }
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

    private void Start()
    {
        LoadHighScore(); // Load the high score when the game starts
        InitializeHearts();
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void LoadHighScore()
    {
        HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
        PlayerPrefs.Save(); // Important to actually write to disk
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

        yield return new WaitForSecondsRealtime(gameOverDelay);

        Time.timeScale = 0f;
        UIManager.Instance.ShowGameOverPanel();
        gameOverPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        // Calculate final score and check high score
        int finalScore = CalculateFinalScore();
        if (finalScore > HighScore)
        {
            HighScore = finalScore;
            SaveHighScore();
        }

        ShowGameStats();
        OnGameOver?.Invoke();
    }

    private int CalculateFinalScore()
    {
        int webPenalty = Mathf.Max(0, websPlaced - (normalBugsCaught + goldenBugsCaught + mothsCaught + menelausCaught)) * 10;
        return Mathf.Max(0, score - webPenalty);
    }

    private void ShowGameStats()
    {
        int finalScore = CalculateFinalScore();

        statsText.text =
            $"Time Played: {FormatTime(timePlayed)}\n" +
            $"Flies: {normalBugsCaught}\n" +
            $"Goldflies: {goldenBugsCaught}\n" +
            $"Moths: {mothsCaught}\n" +
            $"Menelaus: {menelausCaught}\n" +
            $"Webs Placed: {websPlaced}\n" +
            $"Web Penalty: -{Mathf.Max(0, websPlaced - (normalBugsCaught + goldenBugsCaught + mothsCaught + menelausCaught)) * 10}\n" +
            $"High Score: {HighScore}"; // Add high score display

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

    void UpdateScoreUI()
    {
        scoreText.text = "SCORE: " + score;
    }
}




