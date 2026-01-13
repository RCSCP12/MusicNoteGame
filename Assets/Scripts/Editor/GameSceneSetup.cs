#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using MusicNoteGame.Core;
using MusicNoteGame.Gameplay;
using MusicNoteGame.Input;
using MusicNoteGame.UI;

public class GameSceneSetup : EditorWindow
{
    [MenuItem("MusicNoteGame/Setup Game Scene")]
    public static void SetupScene()
    {
        // Create GameManager object
        GameObject gameManagerObj = new GameObject("GameManager");
        var gameManager = gameManagerObj.AddComponent<GameManager>();
        var noteGenerator = gameManagerObj.AddComponent<NoteGenerator>();
        var inputHandler = gameManagerObj.AddComponent<NoteInputHandler>();
        var timerController = gameManagerObj.AddComponent<TimerController>();
        var scoreManager = gameManagerObj.AddComponent<ScoreManager>();
        var levelManager = gameManagerObj.AddComponent<LevelManager>();
        gameManagerObj.AddComponent<GameSceneInitializer>();

        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Staff Container
        GameObject staffContainer = CreateUIElement("StaffContainer", canvasObj.transform);
        var staffRect = staffContainer.GetComponent<RectTransform>();
        staffRect.anchoredPosition = Vector2.zero;
        staffRect.sizeDelta = new Vector2(600, 200);
        var staffDisplay = staffContainer.AddComponent<StaffDisplay>();

        // Create 5 Staff Lines
        for (int i = 0; i < 5; i++)
        {
            GameObject line = CreateUIElement($"StaffLine{i}", staffContainer.transform);
            var lineImage = line.AddComponent<Image>();
            lineImage.color = Color.black;
            var lineRect = line.GetComponent<RectTransform>();
            lineRect.sizeDelta = new Vector2(500, 3);
            lineRect.anchoredPosition = new Vector2(0, (i - 2) * 20); // -40, -20, 0, 20, 40
        }

        // Create Clef Image placeholder
        GameObject clefObj = CreateUIElement("ClefImage", staffContainer.transform);
        var clefImage = clefObj.AddComponent<Image>();
        clefImage.color = new Color(0.3f, 0.3f, 0.3f);
        var clefRect = clefObj.GetComponent<RectTransform>();
        clefRect.sizeDelta = new Vector2(60, 120);
        clefRect.anchoredPosition = new Vector2(-220, 0);

        // Create Note object
        GameObject noteObj = CreateUIElement("Note", staffContainer.transform);
        var noteImage = noteObj.AddComponent<Image>();
        noteImage.color = Color.black;
        var noteRect = noteObj.GetComponent<RectTransform>();
        noteRect.sizeDelta = new Vector2(35, 25);
        noteRect.anchoredPosition = new Vector2(50, 0);
        var noteDisplay = noteObj.AddComponent<NoteDisplay>();

        // Create Ledger Line Container
        GameObject ledgerContainer = CreateUIElement("LedgerLines", staffContainer.transform);

        // Create Ledger Line Prefab (for reference)
        GameObject ledgerPrefab = CreateUIElement("LedgerLinePrefab", staffContainer.transform);
        var ledgerImage = ledgerPrefab.AddComponent<Image>();
        ledgerImage.color = Color.black;
        var ledgerRect = ledgerPrefab.GetComponent<RectTransform>();
        ledgerRect.sizeDelta = new Vector2(50, 3);
        ledgerPrefab.SetActive(false);

        // Create HUD
        GameObject hudObj = new GameObject("HUD");
        hudObj.transform.SetParent(canvasObj.transform, false);
        var hudController = hudObj.AddComponent<HUDController>();

        // Score Text (top right)
        GameObject scoreText = CreateTMPText("ScoreText", hudObj.transform, "Score: 0");
        var scoreRect = scoreText.GetComponent<RectTransform>();
        scoreRect.anchorMin = scoreRect.anchorMax = new Vector2(1, 1);
        scoreRect.anchoredPosition = new Vector2(-100, -30);

        // Streak Text
        GameObject streakText = CreateTMPText("StreakText", hudObj.transform, "");
        var streakRect = streakText.GetComponent<RectTransform>();
        streakRect.anchorMin = streakRect.anchorMax = new Vector2(1, 1);
        streakRect.anchoredPosition = new Vector2(-100, -60);

        // Level Text (top left)
        GameObject levelText = CreateTMPText("LevelText", hudObj.transform, "Level 1/5");
        var levelRect = levelText.GetComponent<RectTransform>();
        levelRect.anchorMin = levelRect.anchorMax = new Vector2(0, 1);
        levelRect.anchoredPosition = new Vector2(100, -30);

        // Progress Text
        GameObject progressText = CreateTMPText("ProgressText", hudObj.transform, "0/5");
        var progressRect = progressText.GetComponent<RectTransform>();
        progressRect.anchorMin = progressRect.anchorMax = new Vector2(0, 1);
        progressRect.anchoredPosition = new Vector2(100, -60);

        // Timer Bar (top center)
        GameObject timerBar = CreateUIElement("TimerBar", hudObj.transform);
        var timerSlider = timerBar.AddComponent<Slider>();
        timerSlider.minValue = 0;
        timerSlider.maxValue = 10;
        timerSlider.value = 10;
        var timerBarRect = timerBar.GetComponent<RectTransform>();
        timerBarRect.anchorMin = timerBarRect.anchorMax = new Vector2(0.5f, 1);
        timerBarRect.sizeDelta = new Vector2(300, 20);
        timerBarRect.anchoredPosition = new Vector2(0, -30);

        // Timer background
        GameObject timerBg = CreateUIElement("Background", timerBar.transform);
        var timerBgImage = timerBg.AddComponent<Image>();
        timerBgImage.color = new Color(0.2f, 0.2f, 0.2f);
        var timerBgRect = timerBg.GetComponent<RectTransform>();
        timerBgRect.anchorMin = Vector2.zero;
        timerBgRect.anchorMax = Vector2.one;
        timerBgRect.sizeDelta = Vector2.zero;

        // Timer fill area
        GameObject fillArea = CreateUIElement("Fill Area", timerBar.transform);
        var fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = Vector2.zero;

        GameObject fill = CreateUIElement("Fill", fillArea.transform);
        var fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.green;
        var fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        timerSlider.fillRect = fillRect;

        // Feedback Text (center)
        GameObject feedbackText = CreateTMPText("FeedbackText", hudObj.transform, "Correct!");
        var feedbackTMP = feedbackText.GetComponent<TextMeshProUGUI>();
        feedbackTMP.fontSize = 36;
        feedbackTMP.alignment = TextAlignmentOptions.Center;
        var feedbackRect = feedbackText.GetComponent<RectTransform>();
        feedbackRect.anchoredPosition = new Vector2(0, -150);
        feedbackText.SetActive(false);

        // Input Hint (bottom)
        GameObject hintText = CreateTMPText("HintText", hudObj.transform, "Press A - G");
        var hintRect = hintText.GetComponent<RectTransform>();
        hintRect.anchorMin = hintRect.anchorMax = new Vector2(0.5f, 0);
        hintRect.anchoredPosition = new Vector2(0, 50);

        // Create Game Over Panel
        GameObject gameOverPanel = CreateUIElement("GameOverPanel", canvasObj.transform);
        var panelImage = gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        var panelRect = gameOverPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        var gameOverPanelScript = gameOverPanel.AddComponent<GameOverPanel>();

        // Panel content container
        GameObject panelContent = CreateUIElement("Content", gameOverPanel.transform);
        var contentRect = panelContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(400, 400);

        GameObject titleGO = CreateTMPText("Title", panelContent.transform, "Congratulations!");
        var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
        titleTMP.fontSize = 40;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);

