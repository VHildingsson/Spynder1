using UnityEngine;

public class GoldenBugVision : BugVision
{
    [Header("Golden Fly Detection")]
    public float detectionRange = 2f; // Larger detection range

    private void Update()
    {
        if (!CompareTag("GoldenFly")) return;

        // Proactively detect webs before they hit the trigger
        Collider2D[] nearbyWebs = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        foreach (var web in nearbyWebs)
        {
            if (web.CompareTag("Web"))
            {
                GetComponent<GoldenBugMovement>().EvadeWeb();
                break;
            }
        }
    }
}
