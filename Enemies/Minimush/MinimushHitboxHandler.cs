using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushHitboxHandler : MonoBehaviour
{
    [SerializeField] BoxCollider2D hitBox;
    private MinimushController controller;

    void Start()
    {
        controller = GetComponentInParent<MinimushController>();
    }

    void Update()
    {
        if(Time.timeScale == 0f) { return; }

        AttackActive();
    }

    void AttackActive()
    {
        if(controller.attack1)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(-0.2f, 0.2f);
            hitBox.size = new Vector2(0.8f, 0.8f);
        }

        else if(controller.attack2)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(-0.2f, 0f);
            hitBox.size = new Vector2(1f, 0.8f);
        }

        else if(controller.heavyAttack)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0f, 0.15f);
            hitBox.size = new Vector2(2.5f, 1.5f);
        }

        else if(controller.grabAttack)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(-0.2f, 0.5f);
            hitBox.size = new Vector2(1f, 0.5f);
        }

        else
        {
            hitBox.enabled = false;
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && controller.attack1)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(0, transform);
                controller.onStandby = true;
            }

            else
            other.GetComponent<PlayerController>().DamagePlayer(15, 25, 0, transform, false, 0, 7); //stagger damage is static
        }

        if(other.tag == "Player" && controller.attack2)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(0, transform);
                controller.onStandby = true;
            }

            else
            other.GetComponent<PlayerController>().DamagePlayer(25, 65, 0, transform, false, 0, 8); //stagger damage is static
        }

        if(other.tag == "Player" && controller.heavyAttack)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(0, transform);
                controller.onStandby = true;
            }

            else
            other.GetComponent<PlayerController>().DamagePlayer(40, 110, 0, transform, false, 0, 13); //stagger damage is static
        }

        if(other.tag == "Player" && controller.grabAttack)
        {
            other.GetComponent<PlayerController>().DamagePlayer(51, 0, 0, transform, true, 0, 7); //stagger damage is static
            //other.GetComponent<PlayerController>().GrabPlayer(0, 51, 0, transform);
            if(!PlayerController.instance.isDamaged)
            controller.onStandby = true;
        }
    }
}
