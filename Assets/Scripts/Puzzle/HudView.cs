using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HudView : MonoBehaviour
{
    public event Action BackClicked;

    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject comboRoot;
    [SerializeField] private Text comboText;
    [SerializeField] private Button backButton;
    [SerializeField] private float comboShowTime = 0.8f;

    private Coroutine comboRoutine;

    private void Awake()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(() => BackClicked?.Invoke());
        }

        SetComboVisible(false);
        SetCoins(0);
    }

    public void SetCoins(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = coins.ToString();
        }
    }

    public void ShowCombo(int reward)
    {
        if (comboRoutine != null)
        {
            StopCoroutine(comboRoutine);
        }

        comboRoutine = StartCoroutine(ShowComboRoutine(reward));
    }

    private IEnumerator ShowComboRoutine(int reward)
    {
        if (comboText != null)
        {
            comboText.text = "+" + reward;
        }

        SetComboVisible(true);

        yield return new WaitForSeconds(comboShowTime);

        SetComboVisible(false);
        comboRoutine = null;
    }

    private void SetComboVisible(bool visible)
    {
        if (comboRoot != null)
        {
            comboRoot.SetActive(visible);
        }
        else if (comboText != null)
        {
            comboText.gameObject.SetActive(visible);
        }
    }
}
