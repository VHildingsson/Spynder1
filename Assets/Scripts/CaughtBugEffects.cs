using UnityEngine;

public class CaughtBugEffects : MonoBehaviour
{
    public enum BugType { Normal, Golden, Moth, Menelaus }
    public BugType bugType;
    public float mothSelfDestructTime = 3f;

    private void Start()
    {
        if (bugType == BugType.Moth)
        {
            Invoke("SelfDestruct", mothSelfDestructTime);
        }
    }

    private void SelfDestruct()
    {
        GetComponent<CaughtBug>()?.StartBreakingSequence();
    }

    public void ApplyEffects()
    {
        Debug.Log($"Applying effects for {bugType}", this);

        if (ScoreManager.Instance == null)
        {
            Debug.LogError("ScoreManager instance is null!", this);
            return;
        }

        switch (bugType)
        {
            case BugType.Golden:
                ScoreManager.Instance?.AddScore(100);
                break;
            case BugType.Moth:
                ScoreManager.Instance?.LoseLife();
                Debug.Log("Life lost from moth", this);
                break;
            case BugType.Menelaus:
                // Check if player already has max lives
                if (ScoreManager.Instance.currentLives >= 3)
                {
                    ScoreManager.Instance?.AddScore(50); // Give 50 points instead
                    Debug.Log("Max lives reached - awarded 50 points for Menelaus", this);
                }
                else
                {
                    ScoreManager.Instance?.AddLife();
                    Debug.Log("Life gained from menelaus", this);
                }
                break;
            default:
                ScoreManager.Instance?.AddScore(10);
                break;
        }
    }
}