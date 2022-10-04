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
