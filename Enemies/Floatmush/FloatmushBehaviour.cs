using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatmushBehaviour : MonoBehaviour
{
    [SerializeField] EnemyHealthController healthController;
    [SerializeField] EnemyController controller;
    [SerializeField] BoxCollider2D hitBox;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] float floatSpeed = 0.1f;
    [SerializeField] float sporeCooldown = 8f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] Transform WallPathfindingPoint;
    [SerializeField] float pathfindingMod = 1.3f;
    [SerializeField] int pathfindingAttemptCycles = 10;
    private float sporeTimer = 0f;
    private float startingHeight;
    private float moddedHeight;
    private bool canDive;
    private int attackId;
    public bool attackSpore;
    public bool attackDiveFall;
    public bool attackDive;
    public bool attackFlail;
    private bool needsReset = false;

    void Start()
    {
        startingHeight = transform.position.y;
        moddedHeight = startingHeight;
    }

    void Update()
    {
        if(Time.timeScale == 0 || healthController.isBroken) { return; }
        
        //extremely simple pathfinding, maybe create a FloaterPathfinding script later instead of manually adding this to behaviour, might upgrade to A* at some point
        Pathfinding();
        FloatUpDown(moddedHeight);

        if(moddedHeight != startingHeight && Mathf.Abs(transform.position.y - moddedHeight) <= .3f)
        {
            if(!Physics2D.Linecast(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.y, startingHeight), controller.whatIsGround))
            {
                moddedHeight = startingHeight;
            }
        }

        if(sporeTimer > 0)
            sporeTimer -= Time.deltaTime;
        
        Attack();
        AttackFrames();
    }

    void Pathfinding()
    {
        if(Physics2D.OverlapCircle(WallPathfindingPoint.position, .85f, controller.whatIsGround) && WallPathfindingPoint.position.y == moddedHeight)
        {
            Debug.Log("Pathfinding.");
            for(int i = 0; i < pathfindingAttemptCycles; i++)
            {
                if(!Physics2D.OverlapCircle(new Vector2(WallPathfindingPoint.transform.position.x, WallPathfindingPoint.transform.position.y + i*pathfindingMod), .85f, controller.whatIsGround))
                {
                    moddedHeight += i*pathfindingMod;
                    Debug.Log("New target height: " + moddedHeight);
                    break;
                }

                else if(!Physics2D.OverlapCircle(new Vector2(WallPathfindingPoint.transform.position.x, WallPathfindingPoint.transform.position.y - i*pathfindingMod), .85f, controller.whatIsGround))
                {
                    moddedHeight -= i*pathfindingMod;
                    Debug.Log("New target height: " + moddedHeight);
                    break;
                }
            }
        }
    }

    void FloatUpDown(float targetHeight)
    {
        if(controller.animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction")) { return; }

        if(Mathf.Abs(transform.position.y - targetHeight) <= .2f)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
            if(needsReset)
                needsReset = false;
        }

        else if(transform.position.y < targetHeight)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y + floatSpeed/2);
        
        else if(transform.position.y > targetHeight)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.y - floatSpeed);
    }

    void Attack()
    {
        if(controller.isChasing && sporeTimer <= 0 && !needsReset)
        {
            attackId = 0;
            controller.animator.SetInteger("attackId", attackId);
            controller.animator.SetBool("isAttacking", true);  
        }

        else if(Mathf.Abs(transform.position.x - PlayerController.instance.transform.position.x) <= attackRange && !needsReset)
        {
            if(Mathf.Abs(transform.position.y - PlayerController.instance.transform.position.y) <= 0.5f)
            {
                attackId = 1;
            }

            else if(!Physics2D.Linecast(transform.position, PlayerController.instance.transform.position, controller.whatIsGround))
            {
                attackId = 2;
            }

            controller.animator.SetInteger("attackId", attackId);
            controller.animator.SetBool("isAttacking", true);  
        }

        else
            controller.animator.SetBool("isAttacking", false); 
    }

    void AttackFrames()
    {
        if(attackSpore)
        {
            sporeTimer = sporeCooldown;
            needsReset = true;

            hitBox.offset = new Vector2(0f, 0f);
            hitBox.size = new Vector2(3.5f, 3.3f);
            hitBox.enabled = true;
        }

        else if(attackDiveFall)
        {
            needsReset = true;

            hitBox.offset = new Vector2(0, 0.4f);
            hitBox.size = new Vector2(1.4f, 1.8f);
            hitBox.enabled = true;
        }

        else if(attackDive)
        {
            needsReset = true;

            hitBox.offset = new Vector2(0f, 1f);
            hitBox.size = new Vector2(4f, 3f);
            hitBox.enabled = true;
        }

        else if(attackFlail)
        {
            needsReset = true;

            hitBox.offset = new Vector2(0f, 0.2f);
            hitBox.size = new Vector2(4f, 1.3f);
            hitBox.enabled = true;
        }

        else
            hitBox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && attackSpore) //summon poison cloud here after making bullet object, also add poisoned status and poison death animation
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(8, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(20, 40, 4, controller.gameObject.transform, false, 8, 7);
        }

        if(other.tag == "Player" && attackDiveFall)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(8, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(15, 60, 0, controller.gameObject.transform, false, 8, 7);
        }

        if(other.tag == "Player" && attackDive)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(8, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(30, 60, 0, controller.gameObject.transform, false, 8, 7);
        }

        if(other.tag == "Player" && attackFlail)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(8, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(15, 80, 0, controller.gameObject.transform, false, 8, 7);
        }
    }
}
