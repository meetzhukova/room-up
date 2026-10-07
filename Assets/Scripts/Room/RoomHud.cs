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

    [SerializeField] private Color panelColor = new Color(0.16f, 0.17f, 0.24f, 0.9f);
    [SerializeField] private Color buttonColor = new Color(0.29f, 0.53f, 0.96f);
    [SerializeField] private Color placeColor = new Color(0.3f, 0.75f, 0.4f);

    private RectTransform inventoryPanel;
    private RectTransform itemsGrid;
    private RectTransform placingPanel;
    private Button rotateButton;
    private Button placeButton;

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

        RectTransform labelRect = CreateRect("Label", rect);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Text text = labelRect.gameObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = label;
        text.fontSize = 48;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        return button;
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
