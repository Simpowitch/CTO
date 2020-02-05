using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Weapon")]
[System.Serializable]
public class Weapon : ScriptableObject
{
    public string name;
    public int damage;
    public int rpm;
    public int bulletsPerBurst;
    //public int bulletsRemaining;
    public int range;
    public GameObject weaponPrefab;
}
