using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItemDescriptionPopup : MonoBehaviour
{
    [SerializeField]
    private Text title;
    [SerializeField]
    private Text description;

    private RectTransform rect;
    private Image image;
    private Vector3 targetWorldPos;
    private bool isActive;

    private void Awake()
    {
        isActive = false;
        image = GetComponent<Image>();
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
        title.text = itemTitle;
        description.text = itemDescription;
        targetWorldPos = itemWorldPosition;

        StartCoroutine(UpdatePopupSize());
    }

    public void DeactivateDesctiptionPopup()
    {
        isActive = false;
        rect.position = Vector3.down * 2000.0f;     // Dirty way to hide the item popup, but solves game object activation problems
    }

    private Vector2 CalculatePopupScreenPosition(Vector3 itemWorldPosition)
    {
        return Camera.main.WorldToScreenPoint(itemWorldPosition) + Vector3.up * 50.0f;
    }

    private IEnumerator UpdatePopupSize()
    {
        yield return null;

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, title.rectTransform.sizeDelta.y + description.rectTransform.sizeDelta.y + 10.0f);

        isActive = true;
    }
}
