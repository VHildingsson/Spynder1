
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance;

    public enum ToolMode { WebTool, SpiderTool }
    public ToolMode currentTool = ToolMode.WebTool;

    public int scoreValue = 10;
    public AudioClip toolSwitchSound;
    public AudioClip[] spiderEatSounds;
    public AudioSource audioSource;

    private bool isHoldingForWeb = false;
    private float webHoldStartTime = 0f;
    private const float webHoldThreshold = 0.3f;
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

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
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
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.UpdateCursor(currentTool);
            }
        }
    }

    private void Update()
    {
        if (Gamepad.current == null) return;

        if (!isInMenuScene && Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            ToggleTool();
        }

        // Change from isPressed to wasPressedThisFrame
        if (Gamepad.current.buttonSouth.wasPressedThisFrame && !isHoldingForWeb)
        {
            if (currentTool == ToolMode.WebTool && !isInMenuScene)
            {
                isHoldingForWeb = true;
                webHoldStartTime = Time.time;
                WebDrawing.Instance?.StartPotentialWeb();
            }
            else
            {
                Vector3 worldPos = CursorManager.Instance.GetCursorWorldPosition();
                worldPos.z = 0;

                Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("WebbedInsect") || (isInMenuScene && hit.CompareTag("UIButton")))
                    {
                        if (!isInMenuScene && ScoreManager.Instance != null)
                        {

                            if (spiderEatSounds != null && spiderEatSounds.Length > 0)
                            {
                                int randomIndex = Random.Range(0, spiderEatSounds.Length);
                                audioSource.pitch = Random.Range(0.9f, 1.1f);
                                audioSource.PlayOneShot(spiderEatSounds[randomIndex]);
                            }
                        }

                        if (hit.CompareTag("UIButton"))
                        {
                            var button = hit.GetComponent<Button>();
                            if (button != null)
                            {
                                button.onClick.Invoke();
                                // Play UI sound only once per button press
                                if (isInMenuScene && toolSwitchSound != null)
                                {
                                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                                    audioSource.PlayOneShot(toolSwitchSound);
                                }
                            }
                        }
                        if (hit.CompareTag("WebbedInsect"))
                        {
                            var effects = hit.GetComponent<CaughtBugEffects>();
                            if (effects != null)
                            {
                                effects.ApplyEffects();
                                Debug.Log($"Controller: Applied effects for {effects.bugType}");
                            }
                            Destroy(hit.gameObject);
                        }
                    }
                }
            }
        }

        if (isHoldingForWeb && Gamepad.current.buttonSouth.wasReleasedThisFrame)
        {
            WebDrawing.Instance?.ReleaseWeb();
            isHoldingForWeb = false;
        }
    }

    void ToggleTool()
    {
        // Play sound before switching
        if (toolSwitchSound != null)
        {
            audioSource.pitch = Random.Range(0.80f, 1.05f);
            audioSource.PlayOneShot(toolSwitchSound);
        }

        currentTool = currentTool == ToolMode.WebTool ? ToolMode.SpiderTool : ToolMode.WebTool;
        CursorManager.Instance?.UpdateCursor(currentTool);
    }
}