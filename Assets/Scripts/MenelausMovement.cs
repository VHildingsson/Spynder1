using UnityEngine;

public class MenelausMovement : BugMovement
{
    public float smoothDirectionChange = 2f;

    public float flutterIntensity = 0.5f;
    public float flutterSpeed = 5f;

    private float flutterTimer;

    protected override void Start()
    {
        base.Start();
        speed *= 1.2f; // Menelaus is faster
    }

    public override void EvadeWeb()
    {
        // More elegant evasion
        Vector2 evadeDirection = (transform.position - WebDrawing.Instance.transform.position).normalized;
        direction = Vector2.Lerp(direction, evadeDirection, 0.7f).normalized;
        RotateBug();
    }

    public override void Update()
    {
        base.Update();

        // Add fluttering movement
        flutterTimer += Time.deltaTime * flutterSpeed;
        Vector2 flutterOffset = new Vector2(
            Mathf.Sin(flutterTimer) * flutterIntensity,
            Mathf.Cos(flutterTimer * 0.7f) * flutterIntensity
        );

        transform.position += (Vector3)flutterOffset * Time.deltaTime;

    }
}
