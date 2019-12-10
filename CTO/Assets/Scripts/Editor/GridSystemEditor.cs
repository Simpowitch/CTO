using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSystem))]
public class GridSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridSystem system = (GridSystem)target;

        //DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Grid Editor", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Squares"))
        {
            system.SpawnSquares();
        }

        if (GUILayout.Button("Delete Squares"))
        {
            system.DeleteAllSquares();
        }
    }
}
