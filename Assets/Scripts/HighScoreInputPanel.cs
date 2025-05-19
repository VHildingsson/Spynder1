using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HighScoreInputPanel : MonoBehaviour
{
    public GameObject panel;
    public TMP_InputField initialsInput;
    public Button submitButton;
    public TextMeshProUGUI scoreText;

    private int currentScore;

    private void Start()
    {
        submitButton.onClick.AddListener(SubmitScore);
        panel.SetActive(false);
    }

    public void Show(int score)
    {
        currentScore = score;
        scoreText.text = $"NEW HIGH SCORE: {score}";
        initialsInput.text = "AAA";
        panel.SetActive(true);
        initialsInput.Select();
    }

    private void SubmitScore()
    {
        string initials = initialsInput.text.ToUpper().Substring(0, Mathf.Min(3, initialsInput.text.Length));
        ScoreManager.Instance.AddHighScore(initials, currentScore);
        panel.SetActive(false);
    }
}
