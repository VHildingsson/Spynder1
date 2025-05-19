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
        onSubmitCallback = callback;
        gameObject.SetActive(true);
        initialsInputField.text = "";
        panelAnimator.SetTrigger(showTrigger);

        // Select the input field after animation completes
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
        isWaitingForInput = false;
        panelAnimator.SetTrigger(hideTrigger);

        // Process input
        string processedInitials = initialsInputField.text.Trim().ToUpper();
        if (processedInitials.Length > 4)
        {
            processedInitials = processedInitials.Substring(0, 4);
        }

        // Return cursor to game mode
        CursorManager.Instance?.SetMenuMode(false);

        // Show game over stats panel
        uiManager.ShowGameOverPanel();

        // Invoke callback
        onSubmitCallback?.Invoke(processedInitials);
    }

    private IEnumerator DisableAfterAnimation(string trigger)
    {
        yield return new WaitForSecondsRealtime(panelAnimator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}
