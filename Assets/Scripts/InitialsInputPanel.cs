using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class InitialsInputPanel : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField initialsInputField;
    public Button submitButton;
    public Animator panelAnimator;

    [SerializeField] private UIManager uiManager;

    [Header("Animation Parameters")]
    public string showTrigger = "Show";
    public string hideTrigger = "Hide";

    private System.Action<string> onSubmitCallback;
    private bool isWaitingForInput = false;

    private bool isSubmitting = false;

    private void Start()
    {
        submitButton.gameObject.tag = "UIButton";
        initialsInputField.gameObject.tag = "UIButton";
        submitButton.onClick.AddListener(OnSubmitClicked);
        initialsInputField.onEndEdit.AddListener(OnEndEdit);
        panelAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void ShowInputPanel(System.Action<string> callback)
    {
        // Ensure spider tool mode is active
        ToolManager.Instance.currentTool = ToolManager.ToolMode.SpiderTool;
        CursorManager.Instance?.UpdateCursor(ToolManager.ToolMode.SpiderTool);

        onSubmitCallback = callback;
        gameObject.SetActive(true);
        initialsInputField.text = "";
        panelAnimator.SetTrigger(showTrigger);

        StartCoroutine(SelectInputFieldAfterAnimation());
    }

    private IEnumerator SelectInputFieldAfterAnimation()
    {
        yield return new WaitForSecondsRealtime(panelAnimator.GetCurrentAnimatorStateInfo(0).length);
        initialsInputField.Select();
        initialsInputField.ActivateInputField();
        isWaitingForInput = true;
    }

    public void OnSubmitClicked()
    {
        if (!isWaitingForInput) return;
        SubmitInitials();
    }

    private void OnEndEdit(string value)
    {
        if (!isWaitingForInput) return;
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            SubmitInitials();
        }
    }

    private void SubmitInitials()
    {
        if (isSubmitting) return;
        isSubmitting = true;

        // Disable input during animation
        initialsInputField.interactable = false;
        submitButton.interactable = false;

        // Process input
        string processedInitials = initialsInputField.text.Trim().ToUpper();
        if (processedInitials.Length > 4)
        {
            processedInitials = processedInitials.Substring(0, 4);
        }

        // Trigger hide animation
        panelAnimator.SetTrigger(hideTrigger);

        // Wait for animation to complete before invoking callback
        StartCoroutine(CompleteSubmissionAfterAnimation(processedInitials));
    }

    private IEnumerator CompleteSubmissionAfterAnimation(string initials)
    {
        // Wait for animation to start
        yield return null;

        // Get animation length
        float animLength = panelAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Wait for animation to complete
        yield return new WaitForSecondsRealtime(animLength);

        // Show game over panel after animation
        uiManager.ShowGameOverPanelAfterDelay(0.1f);

        // Invoke callback
        onSubmitCallback?.Invoke(initials);

        // Reset and hide
        isSubmitting = false;
        gameObject.SetActive(false);

        // Re-enable components for next use
        initialsInputField.interactable = true;
        submitButton.interactable = true;
    }

    private IEnumerator DisableAfterAnimation(string trigger)
    {
        yield return new WaitForSecondsRealtime(panelAnimator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}
