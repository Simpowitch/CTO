using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Placement { Center, Up, Right, Down, Left }
public enum Rotation { Up, Right, Down, Left}

public class Square : MonoBehaviour
{
    public List<GameObject> connectedObjects = new List<GameObject>();

    public void CreateWall(GameObject objectToSpawn, Placement placement, Rotation rotation)
    {
        Vector3 pos = this.transform.position - new Vector3(0, this.transform.lossyScale.y / 2, 0);
        Vector3 rot = Vector3.zero;

        float offset = this.transform.lossyScale.x / 2;
        switch (placement)
        {
            default:
            case Placement.Center:
                break;
            case Placement.Up:
                pos += new Vector3(0, 0, offset);
                break;
            case Placement.Right:
                pos += new Vector3(offset, 0, 0);
                break;
            case Placement.Down:
                pos += new Vector3(0, 0, -offset);
                break;
            case Placement.Left:
                pos += new Vector3(-offset, 0, 0);
                break;
        }

        switch (rotation)
        {
            default:
            case Rotation.Up:
                rot = new Vector3(0, 0, 0);
                break;
            case Rotation.Right:
                rot = new Vector3(0, 90, 0);
                break;
            case Rotation.Down:
                rot = new Vector3(0, 180, 0);
                break;
            case Rotation.Left:
                rot = new Vector3(0, -90, 0);
                break;
        }

        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(objectToSpawn);
        obj.transform.position = pos;
        obj.transform.eulerAngles = rot;
        connectedObjects.Add(obj);
        obj.transform.SetParent(GameObject.Find("ObjectParent").transform);
    }

    public void ClearSquare()
    {
        for (int i = 0; i < connectedObjects.Count; i++)
        {
            DestroyImmediate(connectedObjects[i]);
        }

        connectedObjects.Clear();
    }

    public void DeleteSquare()
    {
        for (int i = 0; i < connectedObjects.Count; i++)
        {
            DestroyImmediate(connectedObjects[i]);
        }

        DestroyImmediate(this.gameObject);
    }
}
