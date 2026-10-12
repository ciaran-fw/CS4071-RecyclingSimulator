using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScreenUI screenUI;
    [SerializeField] private int startingScore = 0;
    [SerializeField] private int correctScore = 10;
    [SerializeField] private int incorrectScore = -5;

    private int score;
    private int bestScore;

    private void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        screenUI.UpdateBestScoreTexts(bestScore);
    }

    public void CorrectTrash()
    {
        score += correctScore;
        UpdateScoreText();
    }

    public void IncorrectTrash()
    {
        score += incorrectScore;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        screenUI.UpdateScoreText(score);
    }

    public void ResetScore()
    {
        score = startingScore;
        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    public void CheckBestScore()
    {
        if (score > bestScore)
        {
            bestScore = score;

            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();

            screenUI.UpdateBestScoreTexts(bestScore);
        }
    }
}
