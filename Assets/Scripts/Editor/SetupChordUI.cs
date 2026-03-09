using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using MusicNoteGame.UI;

public class SetupChordUI
{
    [MenuItem("Tools/Setup Chord UI")]
    public static void Setup()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
        GameObject hardButtonObj = GameObject.Find("HardButton (Level 3)");
        if (hardButtonObj == null) 
        {
            hardButtonObj = GameObject.Find("HardButton");
        }
        
        if (hardButtonObj != null)
        {
            GameObject chordsButtonObj = GameObject.Instantiate(hardButtonObj, hardButtonObj.transform.parent);
            chordsButtonObj.name = "ChordsButton";
            chordsButtonObj.transform.localPosition += new Vector3(0, -60, 0); // shift it down
            
            var text = chordsButtonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null) text.text = "Chords: Off";
            
            MainMenuController mmc = Object.FindAnyObjectByType<MainMenuController>();
            if (mmc != null)
            {
                SerializedObject so = new SerializedObject(mmc);
                so.Update();
                so.FindProperty("chordsButton").objectReferenceValue = chordsButtonObj.GetComponent<Button>();
                
                Transform highlightTransform = chordsButtonObj.transform.Find("Highlight");
                if (highlightTransform != null)
                {
                    so.FindProperty("chordsHighlight").objectReferenceValue = highlightTransform.GetComponent<Image>();
                }
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(mmc);
            }
        }
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        
        EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
        HUDController hud = Object.FindAnyObjectByType<HUDController>();
        if (hud != null)
        {
            GameObject levelTextObj = GameObject.Find("LevelText");
            if (levelTextObj != null)
            {
                GameObject pressedNotesObj = GameObject.Instantiate(levelTextObj, levelTextObj.transform.parent);
                pressedNotesObj.name = "PressedNotesText";
                pressedNotesObj.transform.localPosition += new Vector3(0, -30, 0); // shift it down
                
                var text = pressedNotesObj.GetComponent<TMPro.TextMeshProUGUI>();
                if (text != null) 
                {
                    text.text = "";
                    text.alignment = TMPro.TextAlignmentOptions.Center;
                }
                
                SerializedObject so = new SerializedObject(hud);
                so.Update();
                so.FindProperty("pressedNotesText").objectReferenceValue = text;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(hud);
            }
        }
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Debug.Log("UI Setup Complete!");
    }
}
