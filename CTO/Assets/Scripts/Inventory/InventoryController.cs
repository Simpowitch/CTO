using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryController
{
    public static Inventory inventory;
    public static List<CharacterView> characters = new List<CharacterView>();
    public static CharacterData selectedCharacter;

    public delegate void VisualHandler();
    public static VisualHandler ViewCharacters = UpdateVisuals;


    public static void Initiate()
    {
        foreach (CharacterView character in characters)
        {
            ViewCharacters += character.UpdateVisuals;
        }
        ViewCharacters();  
    }

    public static void UpdateVisuals()
    {
        foreach (Equipment equipment in inventory.inventory)
        {
            inventory.CreateEquipment();
        }
    }
    public static void Method()
    {

    }
}


