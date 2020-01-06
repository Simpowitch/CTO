using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Character", menuName = "Character/New Character")]
public class CharacterData : ScriptableObject
{

    public string characterName;
    public Sprite characterAvatar;

    public Equipment primaryEquipment;
    public Equipment secondaryEquipment;
    public Equipment otherEquipmentOne;
    public Equipment otherEquipmentTwo;

    public CharacterData(string name, Sprite characterAvatar)
    {
        this.characterName = name;
        this.characterAvatar = characterAvatar;
    }
}
