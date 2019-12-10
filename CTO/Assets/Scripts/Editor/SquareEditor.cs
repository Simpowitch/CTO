using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Square))]
public class SquareEditor : Editor
{
    static Placement placement;
    static Rotation rotation;
    static GameObject objectToSpawn;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        Square selectedSquare = (Square)target;

        //DrawDefaultInspector();

        GUILayout.Label("Square Editor", EditorStyles.boldLabel);

        placement = (Placement)EditorGUILayout.EnumPopup("Placement",  placement);
        rotation = (Rotation)EditorGUILayout.EnumPopup("Rotation", rotation);


        objectToSpawn = (GameObject) EditorGUILayout.ObjectField("Object to spawn", objectToSpawn, typeof(GameObject), false, null);

        if (GUILayout.Button("Create Object"))
        {
            selectedSquare.CreateWall(objectToSpawn, placement, rotation);
        }

        if (GUILayout.Button("Clear Square"))
        {
            selectedSquare.ClearSquare();
        }
    }
}
