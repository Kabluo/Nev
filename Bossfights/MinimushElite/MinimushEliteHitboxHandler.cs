using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushEliteHitboxHandler : MonoBehaviour
{
    [SerializeField] BoxCollider2D hitBox;
    private MinimushEliteBehaviour controller;

    void Start()
    {
        controller = GetComponentInParent<MinimushEliteBehaviour>();
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
            hitBox.offset = new Vector2(0f, 3.5f);
            hitBox.size = new Vector2(3.25f, 8.5f);
        }

        else if(controller.teleChargeAttack)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(-0.4f, -0.1f);
            hitBox.size = new Vector2(0.7f, 0.7f);
        }

        else if(controller.teleDive)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0f, -0.2f);
            hitBox.size = new Vector2(0.7f, 1.5f);
        }

        else if(controller.teleDiveAttack)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0f, 0.2f);
            hitBox.size = new Vector2(3.5f, 2f);
        }

        else if(controller.counterAttack)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(-0.1f, 3.2f);
            hitBox.size = new Vector2(3.2f, 8f);
        }

        else
        {
            hitBox.enabled = false;
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other) //get id through controller instead of hardcoding, refactor later
    {
        if(other.tag == "Player" && controller.attack1)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(2, transform);
                controller.ManageStandby();
            }

            else
            {
                other.GetComponent<PlayerController>().DamagePlayer(10, 25, 0, transform, false, 2, 7); //physical

                if(!PlayerController.instance.isBroken)
                other.GetComponent<PlayerController>().DamagePlayer(10, 0, 4, transform, false, 2, 7); //lightning, change sound to a lightning hit sound later
            }
        }

        if(other.tag == "Player" && controller.attack2)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(2, transform);
                controller.ManageStandby();
            }

            else
            {
                other.GetComponent<PlayerController>().DamagePlayer(15, 65, 0, transform, false, 2, 8); //physical

                if(!PlayerController.instance.isBroken)
                other.GetComponent<PlayerController>().DamagePlayer(15, 0, 4, transform, false, 2, 8); //lightning, change sound to a lightning hit sound later
            }
        }

        if(other.tag == "Player" && controller.heavyAttack)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(2, transform);
                controller.ManageStandby();
            }

            else
            {
                other.GetComponent<PlayerController>().DamagePlayer(25, 110, 0, transform, false, 2, 13);

                if(!PlayerController.instance.isBroken)
                other.GetComponent<PlayerController>().DamagePlayer(25, 0, 4, transform, false, 2, 13);
            }
        }

        if(other.tag == "Player" && controller.teleChargeAttack)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(2, transform);
                controller.ManageStandby();
            }

            else
            {
                other.GetComponent<PlayerController>().DamagePlayer(15, 65, 0, transform, false, 2, 8); //physical

                if(!PlayerController.instance.isBroken)
                other.GetComponent<PlayerController>().DamagePlayer(15, 0, 4, transform, false, 2, 8); //lightning, change sound to a lightning hit sound later
            }
        }

        if(other.tag == "Player" && controller.teleDive)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(2, transform);
                controller.ManageStandby();
            }

            else
            {
                other.GetComponent<PlayerController>().DamagePlayer(1, 110, 0, transform, false, 2, 13);

                if(!PlayerController.instance.isBroken)
                other.GetComponent<PlayerController>().DamagePlayer(25, 0, 4, transform, false, 2, 13);
            }
        }

        if(other.tag == "Player" && controller.teleDiveAttack)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(2, transform);
                controller.ManageStandby();
            }

            else
            {
                other.GetComponent<PlayerController>().DamagePlayer(25, 110, 0, transform, false, 2, 13);

                if(!PlayerController.instance.isBroken)
                other.GetComponent<PlayerController>().DamagePlayer(25, 0, 4, transform, false, 2, 13);
            }
        }

        if(other.tag == "Player" && controller.counterAttack)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                other.GetComponent<PlayerController>().ExecutePlayer(2, transform);
                controller.ManageStandby();
            }

            else
            {
                other.GetComponent<PlayerController>().DamagePlayer(40, 110, 4, transform, false, 2, 13);
            }
        }
    }
}
