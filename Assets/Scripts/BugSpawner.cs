using UnityEngine;
using System.Collections;

[System.Serializable]
public class DifficultyStage
{
    public int scoreThreshold;
    public float spawnRate;
}

public class BugSpawner : MonoBehaviour
{
    public GameObject[] bugPrefabs;
    public AudioClip spawnSound;
    private AudioSource audioSource;

    [Header("Spawn Timing")]
    [Tooltip("Delay before spawning starts (in seconds)")]
    public float initialSpawnDelay = 3f; // New adjustable delay field
    public float minSpawnRate = 0.5f; // Fastest spawn rate (hardest)
    public float maxSpawnRate = 3f;   // Slowest spawn rate (easiest)

    [Header("Difficulty Settings")]
    public DifficultyStage[] difficultyStages = new DifficultyStage[10]; // 10 customizable stages

    [Header("Special Insects")]
    public GameObject goldenFlyPrefab;
    public GameObject mothPrefab;
    public GameObject menelausPrefab;

    [Range(0, 100)] public float goldenFlyChance = 5f;
    [Range(0, 100)] public float mothChance = 3f;
    [Range(0, 100)] public float menelausChance = 2f;

    private float currentSpawnRate;
    private Coroutine spawningCoroutine;

    private int totalSpawns;
    private int goldenSpawns;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Initialize with easiest difficulty
        currentSpawnRate = maxSpawnRate;
        StartSpawning();

        // Subscribe to score changes
        ScoreManager.Instance.OnScoreChanged += UpdateDifficulty;
    }

    void OnDestroy()
    {
        // Unsubscribe when destroyed
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= UpdateDifficulty;
        }
    }

    void StartSpawning()
    {
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
        }
        spawningCoroutine = StartCoroutine(SpawnBugs());
    }

    IEnumerator SpawnBugs()
    {
        yield return new WaitForSeconds(initialSpawnDelay); // Now using the configurable delay

        while (true)
        {
            SpawnBug();
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    void SpawnBug()
    {
        totalSpawns++;

        if (bugPrefabs.Length == 0) return;

        // Play sound
        if (spawnSound != null)
        {
            audioSource.pitch = Random.Range(0.7f, 1.1f);
            audioSource.PlayOneShot(spawnSound);
        }

        // Determine which prefab to spawn
        float roll = Random.Range(0f, 100f);
        GameObject bugPrefab;

        if (roll < goldenFlyChance && goldenFlyPrefab != null)
        {
            bugPrefab = goldenFlyPrefab;
            Debug.Log("Spawned Golden Fly");
        }
        else if (roll < goldenFlyChance + mothChance && mothPrefab != null)
        {
            bugPrefab = mothPrefab;
            Debug.Log("Spawned Moth");
        }
        else if (roll < goldenFlyChance + mothChance + menelausChance && menelausPrefab != null)
        {
            bugPrefab = menelausPrefab;
            Debug.Log("Spawned Menelaus");
        }
        else
        {
            bugPrefab = bugPrefabs[Random.Range(0, bugPrefabs.Length)];
        }

        // Instantiate
        GameObject newBug = Instantiate(bugPrefab, transform.position, Quaternion.identity);
        newBug.transform.localScale = bugPrefab.transform.localScale;
    }

    void UpdateDifficulty(int currentScore)
    {
        // Find the highest threshold we've passed
        float targetRate = maxSpawnRate;
        foreach (var stage in difficultyStages)
        {
            if (currentScore >= stage.scoreThreshold)
            {
                targetRate = Mathf.Min(stage.spawnRate, targetRate);
            }
        }

        // Ensure we don't go below minimum
        currentSpawnRate = Mathf.Max(targetRate, minSpawnRate);
    }
}








