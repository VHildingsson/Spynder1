using UnityEngine;

public class MothMovement : BugMovement
{
    public float flutterIntensity = 0.5f;
    public float flutterSpeed = 5f;

    private float flutterTimer;

    protected override void Start()
    {
        base.Start();
        speed *= 0.8f; // Moths are slightly slower
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
