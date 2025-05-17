using UnityEngine;

public class GoldenFlyScore : MonoBehaviour
{
    public int goldenFlyValue = 30; // Normal flies give 10

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null && !this.gameObject.scene.isLoaded)
        {
            // Only give points if destroyed intentionally (not on scene unload)
            ScoreManager.Instance.AddScore(goldenFlyValue, true);
        }
    }
}
