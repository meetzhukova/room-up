using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HudView : MonoBehaviour
{
    [SerializeField] private int coinsFontSize = 72;
    [SerializeField] private int comboFontSize = 60;
    [SerializeField] private Color comboColor = new Color(0.98f, 0.84f, 0.28f);
    [SerializeField] private float comboShowTime = 0.8f;

    private Text coinsText;
    private Text comboText;
    private Coroutine comboRoutine;

    private void Awake()
    {
        Transform canvas = CreateCanvas();

        coinsText = CreateText("Coins", canvas, new Vector2(0f, -140f), coinsFontSize, Color.white);
        comboText = CreateText("Combo", canvas, new Vector2(0f, -250f), comboFontSize, comboColor);
        comboText.gameObject.SetActive(false);

        SetCoins(0);
    }

    public void SetCoins(int coins)
    {
        coinsText.text = "Coins: " + coins;
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
        comboText.text = "Combo! +" + reward;
        comboText.gameObject.SetActive(true);

        yield return new WaitForSeconds(comboShowTime);

        comboText.gameObject.SetActive(false);
        comboRoutine = null;
    }

    private Transform CreateCanvas()
    {
        GameObject canvasObject = new GameObject("HUD", typeof(RectTransform));
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        return canvasObject.transform;
    }

    private Text CreateText(string objectName, Transform parent, Vector2 position, int fontSize, Color color)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(1000f, 120f);

        return text;
    }
}
