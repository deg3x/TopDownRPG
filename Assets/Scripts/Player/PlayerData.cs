
public class PlayerData
{
    public string PlayerName { get; private set; }
    public HeroClass PlayerClass { get; private set; }
    public Weapon Weapon { get; private set; }
    public Equipment[] Equipment { get; private set; }

    public static int EquipmentSlotsMax = System.Enum.GetNames(typeof(EquipmentSlot)).Length;

    private UnitAttributes playerAttributes;

    public PlayerData() : this("Hero", HeroClass.Barbarian) {}
    public PlayerData(string name, HeroClass c)
    {
        PlayerName = name;
        PlayerClass = c;
        Weapon = null;
        Equipment = new Equipment[EquipmentSlotsMax];

        switch (c)
        {
            case HeroClass.Barbarian:
                playerAttributes = new UnitAttributes(10, 5, 5, 5);
                break;
            case HeroClass.Knight:
                playerAttributes = new UnitAttributes(5, 10, 5, 5);
                break;
            case HeroClass.Marksman:
                playerAttributes = new UnitAttributes(5, 5, 10, 5);
                break;
            case HeroClass.Wizard:
                playerAttributes = new UnitAttributes(5, 5, 5, 10);
                break;
            default:
                break;
        }

        playerAttributes.OnLevelUp = () => {
                UIManager.instance.SyncPlayerDataUI(UISyncOperation.ALL);
                UINotificationsManager.instance.PlayNotification(UINotificationType.levelUp, playerAttributes.Level);
            };
        playerAttributes.OnApplyStats = () => UIManager.instance.SyncPlayerDataUI(UISyncOperation.ALL);
        playerAttributes.OnResetStats = () => UIManager.instance.SyncPlayerDataUI(UISyncOperation.Stats);
        playerAttributes.OnHasPendingStatChanges = () => UIManager.instance.SyncPlayerDataUI(UISyncOperation.ALL);
    }

    public float GetDamageValue()
    {
        if (Weapon == null)
        {
            return playerAttributes.AttackDamageMelee;
        }

        float damageValue = Weapon.GetWeaponDamageValue();

        switch (Weapon.type)
        {
            case WeaponType.Melee:
                damageValue *= playerAttributes.AttackDamageMelee;
                break;
            case WeaponType.Ranged:
                damageValue *= playerAttributes.AttackDamageRanged;
                break;
            case WeaponType.Magic:
                damageValue *= playerAttributes.SpellDamage;
                break;
            default:
                UnityEngine.Debug.LogError("[!] Unspecified weapon type. Cant calculate damage value in Player...");
                break;
        }

        return damageValue;
    }

    public float GetAttackRange()
    {
        return Weapon == null ? playerAttributes.AttackRange : Weapon.range;
    }

    public void InitDeveloperCommands()
    {
        DeveloperConsole.instance.AddCommand("levelup", new CommandDescriptor(
            () => GainExperience(playerAttributes.ExperienceMax), 0, "Increase player level by one"));
        DeveloperConsole.instance.AddCommand("healhp", new CommandDescriptor(
            () => TakeDamage(-playerAttributes.HealthMax), 0, "Fully restore player hp"));
        DeveloperConsole.instance.AddCommand("healres", new CommandDescriptor(
            () => SpendResource(-playerAttributes.ResourceMax), 0, "Fully restore player resource"));
    }

    public void AssignWeapon(Weapon newWeapon) => Weapon = newWeapon;
    public void AddPointsStrength(int points) => playerAttributes.AddPointsStrength(points);
    public void AddPointsAgility(int points) => playerAttributes.AddPointsAgility(points);
    public void AddPointsIntelligence(int points) => playerAttributes.AddPointsIntelligence(points);
    public void AddPointsConstitution(int points) => playerAttributes.AddPointsConstitution(points);
    public void TakeDamage(int damage) => playerAttributes.TakeDamage(damage);
    public void SpendResource(int amount) => playerAttributes.SpendResource(amount);
    public void GainExperience(int amount) => playerAttributes.GainExperience(amount);
    public void ApplyStatChanges() => playerAttributes.ApplyStatChanges();
    public void ResetStatChanges() => playerAttributes.ResetStatChanges();
    public bool HasPendingStatChanges() => playerAttributes.HasPendingStatChanges();
    
    public int GetStrength() => playerAttributes.Strength;
    public int GetConstitution() => playerAttributes.Constitution;
    public int GetAgility() => playerAttributes.Agility;
    public int GetIntelligence() => playerAttributes.Intelligence;
    public int GetHealthCurrent() => playerAttributes.HealthCurrent;
    public int GetHealthMax() => playerAttributes.HealthMax;
    public float GetHealthRatio01() => (float)GetHealthCurrent() / GetHealthMax();
    public int GetResourceCurrent() => playerAttributes.ResourceCurrent;
    public int GetResourceMax() => playerAttributes.ResourceMax;
    public float GetResourceRatio01() => (float)GetResourceCurrent() / GetResourceMax();
    public int GetExperienceCurrent() => playerAttributes.ExperienceCurrent;
    public int GetExperienceMax() => playerAttributes.ExperienceMax;
    public float GetExperienceRatio01() => (float)GetExperienceCurrent() / GetExperienceMax();
    public int GetLevel() => playerAttributes.Level;
    public int GetAvailablePoints() => playerAttributes.AvailableStatPoints;

    public int GetCachedPointsStrength() => playerAttributes.Strength - playerAttributes.GetStrengthCache();
    public int GetCachedPointsConstitution() => playerAttributes.Constitution - playerAttributes.GetConstitutionCache();
    public int GetCachedPointsAgility() => playerAttributes.Agility - playerAttributes.GetAgilityCache();
    public int GetCachedPointsIntelligence() => playerAttributes.Intelligence - playerAttributes.GetIntelligenceCache();
    public int GetCachedPoints() => playerAttributes.GetAvailablePointsCache() - playerAttributes.AvailableStatPoints;
    public float GetMovementSpeed() => playerAttributes.MovementSpeed;
}