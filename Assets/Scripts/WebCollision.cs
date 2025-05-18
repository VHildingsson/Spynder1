using UnityEngine;
using static CaughtBugEffects;

public class WebCollision : MonoBehaviour
{
    public GameObject bugCaughtPrefab; // Assign the caught bug prefab in Inspector
    public GameObject startMarker; // Reference to start marker
    public GameObject endMarker; // Reference to end marker

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bug") || other.CompareTag("GoldenFly") || other.CompareTag("Moth") || other.CompareTag("Menelaus")) // Ensure the colliding object is a bug
        {
            // Get bug type before destroying
            BugType type = BugType.Normal;
            if (other.CompareTag("GoldenFly")) type = BugType.Golden;
            else if (other.CompareTag("Moth")) type = BugType.Moth;
            else if (other.CompareTag("Menelaus")) type = BugType.Menelaus;

            Vector3 position = other.transform.position;
            Destroy(other.gameObject);

            GameObject caughtBug = Instantiate(bugCaughtPrefab, position, Quaternion.identity);
            caughtBug.GetComponent<CaughtBugEffects>().bugType = type;

            // Ensure WebDrawing properly removes this web and its markers
            if (WebDrawing.Instance != null)
            {
                WebDrawing.Instance.DestroyWeb(gameObject, startMarker, endMarker);
            }
            else
            {
                // Fallback: Destroy manually if WebDrawing is not available
                Destroy(gameObject);
                if (startMarker != null) Destroy(startMarker);
                if (endMarker != null) Destroy(endMarker);
            }
        }
    }
}







