using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment/New Equipment")]
public class Equipment : ScriptableObject
{
    public enum EquipmentType { Primary, Secondary, Other}
    public EquipmentType type;

    public Sprite mySprite;

    public Equipment()
    {
    }

    public void Use()
    {

    }
}
