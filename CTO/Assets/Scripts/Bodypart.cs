using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bodypart : MonoBehaviour
{
    [SerializeField] float percentageValue = 0.5f;

    Character myCharacter = null;
    public Character GetCharacter() { return myCharacter; }

    private void Start()
    {
        myCharacter = GetComponentInParent<Character>();
    }

    public void ReceiveDamage(int damage)
    {
        myCharacter.TakeDamage(Mathf.RoundToInt(damage * percentageValue));
    }

    private void OnMouseEnter()
    {
        GridSystem.instance.MoveSquareHighlight(myCharacter.squareStandingOn);
        if (GridSystem.instance.debugAIChoice && myCharacter.myTeam == Team.AI)
        {
            myCharacter.DisplayDebug(true);
        }
    }

    private void OnMouseExit()
    {
        if (GridSystem.instance.debugAIChoice && myCharacter.myTeam == Team.AI)
        {
            myCharacter.DisplayDebug(false);
        }
    }
}
