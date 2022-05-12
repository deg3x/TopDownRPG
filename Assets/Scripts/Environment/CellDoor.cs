using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDoor : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;

    private void Start()
    {
        
    }

    private void OnMouseEnter()
    {
        outlineMaterial.SetFloat(Shader.PropertyToID("_OutlineEnabled"), 1.0f);
    }

    private void OnMouseExit()
    {
        outlineMaterial.SetFloat(Shader.PropertyToID("_OutlineEnabled"), 0.0f);
    }

    private void Update()
    {
        
    }
}
