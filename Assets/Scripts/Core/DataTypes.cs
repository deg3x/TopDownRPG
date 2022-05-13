using UnityEngine;
using System;
public enum HeroClass
{
    Barbarian,
    Wizard,
    Marksman,
    Knight
}

public enum EnemyType
{
    SkeletonMinion,
    SkeletonWarrior,
    SkeletonMage,
    SkeletonArcher
}

public enum ItemType
{
    Weapon,
    Equipment,
    Consumable
}

public enum WeaponType
{
    Melee,
    Ranged,
    Magic
}

public enum EquipmentSlot
{
    Head,
    Body,
    Hands,
    Legs
}

public enum StatsModifierType
{
    Health,
    Resource,
    DamageMelee,
    DamageRanged,
    SpellDamage,
    AttackSpeed,
    MovementSpeed,
    Strength,
    Constitution,
    Agility,
    Intelligence
}

public enum StatsModifierMethod
{
    Value,
    Percentage
}

[System.Serializable]
public struct StatsModifier
{
    public StatsModifierType modifierType;
    public StatsModifierMethod modifierMethod;
    public float value;
}

[Serializable]
public struct DamageRange
{
    public float damageMin;
    public float damageMax;
}

public class UnitAttributes
{
    public int Level { get; private set; }
    public int AvailableStatPoints { get; private set; }

    public int Strength { get; private set; }
    public int Constitution { get; private set; }
    public int Agility { get; private set; }
    public int Intelligence { get; private set; }

    public float AttackDamageMelee { get; private set; }
    public float AttackDamageRanged { get; private set; }
    public float SpellDamage { get; private set; }
    public float AttackRange { get; private set; }
    public float AttackSpeed { get; private set; }

    public float MovementSpeed { get; private set; }

    public int HealthMax { get; private set; }
    public int HealthCurrent { get; private set; }
    public int ResourceMax { get; private set; }
    public int ResourceCurrent { get; private set; }
    public int ExperienceMax { get; private set; }
    public int ExperienceCurrent { get; private set; }

    public Action OnLevelUp;
    public Action OnApplyStats;
    public Action OnResetStats;
    public Action OnHasPendingStatChanges;

    private const float BaseAttackDamage = 1.0f;
    private const float BaseAttackSpeed = 0.8f;
    private const float BaseAttackRange = 1.0f;
    private const float BaseMovementSpeed = 5.0f;

    private int strengthCache;
    private int constitutionCache;
    private int agilityCache;
    private int intelligenceCache;
    private int availablePointsCache;

    private bool statSyncPending = false;

    public UnitAttributes(int str, int con, int agi, int intel, int level = 1)
    {
        Level = level;
        AvailableStatPoints = 0;

        Strength = str;
        Constitution = con;
        Agility = agi;
        Intelligence = intel;
        CacheStatValues();

        AttackRange = BaseAttackRange;
        SyncCombatAttributes();

        MovementSpeed = BaseMovementSpeed;

        SyncResourceAttributes();
        HealthCurrent = HealthMax;
        ResourceCurrent = ResourceMax;
        ExperienceMax = ExperienceFormula();
        ExperienceCurrent = 0;
    }

    public void AddPointsStrength(int points)
    {
        if (!HasEnoughAvailablePoints(points))
        {
            return;
        }

        AvailableStatPoints -= points;
        Strength += points;
        RequestStatSync();
    }

    public void AddPointsAgility(int points)
    {
        if (!HasEnoughAvailablePoints(points))
        {
            return;
        }

        AvailableStatPoints -= points;
        Agility += points;
        RequestStatSync();
    }

    public void AddPointsIntelligence(int points)
    {
        if (!HasEnoughAvailablePoints(points))
        {
            return;
        }

        AvailableStatPoints -= points;
        Intelligence += points;
        RequestStatSync();
    }

    public void AddPointsConstitution(int points)
    {
        if (!HasEnoughAvailablePoints(points))
        {
            return;
        }

        AvailableStatPoints -= points;
        Constitution += points;
        RequestStatSync();
    }

