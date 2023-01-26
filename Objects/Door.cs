using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject interactablePopup;
    private Animator animator;
    [SerializeField] string targetScene;
    [SerializeField] Vector3 targetLocation;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
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
                animator.SetTrigger("isOpened");
                PlayerController.instance.isInScene = true;
                PlayerController.instance.myAnimator.SetFloat("speed", 0f); //reset speed float
                RespawnController.instance.ChangeScene(targetScene, targetLocation);
            }
        }
    }
}
