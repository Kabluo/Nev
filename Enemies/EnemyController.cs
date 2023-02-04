using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("LOS")]
    [SerializeField] float lineOfSightDistance = 15f; //causes a circular area around the character, maybe change to conic later (or use triggers, requires moving this to a child object)
    Vector2 playerLocation;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float attackRange = 3f;
    public bool inRange;

    [Header("Movement")]
    private EnemyHealthController healthController;
    [SerializeField] Transform groundPoint;
    [SerializeField] Transform wallCheckPoint;
    private bool isOnGround;
    public LayerMask whatIsGround;
    [SerializeField] Transform[] patrolPoints;
    private int currentPoint = 0;
    [SerializeField] float waitAtPoints = 1f;
    private float waitCounter;
    [SerializeField] Rigidbody2D rigidBody;
    public Animator animator;
    [SerializeField] float jumpForce= 10f;
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float chaseDistance = 25f;
    [SerializeField] float minChaseTimeBase = 8f;
    private float minChaseTime;
    public bool isChasing;
    [SerializeField] float distanceToPlayerWhenEngaged = 0.5f; //at what distance should the character stop approaching player

    [Header("Standby Management")]
    [SerializeField] List<string> playerAnimNames = new List<string>(); //end standby when player reaches said animation stage
    private Vector3 standbyEnterLocation;
    public bool onStandby;

    void Start()
    {
        healthController = GetComponent<EnemyHealthController>();

        waitCounter = waitAtPoints;

        foreach(Transform pPoint in patrolPoints)
            pPoint.SetParent(null);
    }

    void Update()
    {
        if(Time.timeScale == 0 || healthController.isBroken) { return; }

        isOnGround = Physics2D.OverlapCircle(groundPoint.position, .1f, whatIsGround);
        animator.SetBool("isOnGround", isOnGround);
        animator.SetFloat("speed", Mathf.Abs(rigidBody.velocity.x));

        if(onStandby) //standby ending is moved to behaviour scripts
            rigidBody.velocity = new Vector2(0f, 0f);
        
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction") && healthController.knockback != 0)
        rigidBody.velocity = new Vector2(healthController.knockback, rigidBody.velocity.y);
        
        else if(animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        rigidBody.velocity = new Vector2(-transform.localScale.x * animator.GetFloat("attackKickX"), rigidBody.velocity.y + animator.GetFloat("attackKickY"));

        if(!PlayerController.instance.isAlive) //if player is dead, stop attacking and patrol, maybe just get player gameobject at this point
        {
            if(animator.GetBool("isAttacking"))
            animator.SetBool("isAttacking", false);

            Patrol();
            return;
        }

        if(!isChasing)
        Patrol();

        else
        Engaged();
    }

    void FixedUpdate()
    {
        Debug.Log(isChasing);
        if(Time.timeScale == 0 || healthController.isBroken) { return; }
        //Debug.DrawLine(transform.position, new Vector3(transform.position.x + (attackRange*-transform.localScale.x), transform.position.y), Color.cyan);

        playerLocation = PlayerController.instance.gameObject.transform.position;

        CheckRange();
        
        if(Physics2D.OverlapCircle(wallCheckPoint.position, .25f, whatIsGround))
        Jump();

        if(isChasing) { return; }

        if(Vector2.Distance(transform.position, playerLocation) <= lineOfSightDistance * Mathf.Sign(playerLocation.x - transform.position.x) * -transform.localScale.x ||
        healthController.triggeredByDamage)
        {
            if(Physics2D.Linecast(transform.position, playerLocation, whatIsGround) && !healthController.triggeredByDamage)
                return;
            
            else
            {
                isChasing = true;
                minChaseTime = minChaseTimeBase;
            }
        }
    }

    void Jump()
    {
        if(!animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction") && isOnGround)
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
    }

    void CheckRange()
    {
        //Debug.DrawLine(transform.position, new Vector3(transform.position.x - (attackRange*transform.localScale.x), transform.position.y));
        if(Vector3.Distance(PlayerController.instance.transform.position, transform.position) <= attackRange * Mathf.Sign(playerLocation.x - transform.position.x) * -transform.localScale.x)
        {
            Debug.DrawLine(transform.position, PlayerController.instance.transform.position);
            if(!Physics2D.Linecast(transform.position, PlayerController.instance.transform.position, whatIsGround))
            {
                inRange = true;
            }

            else
                inRange = false;
        }
        
        else
            inRange = false;
    }

    void Patrol()
    {
        if(patrolPoints.Length == 0 || animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction")) { return; }

        if(Mathf.Abs(transform.position.x - patrolPoints[currentPoint].position.x) > .1f)
        {
            if(transform.position.x < patrolPoints[currentPoint].position.x)
            {
                transform.localScale = new Vector3 (-1f, 1f, 1f);
                rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y);
            }
                
            else
            {
                transform.localScale = Vector3.one;
                rigidBody.velocity = new Vector2(-moveSpeed, rigidBody.velocity.y);
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

    void Engaged()
    {
        if(minChaseTime > 0)
            minChaseTime -= Time.deltaTime;

        //make different x and y range limits for enemies
        if(Vector3.Distance(PlayerController.instance.transform.position, transform.position) > chaseDistance && minChaseTime <= 0)
        {
            isChasing = false;
            return;
        }    

        else if(!animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        {
            if(Mathf.Abs(transform.position.x - PlayerController.instance.gameObject.transform.position.x) > distanceToPlayerWhenEngaged)
            {
                if(transform.position.x < PlayerController.instance.gameObject.transform.position.x)
                {
                    transform.localScale = new Vector3 (-1f, 1f, 1f);
                    rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y);
                }
                
                else
                {
                    transform.localScale = Vector3.one;
                    rigidBody.velocity = new Vector2(-moveSpeed, rigidBody.velocity.y);
                }
                
            }
            else
                rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
            
        }
    }

    public void ManageStandby(Vector2 standbyOffset)
    {
        if(!onStandby)
        {
            if(!Physics2D.OverlapCircle(new Vector3((PlayerController.instance.gameObject.transform.position.x + (PlayerController.instance.gameObject.transform.localScale.x * standbyOffset.x)), PlayerController.instance.gameObject.transform.position.y + standbyOffset.y, 0f), .2f, whatIsGround))
                gameObject.transform.position = new Vector3((PlayerController.instance.gameObject.transform.position.x + (PlayerController.instance.gameObject.transform.localScale.x * standbyOffset.x)), PlayerController.instance.gameObject.transform.position.y + standbyOffset.y, 0f);

            onStandby = true;
            animator.SetBool("onStandby", true);
            gameObject.layer = LayerMask.NameToLayer("EnemyInvincible"); //make invincible
        }
    }

    public void PlaySoundInAnimation(int index)
    {
        AudioManager.instance.PlaySFXPitchRandomized(index);
    }

}
