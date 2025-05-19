using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject questionPanel;

    [Header("Buttons")]
    public Button questionButton;
    public Button backToMenuButton;
    public Button playAgainButton;
    public Button closeQuestionButton;

    [Header("Animators")]
    public Animator gameOverPanelAnimator;
    public Animator questionPanelAnimator;

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

        // Set animators to use unscaled time
        gameOverPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        questionPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        // Setup button listeners
        questionButton.onClick.AddListener(ShowQuestionPanel);
        backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
        closeQuestionButton.onClick.AddListener(HideQuestionPanel);
    }

    private void OnBackToMenuClicked()
    {
        StartReturnToMainMenu();
    }

    private void OnPlayAgainClicked()
    {
        RestartGame();
    }

    private void LateUpdate()
    {
        if (gameOverPanel.activeSelf)
        {
            RefreshButtonPositions(gameOverPanel);
        }
        if (questionPanel.activeSelf)
        {
            RefreshButtonPositions(questionPanel);
        }
    }

    private void RefreshButtonPositions(GameObject panel)
    {
        Canvas.ForceUpdateCanvases();
        foreach (var button in panel.GetComponentsInChildren<Button>(true))
        {
            if (button.TryGetComponent<RectTransform>(out var rect))
            {
                rect.ForceUpdateRectTransforms();
            }

            if (button.TryGetComponent<BoxCollider2D>(out var collider))
            {
                if (button.TryGetComponent<RectTransform>(out var buttonRect))
                {
                    collider.size = buttonRect.rect.size;
                    collider.offset = Vector2.zero;
                }
            }
        }
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        gameOverPanelAnimator.SetTrigger("Show");
        questionPanel.SetActive(false);
    }

    public void ShowQuestionPanel()
    {
        gameOverPanelAnimator.SetTrigger("Hide");
        questionPanel.SetActive(true);
        questionPanelAnimator.SetTrigger("Show");
    }

    public void HideQuestionPanel()
    {
        questionPanelAnimator.SetTrigger("Hide");
        gameOverPanelAnimator.SetTrigger("Show");
    }

    public void StartReturnToMainMenu()
    {
        Time.timeScale = 1f;
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.LoadMenuScene();
        }
        else
        {
            ReturnToMainMenuImmediate();
        }
    }

    private void ReturnToMainMenuImmediate()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void RestartGame()
    {
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.RestartGame();
        }
        else
        {
            RestartGameImmediate();
        }
    }

    private void RestartGameImmediate()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
