using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class HighScorePanel : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI highScoreText;
    public GameObject panel;
    public Button exitButton;
    public Animator panelAnimator;

    [Header("Score Display References")]
    public TextMeshProUGUI[] initialsTexts; // Size 3
    public TextMeshProUGUI[] scoreTexts;    // Size 3

    [Header("Animation Parameters")]
    public string showTrigger = "Show";
    public string hideTrigger = "Hide";

    private void Start()
    {
        // Setup button listener
        exitButton.onClick.AddListener(HidePanel);

        // Initially hide the panel
        panel.SetActive(false);

        // Make sure animator uses unscaled time
        if (panelAnimator != null)
        {
            panelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public void TogglePanel()
    {
        if (panel.activeSelf)
        {
            HidePanel();
        }
        else
        {
            ShowPanel();
        }
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
        LoadAndDisplayHighScore();

        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger(showTrigger);
        }
    }

    public void HidePanel()
    {
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger(hideTrigger);
            // Disable panel after animation completes (handled by animation event or coroutine)
            StartCoroutine(DisableAfterAnimation(hideTrigger));
        }
        else
        {
            panel.SetActive(false);
        }
    }

    private IEnumerator DisableAfterAnimation(string trigger)
    {
        // Wait for animation to start
        yield return null;

        // Wait for animation to complete
        yield return new WaitForSecondsRealtime(panelAnimator.GetCurrentAnimatorStateInfo(0).length);

        panel.SetActive(false);
    }

    private void LoadAndDisplayHighScore()
    {
        // Check if ScoreManager exists
        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("ScoreManager instance not found!");

            // Display empty scores
            for (int i = 0; i < 3; i++)
            {
                initialsTexts[i].text = "---";
                scoreTexts[i].text = "0";
            }
            return;
        }

        HighScoreData highScoreData = ScoreManager.Instance.GetHighScoreData();

        // Display all top scores
        for (int i = 0; i < 3; i++)
        {
            if (i < highScoreData.topScores.Count)
            {
                initialsTexts[i].text = highScoreData.topScores[i].initials;
                scoreTexts[i].text = highScoreData.topScores[i].score.ToString();
            }
            else
            {
                initialsTexts[i].text = "---";
                scoreTexts[i].text = "0";
            }
        }
    }
}
