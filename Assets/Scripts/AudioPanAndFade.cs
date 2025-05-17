using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPanAndFade : MonoBehaviour
{
    [Header("Volume Settings")]
    [Range(0.1f, 2f)] public float maxVolume = 1f;
    [Range(0.5f, 3f)] public float volumeBoost = 1.5f; // Additional volume multiplier

    [Header("Fade Settings")]
    public float fadeInDuration = 0.3f; // Faster fade-in
    public float minVolume = 0.2f; // Minimum volume at edges

    private AudioSource audioSource;
    private Camera mainCamera;
    private float fadeTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;

        // Initialize with boosted volume settings
        audioSource.volume = 0f;
        audioSource.spatialBlend = 0; // Pure 2D sound
        audioSource.priority = 128; // Normal priority
        audioSource.Play();
    }

    void Update()
    {
        if (mainCamera == null) return;

        // Faster fade-in with cubic easing for smoother start
        if (fadeTimer < fadeInDuration)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeInDuration);
            audioSource.volume = Mathf.SmoothStep(0f, maxVolume * volumeBoost, t);
            return;
        }

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Panning (more pronounced)
        float pan = Mathf.Clamp((viewportPos.x - 0.5f) * 2f, -1f, 1f);
        audioSource.panStereo = pan * 1.2f; // Slightly exaggerated panning

        // Calculate distance from center (0-1)
        float distFromCenter = Vector2.Distance(
            new Vector2(viewportPos.x, viewportPos.y),
            Vector2.one * 0.5f
        ) * 2f;

        // Gentle fade curve that maintains good volume
        float volume = Mathf.Lerp(maxVolume * volumeBoost, minVolume,
                               Mathf.Pow(distFromCenter, 2f)); // Quadratic falloff

        audioSource.volume = volume;
    }
}


