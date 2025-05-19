using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerMenu : MonoBehaviour
{
    public static UIManagerMenu Instance;

    [Header("Panels")]
    public GameObject questionPanel;

    [Header("Buttons")]
    public Button questionButton;
    public Button closeQuestionButton;

    [Header("Animators")]
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
        questionPanelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        // Setup button listeners
        questionButton.onClick.AddListener(ShowQuestionPanel);
        closeQuestionButton.onClick.AddListener(HideQuestionPanel);
    }

    private void LateUpdate()
    {
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

    public void ShowQuestionPanel()
    {
        questionPanel.SetActive(true);
        questionPanelAnimator.SetTrigger("Show");
    }

    public void HideQuestionPanel()
    {
        questionPanelAnimator.SetTrigger("Hide");
    }
}
