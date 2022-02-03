using UnityEngine;

public class EnemySkeletonMinion : EnemyBase
{
    new void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int damage, GameObject source)
    {
        base.TakeDamage(damage, source);
    }

    public override void DealDamageToTarget()
    {
        if (playerTarget == null)
        {
            return;
        }

        playerTarget.TakeDamage(enemyData.GetDamageValue());
    }
}
