using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialScreen : MonoBehaviour
{
    [Header("Tutorial Pages")]
    [SerializeField] private GameObject[] pages;

    [Header("Navigation")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text pageNumberText;

    private int currentPage = 0;

    private void Start()
    {
        ShowPage(currentPage);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    private void ShowPage(int pageIndex)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == pageIndex);
        }

        pageNumberText.text = $"{pageIndex + 1} / {pages.Length}";

        previousButton.gameObject.SetActive(pageIndex > 0);
        nextButton.gameObject.SetActive(pageIndex < pages.Length - 1);
    }
}