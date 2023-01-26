using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushEliteWhatIsAhead : MonoBehaviour
{
    private MinimushEliteBehaviour controller;
    [SerializeField] BoxCollider2D coll;
    [SerializeField] float timeToForget = 1f; //time it takes to realise inRange became false
    private float hasForgotten = 0;
    private bool forgetting;

    void Start()
    {
        controller = GetComponentInParent<MinimushEliteBehaviour>();
    }

    void Update()
    {
        if(Time.timeScale == 0f) { return; }

        Forget();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            controller.inRange = true;
            forgetting = false;
        }
    }

    void OnTriggerStay2D(Collider2D other) //jump while colliding with ground
    {
        if(other.tag == "Ground")
        Jump();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            hasForgotten = timeToForget;
            forgetting = true;
        }
    }

    void Jump()
    {
        if(!controller.animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction") && controller.enemyOnGround)
        controller.rigidBody.velocity = new Vector2(controller.rigidBody.velocity.x, controller.jumpForce);
    }

    void Forget()
    {
        if(!forgetting) { return; }

        if(hasForgotten > 0)
        hasForgotten -= Time.deltaTime;

        if(hasForgotten <= 0)
        {
            controller.inRange = false;
            forgetting = false;
        }
    }
}
