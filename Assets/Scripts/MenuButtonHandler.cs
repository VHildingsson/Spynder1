using UnityEngine;
using UnityEngine.UI;

public class MenuButtonHandler : MonoBehaviour
{
    public Button playButton;

    private void Start()
    {
        playButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        TransitionManager.Instance.LoadGameScene();
    }
}
