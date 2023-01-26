using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    [SerializeField] GameObject interactablePopup;
    private Animator animator;
    public int iconId = 0;
    public bool isActive = false;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        StartCoroutine(CheckIfActive());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        interactablePopup.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        interactablePopup.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player" && !PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement") && !PlayerController.instance.isInScene)
        {
            if(Input.GetKey(KeyCode.E)) //getkeydown is somewhat unresponsive
            {
                PlayerTracker.instance.lastTouched = iconId;
                IconInteraction();
            }
        }
    }

    void IconInteraction()
    {
        if(!isActive)
        {
            isActive = true;
            PlayerTracker.instance.iconIsActive[iconId] = true; //activate icon
            animator.SetBool("isActive", isActive);
        }

        PlayerController.instance.myAnimator.SetFloat("speed", 0f); //stops walking/running animation
        UIController.instance.IconMenu();
        
        DataPersistanceManager.instance.SaveGame();
    }

    IEnumerator CheckIfActive()
    {
        yield return new WaitForSeconds(.1f); //wait a bit before checking as loading happens at the end of frame
        isActive = PlayerTracker.instance.iconIsActive[iconId];
        animator.SetBool("isActive", isActive);
    }
}
