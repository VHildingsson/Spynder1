using UnityEngine;

public class GoldenBugMovement : BugMovement
{
    [Header("Golden Fly Settings")]
    public float speedBoost = 1.5f; // 50% faster than normal flies
    public float evasionBoost = 2f; // Reacts twice as fast

    private bool isGolden = false;

    protected override void Start()
    {
        // Only enhance if this is actually a golden fly
        if (CompareTag("GoldenFly"))
        {
            isGolden = true;
            speed *= speedBoost;
            reactionTime /= evasionBoost;
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        base.Start(); // Call base initialization
    }

    public override void EvadeWeb()
    {
        if (!isGolden)
        {
            base.EvadeWeb();
            return;
        }

        // Enhanced golden fly evasion - more precise dodging
        Vector2 awayFromWeb = (transform.position - WebDrawing.Instance.transform.position).normalized;
        direction = Vector2.Lerp(direction, awayFromWeb, 0.8f).normalized;
        RotateBug();
    }
}
