using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum SquareVisualMode { Default, Available, Selected, Invisible, AIPointDebug }

public class GridSystem : MonoBehaviour
{
    #region Singleton
    public static GridSystem instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Instance of " + this + " was tried to be instantiated again");
        }
    }
    #endregion

    public bool debugAIChoice = false;


    [SerializeField] Material defaultSquareMaterial = null;
    [SerializeField] Material selectedSquareMaterial = null;
    [SerializeField] Material availableSquareMaterial = null;

    [SerializeField] GameObject highlightObject = null;

    [SerializeField] GameObject squarePrefab = null;
    public List<Square> allSquares = new List<Square>();

    [SerializeField] List<Transform> gameObjectFloors = new List<Transform>(); //used for different heights
    public List<Floor> floors = new List<Floor>();


    private static Square selectedSquare;
    public static Square SelectedSquare
    {
        get
        {
            return selectedSquare;
        }
        set
        {
            if (selectedSquare)
            {
                selectedSquare.gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.defaultSquareMaterial;
            }
            selectedSquare = value;
            selectedSquare.gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.selectedSquareMaterial;
        }
    }

    private void Start()
    {
        ChangeSquareVisualsAll(SquareVisualMode.Default);
    }

    public void MoveSquareHighlight(Square highlightedSquare)
    {
        if (highlightedSquare.squareVisualMode != SquareVisualMode.Invisible)
        {
            highlightObject.SetActive(true);
            highlightObject.transform.position = highlightedSquare.transform.position;
        }
    }

    public void HideSquareHighlight()
    {
        highlightObject.SetActive(false);
    }

    public void ChangeSquareVisuals(List<Square> squares, SquareVisualMode mode)
    {
        for (int i = 0; i < squares.Count; i++)
        {
            squares[i].squareVisualMode = mode;
            squares[i].gameObject.GetComponent<MeshRenderer>().enabled = true;
            squares[i].transform.GetChild(0).gameObject.SetActive(false);
            switch (mode)
            {
                case SquareVisualMode.Default:
                    squares[i].gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.defaultSquareMaterial;
                    break;
                case SquareVisualMode.Available:
                    squares[i].gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.availableSquareMaterial;
                    break;
                case SquareVisualMode.Selected:
                    squares[i].gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.selectedSquareMaterial;
                    break;
                case SquareVisualMode.Invisible:
                    squares[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
                    break;
                case SquareVisualMode.AIPointDebug:
                    squares[i].gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.availableSquareMaterial;
                    squares[i].transform.GetChild(0).gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void ChangeSquareVisualsAll(SquareVisualMode mode)
    {
        ChangeSquareVisuals(allSquares, mode);
        HideSquareHighlight();
    }

    public bool squareCollisionStatus = true;
    public void ChangeSquareColliderStatus(bool toggle)
    {
        squareCollisionStatus = toggle;
        for (int i = 0; i < allSquares.Count; i++)
        {
            allSquares[i].gameObject.GetComponent<Collider>().enabled = toggle;
        }
    }

    public void SpawnSquares()
    {
        DeleteAllSquares();

        for (int i = 0; i < gameObjectFloors.Count; i++)
        {
            int floorXSize = Mathf.RoundToInt(gameObjectFloors[i].lossyScale.x);
            int floorZSize = Mathf.RoundToInt(gameObjectFloors[i].lossyScale.z);

            floors.Add(new Floor(floorXSize, floorZSize));


            for (int x = 0; x < floorXSize; x++)
            {
                for (int z = 0; z < floorZSize; z++)
                {
                    Vector3 point = GetNearestPointOnGrid(new Vector3(gameObjectFloors[i].position.x - gameObjectFloors[i].lossyScale.x / 2 + x, gameObjectFloors[i].position.y + gameObjectFloors[i].lossyScale.y / 2, gameObjectFloors[i].position.z - gameObjectFloors[i].lossyScale.z / 2 + z));
                    point += new Vector3(0, squarePrefab.transform.lossyScale.y / 2, 0);

                    GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(squarePrefab);
                    newObj.transform.position = point;
                    allSquares.Add(newObj.GetComponent<Square>());
                    newObj.transform.SetParent(GameObject.Find("SquareParent").transform);
                    newObj.name += gameObjectFloors[i].name + " X: " + x + " - Z: " + z;

                    floors[i].squares[x, z] = newObj.GetComponent<Square>();
                    newObj.GetComponent<Square>().surroundingSquares = new Square[8];
                    newObj.GetComponent<Square>().objects = new GameObject[5];
                }
            }
        }
        //Setup
        SetSurroundingSquares();
    }

    private void SetSurroundingSquares()
    {
        Debug.Log("Setting up surrounding squares");
        foreach (var floor in floors)
        {
            int maxX = floor.squares.GetLength(0);
            int maxZ = floor.squares.GetLength(1);

            for (int x = 0; x < floor.squares.GetLength(0); x++)
            {
                for (int z = 0; z < floor.squares.GetLength(1); z++)
                {
                    //North
                    if (z + 1 < maxZ)
                    {
                        //floor.squares[x, z].northSquare = floor.squares[x, z + 1];
                        floor.squares[x, z].surroundingSquares[(int)Direction.North] = floor.squares[x, z + 1];
                    }

                    //North East
                    if (x + 1 < maxX && z + 1 < maxZ)
                    {
                        //floor.squares[x, z].northEastSquare = floor.squares[x + 1, z + 1];
                        floor.squares[x, z].surroundingSquares[(int)Direction.NorthEast] = floor.squares[x + 1, z + 1];
                    }

                    //East
                    if (x + 1 < maxX)
                    {
                        //floor.squares[x, z].eastSquare = floor.squares[x + 1, z];
                        floor.squares[x, z].surroundingSquares[(int)Direction.East] = floor.squares[x + 1, z];
                    }

                    //South East
                    if (x + 1 < maxX && z - 1 >= 0)
                    {
                        //floor.squares[x, z].southEastSquare = floor.squares[x + 1, z - 1];
                        floor.squares[x, z].surroundingSquares[(int)Direction.SouthEast] = floor.squares[x + 1, z - 1];
                    }

                    //South
                    if (z - 1 >= 0)
                    {
                        //floor.squares[x, z].southSquare = floor.squares[x, z - 1];
                        floor.squares[x, z].surroundingSquares[(int)Direction.South] = floor.squares[x, z - 1];
                    }

                    //South West
                    if (x - 1 >= 0 && z - 1 >= 0)
                    {
                        //floor.squares[x, z].southWestSquare = floor.squares[x - 1, z - 1];
                        floor.squares[x, z].surroundingSquares[(int)Direction.SouthWest] = floor.squares[x - 1, z - 1];
                    }

                    //West
                    if (x - 1 >= 0)
                    {
                        //floor.squares[x, z].westSquare = floor.squares[x - 1, z];
                        floor.squares[x, z].surroundingSquares[(int)Direction.West] = floor.squares[x - 1, z];
                    }

                    //North West
                    if (x - 1 >= 0 && z + 1 < maxZ)
                    {
                        //floor.squares[x, z].northWestSquare = floor.squares[x - 1, z + 1];
                        floor.squares[x, z].surroundingSquares[(int)Direction.NorthWest] = floor.squares[x - 1, z + 1];
                    }
                }
            }
        }
    }

    public void DeleteAllSquares()
    {
        for (int i = 0; i < allSquares.Count; i++)
        {
            allSquares[i].DeleteSquare();
        }
        allSquares.Clear();
        floors.Clear();
    }

    public Vector3 GetNearestPointOnGrid(Vector3 input)
    {
        Vector3 offset = new Vector3(0.51f, 0, 0.51f);

        Vector3 position = input - transform.position + offset;


        int xCount = Mathf.RoundToInt(position.x);
        int yCount = Mathf.RoundToInt(position.y);
        int zCount = Mathf.RoundToInt(position.z);

        Vector3 result = new Vector3(
            (float)xCount,
            (float)yCount,
            (float)zCount);

        result += transform.position - offset;
        return result;
    }

    [Header("Gizmos")]
    [SerializeField] float gizmoSize = 0.1f;
    [SerializeField] Color gizmoColor = Color.blue;
    [SerializeField] Color gizmoSelectedColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        Vector3 point = Vector3.zero;
        for (int i = 0; i < gameObjectFloors.Count; i++)
        {
            for (float x = 0; x < gameObjectFloors[i].lossyScale.x; x++)
            {
                for (float z = 0; z < gameObjectFloors[i].lossyScale.z; z++)
                {
                    point = GetNearestPointOnGrid(new Vector3(gameObjectFloors[i].position.x - gameObjectFloors[i].lossyScale.x / 2 + x, gameObjectFloors[i].position.y + gameObjectFloors[i].lossyScale.y / 2, gameObjectFloors[i].position.z - gameObjectFloors[i].lossyScale.z / 2 + z));
                    Gizmos.DrawWireSphere(point, gizmoSize);
                }
            }
        }

        if (selectedSquare)
        {
            Gizmos.color = gizmoSelectedColor;
            point = GetNearestPointOnGrid(selectedSquare.transform.position);
            Gizmos.DrawWireSphere(point, gizmoSize);
        }
    }
}

[System.Serializable]
public struct Floor
{
    public Square[,] squares;

    public Floor(int x, int z)
    {
        squares = new Square[x, z];
    }
}
