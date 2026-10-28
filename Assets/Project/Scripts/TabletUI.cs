using UnityEngine;
using System.Collections;

public class TabletUI : MonoBehaviour
{
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject stopScreen;

    public void ShowStartScreen()
    {
        stopScreen.SetActive(false);
        StartCoroutine(ShowStartScreenDelayed());
    }

    private IEnumerator ShowStartScreenDelayed()
    {
        yield return new WaitForSeconds(2f);
        startScreen.SetActive(true);
    }

    public void ShowStopScreen()
    {
        startScreen.SetActive(false);
        StartCoroutine(ShowStopScreenDelayed());
    }

    private IEnumerator ShowStopScreenDelayed()
    {
        yield return new WaitForSeconds(2f);
        stopScreen.SetActive(true);
    }
}