        GameObject finalScore = CreateTMPText("FinalScore", panelContent.transform, "Final Score: 0");
        finalScore.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 80);

        GameObject correctGO = CreateTMPText("Correct", panelContent.transform, "Correct: 0");
        correctGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);

        GameObject incorrectGO = CreateTMPText("Incorrect", panelContent.transform, "Incorrect: 0");
        incorrectGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 20);

        GameObject accuracyGO = CreateTMPText("Accuracy", panelContent.transform, "Accuracy: 0%");
        accuracyGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -10);

        GameObject bestStreakGO = CreateTMPText("BestStreak", panelContent.transform, "Best Streak: 0");
        bestStreakGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);

        GameObject newHighScore = CreateTMPText("NewHighScore", panelContent.transform, "NEW HIGH SCORE!");
        var newHSTMP = newHighScore.GetComponent<TextMeshProUGUI>();
        newHSTMP.color = Color.yellow;
        newHSTMP.fontSize = 30;
        newHighScore.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);

        // Buttons
        GameObject retryBtn = CreateButton("RetryButton", panelContent.transform, "Retry");
        retryBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-80, -140);

        GameObject menuBtn = CreateButton("MenuButton", panelContent.transform, "Main Menu");
        menuBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(80, -140);

        gameOverPanel.SetActive(false);

        // Create EventSystem if not exists
        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }

        // Wire up references using SerializedObject
        SerializedObject gmSO = new SerializedObject(gameManager);
        gmSO.FindProperty("noteGenerator").objectReferenceValue = noteGenerator;
        gmSO.FindProperty("inputHandler").objectReferenceValue = inputHandler;
        gmSO.FindProperty("timerController").objectReferenceValue = timerController;
        gmSO.FindProperty("scoreManager").objectReferenceValue = scoreManager;
        gmSO.FindProperty("levelManager").objectReferenceValue = levelManager;
        gmSO.FindProperty("staffDisplay").objectReferenceValue = staffDisplay;
        gmSO.FindProperty("noteDisplay").objectReferenceValue = noteDisplay;
        gmSO.FindProperty("hudController").objectReferenceValue = hudController;
        gmSO.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanelScript;
        gmSO.ApplyModifiedProperties();

        // Wire up StaffDisplay
        SerializedObject sdSO = new SerializedObject(staffDisplay);
        sdSO.FindProperty("clefImage").objectReferenceValue = clefImage;
        sdSO.FindProperty("ledgerLinePrefab").objectReferenceValue = ledgerPrefab;
        sdSO.FindProperty("ledgerLineContainer").objectReferenceValue = ledgerContainer.transform;
        sdSO.FindProperty("lineSpacing").floatValue = 20f;
        sdSO.ApplyModifiedProperties();

        // Wire up NoteDisplay
        SerializedObject ndSO = new SerializedObject(noteDisplay);
        ndSO.FindProperty("noteTransform").objectReferenceValue = noteRect;
        ndSO.FindProperty("noteImage").objectReferenceValue = noteImage;
        ndSO.FindProperty("staffDisplay").objectReferenceValue = staffDisplay;
        ndSO.ApplyModifiedProperties();

        // Wire up HUDController
        SerializedObject hudSO = new SerializedObject(hudController);
        hudSO.FindProperty("scoreText").objectReferenceValue = scoreText.GetComponent<TextMeshProUGUI>();
        hudSO.FindProperty("streakText").objectReferenceValue = streakText.GetComponent<TextMeshProUGUI>();
        hudSO.FindProperty("levelText").objectReferenceValue = levelText.GetComponent<TextMeshProUGUI>();
        hudSO.FindProperty("progressText").objectReferenceValue = progressText.GetComponent<TextMeshProUGUI>();
        hudSO.FindProperty("timerBar").objectReferenceValue = timerSlider;
        hudSO.FindProperty("timerFill").objectReferenceValue = fillImage;
        hudSO.FindProperty("feedbackText").objectReferenceValue = feedbackText.GetComponent<TextMeshProUGUI>();
        hudSO.ApplyModifiedProperties();

        // Wire up GameOverPanel
        SerializedObject gopSO = new SerializedObject(gameOverPanelScript);
        gopSO.FindProperty("panelRoot").objectReferenceValue = gameOverPanel;
        gopSO.FindProperty("titleText").objectReferenceValue = titleGO.GetComponent<TextMeshProUGUI>();
        gopSO.FindProperty("finalScoreText").objectReferenceValue = finalScore.GetComponent<TextMeshProUGUI>();
        gopSO.FindProperty("correctText").objectReferenceValue = correctGO.GetComponent<TextMeshProUGUI>();
        gopSO.FindProperty("incorrectText").objectReferenceValue = incorrectGO.GetComponent<TextMeshProUGUI>();
        gopSO.FindProperty("accuracyText").objectReferenceValue = accuracyGO.GetComponent<TextMeshProUGUI>();
        gopSO.FindProperty("streakText").objectReferenceValue = bestStreakGO.GetComponent<TextMeshProUGUI>();
        gopSO.FindProperty("newHighScoreObj").objectReferenceValue = newHighScore;
        gopSO.FindProperty("retryButton").objectReferenceValue = retryBtn.GetComponent<Button>();
        gopSO.FindProperty("menuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        gopSO.ApplyModifiedProperties();

        // Wire up GameSceneInitializer
        var initializer = gameManagerObj.GetComponent<GameSceneInitializer>();
        SerializedObject initSO = new SerializedObject(initializer);
        initSO.FindProperty("gameManager").objectReferenceValue = gameManager;
        initSO.ApplyModifiedProperties();

        Debug.Log("Game Scene setup complete! Don't forget to create Level Configs.");
        EditorUtility.DisplayDialog("Setup Complete",
            "Game Scene has been set up!\n\nNext steps:\n1. Save this scene as 'Game'\n2. Create Level Config assets\n3. Assign them to LevelManager",
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
        rt.sizeDelta = new Vector2(200, 40);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        return obj;
    }

    private static GameObject CreateButton(string name, Transform parent, string text)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120, 40);
        var image = obj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.3f, 0.8f);
        var button = obj.AddComponent<Button>();

        GameObject textObj = CreateTMPText("Text", obj.transform, text);
        var textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;

        return obj;
    }
}
#endif
