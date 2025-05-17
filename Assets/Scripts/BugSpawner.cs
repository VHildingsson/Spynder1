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

    [Header("Difficulty Settings")]
    public float minSpawnRate = 0.5f; // Fastest spawn rate (hardest)
    public float maxSpawnRate = 3f;   // Slowest spawn rate (easiest)
    public DifficultyStage[] difficultyStages = new DifficultyStage[10]; // 10 customizable stages

    [Header("Golden Fly Settings")]
    public GameObject goldenFlyPrefab; // Drag your golden fly prefab here
    public float goldenFlyChance = 0.05f; // 5% chance to spawn golden fly

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
        yield return new WaitForSeconds(1f); // Initial delay

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
        GameObject bugPrefab;
        float roll = Random.Range(0f, 100f);

        if (roll < goldenFlyChance && goldenFlyPrefab != null)
        {
            bugPrefab = goldenFlyPrefab;
            goldenSpawns++;
            Debug.Log($"GOLDEN! (Rolled {roll} < {goldenFlyChance})");
        }
        else
        {
            bugPrefab = bugPrefabs[Random.Range(0, bugPrefabs.Length)];
            Debug.Log($"Normal (Rolled {roll} >= {goldenFlyChance})");
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








