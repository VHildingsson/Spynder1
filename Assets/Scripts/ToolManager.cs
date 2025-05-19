using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance;

    public enum ToolMode { WebTool, SpiderTool }
    public ToolMode currentTool = ToolMode.WebTool;

    [Header("Settings")]
    public int scoreValue = 10;
    public AudioClip toolSwitchSound;
    public AudioClip[] spiderEatSounds;
    public AudioSource audioSource;

    private bool isHoldingForWeb = false;
    private float webHoldStartTime = 0f;
    private const float webHoldThreshold = 0.3f;
    private bool isInMenuScene;
    public bool isGameOver = false;

    public bool ShouldHandleUIInteractions => isInMenuScene || isGameOver;

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

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        // Subscribe to game over event
        ScoreManager.OnGameOver += HandleGameOver;
    }

    private void OnDestroy()
    {
        ScoreManager.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver()
    {
        isGameOver = true;
        currentTool = ToolMode.SpiderTool; // Force spider tool mode
        CursorManager.Instance?.UpdateCursor(currentTool);
    }

    private void Start()
    {
        CheckScene();
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

    private void CheckScene()
    {
        isInMenuScene = SceneManager.GetActiveScene().name == "MenuScene";
        if (isInMenuScene)
        {
            currentTool = ToolMode.SpiderTool;
            CursorManager.Instance?.UpdateCursor(currentTool);
        }
    }

    private void Update()
    {
        if (Gamepad.current == null) return;

        // Handle tool switching (disabled during menu and game over)
        if (!isGameOver && !isInMenuScene && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            ToggleTool();
        }

        // Handle button presses
        if (Gamepad.current.buttonSouth.wasPressedThisFrame && !isHoldingForWeb)
        {
            if (!isGameOver && !isInMenuScene && currentTool == ToolMode.WebTool)
            {
                // Start web drawing
                isHoldingForWeb = true;
                webHoldStartTime = Time.time;
                WebDrawing.Instance?.StartPotentialWeb();
            }
            else
            {
                // Handle UI interaction in both menu and game over states
                HandleInteraction();
            }
        }

        // Handle web release
        if (isHoldingForWeb && Gamepad.current.buttonSouth.wasReleasedThisFrame)
        {
            WebDrawing.Instance?.ReleaseWeb();
            isHoldingForWeb = false;
        }
    }

    private void HandleInteraction()
    {
        Vector2 cursorPos = CursorManager.Instance.GetCursorScreenPosition();
        var hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(cursorPos), Vector2.zero);

        foreach (var hit in hits)
        {
            // Handle UI buttons in all cases (menu, game over, input panel)
            if (hit.collider != null && (hit.collider.CompareTag("UIButton") || isInMenuScene || isGameOver))
            {
                var button = hit.collider.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                    if (toolSwitchSound != null)
                    {
                        audioSource.pitch = Random.Range(0.9f, 1.1f);
                        audioSource.PlayOneShot(toolSwitchSound);
                    }
                    return;
                }
            }

            // Only handle webbed insects during normal gameplay
            if (!ShouldHandleUIInteractions && hit.collider != null && hit.collider.CompareTag("WebbedInsect"))
            {
                var effects = hit.collider.GetComponent<CaughtBugEffects>();
                if (effects != null)
                {
                    effects.ApplyEffects();
                }
                Destroy(hit.collider.gameObject);

                if (spiderEatSounds != null && spiderEatSounds.Length > 0)
                {
                    int randomIndex = Random.Range(0, spiderEatSounds.Length);
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    audioSource.PlayOneShot(spiderEatSounds[randomIndex]);
                }
                return;
            }
        }
    }

    private void ToggleTool()
    {
        if (toolSwitchSound != null)
        {
            audioSource.pitch = Random.Range(0.80f, 1.05f);
            audioSource.PlayOneShot(toolSwitchSound);
        }

        currentTool = currentTool == ToolMode.WebTool ? ToolMode.SpiderTool : ToolMode.WebTool;
        CursorManager.Instance?.UpdateCursor(currentTool);
    }
}