using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    public bool canMove = false;
    public float range = 8f;
    public Square squareStandingOn;
    public Team myTeam;

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
        squareStandingOn.occupiedSpace = true;
    }

    public bool CanMoveToTarget(Square targetSquare)
    {
        return (Pathfinding.GetPath(squareStandingOn, targetSquare).Count <= range);
    }

    public bool TryToMove(Square targetSquare)
    {
        if (canMove && Pathfinding.GetPath(squareStandingOn, targetSquare).Count <= range)
        {
            MoveCharacter(targetSquare);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MoveCharacter(Square newSquare)
    {
        squareStandingOn.occupiedSpace = false;
        GetComponent<NavMeshAgent>().SetDestination(newSquare.transform.position);
        canMove = false;
        squareStandingOn = newSquare;
        squareStandingOn.occupiedSpace = true;
        CharacterManager.SelectedCharacter = null;
    }

    public void NewTurn()
    {
        canMove = true;
    }
}
