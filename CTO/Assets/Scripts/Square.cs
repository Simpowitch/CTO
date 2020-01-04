using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Placement { Center, North, East, South, West }
public enum Rotation { North, East, South, West }
public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }

[System.Serializable]
public class Square : MonoBehaviour
{
    public SquareVisualMode squareVisualMode;
    public bool occupiedSpace = false;

    public Square[] surroundingSquares = new Square[8]; //direction
    public GameObject[] objects = new GameObject[5]; //placement


    public bool CanMoveTo(Direction dir)
    {
        switch (dir)
        {
            case Direction.North:
                if (objects[(int)Placement.North])
                {
                    return false;
                }
                break;
            case Direction.NorthEast:
                if (objects[(int)Placement.North])
                {
                    return false;
                }
                if (objects[(int)Placement.East])
                {
                    return false;
                }
                break;
            case Direction.East:
                if (objects[(int)Placement.East])
                {
                    return false;
                }
                break;
            case Direction.SouthEast:
                if (objects[(int)Placement.East])
                {
                    return false;
                }
                if (objects[(int)Placement.South])
                {
                    return false;
                }
                break;
            case Direction.South:
                if (objects[(int)Placement.South])
                {
                    return false;
                }
                break;
            case Direction.SouthWest:
                if (objects[(int)Placement.South])
                {
                    return false;
                }
                if (objects[(int)Placement.West])
                {
                    return false;
                }
                break;
            case Direction.West:
                if (objects[(int)Placement.West])
                {
                    return false;
                }
                break;
            case Direction.NorthWest:
                if (objects[(int)Placement.West])
                {
                    return false;
                }
                if (objects[(int)Placement.North])
                {
                    return false;
                }
                break;
        }

        if (surroundingSquares[(int)dir] != null)
        {
            if (surroundingSquares[(int)dir].occupiedSpace)
            {
                return false;
            }
        }

        return true;
    }

    public void CreateObject(GameObject objectToSpawn, Placement placement, Rotation rotation)
    {
        Vector3 pos = this.transform.position - new Vector3(0, this.transform.lossyScale.y / 2, 0);
        Vector3 rot = Vector3.zero;

        float offset = 0.5f; //half a square
        switch (placement)
        {
            default:
            case Placement.Center:
                occupiedSpace = true;
                break;
            case Placement.North:
                pos += new Vector3(0, 0, offset);
                rotation = Rotation.North;
                break;
            case Placement.East:
                pos += new Vector3(offset, 0, 0);
                rotation = Rotation.East;
                break;
            case Placement.South:
                pos += new Vector3(0, 0, -offset);
                rotation = Rotation.South;
                break;
            case Placement.West:
                pos += new Vector3(-offset, 0, 0);
                rotation = Rotation.West;
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
        objects[(int)placement] = obj;

        switch (placement)
        {
            default:
            case Placement.Center:
                break;
            case Placement.North:
                surroundingSquares[(int)Direction.North].objects[(int)Placement.South] = obj;
                EditorUtility.SetDirty(surroundingSquares[(int)Direction.North]);
                break;
            case Placement.East:
                surroundingSquares[(int)Direction.East].objects[(int)Placement.West] = obj;
                EditorUtility.SetDirty(surroundingSquares[(int)Direction.East]);
                break;
            case Placement.South:
                surroundingSquares[(int)Direction.South].objects[(int)Placement.North] = obj;
                EditorUtility.SetDirty(surroundingSquares[(int)Direction.South]);
                break;
            case Placement.West:
                surroundingSquares[(int)Direction.West].objects[(int)Placement.East] = obj;
                EditorUtility.SetDirty(surroundingSquares[(int)Direction.West]);
                break;
        }

        obj.transform.SetParent(GameObject.Find("ObjectParent").transform);
        obj.tag = "Obstacle";

        EditorUtility.SetDirty(this);
    }

    public void ClearSquare()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i])
            {
                DestroyImmediate(objects[i]);
                objects[i] = null;

                Placement placement = (Placement)i;
                switch (placement)
                {
                    case Placement.Center:
                        break;
                    case Placement.North:
                        surroundingSquares[(int)Direction.North].objects[(int)Placement.South] = null;
                        break;
                    case Placement.East:
                        surroundingSquares[(int)Direction.East].objects[(int)Placement.West] = null;
                        break;
                    case Placement.South:
                        surroundingSquares[(int)Direction.South].objects[(int)Placement.North] = null;
                        break;
                    case Placement.West:
                        surroundingSquares[(int)Direction.East].objects[(int)Placement.East] = null;
                        break;
                }
            }
        }
    }

    public void DeleteSquare()
    {
        ClearSquare();
        DestroyImmediate(this.gameObject);
    }


    public List<Square> path = new List<Square>();
    public bool allowedMove = false;



    private void OnMouseEnter()
    {
        if (squareVisualMode != SquareVisualMode.Invisible)
        {
            GridSystem.instance.MoveSquareHighlight(this);
        }
        else
        {
            GridSystem.instance.HideSquareHighlight();
        }

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
