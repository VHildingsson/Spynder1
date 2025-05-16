using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            // Initialize game systems here
        }
    }
}
