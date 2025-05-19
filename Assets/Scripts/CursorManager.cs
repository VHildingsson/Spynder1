using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [Header("Cursor Settings")]
    public RectTransform cursorUI;
    public Image cursorImage;
    public Sprite webToolCursor;
    public Sprite spiderToolCursor;
    public float cursorSpeed = 500f;

    private Vector2 currentCursorPosition;
    private Gamepad gamepad;
    private bool isInMenuScene;

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
        CheckScene();
    }

    private void Update()
    {
        if (gamepad == null)
        {
            gamepad = Gamepad.current;
            if (gamepad == null) return;
        }

        // Get input with deadzone
        Vector2 stickInput = gamepad.leftStick.ReadValue();
        if (stickInput.magnitude < 0.1f) stickInput = Vector2.zero;

        // Use unscaled time so cursor works during pause
        currentCursorPosition += stickInput * cursorSpeed * Time.unscaledDeltaTime;

        // Clamp to screen
        currentCursorPosition.x = Mathf.Clamp(currentCursorPosition.x, 0, Screen.width);
        currentCursorPosition.y = Mathf.Clamp(currentCursorPosition.y, 0, Screen.height);

        // Update visual position
        cursorUI.position = currentCursorPosition;
    }

    public void UpdateCursor(ToolManager.ToolMode toolMode)
    {
        if (cursorImage == null) return;

        cursorImage.sprite = toolMode == ToolManager.ToolMode.WebTool ? webToolCursor : spiderToolCursor;
    }

    public Vector2 GetCursorScreenPosition()
    {
        return currentCursorPosition;
    }

    public Vector2 GetCursorWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(currentCursorPosition);
    }

    private void CheckScene()
    {
        isInMenuScene = SceneManager.GetActiveScene().name == "MenuScene";
        if (isInMenuScene)
        {
            ToolManager.Instance.currentTool = ToolManager.ToolMode.SpiderTool;
            UpdateCursor(ToolManager.ToolMode.SpiderTool);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckScene();
    }
}