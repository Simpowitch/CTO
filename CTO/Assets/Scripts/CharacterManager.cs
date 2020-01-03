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

    static List<Square> affectedSquares = new List<Square>();
    private static Character selectedCharacter;
    public static Character SelectedCharacter
    {
        get
        {
            return selectedCharacter;
        }
        set
        {
            //Reset old
            if (selectedCharacter) //if not null
            {
                //Reset old square
                //selectedCharacter.gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.defaultSquareMaterial;
                GridSystem.instance.ChangeSquareVisualsAll(SquareVisualMode.Invisible);
                affectedSquares.Clear();
            }

            //Set new
            selectedCharacter = value;
            if (selectedCharacter) //if not null
            {
                //selectedCharacter.gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.selectedSquareMaterial;
                affectedSquares = selectedCharacter.CalculateValidMoves();
                GridSystem.instance.ChangeSquareVisualsAll(SquareVisualMode.Default);
                GridSystem.instance.ChangeSquareVisuals(affectedSquares, SquareVisualMode.Available);
            }
            CharacterManager.instance.UpdateCharacterUI();
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
                selectedCharacter.FindPossibleTargets();
                if (selectedCharacter.GetPossibleTargets().Contains(value))
                {
                    targetCharacter = value;
                }
            }
            CharacterManager.instance.UpdateCharacterUI();
        }
    }

    [SerializeField] GameObject actionBar = null;
    [SerializeField] Button shootButton = null;
    private void UpdateCharacterUI()
    {
        actionBar.SetActive((selectedCharacter));
        shootButton.interactable = (selectedCharacter && targetCharacter);
    }
}
