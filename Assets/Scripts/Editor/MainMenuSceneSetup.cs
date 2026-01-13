#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using MusicNoteGame.UI;

public class MainMenuSceneSetup : EditorWindow
{
    [MenuItem("MusicNoteGame/Setup MainMenu Scene")]
    public static void SetupScene()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Background
        GameObject bg = CreateUIElement("Background", canvasObj.transform);
        var bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.15f, 0.15f, 0.25f);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Title
        GameObject title = CreateTMPText("Title", canvasObj.transform, "NOTE MASTER");
        var titleTMP = title.GetComponent<TextMeshProUGUI>();
        titleTMP.fontSize = 72;
        titleTMP.fontStyle = FontStyles.Bold;
        titleTMP.color = Color.white;
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 250);
        titleRect.sizeDelta = new Vector2(600, 100);

        // Subtitle
        GameObject subtitle = CreateTMPText("Subtitle", canvasObj.transform, "Learn to Read Music Notes");
        var subTMP = subtitle.GetComponent<TextMeshProUGUI>();
        subTMP.fontSize = 28;
        subTMP.color = new Color(0.7f, 0.7f, 0.8f);
        var subRect = subtitle.GetComponent<RectTransform>();
        subRect.anchoredPosition = new Vector2(0, 180);

        // Clef Selection Label
        GameObject clefLabel = CreateTMPText("ClefLabel", canvasObj.transform, "Select Clef:");
        var clefLabelRect = clefLabel.GetComponent<RectTransform>();
        clefLabelRect.anchoredPosition = new Vector2(0, 100);

        // Clef Buttons Container
        GameObject clefButtons = CreateUIElement("ClefButtons", canvasObj.transform);
        clefButtons.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);

        // Treble Button
        GameObject trebleBtn = CreateStyledButton("TrebleButton", clefButtons.transform, "TREBLE CLEF");
        trebleBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, 0);
        var trebleHighlight = trebleBtn.GetComponent<Image>();

        // Bass Button
        GameObject bassBtn = CreateStyledButton("BassButton", clefButtons.transform, "BASS CLEF");
        bassBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(120, 0);
        var bassHighlight = bassBtn.GetComponent<Image>();

        // Mode Selection Label
        GameObject modeLabel = CreateTMPText("ModeLabel", canvasObj.transform, "Select Mode:");
        var modeLabelRect = modeLabel.GetComponent<RectTransform>();
        modeLabelRect.anchoredPosition = new Vector2(0, -30);

        // Mode Buttons Container
        GameObject modeButtons = CreateUIElement("ModeButtons", canvasObj.transform);
        modeButtons.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);

        // Practice Button
        GameObject practiceBtn = CreateStyledButton("PracticeButton", modeButtons.transform, "PRACTICE");
        practiceBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, 0);
        var practiceHighlight = practiceBtn.GetComponent<Image>();

        // Timed Button
        GameObject timedBtn = CreateStyledButton("TimedButton", modeButtons.transform, "TIMED");
        timedBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(120, 0);
        var timedHighlight = timedBtn.GetComponent<Image>();

        // Start Button
        GameObject startBtn = CreateStyledButton("StartButton", canvasObj.transform, "START GAME", true);
        var startRect = startBtn.GetComponent<RectTransform>();
        startRect.anchoredPosition = new Vector2(0, -180);
        startRect.sizeDelta = new Vector2(250, 60);
        var startImage = startBtn.GetComponent<Image>();
        startImage.color = new Color(0.2f, 0.6f, 0.3f);

        // High Scores Container
        GameObject highScores = CreateUIElement("HighScores", canvasObj.transform);
        var hsRect = highScores.GetComponent<RectTransform>();
        hsRect.anchoredPosition = new Vector2(0, -280);

        GameObject trebleHS = CreateTMPText("TrebleHighScore", highScores.transform, "Treble High Score: 0");
        var trebleHSTMP = trebleHS.GetComponent<TextMeshProUGUI>();
        trebleHSTMP.fontSize = 20;
        trebleHSTMP.color = new Color(0.6f, 0.6f, 0.7f);
        trebleHS.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);

        GameObject bassHS = CreateTMPText("BassHighScore", highScores.transform, "Bass High Score: 0");
        var bassHSTMP = bassHS.GetComponent<TextMeshProUGUI>();
        bassHSTMP.fontSize = 20;
        bassHSTMP.color = new Color(0.6f, 0.6f, 0.7f);
        bassHS.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -10);

        // Menu Controller
        GameObject menuManager = new GameObject("MenuManager");
        var menuController = menuManager.AddComponent<MainMenuController>();

        // Create EventSystem if not exists
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Wire up MainMenuController
        SerializedObject mcSO = new SerializedObject(menuController);
        mcSO.FindProperty("trebleClefButton").objectReferenceValue = trebleBtn.GetComponent<Button>();
        mcSO.FindProperty("bassClefButton").objectReferenceValue = bassBtn.GetComponent<Button>();
        mcSO.FindProperty("trebleHighlight").objectReferenceValue = trebleHighlight;
        mcSO.FindProperty("bassHighlight").objectReferenceValue = bassHighlight;
        mcSO.FindProperty("practiceButton").objectReferenceValue = practiceBtn.GetComponent<Button>();
        mcSO.FindProperty("timedButton").objectReferenceValue = timedBtn.GetComponent<Button>();
        mcSO.FindProperty("practiceHighlight").objectReferenceValue = practiceHighlight;
        mcSO.FindProperty("timedHighlight").objectReferenceValue = timedHighlight;
        mcSO.FindProperty("startButton").objectReferenceValue = startBtn.GetComponent<Button>();
        mcSO.FindProperty("trebleHighScoreText").objectReferenceValue = trebleHSTMP;
        mcSO.FindProperty("bassHighScoreText").objectReferenceValue = bassHSTMP;
        mcSO.ApplyModifiedProperties();

        Debug.Log("MainMenu Scene setup complete!");
        EditorUtility.DisplayDialog("Setup Complete",
            "MainMenu Scene has been set up!\n\nNext steps:\n1. Save this scene as 'MainMenu'\n2. Add both scenes to Build Settings",
            "OK");
    }

    private static GameObject CreateUIElement(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    private static GameObject CreateTMPText(string name, Transform parent, string text)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 50);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return obj;
    }

    private static GameObject CreateStyledButton(string name, Transform parent, string text, bool large = false)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = large ? new Vector2(200, 50) : new Vector2(200, 45);

        var image = obj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.3f, 0.5f);

        var button = obj.AddComponent<Button>();
        var colors = button.colors;
        colors.highlightedColor = new Color(0.4f, 0.4f, 0.7f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.4f);
        button.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        var textRT = textObj.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;

        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = large ? 28 : 22;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return obj;
    }
}
#endif
