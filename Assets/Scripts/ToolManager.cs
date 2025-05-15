using UnityEngine;
using UnityEngine.InputSystem;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance;

    public enum ToolMode { WebTool, SpiderTool }
    public ToolMode currentTool = ToolMode.WebTool;

    public int scoreValue = 10;

    private bool isHoldingForWeb = false;
    private float webHoldStartTime = 0f;
    private const float webHoldThreshold = 0.3f;

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

    private void Update()
    {
        if (Gamepad.current == null) return;

        // Tool switching with Circle button
        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            ToggleTool();
        }

        // Tool actions with X button
        if (Gamepad.current.buttonSouth.isPressed && !isHoldingForWeb)
        {
            if (currentTool == ToolMode.WebTool)
            {
                isHoldingForWeb = true;
                webHoldStartTime = Time.time;
                WebDrawing.Instance?.StartPotentialWeb();
            }
            else
            {
                // Spider tool action (instant eat)
                Vector3 worldPos = CursorManager.Instance.GetCursorWorldPosition();
                worldPos.z = 0;

                Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("WebbedInsect"))
                    {
                        if (ScoreManager.Instance != null)
                        {
                            ScoreManager.Instance.AddScore(scoreValue);
                        }
                        Destroy(hit.gameObject);
                    }
                }
            }
        }

        // Web release
        if (isHoldingForWeb && Gamepad.current.buttonSouth.wasReleasedThisFrame)
        {
            WebDrawing.Instance.ReleaseWeb();
            isHoldingForWeb = false;
        }
    }

    void ToggleTool()
    {
        currentTool = currentTool == ToolMode.WebTool ? ToolMode.SpiderTool : ToolMode.WebTool;

        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.UpdateCursor(currentTool);
        }
    }
}