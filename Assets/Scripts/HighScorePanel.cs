using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class HighScorePanel : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI[] highScoreTexts; // Array for top 3 scores
    public TextMeshProUGUI[] initialsTexts;  // Array for top 3 initials
    public GameObject panel;
    public Button exitButton;
    public Animator panelAnimator;

    [Header("Animation Parameters")]
    public string showTrigger = "Show";
    public string hideTrigger = "Hide";

    private void Start()
    {
        exitButton.onClick.AddListener(HidePanel);
        panel.SetActive(false);

        if (panelAnimator != null)
        {
            panelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public void ShowPanel()
    {
        panel.SetActive(true);
        LoadAndDisplayHighScores();

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
            StartCoroutine(DisableAfterAnimation());
        }
        else
        {
            panel.SetActive(false);
        }
    }

    private IEnumerator DisableAfterAnimation()
    {
        // Get the length of the current animation
        float animationLength = panelAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Wait for the animation to complete
        yield return new WaitForSecondsRealtime(animationLength);

        panel.SetActive(false);
    }

    private void LoadAndDisplayHighScores()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager instance not found!");
            return;
        }

        var highScores = ScoreManager.Instance.GetHighScores();

        // Display top 3 scores
        for (int i = 0; i < Mathf.Min(3, highScores.Count); i++)
        {
            if (i < highScoreTexts.Length && highScoreTexts[i] != null)
            {
                highScoreTexts[i].text = highScores[i].score.ToString();
            }

            if (i < initialsTexts.Length && initialsTexts[i] != null)
            {
                initialsTexts[i].text = highScores[i].initials;
            }
        }
    }
}
