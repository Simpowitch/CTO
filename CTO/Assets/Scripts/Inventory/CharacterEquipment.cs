using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{
    public Equipment equipment;
    public void OnClick()
    {
        InventoryController.selectedEquipment = equipment;
    }
}
