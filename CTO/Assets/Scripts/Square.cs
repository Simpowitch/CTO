using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Placement { Center, North, East, South, West }
public enum Rotation { North, East, South, West }
//public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }

public class Square : MonoBehaviour
{
    [SerializeField] List<GameObject> connectedObjects = new List<GameObject>();
    public bool occupiedSpace = false;

    public List<Square> surroundingSquares = new List<Square>();

    //public List<Direction> blockedDirections = new List<Direction>();

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

        Square affectedSecondarySquare = null;

        float offset = 0.5f; //set in GridSystem
        switch (placement)
        {
            default:
            case Placement.Center:
                break;
            case Placement.North:
                pos += new Vector3(0, 0, offset);

                //blockedDirections.Add(Direction.North);
                //blockedDirections.Add(Direction.NorthEast);
                //blockedDirections.Add(Direction.NorthWest);

                //affectedSecondarySquare = GetSurroundingSquare(Direction.North);
                //if (affectedSecondarySquare)
                //{
                //    affectedSecondarySquare.blockedDirections.Add(Direction.South);
                //    affectedSecondarySquare.blockedDirections.Add(Direction.SouthEast);
                //    affectedSecondarySquare.blockedDirections.Add(Direction.SouthWest);
                //}
                break;
            case Placement.East:
                pos += new Vector3(offset, 0, 0);

                //blockedDirections.Add(Direction.East);
                //blockedDirections.Add(Direction.NorthEast);
                //blockedDirections.Add(Direction.SouthEast);

                //affectedSecondarySquare = GetSurroundingSquare(Direction.East);
                //if (affectedSecondarySquare)
                //{
                //    affectedSecondarySquare.blockedDirections.Add(Direction.West);
                //    affectedSecondarySquare.blockedDirections.Add(Direction.NorthWest);
                //    affectedSecondarySquare.blockedDirections.Add(Direction.SouthWest);
                //}
                break;
            case Placement.South:
                pos += new Vector3(0, 0, -offset);

                //blockedDirections.Add(Direction.South);
                //blockedDirections.Add(Direction.SouthEast);
                //blockedDirections.Add(Direction.SouthWest);

                //affectedSecondarySquare = GetSurroundingSquare(Direction.South);
                //if (affectedSecondarySquare)
                //{
                //    affectedSecondarySquare.blockedDirections.Add(Direction.North);
                //    affectedSecondarySquare.blockedDirections.Add(Direction.NorthEast);
                //    affectedSecondarySquare.blockedDirections.Add(Direction.NorthWest);
                //}
                break;
            case Placement.West:
                pos += new Vector3(-offset, 0, 0);

                //blockedDirections.Add(Direction.West);
                //blockedDirections.Add(Direction.NorthWest);
                //blockedDirections.Add(Direction.SouthWest);

                //affectedSecondarySquare = GetSurroundingSquare(Direction.West);
                //if (affectedSecondarySquare)
                //{
                //    affectedSecondarySquare.blockedDirections.Add(Direction.East);
                //    affectedSecondarySquare.blockedDirections.Add(Direction.NorthEast);
                //    affectedSecondarySquare.blockedDirections.Add(Direction.SouthEast);
                //}
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
        ////REMOVE BLCOKER DIRECTIONS FROM THIS AND OPPOSING SQUARE
        //for (int i = 0; i < blockedDirections.Count; i++)
        //{
        //    switch (blockedDirections[i])
        //    {
        //        case Direction.North:
        //            break;
        //        case Direction.NorthEast:
        //            break;
        //        case Direction.East:
        //            break;
        //        case Direction.SouthEast:
        //            break;
        //        case Direction.South:
        //            break;
        //        case Direction.SouthWest:
        //            break;
        //        case Direction.West:
        //            break;
        //        case Direction.NorthWest:
        //            break;
        //    }
        //}

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
        if (CharacterManager.SelectedCharacter)
        {
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
        Gizmos.color = allowedMove ? Color.green : Color.red;

        for (int i = 0; i < path.Count; i++)
        {
            Gizmos.DrawWireSphere(path[i].transform.position, 0.1f);
        }
    }
}
