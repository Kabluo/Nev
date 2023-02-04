using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushBehaviour : MonoBehaviour
{
    [SerializeField] EnemyHealthController healthController;
    [SerializeField] EnemyController controller;
    [SerializeField] BoxCollider2D hitBox;
    [SerializeField] float grabAttackCooldown = 12f;
    private float grabAttackTimer = 0f;
    [SerializeField] float heavyAttackCooldown = 8f;
    private float heavyAttackTimer = 0f;
    private int attackId;
    public bool attackCombo1;
    public bool attackCombo2;
    public bool attackHeavy;
    public bool attackGrab;
    [SerializeField] Vector2 grabAttackOffset;

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0 || healthController.isBroken) { return; }

        if(grabAttackTimer > 0)
            grabAttackTimer -= Time.deltaTime;
        
        if(heavyAttackTimer > 0)
            heavyAttackTimer -= Time.deltaTime;

        if(controller.onStandby)
        {
            EndStandby();
            return;
        }
        
        Attack();
        AttackFrames();
    }

    void Attack()
    {
        if(controller.inRange)
        {
            if(grabAttackTimer <= 0)
            attackId = 1; //grab, change grab timer upon performing a grab attack
        
            else
            attackId = 0;

            controller.animator.SetInteger("attackId", attackId);
            controller.animator.SetBool("isAttacking", true);
        }
        
        else if(!Physics2D.Linecast(transform.position, PlayerController.instance.transform.position, controller.whatIsGround) && heavyAttackTimer <= 0 && controller.isChasing)
        {
            attackId = 2; //charge / uppercut
            controller.animator.SetInteger("attackId", attackId);
            controller.animator.SetBool("isAttacking", true);
        }

        else
            controller.animator.SetBool("isAttacking", false);
    }

    void AttackFrames()
    {
        if(attackGrab)
        {
            grabAttackTimer = grabAttackCooldown;
            
            hitBox.offset = new Vector2(-0.3f, 0.45f);
            hitBox.size = new Vector2(0.7f, 0.9f);
            hitBox.enabled = true;
        }

        else if(attackCombo1)
        {
            hitBox.offset = new Vector2(-0.3f, 0.1f);
            hitBox.size = new Vector2(0.6f, 1f);
            hitBox.enabled = true;
        }

        else if(attackCombo2)
        {
            hitBox.offset = new Vector2(-0.1f, -0.1f);
            hitBox.size = new Vector2(1.3f, 0.8f);
            hitBox.enabled = true;
        }

        else if(attackHeavy)
        {
            heavyAttackTimer = heavyAttackCooldown;
            
            hitBox.offset = new Vector2(0f, 0.14f);
            hitBox.size = new Vector2(2.5f, 1.5f);
            hitBox.enabled = true;
        }

        else
        {
            hitBox.enabled = false;
            return;
        }
    }

    void EndStandby()
    {
        if(PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Standby"))
        {
            controller.animator.SetBool("onStandby", false);
            controller.gameObject.layer = LayerMask.NameToLayer("Enemy"); //just doing gameObject updates the child's layer
            controller.onStandby = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && attackCombo1)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            other.GetComponent<PlayerController>().DamagePlayer(10, 25, 0, transform, false, 0, 7);
        }

        else if(other.tag == "Player" && attackCombo2)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            other.GetComponent<PlayerController>().DamagePlayer(25, 60, 0, transform, false, 0, 7);
        }

        else if(other.tag == "Player" && attackHeavy)
        {
            if(other.GetComponent<PlayerController>().isBroken)
            {
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            other.GetComponent<PlayerController>().DamagePlayer(35, 60, 0, transform, false, 0, 7);
        }

        else if(other.tag == "Player" && attackGrab)
        {
            other.GetComponent<PlayerController>().DamagePlayer(51, 0, 0, transform, true, 1, 7);

            if(!PlayerController.instance.isDamaged || PlayerController.instance.isBroken)
            controller.ManageStandby(grabAttackOffset);
        }
    }
}
