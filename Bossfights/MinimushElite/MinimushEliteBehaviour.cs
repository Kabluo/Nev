using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushEliteBehaviour : MonoBehaviour
{
    [SerializeField] EnemyHealthController healthController;
    [SerializeField] Transform groundPoint;
    [SerializeField] LayerMask whatIsGround;
    public Rigidbody2D rigidBody;
    public Animator animator;

    [SerializeField] Transform arenaLeftEdge;
    [SerializeField] Transform arenaRightEdge;
    
    public float jumpForce;
    [SerializeField] float moveSpeed;
    public bool enemyOnGround;
    public bool inRange = false;
    public bool onStandby;

    public bool phaseTwo = false; //phase 2, unlocks telegrab, causes lightningstrikes on attacks -75%
    public bool phaseThree = false; //phase 3, causes lightningstrikes to become grabs
    private float phaseTwoThreshold;
    private float phaseThreeThreshold;
    [SerializeField] float teleportCooldownBase = 15f;
    [SerializeField] float trapCooldownBase = 8f;
    [SerializeField] float counterCooldownBase = 20f;
    [SerializeField] float heavyAttackCooldownBase = 8f;
    private float teleportCooldown = 0f;
    private float trapCooldown = 0f;
    private float counterCooldown = 0f;
    private float heavyAttackCooldown = 0f;

    private bool isCountering = false;
    private bool resetCounterDuration = false;
    private int healthAtCounter;
    private float counterDuration;
    private float teleportDuration = 4f; //time spent in standby
    private int teleportAttackId = 0;
    private bool hasTeleported = false;

    [SerializeField] GameObject lightningStrike;
    [SerializeField] GameObject lightningGrab; //phase 3 lightning strike
    [SerializeField] GameObject lightningTrap;

    public bool grabbedNev = false; //changes after teleport spawn-in behaviour

    public bool attack1 = false;
    public bool attack2 = false;
    public bool heavyAttack = false;
    public bool counterAttack = false;
    public bool teleChargeAttack = false;
    public bool teleDive = false;
    public bool teleDiveAttack = false;

    void Start()
    {
        phaseTwoThreshold = healthController.currentHealth*(.75f);
        phaseThreeThreshold = healthController.currentHealth*(.45f);

        teleportCooldown = teleportCooldownBase;
        trapCooldown = trapCooldownBase;
        counterCooldown = counterCooldownBase;
        heavyAttackCooldown = heavyAttackCooldownBase;

        gameObject.transform.SetParent(null);
    }

    void Update()
    {
        if(Time.timeScale == 0f) { return; }

        if(teleportCooldown >= 0)
        teleportCooldown -= Time.deltaTime;

        if(trapCooldown >= 0)
        trapCooldown -= Time.deltaTime;

        if(counterCooldown >= 0)
        counterCooldown -= Time.deltaTime;

        if(heavyAttackCooldown >= 0)
        heavyAttackCooldown -= Time.deltaTime;

        if(healthController.currentHealth <= phaseTwoThreshold && !phaseTwo) //might refactor and get this as a variable, similar to slider manip for bossfights
        phaseTwo = true;

        if(healthController.currentHealth <= phaseThreeThreshold && !phaseThree)
        phaseThree = true;

        enemyOnGround = Physics2D.OverlapCircle(groundPoint.position, .05f, whatIsGround);
        animator.SetBool("isOnGround", enemyOnGround);
        animator.SetFloat("speed", Mathf.Abs(rigidBody.velocity.x));

        if(hasTeleported && !grabbedNev)
        {
            teleportDuration -= Time.deltaTime;

            if(teleportDuration <= 0)
            {
                hasTeleported = false;
                teleportCooldown = Random.Range(teleportCooldownBase/2, teleportCooldownBase);

                if(teleportAttackId == 0) //telecharge
                {
                    //closer to the right
                    if(Vector3.Distance(PlayerController.instance.transform.position, arenaLeftEdge.transform.position) >= Vector3.Distance(PlayerController.instance.transform.position, arenaRightEdge.transform.position))
                    {
                        gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
                        gameObject.transform.position = new Vector3(PlayerController.instance.transform.position.x - 6f, gameObject.transform.position.y, gameObject.transform.position.z);
                    }
                    else
                    {
                        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                        gameObject.transform.position = new Vector3(PlayerController.instance.transform.position.x + 6f, gameObject.transform.position.y, gameObject.transform.position.z);   
                    }
                }

                else //telefall
                {
                    gameObject.transform.position = new Vector3(PlayerController.instance.transform.position.x, -10f, gameObject.transform.position.z);
                }

                animator.SetBool("onStandby", false);
                gameObject.layer = LayerMask.NameToLayer("Enemy"); //make invincible
                
                animator.SetBool("isTeleAttacking", true); //break teleAttacking at the end of animation
                animator.SetInteger("teleAttackId", teleportAttackId);
            }

            return;
        }

        else if(grabbedNev)
        {
            hasTeleported = false;
            animator.SetBool("isTeleAttacking", false);

            if(!Physics2D.OverlapCircle(new Vector3((PlayerController.instance.gameObject.transform.position.x + (PlayerController.instance.gameObject.transform.localScale.x + 0.86f)), PlayerController.instance.gameObject.transform.position.y, 0f), .2f, whatIsGround)) //creates a circle around point with a size of .2, if it overlaps the layermask set true
            gameObject.transform.position = new Vector3((PlayerController.instance.gameObject.transform.position.x + (PlayerController.instance.gameObject.transform.localScale.x*0.86f)), PlayerController.instance.gameObject.transform.position.y, 0f);

            if(PlayerController.instance.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle") && PlayerController.instance.isAlive) //end standby upon returning to idle, second  line is failsafe
            {
                grabbedNev = false;
                animator.SetBool("onStandby", false);
                gameObject.layer = LayerMask.NameToLayer("Enemy"); //end invincible
            }
        }

        if(animator.GetFloat("attackKick") > 0) //prevents flying up due to attack kick
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rigidBody.constraints &= ~RigidbodyConstraints2D.FreezePositionY; //unfreeze y position
        }

        if(!PlayerController.instance.isAlive) //if player is dead, stop attacking and patrol, maybe just get player gameobject at this point
        {
            if(animator.GetBool("isAttacking"))
            animator.SetBool("isAttacking", false);
            return;
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction") && healthController.knockback != 0)
        rigidBody.velocity = new Vector2(healthController.knockback, rigidBody.velocity.y);
        
        else if(animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        rigidBody.velocity = new Vector2(-transform.localScale.x * animator.GetFloat("attackKick"), rigidBody.velocity.y);

        //combat behaviour
        animator.SetBool("isAttacking", inRange); //autoattacks when in melee range

        if(!inRange)
        ChasePlayer(); //default chase behaviour, run towards player

        if(!isCountering)
        Combat();

        else
        CounterStance();
    }

    void CounterStance()
    {
        if(resetCounterDuration)
        {
            counterDuration = Random.Range(5f, 8f);
            resetCounterDuration = false;
        }
        
        counterDuration -= Time.deltaTime;

        //if duration ends, player gets close or (6f) or damage is taken, end counter
        if(counterDuration <= 0 || healthController.currentHealth < healthAtCounter)
        {
            counterCooldown = Random.Range(counterCooldownBase/2, counterCooldownBase); //reset cooldown
            isCountering = false;
            animator.SetTrigger("triggerCounter"); //proceeds to next step of counter
            return;
        }

    }

    void Combat()
    {
        if(heavyAttackCooldown <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        {
            animator.SetTrigger("heavyAttack");
            heavyAttackCooldown = Random.Range(heavyAttackCooldownBase/2, heavyAttackCooldownBase);
            return;
        }

        if(trapCooldown <= 0 && !animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        {
            animator.SetTrigger("layingTrap"); //lay trap, call event through anim
            return;
        }

        if(counterCooldown <= 0 && animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        {
            isCountering = true;
            healthAtCounter = healthController.currentHealth;
            resetCounterDuration = true; //reset duration in counter function this way, prevents pre-counting
            animator.SetTrigger("counterStance"); //starts counter, exists in a loop
            return;
        }

        if(Vector3.Distance(transform.position, PlayerController.instance.transform.position) >= 6f && teleportCooldown <= 0 && animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        {
            animator.SetTrigger("hasTeleported"); //triggers animation
            return;
        }
    }

    public void CommenceTeleport() //called through teleport animation
    {
        ManageStandby();

        teleportAttackId = Random.Range(0, 2);

        if(phaseThree)
        teleportDuration = Random.Range(3f, 4f);

        else
        teleportDuration = Random.Range(0.5f, 4f);

        hasTeleported = true;
    }

    public void LayTrap()
    {
        Instantiate(lightningTrap, new Vector3(transform.position.x, -17.9f, transform.position.z), Quaternion.identity);
        trapCooldown = Random.Range(trapCooldownBase/2, trapCooldownBase);

        if(phaseTwo)
        {
            int amountToLay;
            amountToLay = Random.Range(1, 3);

            if(phaseThree)
            amountToLay *= 2;

            for(int i = 0; i < amountToLay; i++)
            {
                StartCoroutine(LayTrapCO(new Vector3(Random.Range(arenaLeftEdge.transform.position.x, arenaRightEdge.transform.position.x), -17.9f, transform.position.z)));
            }
        }
    }

    public void ComboAttackLightningTrap()
    {
        if(!phaseTwo) { return; }

        Instantiate(lightningTrap, new Vector3(transform.position.x, -17.9f, transform.position.z), Quaternion.identity);
    }

    IEnumerator LayTrapCO(Vector3 location)
    {
        float timeToWait;
        timeToWait = Random.Range(0.5f, 2f);

        yield return new WaitForSeconds(timeToWait);
        Instantiate(lightningTrap, location, Quaternion.identity);
    }

    void ChasePlayer()
    {
        if(!animator.GetCurrentAnimatorStateInfo(0).IsTag("PreventsAction"))
        {
            if(Mathf.Abs(transform.position.x - PlayerController.instance.gameObject.transform.position.x) > 0.6f)
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

    public void CallLightning() //called through animator
    {
        if(!phaseTwo) { return ;}

        int amountToCall;
        amountToCall = Random.Range(2, 7);

        if(phaseThree)
        amountToCall *= 2;

        for(int i = 0; i < amountToCall; i++)
        {
            if(i == 0)
            StartCoroutine(CallLightningCO(new Vector3(PlayerController.instance.gameObject.transform.position.x, -18.09f, transform.position.z), phaseThree));

            else
            {
                StartCoroutine(CallLightningCO(new Vector3(Random.Range(arenaLeftEdge.transform.position.x, arenaRightEdge.transform.position.x), -18.09f, transform.position.z), phaseThree));
            }
        }
    }

    public void HeavyAttackLightning()
    {
        if(!phaseTwo) { return; }

        for(int i = 0; i < 4; i++)
        {
            StartCoroutine(CallLightningCO(new Vector3(transform.position.x + Random.Range(-3f, 3f), -18.09f, transform.position.z), false));
        }
    }

    IEnumerator CallLightningCO(Vector3 location, bool isGrab)
    {
        float timeToWait;
        timeToWait = Random.Range(0.5f, 2f);

        yield return new WaitForSeconds(timeToWait);

        if(isGrab)
        Instantiate(lightningGrab, location, Quaternion.identity);

        else
        Instantiate(lightningStrike, location, Quaternion.identity);
    }

    public void ManageStandby() //called through animation to enter standby
    {
        //if(animator.GetBool("onStandby"))
        //{
        //    animator.SetBool("onStandby", false);
        //    gameObject.layer = LayerMask.NameToLayer("Enemy"); //end invincible
        //}

        if(!animator.GetBool("onStandby"))
        {
            animator.SetBool("onStandby", true);
            gameObject.layer = LayerMask.NameToLayer("EnemyInvincible"); //make invincible
        }
    }
}
