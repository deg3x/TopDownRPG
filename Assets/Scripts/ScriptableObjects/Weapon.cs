using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class Weapon : ScriptableObject
{
    public WeaponType type;
    public DamageRange damage;
    public float range;
    public string title;
    public StatsModifier[] modifiers;

    public float GetWeaponDamageValue()
    {
        return Random.Range(damage.damageMin, damage.damageMax);
    }
}
