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
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = $"HIGH SCORE: {highScore}";
    }
}
