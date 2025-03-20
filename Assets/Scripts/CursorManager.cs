using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    public RectTransform cursorUI; // Assign UI Cursor Image in Inspector
    public Image cursorImage; // UI Image component to change sprite
    public Sprite webToolCursor; // Assign in Inspector
    public Sprite pickupToolCursor; // Assign in Inspector

    public Vector2 cursorOffset = new Vector2(0, 0); // Adjust alignment if needed

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
        Cursor.visible = false; // Hide system cursor
        UpdateCursor(ToolManager.Instance.currentTool); // Set initial cursor
    }

    private void Update()
    {
        // Move UI cursor with the mouse
        Vector2 mousePos = Input.mousePosition;
        cursorUI.position = mousePos + cursorOffset;
    }

    public void UpdateCursor(ToolManager.ToolMode toolMode)
    {
        if (cursorImage != null)
        {
            if (toolMode == ToolManager.ToolMode.WebTool)
            {
                cursorImage.sprite = webToolCursor;
            }
            else
            {
                cursorImage.sprite = pickupToolCursor;
            }
        }
    }
}








