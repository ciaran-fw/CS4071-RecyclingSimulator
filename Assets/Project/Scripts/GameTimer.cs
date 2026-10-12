using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private ScreenUI screenUI;
    [SerializeField] private float gameTime = 60f;
    [SerializeField] private UnityEvent onTimerFinished;

    private float timeRemaining;
    private bool timerRunning;

    private void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;

            onTimerFinished.Invoke();
            
            return;
        }

        UpdateTimerText();
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void ResetTimer()
    {
        timeRemaining = gameTime;
        timerRunning = false;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        screenUI.UpdateTimeText(timeRemaining);
    }
}