
public class EnemyData
{
    public EnemyType Type { get; private set; }
    public Weapon Weapon { get; private set; }
    public float DetectionRange { get; private set; }
    public float EngageRange { get; private set; }
    public int ExperienceReward { get; private set; }

    private UnitAttributes enemyAttributes;

    public EnemyData() : this(EnemyType.SkeletonMinion) {}
    public EnemyData(EnemyType type)
    {
        Weapon = null;
        DetectionRange = 10.0f;
        EngageRange = 8.0f;

        switch(type)
        {
            case EnemyType.SkeletonMinion:
                enemyAttributes = new UnitAttributes(5, 5, 5, 5);
                ExperienceReward = 5;
                break;
            case EnemyType.SkeletonWarrior:
                break;
            case EnemyType.SkeletonArcher:
                break;
            case EnemyType.SkeletonMage:
                break;
            default:
                break;
        }
    }

    public float GetDamageValue()
    {
        if (Weapon == null)
        {
            return enemyAttributes.AttackDamageMelee;
        }

        float damageValue = Weapon.GetWeaponDamageValue();

        switch (Weapon.type)
        {
            case WeaponType.Melee:
                damageValue *= enemyAttributes.AttackDamageMelee;
                break;
            case WeaponType.Ranged:
                damageValue *= enemyAttributes.AttackDamageRanged;
                break;
            case WeaponType.Magic:
                damageValue *= enemyAttributes.SpellDamage;
                break;
            default:
                UnityEngine.Debug.LogError("[!] Unspecified weapon type. Cant calculate damage value in Enemy...");
                break;
        }

        return damageValue;
    }

    public float GetAttackRange()
    {
        return Weapon == null ? enemyAttributes.AttackRange : Weapon.range;
    }

    public void AssignWeapon(Weapon newWeapon) => Weapon = newWeapon;
    public void TakeDamage(int damage) => enemyAttributes.TakeDamage(damage);
    public int GetExperienceReward() => ExperienceReward;
    public int GetHealthCurrent() => enemyAttributes.HealthCurrent;
    public int GetHealthMax() => enemyAttributes.HealthMax;
    public float GetHealthInterp01() => (float)GetHealthCurrent() / GetHealthMax();
    public float GetMovementSpeed() => enemyAttributes.MovementSpeed;
}
