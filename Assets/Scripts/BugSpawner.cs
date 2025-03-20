using UnityEngine;

public class BugSpawner : MonoBehaviour
{
    public GameObject[] bugPrefabs; // Array of bug prefabs
    public float spawnRate = 2f; // Time between spawns

    void Start()
    {
        InvokeRepeating("SpawnBug", 1f, spawnRate);
    }

    void SpawnBug()
    {
        if (bugPrefabs.Length == 0)
        {
            Debug.LogError("No Bug Prefabs assigned! Assign at least one bug prefab in the Inspector.");
            return;
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








