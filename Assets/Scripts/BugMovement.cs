using UnityEngine;

public class BugMovement : MonoBehaviour
{
    public float speed = 3f;
    public Vector2 direction;
    public enum FlyType { Dumb, Slow, Smart }
    public FlyType flyType; // AI type of this fly

    public float reactionTime; // Reaction delay

    protected virtual void Start()
    {
        direction = Random.insideUnitCircle.normalized;
        RotateBug();

        // Assign random AI type when the fly spawns
        float randomValue = Random.value;
        if (randomValue < 0.4f) flyType = FlyType.Dumb;    // 40% chance - Doesn't evade
        else if (randomValue < 0.7f) flyType = FlyType.Slow; // 30% chance - Reacts slowly
        else flyType = FlyType.Smart;   // 30% chance - Reacts fast

        // Assign reaction time for smart & slow flies
        reactionTime = (flyType == FlyType.Slow) ? Random.Range(0.5f, 1.2f) : 0.2f;
    }

    public virtual void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Destroy bug if it leaves the screen
        if (Mathf.Abs(transform.position.x) > 8 || Mathf.Abs(transform.position.y) > 5)
        {
            if (!gameObject.CompareTag("Menelaus"))
            {
                ScoreManager.Instance.BugEscaped();
            }
            Destroy(gameObject);
        }
    }

    public virtual void EvadeWeb()
    {
        if (flyType == FlyType.Dumb) return; // Dumb flies ignore webs

        // Slow flies react after a delay
        if (flyType == FlyType.Slow)
        {
            Invoke("ChangeDirection", reactionTime);
        }
        else
        {
            ChangeDirection(); // Smart flies react instantly
        }
    }

    private void ChangeDirection()
    {
        direction = Random.insideUnitCircle.normalized;
        RotateBug();
        Debug.Log($"? {flyType} Fly is avoiding the web!");
    }

    public void RotateBug()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}











