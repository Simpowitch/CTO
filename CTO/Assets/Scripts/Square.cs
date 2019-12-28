using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Placement { Center, North, East, South, West }
public enum Rotation { North, East, South, West }
//public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }

[System.Serializable]
public class Square : MonoBehaviour
{
    [SerializeField] List<GameObject> connectedObjects = new List<GameObject>();
    public bool occupiedSpace = false;

    public List<Square> surroundingSquares = new List<Square>();


    private void Start()
    {
        SetSurroundingSquares();
    }

    private void SetSurroundingSquares()
    {
        for (int i = 0; i < GridSystem.instance.allSquares.Count; i++)
        {
            GridSystem.instance.allSquares[i].surroundingSquares.Clear();

            Collider[] collisions = Physics.OverlapBox(GridSystem.instance.allSquares[i].transform.position, new Vector3(GridSystem.instance.GetSquareSize(), 0, GridSystem.instance.GetSquareSize()));

            foreach (var item in collisions)
            {
                if (item.GetComponent<Square>() && item.GetComponent<Square>() != GridSystem.instance.allSquares[i])
                {
                    GridSystem.instance.allSquares[i].surroundingSquares.Add(item.GetComponent<Square>());
                }
            }
        }
    }

    //public Square GetSurroundingSquare(Direction direction)
    //{
    //    Vector3 posToCheck = this.transform.position;
    //    float offset = 1; //Set in gridsystem

    //    switch (direction)
    //    {
    //        case Direction.North:
    //            posToCheck += new Vector3(0, 0, offset);
    //            break;
    //        case Direction.NorthEast:
    //            posToCheck += new Vector3(offset, 0, offset);
    //            break;
    //        case Direction.East:
    //            posToCheck += new Vector3(offset, 0, 0);
    //            break;
    //        case Direction.SouthEast:
    //            posToCheck += new Vector3(offset, 0, -offset);
    //            break;
    //        case Direction.South:
    //            posToCheck += new Vector3(0, 0, -offset);
    //            break;
    //        case Direction.SouthWest:
    //            posToCheck += new Vector3(-offset, 0, -offset);
    //            break;
    //        case Direction.West:
    //            posToCheck += new Vector3(-offset, 0, 0);
    //            break;
    //        case Direction.NorthWest:
    //            posToCheck += new Vector3(offset, 0, -offset);
    //            break;
    //    }

    //    for (int i = 0; i < surroundingSquares.Count; i++)
    //    {
    //        if (posToCheck == surroundingSquares[i].transform.position)
    //        {
    //            return surroundingSquares[i];
    //        }
    //    }
    //    Debug.LogWarning("Square not found");
    //    return null;
    //}

    public void CreateWall(GameObject objectToSpawn, Placement placement, Rotation rotation)
    {
        Vector3 pos = this.transform.position - new Vector3(0, this.transform.lossyScale.y / 2, 0);
        Vector3 rot = Vector3.zero;

        //Square affectedSecondarySquare = null;

        float offset = 0.5f; //set in GridSystem
        switch (placement)
        {
            default:
            case Placement.Center:
                break;
            case Placement.North:
                pos += new Vector3(0, 0, offset);
                break;
            case Placement.East:
                pos += new Vector3(offset, 0, 0);
                break;
            case Placement.South:
                pos += new Vector3(0, 0, -offset);
                break;
            case Placement.West:
                pos += new Vector3(-offset, 0, 0);
                break;
        }

        switch (rotation)
        {
            default:
            case Rotation.North:
                rot = new Vector3(0, 0, 0);
                break;
            case Rotation.East:
                rot = new Vector3(0, 90, 0);
                break;
            case Rotation.South:
                rot = new Vector3(0, 180, 0);
                break;
            case Rotation.West:
                rot = new Vector3(0, -90, 0);
                break;
        }

        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(objectToSpawn);
        obj.transform.position = pos;
        obj.transform.eulerAngles = rot;
        connectedObjects.Add(obj);
        obj.transform.SetParent(GameObject.Find("ObjectParent").transform);
        obj.tag = "Obstacle";

        EditorUtility.SetDirty(this);
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

    public List<Square> path = new List<Square>();
    public bool allowedMove = false;



    private void OnMouseEnter()
    {
        GridSystem.instance.MoveHighlight(this.transform.position);

        if (CharacterManager.SelectedCharacter)
        {
            if (this.gameObject.name == "SquareGroundX:9Z:9")
            {
                Debug.Log("Found");
            }
            path = Pathfinding.GetPath(CharacterManager.SelectedCharacter.squareStandingOn, this);
            //display pathfinding
            allowedMove = (CharacterManager.SelectedCharacter.CanMoveToTarget(this));
        }
    }

    private void OnMouseExit()
    {
        path.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!CharacterManager.SelectedCharacter)
        {
            return;
        }
        Gizmos.color = allowedMove ? Color.green : Color.red;

        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.DrawWireSphere(path[i].transform.position, 0.1f);
        }
    }
}
