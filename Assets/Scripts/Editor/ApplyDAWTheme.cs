#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Applies a Logic Pro / Final Cut Pro inspired dark professional theme to the current scene.
/// Run via MusicNoteGame > Apply DAW Theme.
/// </summary>
public class ApplyDAWTheme : Editor
{
    // ── Palette ────────────────────────────────────────────────────────────────
    static readonly Color BG          = Hex("#0F0F14"); // deep near-black
    static readonly Color Panel       = Hex("#181820"); // background panel
    static readonly Color Card        = Hex("#1E1E2A"); // elevated surface
    static readonly Color Elevated    = Hex("#252535"); // buttons / cards
    static readonly Color BorderColor = Hex("#353548"); // subtle border tint
    static readonly Color Accent      = Hex("#4B9EFF"); // Logic Pro blue
    static readonly Color AccentDark  = Hex("#193D8F"); // selected state
    static readonly Color AccentDeep  = Hex("#0F2660"); // pressed state
    static readonly Color Success     = Hex("#32D74B"); // correct green
    static readonly Color ErrorRed    = Hex("#FF453A"); // wrong / warning red
    static readonly Color StartBlue   = Hex("#1A5FFF"); // start button
    static readonly Color StartHover  = Hex("#3A7AFF");
    static readonly Color StartPress  = Hex("#0D3FCC");
    static readonly Color TextPri     = Hex("#F0F0F5"); // primary text
    static readonly Color TextSec     = Hex("#8B8B9E"); // secondary / labels
    static readonly Color TextDim     = Hex("#52526A"); // tertiary / hints
    static readonly Color StaffLine   = Hex("#4A4A62"); // staff line color
    static readonly Color NoteWhite   = Hex("#E8E8F0"); // note head
    static readonly Color Overlay     = new Color(0.04f, 0.04f, 0.07f, 0.93f);

    // ── Entry point ────────────────────────────────────────────────────────────
    [MenuItem("MusicNoteGame/Apply DAW Theme")]
    public static void ApplyTheme()
    {
        Apply();
        EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[ApplyDAWTheme] Theme applied. Save the scene to persist.");
    }

