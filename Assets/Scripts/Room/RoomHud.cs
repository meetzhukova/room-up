using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class RoomHud : MonoBehaviour
{
    public event Action<int> ItemClicked;
    public event Action RotateClicked;
    public event Action PlaceClicked;
    public event Action StoreClicked;
    public event Action PlayClicked;

    [SerializeField] private Color panelColor = new Color(0.16f, 0.17f, 0.24f, 0.9f);
    [SerializeField] private Color buttonColor = new Color(0.29f, 0.53f, 0.96f);
    [SerializeField] private Color placeColor = new Color(0.3f, 0.75f, 0.4f);

    private RectTransform inventoryPanel;
    private RectTransform itemsGrid;
    private RectTransform placingPanel;
    private Button rotateButton;
    private Button placeButton;
    private Text coinsText;

    private void Awake()
    {
        EnsureEventSystem();
        Build();
    }

    public void ShowInventory(IReadOnlyList<FurnitureData> items)
    {
        foreach (Transform child in itemsGrid)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            CreateButton(items[i].name, itemsGrid, buttonColor, () => ItemClicked?.Invoke(index));
        }

        inventoryPanel.gameObject.SetActive(true);
        placingPanel.gameObject.SetActive(false);
    }

    public void ShowPlacing(bool canRotate)
    {
        rotateButton.interactable = canRotate;
        inventoryPanel.gameObject.SetActive(false);
        placingPanel.gameObject.SetActive(true);
    }

    public void SetCoins(int coins)
    {
        coinsText.text = "Coins: " + coins;
    }

    public void SetPlaceEnabled(bool isEnabled)
    {
        placeButton.interactable = isEnabled;
    }

    private void Build()
    {
        GameObject root = new GameObject("Room Hud", typeof(RectTransform));
        root.transform.SetParent(transform, false);

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;

        root.AddComponent<GraphicRaycaster>();

        RectTransform topBar = CreateRect("Top Bar", root.transform);
        topBar.anchorMin = new Vector2(0f, 1f);
        topBar.anchorMax = new Vector2(1f, 1f);
        topBar.pivot = new Vector2(0.5f, 1f);
        topBar.offsetMin = new Vector2(40f, -260f);
        topBar.offsetMax = new Vector2(-40f, -120f);

        coinsText = CreateLabel("Coins", topBar, "Coins: 0", 64, TextAnchor.MiddleLeft);
        Stretch((RectTransform)coinsText.transform);

        Button playButton = CreateButton("Play", topBar, placeColor, () => PlayClicked?.Invoke());
        RectTransform playRect = (RectTransform)playButton.transform;
        playRect.anchorMin = new Vector2(1f, 0.5f);
        playRect.anchorMax = new Vector2(1f, 0.5f);
        playRect.pivot = new Vector2(1f, 0.5f);
        playRect.sizeDelta = new Vector2(300f, 130f);

        inventoryPanel = CreateBottomPanel("Inventory", root.transform, 440f);
        itemsGrid = CreateRect("Items", inventoryPanel);
        itemsGrid.anchorMin = Vector2.zero;
        itemsGrid.anchorMax = Vector2.one;
        itemsGrid.offsetMin = Vector2.zero;
        itemsGrid.offsetMax = Vector2.zero;

        GridLayoutGroup layout = itemsGrid.gameObject.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(320f, 120f);
        layout.spacing = new Vector2(20f, 20f);
        layout.padding = new RectOffset(30, 30, 40, 40);
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 3;

        placingPanel = CreateBottomPanel("Placing", root.transform, 240f);
        HorizontalLayoutGroup row = placingPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
        row.spacing = 20f;
        row.padding = new RectOffset(30, 30, 50, 50);
        row.childAlignment = TextAnchor.MiddleCenter;
        row.childForceExpandWidth = true;
        row.childForceExpandHeight = true;

        rotateButton = CreateButton("Rotate", placingPanel, buttonColor, () => RotateClicked?.Invoke());
        placeButton = CreateButton("Place", placingPanel, placeColor, () => PlaceClicked?.Invoke());
        CreateButton("Store", placingPanel, buttonColor, () => StoreClicked?.Invoke());

        placingPanel.gameObject.SetActive(false);
    }

    private RectTransform CreateBottomPanel(string objectName, Transform parent, float height)
    {
        RectTransform panel = CreateRect(objectName, parent);
        panel.gameObject.AddComponent<Image>().color = panelColor;
        panel.anchorMin = new Vector2(0f, 0f);
        panel.anchorMax = new Vector2(1f, 0f);
        panel.pivot = new Vector2(0.5f, 0f);
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = new Vector2(0f, height);
        return panel;
    }

    private RectTransform CreateRect(string objectName, Transform parent)
    {
        GameObject rectObject = new GameObject(objectName, typeof(RectTransform));
        rectObject.transform.SetParent(parent, false);
        return (RectTransform)rectObject.transform;
    }

    private Button CreateButton(string label, Transform parent, Color color, Action onClick)
    {
        RectTransform rect = CreateRect(label + " Button", parent);
        rect.gameObject.AddComponent<Image>().color = color;

        Button button = rect.gameObject.AddComponent<Button>();
        button.onClick.AddListener(() => onClick());

        Text text = CreateLabel("Label", rect, label, 48, TextAnchor.MiddleCenter);
        Stretch((RectTransform)text.transform);

        return button;
    }

    private Text CreateLabel(string objectName, Transform parent, string value, int fontSize, TextAnchor alignment)
    {
        RectTransform rect = CreateRect(objectName, parent);

        Text text = rect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = alignment;
        text.color = Color.white;

        return text;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
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
