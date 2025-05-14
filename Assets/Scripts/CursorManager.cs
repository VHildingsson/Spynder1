using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    public RectTransform cursorUI;
    public Image cursorImage;
    public Sprite webToolCursor;
    public Sprite spiderToolCursor;
    public float cursorSpeed = 500f;

    private Vector2 currentCursorPosition;
    private Gamepad gamepad;

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
    }

    private void Start()
    {
        Cursor.visible = false;
        currentCursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        gamepad = Gamepad.current;
        UpdateCursor(ToolManager.Instance.currentTool);
    }

    private void Update()
    {
        if (gamepad == null)
        {
            gamepad = Gamepad.current;
            if (gamepad == null) return;
        }

        // Get left stick input with deadzone
        Vector2 stickInput = gamepad.leftStick.ReadValue();
        if (stickInput.magnitude < 0.1f) stickInput = Vector2.zero;

        // Move cursor
        currentCursorPosition += stickInput * cursorSpeed * Time.deltaTime;

        // Clamp to screen bounds
        currentCursorPosition.x = Mathf.Clamp(currentCursorPosition.x, 0, Screen.width);
        currentCursorPosition.y = Mathf.Clamp(currentCursorPosition.y, 0, Screen.height);

        // Update cursor position
        cursorUI.position = currentCursorPosition;
    }

    public Vector2 GetCursorScreenPosition()
    {
        return currentCursorPosition;
    }

    public Vector2 GetCursorWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(currentCursorPosition);
    }

    public void UpdateCursor(ToolManager.ToolMode toolMode)
    {
        if (cursorImage == null) return;

        cursorImage.sprite = toolMode switch
        {
            ToolManager.ToolMode.WebTool => webToolCursor,
            ToolManager.ToolMode.SpiderTool => spiderToolCursor,
            _ => webToolCursor
        };
    }
}