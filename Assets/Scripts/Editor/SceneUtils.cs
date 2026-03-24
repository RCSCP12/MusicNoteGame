#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Miscellaneous scene maintenance utilities.
/// </summary>
public static class SceneUtils
{
    /// <summary>
    /// Removes all MonoBehaviour components with missing scripts from every
    /// GameObject in the active scene.
    /// </summary>
    [MenuItem("MusicNoteGame/Remove Missing Scripts")]
    public static void RemoveMissingScripts()
    {
        int removed = 0;
        foreach (var go in Object.FindObjectsByType<GameObject>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (count > 0)
            {
                removed += count;
                EditorUtility.SetDirty(go);
            }
        }

        EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"[SceneUtils] Removed {removed} missing-script component(s). Save the scene to persist.");
    }
}
#endif
