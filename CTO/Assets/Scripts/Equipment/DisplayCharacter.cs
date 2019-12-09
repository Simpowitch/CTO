using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCharacter : MonoBehaviour
{

    public Image characterImage;
    public Image primaryEquipmentImage;
    public Image secondaryEquipmentImage;
    public List<Image> otherEquipmentsImages;


    public PrimaryEquipment primaryEquipment;
    public SecondaryEquipment secondaryEquipment;
    public Equipment[] otherEquipments;

    void Start()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (primaryEquipment != null)
        {
            primaryEquipmentImage.sprite = primaryEquipment.equipmentSprite;
        }
        if (secondaryEquipment != null)
        {
            secondaryEquipmentImage.sprite = secondaryEquipment.equipmentSprite;
        }

        foreach (Equipment equipment in otherEquipments)
        {
            foreach (Image equipmentImage in otherEquipmentsImages)
            {

                if (equipmentImage.sprite == null)
                {
                    equipmentImage.sprite = equipment.equipmentSprite;
                    break;
                }
            }
        }
    }
}
