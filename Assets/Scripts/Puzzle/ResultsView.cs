using System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ResultsView : MonoBehaviour
{
    public event Action PlayAgainClicked;
    public event Action HomeClicked;

    [SerializeField] private GameObject root;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text bestText;
    [SerializeField] private GameObject newBestMark;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button homeButton;

    private void Awake()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(() => PlayAgainClicked?.Invoke());
        }

        if (homeButton != null)
        {
            homeButton.onClick.AddListener(() => HomeClicked?.Invoke());
        }

        Hide();
    }

    public void Show(int roundCoins, int bestCoins, bool isNewBest)
    {
        if (coinsText != null)
        {
            coinsText.text = roundCoins.ToString();
        }

        if (bestText != null)
        {
            bestText.text = bestCoins.ToString();
        }

        if (newBestMark != null)
        {
            newBestMark.SetActive(isNewBest);
        }

        if (root != null)
        {
            root.SetActive(true);
        }
    }

    public void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
        }
    }

    public bool IsVisible()
    {
        return root != null && root.activeSelf;
    }
}
