using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TrashSpawner trashSpawner;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameTimer gameTimer;

    [SerializeField] private ScreenUI screenUI;
    [SerializeField] private TabletUI tabletUI;

    public void StartGame()
    {
        ResetGame();

        trashSpawner.StartSpawning();
        gameTimer.StartTimer();

        screenUI.ShowGameScreen();
        tabletUI.ShowStopScreen();
    }

    public void ResetGame()
    {
        scoreManager.ResetScore();
        trashSpawner.ResetSpawner();
        gameTimer.ResetTimer();
    }

    public void GameOver()
    {
        trashSpawner.ResetSpawner();
        trashSpawner.RemoveAllTrash();

        scoreManager.CheckBestScore();

        screenUI.ShowGameOverScreen(scoreManager.GetScore());
        tabletUI.ShowStartScreen();
    }
}
