using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    #region Singleton
    public static CharacterManager instance;

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

    static List<Square> affectedSquares;
    private static Character selectedCharacter;
    public static Character SelectedCharacter
    {
        get
        {
            return selectedCharacter;
        }
        set
        {
            if (selectedCharacter) //if not null
            {
                //Reset old square
                //selectedCharacter.gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.defaultSquareMaterial;
                GridSystem.instance.ChangeSquareVisuals(affectedSquares, SquareVisualMode.Default);
                affectedSquares.Clear();
                CharacterManager.instance.DisplayCharacterUI(false);
            }

            selectedCharacter = value;
            if (selectedCharacter) //if not null
            {
                //selectedCharacter.gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.selectedSquareMaterial;
                affectedSquares = selectedCharacter.CalculateValidMoves();
                GridSystem.instance.ChangeSquareVisuals(affectedSquares, SquareVisualMode.Available);
                CharacterManager.instance.DisplayCharacterUI(true);
            }
        }
    }

    private static Character targetCharacter;
    public static Character TargetCharacter
    {
        get
        {
            return targetCharacter;
        }
        set
        {
            if (value == null)
            {
                targetCharacter = value;
            }
            else if (selectedCharacter)
            {
                if (selectedCharacter.GetPossibleTargets().Contains(value))
                {
                    targetCharacter = value;
                }
            }
        }
    }


    [SerializeField] GameObject actionBar = null;
    private void DisplayCharacterUI(bool state)
    {
        actionBar.SetActive(state);
        selectedCharacter.FindPossibleTargets();
    }
}
