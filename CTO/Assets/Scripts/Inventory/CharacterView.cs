using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{

    public Image characterImage;
    public Text nameBox;
    public Button primaryButton;
    public Button secondaryButton;
    public Button otherEquipmentSlotOne;
    public Button otherEquipmentSlotTwo;

    public CharacterData character;


    

    public void UpdateVisuals()
    {

        characterImage.sprite = character.characterAvatar;
        nameBox.text = character.name;
        primaryButton.image.sprite = character.primaryEquipment.mySprite;
        secondaryButton.image.sprite = character.secondaryEquipment.mySprite;
        otherEquipmentSlotOne.image.sprite = character.otherEquipmentOne.mySprite;
        otherEquipmentSlotTwo.image.sprite = character.otherEquipmentTwo.mySprite;
    }


}
