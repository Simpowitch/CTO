using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Turn { Player, AI}
public class TurnManager : MonoBehaviour
{
    #region Singleton
    public static TurnManager instance;

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

    [SerializeField] Turn currentTurn;
    [SerializeField] List<Character> playerCharacters = new List<Character>();
    [SerializeField] List<Character> aiCharacters = new List<Character>();

    private void Start()
    {
        if (currentTurn == Turn.Player)
        {
            for (int i = 0; i < playerCharacters.Count; i++)
            {
                playerCharacters[i].NewTurn();
            }
        }
        else
        {
            for (int i = 0; i < aiCharacters.Count; i++)
            {
                aiCharacters[i].NewTurn();
            }
        }
    }

    public void ChangeTurn()
    {
        currentTurn = currentTurn == Turn.Player ? Turn.AI : Turn.Player;

        if (currentTurn == Turn.Player)
        {
            for (int i = 0; i < playerCharacters.Count; i++)
            {
                playerCharacters[i].NewTurn();
            }
        }
        else
        {
            for (int i = 0; i < aiCharacters.Count; i++)
            {
                aiCharacters[i].NewTurn();
            }
        }
    }
}
