using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private Item itemData;
    [SerializeField] private Material outlineMaterial;

    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Setting material variables with manual indexing is dirty and I should be ashamed
    private void OnMouseEnter()
    {
        meshRenderer.materials[2].SetFloat(Shader.PropertyToID("_OutlineEnabled"), 1.0f);
        UIManager.instance.ActivateItemPopup(transform.position, itemData.title, itemData.description);
    }

    private void OnMouseExit()
    {
        meshRenderer.materials[2].SetFloat(Shader.PropertyToID("_OutlineEnabled"), 0.0f);
        UIManager.instance.DeactivateItemPopup();
    }
}
