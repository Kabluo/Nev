using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldiermushBehaviour : MonoBehaviour
{
    [SerializeField] EnemyHealthController healthController;
    [SerializeField] EnemyController controller;
    [SerializeField] BoxCollider2D hitBox;
    [SerializeField] float grabAttackCooldown = 8f;
    [SerializeField] float chargeAttackCooldown = 8f;
    private float grabAttackTimer = 0f;
    private float chargeAttackTimer = 0f;
    private int attackId;
    public bool attackStomp;
    public bool attackHeadbutt;
    public bool attackCharge;
    public bool attackGrab;
    [SerializeField] Vector2 grabAttackOffset;

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0 || healthController.isBroken) { return; }

        if(grabAttackTimer > 0)
            grabAttackTimer -= Time.deltaTime;
        
        if(chargeAttackTimer > 0)
            chargeAttackTimer -= Time.deltaTime;

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
            attackId = 3; //grab, change grab timer upon performing a grab attack
        
            else
            attackId = Random.Range(0, 2); //0 = kick, 1 = headbutt

            controller.animator.SetInteger("attackId", attackId);
            controller.animator.SetBool("isAttacking", true);
        }
        
        else if(!Physics2D.Linecast(transform.position, PlayerController.instance.transform.position, controller.whatIsGround) && chargeAttackTimer <= 0 && controller.isChasing)
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
        if(attackStomp)
        {
            hitBox.offset = new Vector2(-0.7f, -0.35f);
            hitBox.size = new Vector2(1.2f, 1.2f);
            hitBox.enabled = true;
        }

        else if(attackHeadbutt)
        {
            hitBox.offset = new Vector2(-0.7f, -0.3f);
            hitBox.size = new Vector2(2.6f, 1.7f);
            hitBox.enabled = true;
        }

        else if(attackCharge)
        {
            chargeAttackTimer = chargeAttackCooldown;

            hitBox.offset = new Vector2(-1f, 0.2f);
            hitBox.size = new Vector2(1.4f, 2.3f);
            hitBox.enabled = true;
        }

        else if(attackGrab)
        {
            grabAttackTimer = grabAttackCooldown;
            
            hitBox.offset = new Vector2(-0.6f, 0f);
            hitBox.size = new Vector2(1.3f, 1.2f);
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
        if(PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("OnGround")  && controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Standby"))
        {
            controller.animator.SetBool("onStandby", false);
            controller.gameObject.layer = LayerMask.NameToLayer("Enemy"); //just doing gameObject updates the child's layer
            controller.onStandby = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && attackStomp)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(2, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(20, 25, 0, controller.gameObject.transform, false, 2, 7);
        }

        else if(other.tag == "Player" && attackHeadbutt)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(2, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(25, 30, 0, controller.gameObject.transform, false, 2, 7);
        }

        else if(other.tag == "Player" && attackCharge)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(2, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(35, 60, 0, controller.gameObject.transform, false, 2, 7);
        }

        else if(other.tag == "Player" && attackGrab)
        {
            PlayerController.instance.DamagePlayer(51, 0, 0, controller.gameObject.transform, true, 3, 7);

            if(!PlayerController.instance.isDamaged || PlayerController.instance.isBroken)
            controller.ManageStandby(grabAttackOffset); //update offset later
        }
    }
}
