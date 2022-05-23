using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDescriptionPopup : MonoBehaviour
{
    [SerializeField]
    private Text title;
    [SerializeField]
    private Text description;

    private RectTransform rect;
    private Vector3 targetWorldPos;
    private bool isActive;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        rect.position = CalculatePopupScreenPosition(targetWorldPos);
    }

    public void ActivateDescriptionPopup(Vector3 itemWorldPosition, string itemTitle, string itemDescription)
    {
        gameObject.SetActive(true);
        title.text = itemTitle;
        description.text = itemDescription;

        targetWorldPos = itemWorldPosition;

        isActive = true;
    }

    public void DeactivateDesctiptionPopup()
    {
        gameObject.SetActive(false);
        isActive = false;
    }

    private Vector2 CalculatePopupScreenPosition(Vector3 itemWorldPosition)
    {
        return Camera.main.WorldToScreenPoint(itemWorldPosition) + Vector3.up * 50.0f;
    }
}
