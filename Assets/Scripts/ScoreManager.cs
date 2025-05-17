using UnityEngine;
using TMPro;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText; // New UI for lives
    private int score = 0;
    private int lives = 3;

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
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(int value, bool isGolden = false)
    {
        score += isGolden ? value * 3 : value; // 3x points for golden flies
        UpdateUI();
        OnScoreChanged?.Invoke(score);
    }

    public void BugEscaped()
    {
        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        scoreText.text = "Game Over! Final Score: " + score;
        livesText.text = ""; // Hide lives UI on Game Over
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + lives;
    }
}




