using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushController : MonoBehaviour
{
    [SerializeField] EnemyHealthController healthController;
    [SerializeField] Transform groundPoint;
    [SerializeField] LayerMask whatIsGround;
    public Rigidbody2D rigidBody;
    public Animator animator;
    
    public float jumpForce;
    [SerializeField] float moveSpeed, waitAtPoints;
    public bool enemyOnGround;
    public bool isChasing = false;
    public float minChaseTimeBase = 8f;
    public float minChaseTime;
    public bool inRange = false;
    [SerializeField] float heavyAttackRange = 4f;
    private float heavyAttackChance = 0f;
    public int attackId; //used to determine attack
    public bool attack1;
    public bool attack2;
    public bool heavyAttack;
    public bool grabAttack;
    [SerializeField] float grabAttackCooldown = 8f;
    private float grabAttackTimer;
    public bool onStandby;
    [SerializeField] float chaseDistance = 50f;
    [SerializeField] GameObject chaseObject;

    [SerializeField] Transform[] patrolPoints;
    private int currentPoint = 0;
    private float waitCounter;

    // Start is called before the first frame update
    void Start()
    {
        grabAttackTimer = grabAttackCooldown;
        waitCounter = waitAtPoints; //wait at spawn

        foreach(Transform pPoint in patrolPoints)
        {
            pPoint.SetParent(null); //detach patrol points from parent to prevent them from moving with the gameobject
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0f) { return; }

        enemyOnGround = Physics2D.OverlapCircle(groundPoint.position, .05f, whatIsGround);
        animator.SetBool("isOnGround", enemyOnGround);
        animator.SetFloat("speed", Mathf.Abs(rigidBody.velocity.x));

        if(animator.GetFloat("attackKick") > 0) //prevents flying up due to attack kick
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rigidBody.constraints &= ~RigidbodyConstraints2D.FreezePositionY; //unfreeze y position
        }

        if(onStandby)
        {
            rigidBody.velocity = new Vector2(0f, 0f); //stop from flying around
            OnStandby();
            return;
        }

        if(!PlayerController.instance.isAlive) //if player is dead, stop attacking and patrol, maybe just get player gameobject at this point
        {
            if(animator.GetBool("isAttacking"))
            animator.SetBool("isAttacking", false);

            Patrol();
            return;
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction") && healthController.knockback != 0)
        rigidBody.velocity = new Vector2(healthController.knockback, rigidBody.velocity.y);
        
        else if(animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        rigidBody.velocity = new Vector2(-transform.localScale.x * animator.GetFloat("attackKick"), rigidBody.velocity.y);

        if(!isChasing)
        Patrol();

        else
        Engaged();
    }

    void OnStandby()
    {
        animator.SetBool("onStandby", onStandby);
        gameObject.layer = LayerMask.NameToLayer("EnemyInvincible"); //make invincible

        if(!Physics2D.OverlapCircle(new Vector3((PlayerController.instance.gameObject.transform.position.x + (PlayerController.instance.gameObject.transform.localScale.x * 0.86f)), PlayerController.instance.gameObject.transform.position.y, 0f), .2f, whatIsGround)) //creates a circle around point with a size of .2, if it overlaps the layermask set true
            gameObject.transform.position = new Vector3((PlayerController.instance.gameObject.transform.position.x + (PlayerController.instance.gameObject.transform.localScale.x*0.86f)), PlayerController.instance.gameObject.transform.position.y, 0f);

        if(PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && animator.GetCurrentAnimatorStateInfo(0).IsName("Standby")) //end standby upon returning to idle, second  line is failsafe
        {
            onStandby = false;
            animator.SetBool("onStandby", onStandby);
            gameObject.layer = LayerMask.NameToLayer("Enemy"); //end invincible
        }
    }

    void Patrol()
    {
        if(patrolPoints.Length == 0) { return; }

        if(Mathf.Abs(transform.position.x - patrolPoints[currentPoint].position.x) > 0.1f)
        {
            if(transform.position.x < patrolPoints[currentPoint].position.x)
            {
                rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y);
                transform.localScale  = new Vector3(-1, 1, 1); //flip gameobject
            }
            else
            {
                rigidBody.velocity = new Vector2(-moveSpeed, rigidBody.velocity.y);
                transform.localScale  = new Vector3(1, 1, 1);
            }
        }

        else
        {
            rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);

            waitCounter  -= Time.deltaTime;
            if(waitCounter <= 0)
            {
                waitCounter  = waitAtPoints;
                currentPoint++;

                if(currentPoint >= patrolPoints.Length)
                currentPoint = 0; //loop back to start
            }
        }
    }

    void Engaged() //chase player, other actions will be handled through WhatIsAhead
    {
        Attack();

        if(Vector3.Distance(PlayerController.instance.gameObject.transform.position, transform.position) > chaseDistance && minChaseTime <= 0)
        {
            if(minChaseTime > 0)
            minChaseTime -= Time.deltaTime;
            
            isChasing = false;
            chaseObject.SetActive(true);
        }

        else if(!animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        {
            if(Mathf.Abs(transform.position.x - PlayerController.instance.gameObject.transform.position.x) > 0.5f)
            {
                if(transform.position.x < PlayerController.instance.gameObject.transform.position.x)
                {
                    rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y);
                    transform.localScale  = new Vector3(-1, 1, 1); //flip gameobject
                }
                else
                {
                    rigidBody.velocity = new Vector2(-moveSpeed, rigidBody.velocity.y);
                    transform.localScale  = new Vector3(1, 1, 1);
                }
            }
            else
            {
                rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
            }
        }
    }

    void Attack() //fix attack kick, attack trigger and add attack hitbox, pretty much remake the whole system, also fix animation bug
    {
        grabAttackTimer -= Time.deltaTime;

        if(inRange && grabAttackTimer > 0) //add a cooldown to grab attack to prevent spam, check for transform location after grab attack to prevent falling off the world on going out of standby, try casting collider for it
        attackId = 0;

        else if(inRange)
        {
            attackId = 1;
            grabAttackTimer = grabAttackCooldown;
        }

        else if(Vector3.Distance(PlayerController.instance.gameObject.transform.position, transform.position) <= heavyAttackRange && !animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        attackId = 2;

        if(attackId != 2)
        {
            animator.SetInteger("attackId", attackId);
            animator.SetBool("isAttacking", inRange);
        }

        else if(attackId == 2)
        {
            heavyAttackChance = Random.Range(0f, 10f);
            if(heavyAttackChance > 9.95f)
            {
                animator.SetInteger("attackId", attackId);
                animator.SetBool("isAttacking", true);
            }
            else
            {
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("Minimush Heavy Attack"))
                animator.SetBool("isAttacking", false);
                return;
            }
        }
    }
}
