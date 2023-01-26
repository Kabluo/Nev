using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimationHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] Animator animator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("isHighlighted", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("isHighlighted", false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        animator.SetBool("isHighlighted", true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        animator.SetBool("isHighlighted", false);
    }
}
