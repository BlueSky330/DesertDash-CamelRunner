using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Runtime skin selection panel — Pharaoh / Racing / Mummy / Golden unlock + equip UI.
///
/// Fully self-bootstrapping: registers itself after scene load so no prefab or
/// Inspector wiring is required.  Call SkinSelectionPanel.Instance.Show() /
/// SkinSelectionPanel.Instance.Hide() from any other script (e.g., UIManager).
///
/// Skin rows show:
///   - Skin name
///   - Cost in coins (or "FREE" for base skin)
///   - Status badge  (EQUIPPED | UNLOCKED | LOCKED)
///   - Action button (EQUIP | BUY | greyed-out BUY when insufficient coins)
/// </summary>
[DisallowMultipleComponent]
public class SkinSelectionPanel : MonoBehaviour
{
    public static SkinSelectionPanel Instance { get; private set; }

    // ── Bootstrap ──────────────────────────────────────────────────────────────

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindObjectOfType<SkinSelectionPanel>() != null) return;
        var go = new GameObject("[SkinSelectionPanel]");
        go.AddComponent<SkinSelectionPanel>();
        DontDestroyOnLoad(go);
    }

    // ── Internal row tracking ──────────────────────────────────────────────────

    private struct SkinRow
    {
        public string  skinName;
        public Text    statusText;
        public Button  actionButton;
        public Text    actionLabel;
        public Image   buttonBackground;
    }

    // ── References ─────────────────────────────────────────────────────────────

    private GameObject       _rootPanel;
    private Transform        _skinListRoot;
    private Text             _coinsText;
    private readonly List<SkinRow> _rows = new();

    // Colour palette
    private static readonly Color ColBg         = new(0.10f, 0.08f, 0.05f, 0.96f);
    private static readonly Color ColTitle       = new(1.00f, 0.84f, 0.20f, 1.00f);
    private static readonly Color ColCoins       = new(1.00f, 0.84f, 0.00f, 1.00f);
    private static readonly Color ColRowAlt      = new(1.00f, 1.00f, 1.00f, 0.04f);
    private static readonly Color ColBtnEquip    = new(0.15f, 0.55f, 0.15f, 1.00f);
    private static readonly Color ColBtnBuy      = new(0.20f, 0.45f, 0.75f, 1.00f);
    private static readonly Color ColBtnDisabled = new(0.30f, 0.30f, 0.30f, 1.00f);
    private static readonly Color ColBtnClose    = new(0.60f, 0.10f, 0.10f, 1.00f);
    private static readonly Color ColStatusEquip = new(0.20f, 0.85f, 0.20f, 1.00f);
    private static readonly Color ColStatusLock  = new(0.65f, 0.65f, 0.65f, 1.00f);
    private static readonly Color ColStatusUnlck = new(0.80f, 0.75f, 0.10f, 1.00f);

    // ── Unity lifecycle ────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        BuildCanvas();
        Hide();
    }

    private void Start()
    {
        PopulateSkinRows();
        RefreshAllRows();
        RefreshCoinsDisplay(CoinEconomy.Instance != null ? CoinEconomy.Instance.Coins : 0);

        SkinManager.onSkinEquipped  += OnSkinEquipped;
        CoinEconomy.onCoinsChanged  += RefreshCoinsDisplay;
    }

    private void OnDestroy()
    {
        SkinManager.onSkinEquipped  -= OnSkinEquipped;
        CoinEconomy.onCoinsChanged  -= RefreshCoinsDisplay;
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    public void Show()
    {
        _rootPanel.SetActive(true);
        RefreshAllRows();
    }

    public void Hide() => _rootPanel.SetActive(false);

    public bool IsVisible => _rootPanel != null && _rootPanel.activeSelf;

    // ── Canvas construction ────────────────────────────────────────────────────

    private void BuildCanvas()
    {
        // ── Canvas ──
        var canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(transform, false);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ── Dim overlay ──
        _rootPanel = MakePanel(canvasGO.transform, "Overlay",
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            new Color(0, 0, 0, 0.65f));

        // ── Card ──
        var card = MakePanel(_rootPanel.transform, "Card",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(-260, -280), new Vector2(260, 280),
            ColBg);

        // ── Title ──
        MakeText(card.transform, "Title", "SELECT SKIN",
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(10, -70), new Vector2(-10, 0),
            22, FontStyle.Bold, ColTitle, TextAnchor.MiddleCenter);

        // ── Divider ──
        MakeDivider(card.transform, -72f);

        // ── Coins display ──
        _coinsText = MakeText(card.transform, "CoinsLabel", "Coins: ---",
            new Vector2(0, 1), new Vector2(1, 1),
            new Vector2(10, -110), new Vector2(-10, -72),
            15, FontStyle.Normal, ColCoins, TextAnchor.MiddleRight);

        // ── Skin list container (populated in Start once SkinManager is ready) ──
        _skinListRoot = MakePanel(card.transform, "SkinList",
            new Vector2(0, 0), new Vector2(1, 1),
            new Vector2(5, 5), new Vector2(-5, -115),
            Color.clear).transform;

        // ── Close button (top-right) ──
        var closeBtn = MakeButton(card.transform, "CloseBtn", "✕",
            new Vector2(1, 1), new Vector2(1, 1),
            new Vector2(-52, -52), new Vector2(0, 0),
            ColBtnClose, 18, FontStyle.Bold);
        closeBtn.onClick.AddListener(Hide);
    }

    // ── Skin row population (called in Start after SkinManager is ready) ────────

    private void PopulateSkinRows()
    {
        if (SkinManager.Instance == null || _skinListRoot == null) return;

        const float rowH = 52f;
        int idx = 0;
        foreach (var skin in SkinManager.Instance.availableSkins)
        {
            BuildSkinRow(_skinListRoot, skin, idx, rowH);
            idx++;
        }
    }

    private void BuildSkinRow(Transform parent, SkinManager.CamelSkin skin, int index, float rowH)
    {
        float yTop = -index * rowH;

        var rowGO = new GameObject($"Row_{skin.skinName}");
        rowGO.transform.SetParent(parent, false);

        var rt = rowGO.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(4, yTop - rowH + 2);
        rt.offsetMax = new Vector2(-4, yTop - 2);

        // Alternating row tint
        var bg = rowGO.AddComponent<Image>();
        bg.color = index % 2 == 0 ? ColRowAlt : Color.clear;

        Font font = GetDefaultFont();

        // Name label (left 40%)
        AddTextChild(rowGO.transform, "Name", skin.skinName,
            new Vector2(0, 0), new Vector2(0.40f, 1),
            Vector2.zero, Vector2.zero,
            13, FontStyle.Normal, Color.white, TextAnchor.MiddleLeft, font);

        // Cost label (next 22%)
        string costStr = skin.cost == 0 ? "FREE" : $"{skin.cost} \u25ce";
        AddTextChild(rowGO.transform, "Cost", costStr,
            new Vector2(0.40f, 0), new Vector2(0.62f, 1),
            Vector2.zero, Vector2.zero,
            12, FontStyle.Normal, ColCoins, TextAnchor.MiddleCenter, font);

        // Status badge (next 18%)
        var statusText = AddTextChild(rowGO.transform, "Status", "",
            new Vector2(0.62f, 0), new Vector2(0.80f, 1),
            Vector2.zero, Vector2.zero,
            11, FontStyle.Italic, ColStatusLock, TextAnchor.MiddleCenter, font);

        // Action button (last 20%)
        var btnGO = new GameObject("ActionBtn");
        btnGO.transform.SetParent(rowGO.transform, false);
        var btnRT = btnGO.AddComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.80f, 0.10f);
        btnRT.anchorMax = new Vector2(1.00f, 0.90f);
        btnRT.offsetMin = new Vector2(2, 0);
        btnRT.offsetMax = new Vector2(-2, 0);

        var btnImg   = btnGO.AddComponent<Image>();
        var actionBtn = btnGO.AddComponent<Button>();
        actionBtn.targetGraphic = btnImg;

        var lblGO = new GameObject("Lbl");
        lblGO.transform.SetParent(btnGO.transform, false);
        var lblRT = lblGO.AddComponent<RectTransform>();
        lblRT.anchorMin = Vector2.zero;
        lblRT.anchorMax = Vector2.one;
        lblRT.offsetMin = Vector2.zero;
        lblRT.offsetMax = Vector2.zero;
        var actionLbl = lblGO.AddComponent<Text>();
        actionLbl.font      = font;
        actionLbl.fontSize  = 12;
        actionLbl.fontStyle = FontStyle.Bold;
        actionLbl.alignment = TextAnchor.MiddleCenter;
        actionLbl.color     = Color.white;

        string capturedName = skin.skinName;
        actionBtn.onClick.AddListener(() => OnActionClicked(capturedName));

        _rows.Add(new SkinRow
        {
            skinName        = skin.skinName,
            statusText      = statusText,
            actionButton    = actionBtn,
            actionLabel     = actionLbl,
            buttonBackground = btnImg,
        });
    }

    // ── Row refresh ────────────────────────────────────────────────────────────

    private void RefreshAllRows()
    {
        foreach (var row in _rows)
            RefreshRow(row);
    }

    private void RefreshRow(SkinRow row)
    {
        if (SkinManager.Instance == null) return;
        var skin = SkinManager.Instance.GetSkin(row.skinName);
        if (skin == null) return;

        int playerCoins = CoinEconomy.Instance != null ? CoinEconomy.Instance.Coins : 0;

        if (skin.isEquipped)
        {
            row.statusText.text  = "EQUIPPED";
            row.statusText.color = ColStatusEquip;
            row.actionLabel.text = "EQUIP";
            row.buttonBackground.color = ColBtnEquip;
            row.actionButton.interactable = false;
        }
        else if (skin.isUnlocked)
        {
            row.statusText.text  = "UNLOCKED";
            row.statusText.color = ColStatusUnlck;
            row.actionLabel.text = "EQUIP";
            row.buttonBackground.color = ColBtnEquip;
            row.actionButton.interactable = true;
        }
        else
        {
            row.statusText.text  = "LOCKED";
            row.statusText.color = ColStatusLock;
            row.actionLabel.text = "BUY";

            bool canAfford = playerCoins >= skin.cost;
            row.buttonBackground.color    = canAfford ? ColBtnBuy : ColBtnDisabled;
            row.actionButton.interactable = canAfford;
        }
    }

    // ── Button callbacks ───────────────────────────────────────────────────────

    private void OnActionClicked(string skinName)
    {
        if (SkinManager.Instance == null) return;
        var skin = SkinManager.Instance.GetSkin(skinName);
        if (skin == null) return;

        if (skin.isUnlocked)
            SkinManager.Instance.EquipSkin(skinName);
        else
            SkinManager.Instance.TryUnlockSkin(skinName);
    }

    // ── Event handlers ─────────────────────────────────────────────────────────

    private void OnSkinEquipped(string _) => RefreshAllRows();

    private void RefreshCoinsDisplay(int coins)
    {
        if (_coinsText != null)
            _coinsText.text = $"Coins: {coins} \u25ce";
        RefreshAllRows();
    }

    // ── UI primitive helpers ───────────────────────────────────────────────────

    private static Font GetDefaultFont()
    {
        var font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return font;
    }

    private static GameObject MakePanel(Transform parent, string goName,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax,
        Color color)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        var img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    private static Text MakeText(Transform parent, string goName, string content,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax,
        int size, FontStyle style, Color color, TextAnchor alignment)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        var t = go.AddComponent<Text>();
        t.font      = GetDefaultFont();
        t.text      = content;
        t.fontSize  = size;
        t.fontStyle = style;
        t.color     = color;
        t.alignment = alignment;
        return t;
    }

    private static Text AddTextChild(Transform parent, string goName, string content,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax,
        int size, FontStyle style, Color color, TextAnchor alignment, Font font)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        var t = go.AddComponent<Text>();
        t.font      = font;
        t.text      = content;
        t.fontSize  = size;
        t.fontStyle = style;
        t.color     = color;
        t.alignment = alignment;
        return t;
    }

    private static Button MakeButton(Transform parent, string goName, string label,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax,
        Color bgColor, int fontSize, FontStyle fontStyle)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var lblGO = new GameObject("Lbl");
        lblGO.transform.SetParent(go.transform, false);
        var lblRT = lblGO.AddComponent<RectTransform>();
        lblRT.anchorMin = Vector2.zero;
        lblRT.anchorMax = Vector2.one;
        lblRT.offsetMin = Vector2.zero;
        lblRT.offsetMax = Vector2.zero;
        var t = lblGO.AddComponent<Text>();
        t.font      = GetDefaultFont();
        t.text      = label;
        t.fontSize  = fontSize;
        t.fontStyle = fontStyle;
        t.color     = Color.white;
        t.alignment = TextAnchor.MiddleCenter;
        return btn;
    }

    private static void MakeDivider(Transform parent, float yOffset)
    {
        var go = new GameObject("Divider");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = new Vector2(10, yOffset - 1);
        rt.offsetMax = new Vector2(-10, yOffset);
        var img = go.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0.12f);
    }
}