    static void Apply()
    {
        // Camera
        if (Camera.main != null)
            Camera.main.backgroundColor = BG;

        // ── Backgrounds ────────────────────────────────────────────────────────
        SetImageColor("Background", BG);

        // Staff lines (named StaffLine0 … StaffLine4)
        foreach (var img in All<Image>())
            if (img.name.StartsWith("StaffLine")) img.color = StaffLine;

        // Ledger line prefab / live ledger lines
        foreach (var img in All<Image>())
            if (img.name.StartsWith("LedgerLine") || img.name == "LedgerLinePrefab")
                img.color = StaffLine;

        // Clef image placeholder
        SetImageColor("ClefImage", new Color(0.69f, 0.69f, 0.78f));

        // Note head
        SetImageColor("Note", NoteWhite);

        // ── Main Menu ──────────────────────────────────────────────────────────
        SetTextColor("Title",    TextPri, 3f);
        SetTextColor("Subtitle", TextSec);
        SetTextColor("ClefLabel",  TextDim);
        SetTextColor("ModeLabel",  TextDim);
        SetTextColor("DifficultyDescText", TextSec);

        SetTextColor("TrebleHighScore", TextDim);
        SetTextColor("BassHighScore",   TextDim);

        // Selection buttons (clef / mode / difficulty / chords)
        StyleButton("TrebleButton",   Elevated, BorderColor, AccentDark, AccentDeep);
        StyleButton("BassButton",     Elevated, BorderColor, AccentDark, AccentDeep);
        StyleButton("PracticeButton", Elevated, BorderColor, AccentDark, AccentDeep);
        StyleButton("TimedButton",    Elevated, BorderColor, AccentDark, AccentDeep);
        StyleButton("EasyButton",     Elevated, BorderColor, AccentDark, AccentDeep);
        StyleButton("MediumButton",   Elevated, BorderColor, AccentDark, AccentDeep);
        StyleButton("HardButton",     Elevated, BorderColor, AccentDark, AccentDeep);
        StyleButton("ChordsButton",   Elevated, BorderColor, AccentDark, AccentDeep);

        // Start button
        StyleButton("StartButton", StartBlue, StartHover, StartPress, StartPress);
        SetButtonTextColor("StartButton", TextPri);

        // Back button (subtle)
        StyleButton("BackButton", Card, Elevated, Panel, Panel);

        // MainMenuController serialized colors
        var mmc = Object.FindFirstObjectByType<MusicNoteGame.UI.MainMenuController>();
        if (mmc != null)
        {
            var so = new SerializedObject(mmc);
            so.FindProperty("selectedColor").colorValue   = AccentDark;
            so.FindProperty("unselectedColor").colorValue = Elevated;
            so.ApplyModifiedProperties();
        }

        // ── Game Scene HUD ─────────────────────────────────────────────────────
        SetTextColor("ScoreText",    TextPri);
        SetTextColor("StreakText",   Accent);
        SetTextColor("LevelText",    TextSec);
        SetTextColor("ProgressText", TextSec);
        SetTextColor("HintText",     TextDim);
        SetTextColor("AccidentalIndicatorText", Accent);
        SetTextColor("PressedNotesText",        Accent);
        // FeedbackText color is set dynamically by HUDController — leave it

        // Timer bar background
        foreach (var img in All<Image>())
            if (img.name == "Background" && img.transform.parent != null &&
                img.transform.parent.name == "TimerBar")
                img.color = Elevated;

        // Progress bar background & fill
        foreach (var img in All<Image>())
        {
            if (img.transform.parent == null) continue;
            if (img.name == "Background" && img.transform.parent.name == "ProgressBar")
                img.color = Elevated;
            if (img.name == "Fill" && img.transform.parent.name == "Fill Area")
                img.color = Accent; // base fill color (HUDController will also tint it)
        }

        // HUDController theme colors
        var hud = Object.FindFirstObjectByType<MusicNoteGame.UI.HUDController>();
        if (hud != null)
        {
            var so = new SerializedObject(hud);
            so.FindProperty("normalColor").colorValue   = Accent;
            so.FindProperty("warningColor").colorValue  = ErrorRed;
            so.FindProperty("correctColor").colorValue  = Success;
            so.FindProperty("incorrectColor").colorValue = ErrorRed;
            so.ApplyModifiedProperties();
        }

        // ── Game Over Panel ────────────────────────────────────────────────────
        // Overlay
        foreach (var img in All<Image>())
            if (img.name == "GameOverPanel") img.color = Overlay;

        // Content card
        foreach (var img in All<Image>())
            if (img.name == "Content") img.color = Card;

        SetTextColor("NewHighScore", Accent);

        StyleButton("RetryButton",     AccentDark, Accent,     AccentDeep, AccentDeep);
        StyleButton("NextLevelButton", StartBlue,  StartHover, StartPress, StartPress);
        StyleButton("MenuButton",      Card,       Elevated,   Panel,      Panel);

        SetButtonTextColor("RetryButton",     TextPri);
        SetButtonTextColor("NextLevelButton", TextPri);
        SetButtonTextColor("MenuButton",      TextSec);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    static void SetImageColor(string goName, Color color)
    {
        foreach (var img in All<Image>())
            if (img.name == goName) img.color = color;
    }

    static void SetTextColor(string goName, Color color, float charSpacing = 0f)
    {
        foreach (var t in AllInactive<TextMeshProUGUI>())
        {
            if (t.name != goName) continue;
            t.color = color;
            if (charSpacing != 0f) t.characterSpacing = charSpacing;
        }
    }

    /// <summary>
    /// Sets ColorBlock so normal=normalBg, highlighted=hoverBg, pressed=pressedBg.
    /// Image.color is set to white so the ColorBlock states drive the visible color.
    /// </summary>
    static void StyleButton(string goName, Color normalBg, Color hoverBg, Color pressedBg, Color selectedBg)
    {
        foreach (var btn in AllInactive<Button>())
        {
            if (btn.name != goName) continue;
            var img = btn.GetComponent<Image>();
            if (img != null) img.color = Color.white;

            var cb = btn.colors;
            cb.normalColor      = normalBg;
            cb.highlightedColor = hoverBg;
            cb.pressedColor     = pressedBg;
            cb.selectedColor    = selectedBg;
            cb.disabledColor    = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            cb.colorMultiplier  = 1f;
            cb.fadeDuration     = 0.08f;
            btn.colors = cb;
        }
    }

    static void SetButtonTextColor(string goName, Color color)
    {
        foreach (var btn in AllInactive<Button>())
        {
            if (btn.name != goName) continue;
            foreach (var t in btn.GetComponentsInChildren<TextMeshProUGUI>(true))
                t.color = color;
        }
    }

    static T[] All<T>() where T : Object
        => Object.FindObjectsByType<T>(FindObjectsSortMode.None);

    static T[] AllInactive<T>() where T : Object
        => Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }
}
#endif
