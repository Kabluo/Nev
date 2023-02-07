using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushgrowthBehaviour : MonoBehaviour
{
    [SerializeField] EnemyHealthController healthController;
    [SerializeField] EnemyController controller;
    [SerializeField] BoxCollider2D hitBox;
    [SerializeField] float attackCooldown = 5f;
    [SerializeField] float sporeDamageTickRate = 1f;
    [SerializeField] GameObject spikeObject;
    [SerializeField] GameObject vineObject;
    private float sporeDamageTimer = 0;
    private bool hasDealtDamage = false;
    private float attackTimer = 0f;
    private int attackId = 0;
    public bool sporeAttackOne;
    public bool sporeAttackTwo;
    public bool spikeAttack;
    public bool grabAttack;

    void Update()
    {
        if(Time.timeScale == 0 || healthController.isBroken) { return; }

        if(attackTimer > 0)
            attackTimer -= Time.deltaTime;
        
        if(hasDealtDamage && sporeDamageTimer > 0)
        {
            sporeDamageTimer -= Time.deltaTime;
            if(sporeDamageTimer <= 0)
                hasDealtDamage = false;
        }
        
        if(!controller.isChasing) { return; }

        Attack();
        AttackFrames();
    }

    void Attack()
    {
        if(attackTimer > 0)
        {
            controller.animator.SetBool("isAttacking", false);
            return;
        }

        else
        {
            //if(controller.inRange) //sporeattack, use this after updating EnemyController inRange behaviour
            if(Vector3.Distance(PlayerController.instance.gameObject.transform.position, controller.gameObject.transform.position) <= 6f)
                attackId = 0;
            
            else //random between grab and spike
                attackId = Random.Range(1, 3);
            
            controller.animator.SetInteger("attackId", attackId);
            controller.animator.SetBool("isAttacking", true);
        }
    }

    void AttackFrames()
    {
        if(sporeAttackOne)
        {
            attackTimer = attackCooldown;

            hitBox.offset = new Vector2(0f, 0f);
            hitBox.size = new Vector2(1.5f, 2f);
            hitBox.enabled = true;
        }

        if(sporeAttackTwo)
        {
            attackTimer = attackCooldown;

            hitBox.offset = new Vector2(0f, 0f);
            hitBox.size = new Vector2(7f, 3f);
            hitBox.enabled = true;
        }

        else if(grabAttack || spikeAttack)
        {
            attackTimer = attackCooldown;

            hitBox.offset = new Vector2(0f, -0.3f);
            hitBox.size = new Vector2(2.4f, 1.5f);
            hitBox.enabled = true;
        }

        else
        {
            hitBox.enabled = false;
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(sporeAttackOne)
            {
                if(PlayerController.instance.isBroken)
                {
                    PlayerController.instance.ExecutePlayer(5, controller.gameObject.transform);
                    controller.animator.SetTrigger("performExecutionAnimation");
                }

                else
                    PlayerController.instance.DamagePlayer(20, 15, 0, controller.gameObject.transform, false, 5, 7);
            }

            if(sporeAttackTwo)
            {
                //attach poison object, deal damage in onTriggerStay
            }

            if(grabAttack || spikeAttack)
            {
                if(PlayerController.instance.isBroken)
                {
                    PlayerController.instance.ExecutePlayer(5, controller.gameObject.transform);
                    controller.animator.SetTrigger("performExecutionAnimation");
                }

                else
                    PlayerController.instance.DamagePlayer(1, 20, 0, controller.gameObject.transform, false, 5, 7);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player" && sporeAttackTwo)
        {
            if(!hasDealtDamage)
            {
                if(PlayerController.instance.isBroken)
                {
                    PlayerController.instance.ExecutePlayer(5, controller.gameObject.transform);
                    controller.animator.SetTrigger("performExecutionAnimation");
                }

                else
                    PlayerController.instance.DamagePlayer(15, 5, 4, controller.gameObject.transform, false, 5, 7);
                
                hasDealtDamage = true;
                sporeDamageTimer = sporeDamageTickRate;
            }
        }
    }

    public void SummonSpike()
    {
        Instantiate(spikeObject, PlayerController.instance.lastOnGround, Quaternion.identity);
    }

    public void SummonVine()
    {
        Instantiate(vineObject, PlayerController.instance.lastOnGround, Quaternion.identity);
    }
}
