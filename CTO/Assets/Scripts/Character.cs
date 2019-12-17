using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    public bool canMove = false;
    public int maxMovement = 8;
    public int remainingMovement = 8;
    public Square squareStandingOn;
    public Team myTeam;

    List<Square> validEndSquares = new List<Square>();


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
        return (canMove && validEndSquares.Contains(targetSquare));
    }

    public bool TryToMove(Square targetSquare)
    {
        if (CanMoveToTarget(targetSquare))
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
        remainingMovement -= Pathfinding.GetPath(squareStandingOn, newSquare).Count;
        squareStandingOn.occupiedSpace = false;
        GetComponent<NavMeshAgent>().SetDestination(newSquare.transform.position);
        squareStandingOn = newSquare;
        squareStandingOn.occupiedSpace = true;
        CharacterManager.SelectedCharacter = null;
    }

    public void NewTurn()
    {
        canMove = true;
        remainingMovement = maxMovement;
    }

    //Check for squares and squares that we can go to
    public List<Square> CalculateValidMoves()
    {
        Vector3 checkBox = new Vector3(remainingMovement * GridSystem.instance.GetSquareSize(), 100, remainingMovement * GridSystem.instance.GetSquareSize());
        Collider[] collisions = Physics.OverlapBox(this.transform.position, checkBox);

        foreach (var item in collisions)
        {
            if (item.GetComponent<Square>() != null)
            {
                if (Pathfinding.GetPath(squareStandingOn, item.GetComponent<Square>()).Count <= remainingMovement && Vector3.Distance(squareStandingOn.transform.position, item.transform.position) <= remainingMovement)
                {
                    validEndSquares.Add(item.GetComponent<Square>());
                }
            }
        }
        return validEndSquares;
    }
}
