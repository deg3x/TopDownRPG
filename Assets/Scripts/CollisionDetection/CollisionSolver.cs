using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class CollisionSolver : MonoBehaviour
{
    private CapsuleCollider col;
    private Collider[] collidersOverlap;

    void Start()
    {
        collidersOverlap = new Collider[64];
        col = GetComponent<CapsuleCollider>();
    }

    void LateUpdate()
    {
        Vector3 capsuleStart = transform.TransformPoint(col.center - (col.height * 0.5f - col.radius) * Vector3.up);
        Vector3 capsuleEnd = transform.TransformPoint(col.center + (col.height * 0.5f - col.radius) * Vector3.up);
        float radius = transform.TransformVector(col.radius, col.radius, col.radius).y;

        int collisions = Physics.OverlapCapsuleNonAlloc(capsuleStart, capsuleEnd, radius, collidersOverlap, LayerMask.GetMask("Enemy", "EnvironmentBlock", "Player"), QueryTriggerInteraction.Ignore);

        Vector3 offsetVector;
        Vector3 direction;
        float distance;

        for (int i = 0; i < collisions; i++)
        {
            if (collidersOverlap[i] == col)
            {
                continue;
            }

            if (Physics.ComputePenetration(col, transform.position, transform.rotation, collidersOverlap[i], collidersOverlap[i].transform.position, collidersOverlap[i].transform.rotation, out direction, out distance))
            {
                offsetVector = direction * distance;
                offsetVector = new Vector3(offsetVector.x, 0.0f, offsetVector.z);
                transform.position += offsetVector;
            }
        }
    }
}
