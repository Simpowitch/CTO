using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

interface IEquipment
{
    int equipmentValue { get; set; }
    Sprite equipmentSprite { get; set; }

    void Use();
}