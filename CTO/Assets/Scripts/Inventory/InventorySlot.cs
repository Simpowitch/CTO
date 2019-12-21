using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public Equipment equipment;

    public void OnClick()
    {
        for (int equipmentIndex = 0; equipmentIndex < InventoryController.inventory.inventory.Count; equipmentIndex++)
        {
            if (InventoryController.inventory.inventory[equipmentIndex] == equipment)
            {
                InventoryController.inventory.inventory.RemoveAt(equipmentIndex);
            }
        }
        Destroy(this);
    }
}
