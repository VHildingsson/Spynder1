using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public HeartUI[] heartContainers; // Assign 3 heart UI elements in Inspector
    public Animator livesPanelAnimator;

    [Header("Sound Effects")]
    public AudioClip lifeGainedSound;
    public AudioClip lifeLostSound;
    public AudioSource audioSource;

    private int score = 0;
    public int currentLives = 3;
    private List<int> activeHeartIndices = new List<int>();

    public event Action<int> OnScoreChanged;

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

        // Add audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Start()
    {
        InitializeHearts();
    }

    void InitializeHearts()
    {
        // Clear any existing active hearts
        activeHeartIndices.Clear();

        // Enable and animate in starting hearts
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

        // Play life gained sound
        if (lifeGainedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(lifeGainedSound);
        }

        // Find first inactive heart
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
        if (currentLives <= 0) return;

        currentLives--;

        // Play life lost sound
        if (lifeLostSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(lifeLostSound);
        }

        // Visual feedback
        if (activeHeartIndices.Count > 0)
        {
            int lastIndex = activeHeartIndices[activeHeartIndices.Count - 1];
            heartContainers[lastIndex].PlayLoseAnimation();
            activeHeartIndices.RemoveAt(activeHeartIndices.Count - 1);
        }

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    public void BugEscaped()
    {
        if (currentLives <= 0) return;

        currentLives--;

        // Play life lost sound for escaped bugs too
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
            GameOver();
        }
    }

    void GameOver()
    {
        scoreText.text = "Game Over! Final Score: " + score;
        livesPanelAnimator.SetTrigger("GameOver");
    }

    void UpdateScoreUI()
    {
        scoreText.text = "SCORE: " + score;
    }

    void UpdateLivesUI()
    {
        // Lives are now handled by animations
    }
}




