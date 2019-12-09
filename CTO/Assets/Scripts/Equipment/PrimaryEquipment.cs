using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment/New Primary Equipment")]
public class PrimaryEquipment : Equipment
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
