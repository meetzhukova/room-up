using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ShopView : MonoBehaviour
{
    public event Action<int> BuyClicked;
    public event Action CloseClicked;

    [SerializeField] private Color dimColor = new Color(0f, 0f, 0f, 0.65f);
    [SerializeField] private Color panelColor = new Color(0.16f, 0.17f, 0.24f);
    [SerializeField] private Color cardColor = new Color(0.23f, 0.24f, 0.33f);
    [SerializeField] private Color buyColor = new Color(0.3f, 0.75f, 0.4f);
    [SerializeField] private Color closeColor = new Color(0.29f, 0.53f, 0.96f);
    [SerializeField] private Color accentColor = new Color(0.98f, 0.84f, 0.28f);

    private GameObject root;
    private RectTransform cardsGrid;
    private Text coinsText;

    private void Awake()
    {
        Build();
        Hide();
    }

    public void Show(IReadOnlyList<FurnitureData> items, int coins)
    {
        coinsText.text = "Coins: " + coins;

        foreach (Transform child in cardsGrid)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < items.Count; i++)
        {
            CreateCard(items[i], i, coins >= items[i].price);
        }

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
        root = new GameObject("Shop", typeof(RectTransform));
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
        Stretch(dim);

        RectTransform panel = CreateImage("Panel", root.transform, panelColor);
        panel.sizeDelta = new Vector2(980f, 1560f);

        Text title = CreateText("Title", panel, "Shop", 96, accentColor, TextAnchor.MiddleCenter);
        Place(title.rectTransform, new Vector2(0f, 680f), new Vector2(900f, 130f));

        coinsText = CreateText("Coins", panel, "", 60, Color.white, TextAnchor.MiddleCenter);
        Place(coinsText.rectTransform, new Vector2(0f, 570f), new Vector2(900f, 90f));

        cardsGrid = CreateRect("Cards", panel);
        Place(cardsGrid, new Vector2(0f, -20f), new Vector2(920f, 1080f));

        GridLayoutGroup layout = cardsGrid.gameObject.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(440f, 255f);
        layout.spacing = new Vector2(20f, 20f);
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 2;

        Button closeButton = CreateButton("Close", panel, closeColor, () => CloseClicked?.Invoke());
        Place((RectTransform)closeButton.transform, new Vector2(0f, -680f), new Vector2(500f, 130f));
    }

    private void CreateCard(FurnitureData item, int index, bool canBuy)
    {
        RectTransform card = CreateImage(item.name, cardsGrid, cardColor);

        RectTransform swatch = CreateImage("Color", card, item.color);
        Place(swatch, new Vector2(-170f, 70f), new Vector2(60f, 60f));

        Text nameText = CreateText("Name", card, item.name, 52, Color.white, TextAnchor.MiddleLeft);
        Place(nameText.rectTransform, new Vector2(40f, 70f), new Vector2(320f, 70f));

        string place = item.isWallItem ? "Wall" : "Floor";
        Text infoText = CreateText("Info", card, item.size.x + "×" + item.size.y + " · " + place, 36,
            new Color(1f, 1f, 1f, 0.7f), TextAnchor.MiddleLeft);
        Place(infoText.rectTransform, new Vector2(40f, 10f), new Vector2(320f, 50f));

        Button buyButton = CreateButton("Buy " + item.price, card, buyColor, () => BuyClicked?.Invoke(index));
        Place((RectTransform)buyButton.transform, new Vector2(0f, -70f), new Vector2(380f, 100f));
        buyButton.interactable = canBuy;
    }

    private RectTransform CreateRect(string objectName, Transform parent)
    {
        GameObject rectObject = new GameObject(objectName, typeof(RectTransform));
        rectObject.transform.SetParent(parent, false);
        return (RectTransform)rectObject.transform;
    }

    private RectTransform CreateImage(string objectName, Transform parent, Color color)
    {
        RectTransform rect = CreateRect(objectName, parent);
        rect.gameObject.AddComponent<Image>().color = color;
        return rect;
    }

    private Text CreateText(string objectName, Transform parent, string value, int fontSize, Color color, TextAnchor alignment)
    {
        RectTransform rect = CreateRect(objectName, parent);

        Text text = rect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;

        return text;
    }

    private Button CreateButton(string label, Transform parent, Color color, Action onClick)
    {
        RectTransform rect = CreateImage(label + " Button", parent, color);

        Button button = rect.gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => onClick());

        Text text = CreateText("Label", rect, label, 48, Color.white, TextAnchor.MiddleCenter);
        Stretch(text.rectTransform);

        return button;
    }

    private static void Place(RectTransform rect, Vector2 position, Vector2 size)
    {
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
