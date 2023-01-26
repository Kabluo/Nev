using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconOnMouseover : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] int index = 0;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!UIController.instance.iconDescriptionContainer.activeSelf)
        {
            UIController.instance.iconDescriptionContainer.SetActive(true);
        }

        UIController.instance.iconImage.sprite = UIController.instance.iconScriptableObjects[index].GetIconImage();
        UIController.instance.iconDescription.text = UIController.instance.iconScriptableObjects[index].GetIconDescription();
    }
}
