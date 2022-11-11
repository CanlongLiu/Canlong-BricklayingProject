using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameMaster))]
public class GameMasrerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("This script is responsbile for control the game timmer.", MessageType.Info);

        GameMaster gameMaster = (GameMaster)target;

        if (GUILayout.Button("Load Warmup!"))
        {
            gameMaster.LoadWarmup();
        }

        if (GUILayout.Button("Load Traditional!"))
        {
            gameMaster.LoadTraditional();
        }

        if (GUILayout.Button("Load Experiment!"))
        {
            gameMaster.LoadExperiment();
        }

        if (GUILayout.Button("Reload Game!"))
        {
            gameMaster.ReloadGame();
        }

        if (GUILayout.Button("Start Timer"))
        {
            gameMaster.StartGame();
        }

        if (GUILayout.Button("Pause Timer"))
        {
            gameMaster.PauseGame();
        }

        if (GUILayout.Button("Resume Timer"))
        {
            gameMaster.ResumeGame();
        }

        if (GUILayout.Button("IncreaseTimer"))
        {
            gameMaster.IncreasTime();
        }
    }
}
