using UnityEngine;

public class WebCollision : MonoBehaviour
{
    public GameObject bugCaughtPrefab; // Assign the caught bug prefab in Inspector
    public GameObject startMarker; // Reference to start marker
    public GameObject endMarker; // Reference to end marker

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bug")) // Ensure the colliding object is a bug
        {
            // Get bug position before destroying it
            Vector3 bugPosition = other.transform.position;

            // Destroy the bug
            Destroy(other.gameObject);

            // Spawn the caught bug at the same position
            Instantiate(bugCaughtPrefab, bugPosition, Quaternion.identity);

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







