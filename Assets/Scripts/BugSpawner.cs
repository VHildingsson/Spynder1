using UnityEngine;

public class BugSpawner : MonoBehaviour
{
    public GameObject[] bugPrefabs; // Array of bug prefabs
    public float spawnRate = 2f; // Time between spawns
    public AudioClip spawnSound; // Sound to play when spawning a bug
    private AudioSource audioSource;

    void Start()
    {
        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        InvokeRepeating("SpawnBug", 1f, spawnRate);
    }

    void SpawnBug()
    {
        if (bugPrefabs.Length == 0)
        {
            Debug.LogError("No Bug Prefabs assigned! Assign at least one bug prefab in the Inspector.");
            return;
        }

        // Play spawn sound if available
        if (spawnSound != null)
        {
            audioSource.pitch = Random.Range(0.7f, 1.1f);
            audioSource.PlayOneShot(spawnSound);
        }

        // Select a random bug prefab
        GameObject bugPrefab = bugPrefabs[Random.Range(0, bugPrefabs.Length)];

        // Spawn the bug at the spawner's position
        GameObject newBug = Instantiate(bugPrefab, transform.position, Quaternion.identity);

        // Ensure the scale matches the prefab
        newBug.transform.localScale = bugPrefab.transform.localScale;

        Debug.Log($"Spawned {newBug.name} at {transform.position} with scale {newBug.transform.localScale}");
    }
}








