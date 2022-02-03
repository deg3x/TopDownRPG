using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Equipment", order = 2)]
public class Equipment : ScriptableObject
{
    public EquipmentSlot slot;
    public float armor;
    public StatsModifier[] modifiers;
}