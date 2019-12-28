﻿using System.Collections;
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

    [SerializeField] int maxHP = 5;
    [SerializeField] int currentHP = 5;
    [SerializeField] int armor = 0;

    public Weapon weapon = null;

    List<Square> validEndSquares = new List<Square>();
    public List<Square> GetValidEndSquares() { return validEndSquares; }


    public List<SquarePoint> aiSquareRanking = new List<SquarePoint>();
    Vector3[] bodySensors = new Vector3[3] { new Vector3(0, 1.4f, 0), new Vector3(0, 1, 0), new Vector3(0, 0.3f, 0) };
    public Vector3[] GetBodySensors() { return bodySensors; }
    public float weaponHeight = 1.2f;

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

        //Temp weapon
        weapon = new Weapon();
        weapon.range = 10;
        weapon.name = "TestWeapon2000";
        weapon.damage = 1;
        weapon.bulletsPerBurst = 3;
        weapon.rpm = 500;
        weapon.bulletsRemaining = weapon.bulletsPerBurst;
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
        bool originalSquareCollisionStatus = GridSystem.instance.squareCollisionStatus;
        GridSystem.instance.ChangeSquareColliderStatus(true);

        validEndSquares.Clear();
        validEndSquares.Add(squareStandingOn);

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

        GridSystem.instance.ChangeSquareColliderStatus(originalSquareCollisionStatus);
        return validEndSquares;
    }

    List<Character> possibleTargets = new List<Character>();
    public List<Character> GetPossibleTargets() { return possibleTargets; }

    public List<Character> FindPossibleTargets()
    {
        possibleTargets.Clear();
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, weapon.range);

        foreach (var item in colliders)
        {
            if (item.GetComponent<Character>())
            {
                if (Physics.Raycast(this.transform.position, item.transform.position - this.transform.position, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.transform.GetComponent<Character>() && hit.transform.GetComponent<Character>().myTeam != myTeam)
                    {
                        possibleTargets.Add(item.GetComponent<Character>());
                    }
                }
            }
        }
        return possibleTargets;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= (damage - armor);
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //insert animations etc.

        GameObject.Destroy(this.gameObject);
    }
}
