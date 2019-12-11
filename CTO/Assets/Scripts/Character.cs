using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Character : MonoBehaviour
{
    public bool canMove = false;
    public float range = 8f;
    public Square squareStandingOn;

    private void Start()
    {
        Square closestSquare = null;
        float distanceToTarget = float.MaxValue;
        for (int i = 0; i < GridSystem.instance.allSquares.Count; i++)
        {
            float testDistance = Vector3.Distance(this.transform.position, GridSystem.instance.allSquares[i].transform.position);
            if (testDistance < distanceToTarget)
            {
                distanceToTarget = testDistance;
                closestSquare = GridSystem.instance.allSquares[i];
            }
        }
        squareStandingOn = closestSquare;
    }

    public void MoveCharacter(Square square)
    {
        squareStandingOn.occupiedSpace = false;
        GetComponent<NavMeshAgent>().SetDestination(square.transform.position);
        canMove = false;
        squareStandingOn = square;
        squareStandingOn.occupiedSpace = true;
    }

    public void NewTurn()
    {
        canMove = true;
    }
}
