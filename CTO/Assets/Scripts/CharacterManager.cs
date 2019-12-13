using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private static Character selectedCharacter;
    public static Character SelectedCharacter
    {
        get
        {
            return selectedCharacter;
        }
        set
        {
            if (selectedCharacter)
            {
                //selectedCharacter.gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.defaultSquareMaterial;
            }
            selectedCharacter = value;
            //selectedCharacter.gameObject.GetComponent<MeshRenderer>().material = GridSystem.instance.selectedSquareMaterial;
        }
    }
}
