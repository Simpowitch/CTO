using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment/New Equipment")]
public class Equipment : ScriptableObject, IEquipment
{

    public int myValue;
    public Sprite mySprite;

    public virtual int equipmentValue
    {
        get => myValue;
        set => equipmentValue = value;
    }

    public virtual Sprite equipmentSprite
    {
        get => mySprite;
        set => equipmentSprite = value;
    }



    public void Use()
    {

    }
}
