using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] string targetScene;
    [SerializeField] Vector3 targetLocation;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" || other.tag == "PlayerInvincible")
        {
                PlayerController.instance.isInScene = true;
                //PlayerController.instance.myAnimator.SetFloat("speed", 0f); //reset speed float
                RespawnController.instance.ChangeScene(targetScene, targetLocation);
        }
    }
}
