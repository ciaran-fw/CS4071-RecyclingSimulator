using UnityEngine;

public class TabletUI : MonoBehaviour
{
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject stopScreen;

    public void ShowStartScreen()
    {
        startScreen.SetActive(true);
        stopScreen.SetActive(false);
    }

    public void ShowStopScreen()
    {
        startScreen.SetActive(false);
        stopScreen.SetActive(true);
    }
}
