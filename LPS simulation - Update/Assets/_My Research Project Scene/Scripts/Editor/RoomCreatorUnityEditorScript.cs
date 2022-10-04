using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomCreator))]
public class RoomCreatorEditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("This script is responsbile for creating and joining rooms", MessageType.Info);

        RoomCreator roomCreator = (RoomCreator)target;

       

     

        if (GUILayout.Button("Join Experiment Room"))
        {
            roomCreator.OnEnterButtonCicked_Experiment();
        }

        else if (GUILayout.Button("Join Experiment Traditional Room"))
        {
            roomCreator.OnEnterButtonCicked_ExperimentTraditional();
        }

        else if (GUILayout.Button("Join Tutorial Room"))
        {
            roomCreator.OnEnterButtonCicked_Tutorial();
        }

      
      
 



    }
}
