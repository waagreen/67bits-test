using UnityEngine;
using UnityEngine.SceneManagement;

public class GameReloader : MonoBehaviour
{
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private string gameSceneName;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnEndGame>(ShowEndScreen);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnEndGame>(ShowEndScreen);
    }

    private void ShowEndScreen(OnEndGame evt)
    {
        endGameScreen.SetActive(true);
    }

    public void ReloadGame()
    {
        PlayerProfile.CleanupStaticData();
        EventsManager.ClearAll();
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
