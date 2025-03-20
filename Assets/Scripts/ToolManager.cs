using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance;

    public enum ToolMode { WebTool, PickupTool }
    public ToolMode currentTool = ToolMode.WebTool;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ToggleTool();
        }
    }

    void ToggleTool()
    {
        if (currentTool == ToolMode.WebTool)
        {
            currentTool = ToolMode.PickupTool;
            Debug.Log("Switched to Pickup Tool");
        }
        else
        {
            currentTool = ToolMode.WebTool;
            Debug.Log("Switched to Web Tool");
        }

        // Update the cursor whenever the tool changes
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.UpdateCursor(currentTool);
        }
    }
}


