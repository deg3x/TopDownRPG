using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    void Update()
    {
        this.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
    }
}
