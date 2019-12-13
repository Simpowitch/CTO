using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { Player, AI }
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

    public Team currentTurn;
    [SerializeField] Transform charactersParent = null;
    List<Character> allCharacters = new List<Character>();

    private void Start()
    {
        for (int i = 0; i < charactersParent.childCount; i++)
        {
            allCharacters.Add(charactersParent.GetChild(i).GetComponent<Character>());
        }

        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (allCharacters[i].myTeam == currentTurn)
            {
                allCharacters[i].NewTurn();
            }
        }
    }

    public void ChangeTurn()
    {
        currentTurn = currentTurn == Team.Player ? Team.AI : Team.Player;

        for (int i = 0; i < allCharacters.Count; i++)
        {
            if (allCharacters[i].myTeam == currentTurn)
            {
                allCharacters[i].NewTurn();
            }
        }
    }
}
