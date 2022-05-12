using UnityEngine;
using UnityEngine.EventSystems;
using System;

[SelectionBase]
public class Player : MonoBehaviour
{
    [Range(0.001f, 0.1f)]
    public float movementPrecision = 0.05f;
    [Range(0.5f, 2f)]
    public float enemyDistanceForgiveness = 1f;
    [Range(0.01f, 1f)]
    public float mouseClickCachingInterval = 0.01f;
    [Range(120.0f, 1080.0f)]
    public float rotationSpeed = 720.0f;

    public GameObject mouseClickEffect;

    // THIS IS JUST FOR TESTING! REMOVE LATER!!!
    public Weapon weaponToAssign;

    private PlayerData playerData;
    private bool hasDestination;
    private bool isFacingTarget;
    private float mouseClickedTime;
    private Vector3 pointToMove;
    private Animator anim;
    private EnemyBase enemyTarget;
    private GameObject latestClickEffect;
    private Camera mainCamera;
    private CapsuleCollider capsuleCollider;

    private void Start()
    {
        mainCamera = Camera.main;
        latestClickEffect = null;
        isFacingTarget = true;
        hasDestination = false;
        mouseClickedTime = 0f;
        pointToMove = transform.forward;
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        enemyTarget = null;

        playerData = new PlayerData();
        playerData.AssignWeapon(weaponToAssign);
        playerData.InitDeveloperCommands();

        UIManager.instance.SetCurrentPlayerData(playerData);
        
        UIManager.instance.SyncPlayerDataUI(UISyncOperation.ALL);
    }
    
    private void Update()
    {
        GetMouseData();
        CheckEnemyOutOfAttackRange();
        CheckEnemyIsDead();
        
        if (hasDestination)
        {
            if (enemyTarget != null)
            {
                pointToMove = enemyTarget.transform.position - (enemyTarget.transform.position.y * Vector3.up);
            }

            Move();

            if (hasDestination)
            {
               anim.SetBool(Animator.StringToHash("running"), true);
            }
        }
        else
        {
            DestroyClickFX();

            anim.SetBool(Animator.StringToHash("running"), false);
        }
        
        RotateTowardsDestination();

        if(isFacingTarget && enemyTarget != null && !hasDestination)
        {
            anim.SetBool(Animator.StringToHash("attack"), true);
        }
        else
        {
            anim.SetBool(Animator.StringToHash("attack"), false);
        }
    }

    private void Move()
    {
        float distanceToStop = movementPrecision;
        if (enemyTarget != null)
        {
            distanceToStop += playerData.GetAttackRange() + enemyTarget.GetCollider().radius + capsuleCollider.radius;
        }

        if(Vector3.Distance(transform.position, pointToMove) < distanceToStop)
        {
            hasDestination = false;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, pointToMove, playerData.GetMovementSpeed() * Time.deltaTime);
    }

    private void RotateTowardsDestination()
    {
        Quaternion targetRotation = Quaternion.LookRotation(pointToMove - transform.position, Vector3.up);
        Quaternion currentRotation = transform.rotation;

        float angle = Quaternion.Angle(currentRotation, targetRotation);

        if ( angle < 1.0f)
        {
            isFacingTarget = true;

            return;
        }

        // Test case: start with low rotation speed and increase modifier (exponentially?) the longer we turning
        // The following is dirty anyway
        float rotationSpeedModifier = 1.0f;
        rotationSpeedModifier *= angle < 60.0f ? 0.5f : 1.0f;
        rotationSpeedModifier *= angle < 15.0f ? 0.5f : 1.0f;
        transform.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeedModifier * rotationSpeed * Time.deltaTime);
    }

    private void CheckEnemyOutOfAttackRange()
    {
        if (enemyTarget == null)
        {
            return;
        }

        if (hasDestination)
        {
            return;
        }

        float attackDistance = enemyTarget.GetCollider().radius + capsuleCollider.radius + movementPrecision + enemyDistanceForgiveness + playerData.GetAttackRange();
        if (Vector3.Distance(enemyTarget.transform.position, transform.position) <= attackDistance)
        {
            return;
        }

        hasDestination = true;
        isFacingTarget = false;
    }
    
    private void GetMouseData()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetMovementData(false);
        }
        if (Input.GetMouseButton(0))
        {
            mouseClickedTime += Time.deltaTime;
            if (mouseClickedTime <= mouseClickCachingInterval)
            {
                return;
            }
            
            GetMovementData(true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseClickedTime = 0f;
        }
    }

    private void GetMovementData(bool mouseDrag)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 9;
        if (mouseDrag)
        {
            layerMask |= 1 << 10;
        }

        layerMask = ~layerMask;

        if (Physics.Raycast(ray, out RaycastHit hit, 50f, layerMask))
        {
            if (hit.collider.CompareTag("Environment"))
            {
                return;
            }

            if (PositionApproximate(hit.point, transform.position))
            {
                hasDestination = false;
                return;
            }

            isFacingTarget = false;
            hasDestination = true;
            pointToMove = hit.point - (hit.point.y * Vector3.up);
            enemyTarget = null;

            if (hit.collider.CompareTag("Enemy"))
            {
                enemyTarget = hit.transform.GetComponent<EnemyBase>();
                DestroyClickFX();
            }
            else
            {
                if (mouseDrag)
                {
                    if (latestClickEffect != null)
                    {
                        latestClickEffect.transform.position = hit.point + (Vector3.up * 0.1f);
                    }
                    else
                    {
                        latestClickEffect = Instantiate(mouseClickEffect, hit.point + (Vector3.up * 0.1f), mouseClickEffect.transform.rotation);
                    }
                }
                else
                {
                    DestroyClickFX();
                    latestClickEffect = Instantiate(mouseClickEffect, hit.point + (Vector3.up * 0.1f), mouseClickEffect.transform.rotation);
                }
            }
        }
    }

    private void CheckEnemyIsDead()
    {
        if (enemyTarget == null)
        {
            return;
        }

        if (enemyTarget.isActiveAndEnabled)
        {
            return;
        }

        enemyTarget = null;
    }

    private void DestroyClickFX()
    {
        if (latestClickEffect != null)
        {
            Destroy(latestClickEffect);
        }

        latestClickEffect = null;
    }

    private bool PositionApproximate(Vector3 pointA, Vector3 pointB)
    {
        return Mathf.Abs(pointA.x - pointB.x) <= movementPrecision && Mathf.Abs(pointA.z - pointB.z) <= movementPrecision;
    }

    private bool CheckShouldDie()
    {
        return playerData.GetHealthCurrent() <= 0;
    }

    private void Die()
    {
        anim.SetTrigger(Animator.StringToHash("die"));
        DestroyClickFX();
        this.enabled = false;
    }

    public void TakeDamage(float damage)
    {
        playerData.TakeDamage((int)damage);
        UIManager.instance.SyncPlayerDataUI(UISyncOperation.Health);

        if (CheckShouldDie())
        {
            Die();
        }
    }

    public void DealDamageToEnemy()
    {
        if (enemyTarget == null)
        {
            return;
        }

        enemyTarget.TakeDamage((int)playerData.GetDamageValue(), this.gameObject);
    }

    public void EnemyKilled(EnemyBase enemy)
    {
        playerData.GainExperience(enemy.GetExperienceReward());
        UIManager.instance.SyncPlayerDataUI(UISyncOperation.Experience);

        if (enemyTarget == enemy)
        {
            enemyTarget = null;
        }
    }
}
