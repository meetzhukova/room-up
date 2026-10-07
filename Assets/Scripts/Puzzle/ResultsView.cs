using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ResultsView : MonoBehaviour
{
    public event Action PlayAgainClicked;
    public event Action HomeClicked;

    [SerializeField] private Color dimColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] private Color panelColor = new Color(0.16f, 0.17f, 0.24f);
    [SerializeField] private Color buttonColor = new Color(0.29f, 0.53f, 0.96f);
    [SerializeField] private Color accentColor = new Color(0.98f, 0.84f, 0.28f);

    private GameObject root;
    private Text coinsText;
    private Text bestText;

    private void Awake()
    {
        EnsureEventSystem();
        Build();
        Hide();
    }

    public void Show(int roundCoins, int bestCoins, bool isNewBest)
    {
        coinsText.text = "Coins: " + roundCoins;
        bestText.text = isNewBest ? "New best!" : "Best: " + bestCoins;
        bestText.color = isNewBest ? accentColor : Color.white;
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    public bool IsVisible()
    {
        return root.activeSelf;
    }

    private void Build()
    {
        root = new GameObject("Results", typeof(RectTransform));
        root.transform.SetParent(transform, false);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        root.AddComponent<GraphicRaycaster>();

        RectTransform dim = CreateImage("Dim", root.transform, dimColor);
        dim.anchorMin = Vector2.zero;
        dim.anchorMax = Vector2.one;
        dim.offsetMin = Vector2.zero;
        dim.offsetMax = Vector2.zero;

        RectTransform panel = CreateImage("Panel", root.transform, panelColor);
        panel.sizeDelta = new Vector2(820f, 900f);

        CreateText("Title", panel, "Round Over", 96, accentColor, new Vector2(0f, 310f));
        coinsText = CreateText("Coins", panel, "", 72, Color.white, new Vector2(0f, 150f));
        bestText = CreateText("Best", panel, "", 56, Color.white, new Vector2(0f, 50f));

        CreateButton("Play Again", panel, new Vector2(0f, -140f), () => PlayAgainClicked?.Invoke());
        CreateButton("Home", panel, new Vector2(0f, -310f), () => HomeClicked?.Invoke());
    }

    private RectTransform CreateImage(string objectName, Transform parent, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform));
        imageObject.transform.SetParent(parent, false);

        Image image = imageObject.AddComponent<Image>();
        image.color = color;

        return image.rectTransform;
    }

    private Text CreateText(string objectName, Transform parent, string value, int fontSize,
        Color color, Vector2 position)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);

        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;

        RectTransform rect = text.rectTransform;
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(780f, 120f);

        return text;
    }

    private void CreateButton(string label, Transform parent, Vector2 position, Action onClick)
    {
        RectTransform rect = CreateImage(label + " Button", parent, buttonColor);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(560f, 130f);

        Button button = rect.gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => onClick());

        CreateText("Label", rect, label, 60, Color.white, Vector2.zero);
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }
}
