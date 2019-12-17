using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum SquareVisualMode { Default, Available, Selected }

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

    [SerializeField] Material defaultSquareMaterial = null;
    [SerializeField] Material selectedSquareMaterial = null;
    [SerializeField] Material availableSquareMaterial = null;

    [SerializeField] GameObject highlightObject = null;

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

    public void MoveHighlight(Vector3 newPos)
    {
        highlightObject.transform.position = newPos;
    }

    public void ChangeSquareVisuals(List<Square> squares, SquareVisualMode mode)
    {
        for (int i = 0; i < squares.Count; i++)
        {
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
            }
        }
    }


    [SerializeField] float squareSize = 1f;
    public float GetSquareSize()
    {
        return squareSize;
    }

    [SerializeField] GameObject squarePrefab = null;
    public List<Square> allSquares = new List<Square>();

    public List<Transform> layers = new List<Transform>(); //used for different heights

    public void SpawnSquares()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            for (float x = 0; x < layers[i].lossyScale.x; x += squareSize)
            {
                for (float z = 0; z < layers[i].lossyScale.z; z += squareSize)
                {
                    Vector3 point = GetNearestPointOnGrid(new Vector3(layers[i].position.x - layers[i].lossyScale.x / 2 + x, layers[i].position.y + layers[i].lossyScale.y / 2, layers[i].position.z - layers[i].lossyScale.z / 2 + z));
                    point += new Vector3(0, squarePrefab.transform.lossyScale.y / 2, 0);
                    if (CheckIfUniqueSquarePos(point))
                    {
                        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(squarePrefab);
                        newObj.transform.position = point;
                        allSquares.Add(newObj.GetComponent<Square>());
                        newObj.transform.SetParent(GameObject.Find("SquareParent").transform);
                        newObj.name += layers[i].name + "X:" + x + "Z:" + z;
                    }
                }
            }
        }
    }

    private bool CheckIfUniqueSquarePos(Vector3 pos)
    {
        for (int i = 0; i < allSquares.Count; i++)
        {
            if (allSquares[i].transform.position == pos)
            {
                return false;
            }
        }
        return true;
    }

    public void DeleteAllSquares()
    {
        for (int i = 0; i < allSquares.Count; i++)
        {
            allSquares[i].DeleteSquare();
        }
        allSquares.Clear();
    }

    public Vector3 GetNearestPointOnGrid(Vector3 input)
    {
        Vector3 offset = new Vector3(0.51f, 0, 0.51f);

        Vector3 position = input - transform.position + offset;


        int xCount = Mathf.RoundToInt(position.x / squareSize);
        int yCount = Mathf.RoundToInt(position.y / squareSize);
        int zCount = Mathf.RoundToInt(position.z / squareSize);

        Vector3 result = new Vector3(
            (float)xCount * squareSize,
            (float)yCount * squareSize,
            (float)zCount * squareSize);

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
        for (int i = 0; i < layers.Count; i++)
        {
            for (float x = 0; x < layers[i].lossyScale.x; x += squareSize)
            {
                for (float z = 0; z < layers[i].lossyScale.z; z += squareSize)
                {
                    point = GetNearestPointOnGrid(new Vector3(layers[i].position.x - layers[i].lossyScale.x / 2 + x, layers[i].position.y + layers[i].lossyScale.y / 2, layers[i].position.z - layers[i].lossyScale.z / 2 + z));
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

