using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment/New Secondary Equipment")]
public class SecondaryEquipment : Equipment
{
    
    public override Sprite equipmentSprite
    {
        get => base.mySprite;
        set => base.equipmentSprite = value;
    }

    public override int equipmentValue
    {
        get => base.myValue;
        set => base.equipmentValue = value;
    }
}