    public void ApplyStatChanges()
    {
        if (!statSyncPending)
        {
            return;
        }

        SyncCombatAttributes();
        SyncResourceAttributes();
        CacheStatValues();
        CacheAvailablePoints();

        statSyncPending = false;

        OnApplyStats?.Invoke();
    }

    public void ResetStatChanges()
    {
        if (!statSyncPending)
        {
            return;
        }

        Strength = strengthCache;
        Constitution = constitutionCache;
        Agility = agilityCache;
        Intelligence = intelligenceCache;
        AvailableStatPoints = availablePointsCache;
        
        statSyncPending = false;

        OnResetStats?.Invoke();
    }

    private void SyncCombatAttributes()
    {
        AttackDamageMelee = DamageMeleeFormula();
        AttackDamageRanged = DamageRangedFormula();
        SpellDamage = SpellDamageFormula();
        AttackSpeed = AttackSpeedFormula();
    }

    private void SyncResourceAttributes()
    {
        HealthMax = HealthFormula();
        ResourceMax = ResourceFormula();
    }

    public void TakeDamage(int damage)
    {
        this.HealthCurrent = Mathf.Clamp(HealthCurrent - damage, 0, HealthMax);
        RequestStatSync();
    }

    public void SpendResource(int amount)
    {
        ResourceCurrent = Mathf.Clamp(ResourceCurrent - amount, 0, ResourceMax);
        RequestStatSync();
    }

    public void GainLevels(int amount)
    {
        ResetStatChanges();

        Level += amount;
        ExperienceMax = ExperienceFormula();
        AvailableStatPoints += PointsPerLevelFormula() * amount;

        CacheAvailablePoints();

        HealthCurrent = HealthMax;
        ResourceCurrent = ResourceMax;

        OnLevelUp?.Invoke();
    }

    public void GainExperience(int amount)
    {
        ExperienceCurrent += amount;
        while (ExperienceCurrent >= ExperienceMax)
        {
            ExperienceCurrent -= ExperienceMax;
            GainLevels(1);
        }
    }

    private int PointsPerLevelFormula()
    {
        return 5;
    }

    private int HealthFormula()
    {
        return Constitution * 10;
    }

    private int ResourceFormula()
    {
        return Intelligence * 10;
    }

    private int ExperienceFormula()
    {
        return Level * 10;
    }

    private float DamageMeleeFormula()
    {
        return BaseAttackDamage + Strength * 0.2f;
    }

    private float DamageRangedFormula()
    {
        return BaseAttackDamage + Agility * 0.2f;
    }

    private float SpellDamageFormula()
    {
        return BaseAttackDamage + Intelligence * 0.2f;
    }

    private float AttackSpeedFormula()
    {
        return BaseAttackSpeed + Agility * 0.05f;
    }

    private void RequestStatSync(bool force = false)
    {
        statSyncPending = availablePointsCache != AvailableStatPoints || force;
        OnHasPendingStatChanges?.Invoke();
    }

    private void CacheStatValues()
    {
        strengthCache = Strength;
        constitutionCache = Constitution;
        agilityCache = Agility;
        intelligenceCache = Intelligence;
    }

    private bool HasEnoughAvailablePoints(int points)
    {
        if (points > 0 && AvailableStatPoints < points)
        {
            return false;
        }

        if (points < 0 && AvailableStatPoints - points > availablePointsCache)
        {
            return false;
        }

        return true;
    }

    private void CacheAvailablePoints()
    {
        availablePointsCache = AvailableStatPoints;
    }

    public int GetStrengthCache() => strengthCache;
    public int GetConstitutionCache() => constitutionCache;
    public int GetAgilityCache() => agilityCache;
    public int GetIntelligenceCache() => intelligenceCache;
    public int GetAvailablePointsCache() => availablePointsCache;
    public bool HasPendingStatChanges() => statSyncPending;
}