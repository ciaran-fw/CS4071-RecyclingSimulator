using UnityEngine;
using TMPro;

public class ScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text[] bestScoreTexts;

    public void UpdateBestScoreTexts(int bestScore)
    {
        foreach (TMP_Text bestScoreText in bestScoreTexts)
        {
            bestScoreText.text = "Best Score: " + bestScore;
        }
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score;
    }

    public void UpdateTimeText(float timeRemaining)
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void ShowGameScreen()
    {
        gameOverScreen.SetActive(false);
        gameScreen.SetActive(true);
    }

    public void ShowGameOverScreen(int finalScore)
    {
        gameScreen.SetActive(false);
        gameOverScreen.SetActive(true);

        finalScoreText.text = finalScore.ToString();
    }
}
