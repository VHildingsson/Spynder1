using UnityEngine;

public class BugVision : MonoBehaviour
{
    private BugMovement bugMovement;

    void Start()
    {
        bugMovement = GetComponentInParent<BugMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Web") && bugMovement != null)
        {
            bugMovement.EvadeWeb();
        }
    }
}


