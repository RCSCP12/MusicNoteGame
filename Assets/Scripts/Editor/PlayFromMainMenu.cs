#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Automatically loads MainMenu when entering Play mode, regardless of which scene is open.
/// Toggle via MusicNoteGame > Play From Main Menu (checkbox).
/// </summary>
[InitializeOnLoad]
public static class PlayFromMainMenu
{
    private const string MenuPath = "MusicNoteGame/Play From Main Menu";
    private const string PrefKey  = "PlayFromMainMenu_Enabled";

    static PlayFromMainMenu()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    [MenuItem(MenuPath, isValidateFunction: false, priority = 200)]
    private static void ToggleEnabled()
    {
        bool current = EditorPrefs.GetBool(PrefKey, true);
        EditorPrefs.SetBool(PrefKey, !current);
    }

    [MenuItem(MenuPath, isValidateFunction: true)]
    private static bool ToggleEnabledValidate()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(PrefKey, true));
        return true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (!EditorPrefs.GetBool(PrefKey, true)) return;
        if (state != PlayModeStateChange.ExitingEditMode) return;

        // Find the MainMenu scene path from build settings
        string mainMenuPath = null;
        foreach (var s in EditorBuildSettings.scenes)
        {
            if (!s.enabled) continue;
            if (s.path.Contains("MainMenu"))
            {
                mainMenuPath = s.path;
                break;
            }
        }

        if (mainMenuPath == null) return;
        if (EditorSceneManager.GetActiveScene().path == mainMenuPath) return;

        // Save the current scene, then load MainMenu
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene(mainMenuPath);
    }
}
#endif
