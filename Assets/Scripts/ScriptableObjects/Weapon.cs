using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class Weapon : Item
{
    public WeaponType type;
    public DamageRange damage;
    public float range;
    public StatsModifier[] modifiers;

    public float GetWeaponDamageValue()
    {
        return Random.Range(damage.damageMin, damage.damageMax);
    }
}
