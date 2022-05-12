using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SelectionBase]
public abstract class EnemyBase : MonoBehaviour
{
    public Weapon weapon;
    public Image healthBarImage;
    public Material healthBarMaterial;
    public DamageText damageText;
    public GameObject detectionIndicator;
    [Range(0.1f, 8.0f)]
    public float timeAllowedDetectedBeforeEngage = 5.0f;

    protected EnemyData enemyData;
    protected CapsuleCollider capsuleCollider;
    protected Player playerTarget;
    
    private Animator anim;
    private float timeDetectedNoEngage;
    private bool shouldScanForTarget;
    private bool forceEngage;

    protected void Start()
    {
        enemyData = new EnemyData();
        enemyData.AssignWeapon(weapon);

        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        anim = GetComponent<Animator>();
        healthBarImage.gameObject.SetActive(false);
        healthBarImage.material = new Material(healthBarMaterial);
        detectionIndicator.SetActive(false);
        
        timeDetectedNoEngage = 0.0f;
        shouldScanForTarget = true;
        forceEngage = false;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        UIManager.instance.SetCursorOnEnemy();
    }

    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            UIManager.instance.SetCursorDefault();
            return;
        }

        UIManager.instance.SetCursorOnEnemy();
    }

    private void OnMouseExit()
    {
        UIManager.instance.SetCursorDefault();
    }

    protected void FixedUpdate()
    {
        ScanForTarget();
    }

    protected void Update()
    {
        SyncHealthBar();
        CheckPlayerIsDead();
        ActBasedOnTarget();
    }

    private void SyncHealthBar()
    {
        healthBarImage.material.SetFloat(Shader.PropertyToID("_FillAmount"), enemyData.GetHealthInterp01());
    }

    private void ActBasedOnTarget()
    {
        ResetAnimatorVariables();
        
        if (playerTarget == null)
        {
            detectionIndicator.SetActive(false);
            
            return;
        }

        RotateTowardsTarget();

        if (IsPlayerInAttackRange())
        {
            detectionIndicator.SetActive(false);
            shouldScanForTarget = false;
            
            AttackTarget();

            return;
        }

        if (IsPlayerInEngageRange() || forceEngage)
        {
            detectionIndicator.SetActive(false);
            forceEngage = true;
            shouldScanForTarget = false;
            
            MoveTowardsTarget();

            return;
        }

        if (timeDetectedNoEngage > timeAllowedDetectedBeforeEngage)
        {
            forceEngage = true;
        }
        
        detectionIndicator.SetActive(true);
        timeDetectedNoEngage += Time.deltaTime;
    }

    private void ScanForTarget()
    {
        if (!shouldScanForTarget)
        {
            return;
        }
        
        playerTarget = null;
        Collider[] hits = Physics.OverlapSphere(transform.position, enemyData.DetectionRange, LayerMask.GetMask("Player"));

        if (hits.Length == 1)
        {
            playerTarget = hits[0].gameObject.GetComponent<Player>();
        }

        if (hits.Length > 1)
        {
            Debug.LogWarning("Weird behavior in enemy script... Detected > 1 players...");
        }
    }

    private bool IsPlayerInAttackRange()
    {
        if (playerTarget == null)
        {
            return false;
        }

        return Vector3.Distance(transform.position, playerTarget.transform.position) <= enemyData.GetAttackRange();
    }

    private bool IsPlayerInEngageRange()
    {
        return Vector3.Distance(playerTarget.transform.position, transform.position) <= enemyData.EngageRange;
    }

    private void CheckPlayerIsDead()
    {
        if (playerTarget == null)
        {
            return;
        }

        if (playerTarget.isActiveAndEnabled)
        {
            return;
        }

        playerTarget = null;
    }

    private void AttackTarget()
    {
        anim.SetBool(Animator.StringToHash("attack"), true);
    }

    private void MoveTowardsTarget()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack(1h)"))
        {
            return;
        }

        anim.SetBool(Animator.StringToHash("run"), true);
        transform.position = Vector3.MoveTowards(transform.position, playerTarget.transform.position, Time.deltaTime * enemyData.GetMovementSpeed());
    }

    private void RotateTowardsTarget()
    {
        transform.rotation = Quaternion.LookRotation(playerTarget.transform.position - transform.position, Vector3.up);
    }

    private void ResetAnimatorVariables()
    {
        anim.SetBool(Animator.StringToHash("run"), false);
        anim.SetBool(Animator.StringToHash("attack"), false);
    }

    private bool CheckShouldDie()
    {
        return enemyData.GetHealthCurrent() <= 0;
    }

    private void Die()
    {
        anim.SetTrigger(Animator.StringToHash("die"));
        healthBarImage.gameObject.SetActive(false);
        this.enabled = false;
        capsuleCollider.enabled = false;
        Destroy(this.gameObject, 3.0f);
        UIManager.instance.SetCursorDefault();
    }

    public virtual void DealDamageToTarget() {}

    public virtual void TakeDamage(int damage, GameObject source) 
    {
        if (!healthBarImage.IsActive())
        {
            healthBarImage.gameObject.SetActive(true);
        }

        enemyData.TakeDamage(damage);
        damageText.ActivateDamageText(damage);

        if (CheckShouldDie())
        {
            Die();

            source.TryGetComponent(out Player playerComponent);

            if (playerComponent != null)
            {
                playerComponent.EnemyKilled(this);
            }
        }
    }

    public EnemyType GetEnemyType() => enemyData.Type;
    public int GetExperienceReward() => enemyData.GetExperienceReward();
    public CapsuleCollider GetCollider() => capsuleCollider;
}
