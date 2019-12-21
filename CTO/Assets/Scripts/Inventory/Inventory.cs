using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Equipment> inventory;

    public GameObject InventoryButton;

    public void Start()
    {
        InventoryController.inventory = this;
        InventoryController.Initiate();
    }

    public GameObject CreateEquipment()
    {
        return Instantiate(InventoryButton);
    }

    public void ClearList()
    {
        foreach (Transform button in transform)
        {
            Destroy(button.gameObject);
        }
    }

}
