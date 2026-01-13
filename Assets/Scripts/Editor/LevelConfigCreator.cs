#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using MusicNoteGame.Gameplay;
using System.Collections.Generic;

public class LevelConfigCreator : EditorWindow
{
    [MenuItem("MusicNoteGame/Create All Level Configs")]
    public static void CreateAllLevels()
    {
        string path = "Assets/ScriptableObjects/LevelConfigs";

        // Ensure directory exists
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "LevelConfigs");

        // Level 1: Basic notes on lines (E4, G4, B4 treble / G2, B2, D3 bass)
        CreateLevel(1, "Getting Started", 10f, 5, false,
            new[] { (NoteName.E, 4), (NoteName.G, 4), (NoteName.B, 4) },
            new[] { (NoteName.G, 2), (NoteName.B, 2), (NoteName.D, 3) },
            path);

        // Level 2: Add spaces (F4, A4 treble / A2, C3 bass)
        CreateLevel(2, "Lines and Spaces", 8f, 8, false,
            new[] { (NoteName.E, 4), (NoteName.F, 4), (NoteName.G, 4), (NoteName.A, 4), (NoteName.B, 4) },
            new[] { (NoteName.G, 2), (NoteName.A, 2), (NoteName.B, 2), (NoteName.C, 3), (NoteName.D, 3) },
            path);

        // Level 3: Full staff (D4-F5 treble / F2-A3 bass)
        CreateLevel(3, "Full Staff", 6f, 10, false,
            new[] { (NoteName.D, 4), (NoteName.E, 4), (NoteName.F, 4), (NoteName.G, 4), (NoteName.A, 4), (NoteName.B, 4), (NoteName.C, 5), (NoteName.D, 5), (NoteName.E, 5), (NoteName.F, 5) },
            new[] { (NoteName.F, 2), (NoteName.G, 2), (NoteName.A, 2), (NoteName.B, 2), (NoteName.C, 3), (NoteName.D, 3), (NoteName.E, 3), (NoteName.F, 3), (NoteName.G, 3), (NoteName.A, 3) },
            path);

        // Level 4: Add ledger line below (C4 treble / E2 bass)
        CreateLevel(4, "Ledger Lines Below", 5f, 12, true,
            new[] { (NoteName.C, 4), (NoteName.D, 4), (NoteName.E, 4), (NoteName.F, 4), (NoteName.G, 4), (NoteName.A, 4), (NoteName.B, 4), (NoteName.C, 5), (NoteName.D, 5), (NoteName.E, 5), (NoteName.F, 5) },
            new[] { (NoteName.E, 2), (NoteName.F, 2), (NoteName.G, 2), (NoteName.A, 2), (NoteName.B, 2), (NoteName.C, 3), (NoteName.D, 3), (NoteName.E, 3), (NoteName.F, 3), (NoteName.G, 3), (NoteName.A, 3) },
            path);

        // Level 5: Full range with ledger lines above and below
        CreateLevel(5, "Master Level", 4f, 15, true,
            new[] { (NoteName.C, 4), (NoteName.D, 4), (NoteName.E, 4), (NoteName.F, 4), (NoteName.G, 4), (NoteName.A, 4), (NoteName.B, 4), (NoteName.C, 5), (NoteName.D, 5), (NoteName.E, 5), (NoteName.F, 5), (NoteName.G, 5), (NoteName.A, 5) },
            new[] { (NoteName.E, 2), (NoteName.F, 2), (NoteName.G, 2), (NoteName.A, 2), (NoteName.B, 2), (NoteName.C, 3), (NoteName.D, 3), (NoteName.E, 3), (NoteName.F, 3), (NoteName.G, 3), (NoteName.A, 3), (NoteName.B, 3), (NoteName.C, 4) },
            path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Created 5 Level Configs in " + path);
        EditorUtility.DisplayDialog("Levels Created",
            "Created 5 Level Configs!\n\nNow:\n1. Open the Game scene\n2. Select GameManager\n3. Find LevelManager component\n4. Drag all 5 level configs into the Levels list",
            "OK");
    }

    private static void CreateLevel(int levelNum, string name, float time, int notesToComplete, bool usesLedger,
        (NoteName note, int octave)[] trebleNotes, (NoteName note, int octave)[] bassNotes, string path)
    {
        LevelConfig config = ScriptableObject.CreateInstance<LevelConfig>();
        config.levelNumber = levelNum;
        config.levelName = name;
        config.timePerNote = time;
        config.notesToComplete = notesToComplete;
        config.usesLedgerLines = usesLedger;

        config.trebleClefNotes = new List<NoteDefinition>();
        foreach (var n in trebleNotes)
            config.trebleClefNotes.Add(new NoteDefinition { noteName = n.note, octave = n.octave });

        config.bassClefNotes = new List<NoteDefinition>();
        foreach (var n in bassNotes)
            config.bassClefNotes.Add(new NoteDefinition { noteName = n.note, octave = n.octave });

        string assetPath = $"{path}/Level{levelNum}.asset";
        AssetDatabase.CreateAsset(config, assetPath);
    }
}
#endif
