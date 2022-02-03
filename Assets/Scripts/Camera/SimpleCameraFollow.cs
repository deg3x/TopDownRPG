using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;

    private Vector3 targetOffset;

    void Start()
    {
        targetOffset = this.transform.position - target.position;
    }
    
    void LateUpdate()
    {
        this.transform.position = target.transform.position + targetOffset;
    }
}
