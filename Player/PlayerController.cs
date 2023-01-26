using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDataPersistance
{
    private bool originalObject = false;

    public bool isAlive = true;
    public bool isInScene;
    public Rigidbody2D myRigidbody;
    public Animator myAnimator;
    [SerializeField] ParticleSystem bloodEffect;
    public ParticleSystem brokenEffect;
    //public float storedInput;
    public bool isLightAttacking;
    public bool lightAttack1;
    public bool lightAttack2;
    public bool lightAttack3;
    //public bool lightAttack4; unused right now, manipulated through the animator, done like this as it can't edit lists or arrays
    //public bool lightAttack5;
    //public bool lightAttack6;
    //public bool lightAttack7;
    //public bool lightAttack8;
    //public bool lightAttack9;
    //public bool lightAttack10;
    public bool isSoulAttacking;
    public bool soulAttack1;
    public bool soulAttack2;
    public bool soulAttack3;
    public bool soulAttack4;
    //public bool isSoulAttack5;
    //public bool isSoulAttack6;
    //public bool isSoulAttack7;
    //public bool isSoulAttack8;
    //public bool isSoulAttack9;
    //public bool isSoulAttack10;


    [SerializeField] Transform groundPoint;
    [SerializeField] LayerMask whatIsGround;
    public Vector2 lastOnGround;
    public bool isOnGround;
    public bool swappingStance = false;
    public bool swappingSoul = false;

    public float moveSpeed;
    [SerializeField] float sprintMod = 8f;
    public float jumpForce;
    public float baseMoveSpeed;

    private float takenDamageAffinity = 0f;
    [SerializeField] float staggerRegenDelay = 2f;
    private float currentStaggerRegenDelay;
    private bool staggerRegenInterrupted;
    private float knockback = 0f;
    [SerializeField] float knockbackMod = 5f;
    public bool knockedBack;
    public bool isBroken = false;
    public bool isInvincible = false;
    public bool isDamaged = false;
    [SerializeField] float iFrameAfterStagger = .5f;
    [SerializeField] float iFrameAfterKnockback = 1f;
    private float isDamagedCounter = 0f;
    [SerializeField] float dodgeTimerBase = 0.45f;
    private bool isDodging = false;
    private float dodgeTimer;
    private float dodgeTimeAtAnim;
    private float dodgeDirection = 1f; //-1 left, 1 right
    [SerializeField] float dodgeStoredInput = 0.45f;

    public bool endEnemyStandby = false;

    public bool fireProjectile;
    public int healAmount;

    public static PlayerController instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //gameobject attached to this script is not destroyed on scene change
            originalObject = true;
        }

        else //if there is already a player, destroy the new  one / don't create a new one
        Destroy(this.gameObject);
    }

    void Start()
    {
        UIController.instance.UpdateHealth(PlayerTracker.instance.health, PlayerTracker.instance.maxHealth);
        UIController.instance.UpdateStagger(PlayerTracker.instance.stagger, PlayerTracker.instance.maxStagger);
        UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);

        Instantiate(PlayerTracker.instance.stanceObjects[PlayerTracker.instance.activeStanceIndex], transform);
        Instantiate(PlayerTracker.instance.soulObjects[PlayerTracker.instance.activeSoulIndex], transform); //created as a child of this object at transform location
        baseMoveSpeed = moveSpeed;
    }

    /*
    change hardcoded inputs to use the unity input system for controller support
    */

    void Update()
    {
        isOnGround = Physics2D.OverlapCircle(groundPoint.position, .2f, whatIsGround); //creates a circle around point with a size of .2, if it overlaps the layermask set true

        if(isOnGround)
        lastOnGround = transform.position;

        myAnimator.SetBool("isOnGround", isOnGround); //isOnGround is calculated in Jump() function

        //removed isDamaged from ere
        if((isInvincible || !isAlive || isInScene) && !isBroken) //not invincible while isBroken
        gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");

        else
        gameObject.layer = LayerMask.NameToLayer("Player");

        //myAnimator.SetInteger("lightAttackID", PlayerTracker.instance.equippedStances[PlayerTracker.instance.activeStanceIndex]); //maybe move to stanceswap later to stop
        //myAnimator.SetInteger("soulAttackID", PlayerTracker.instance.equippedSouls[PlayerTracker.instance.activeSoulIndex]); //repeated calls

        if(!isAlive) //remake later, also figure out a way so that isBroken animator state only triggers after certain animations
        {
            myRigidbody.velocity = new Vector2 (0f, myRigidbody.velocity.y); //stop if not alive
            gameObject.tag = "Corpse";
            knockback = 0; //reset knockback, carries into respawn otherwise
            return;
        }

        if(isInScene)
        {
            myRigidbody.velocity = new Vector2 (0f, myRigidbody.velocity.y); //stop if in scene
            return;
        }

        myAnimator.SetBool("isBroken", isBroken);
        if(isBroken) //stop movement if broken
        {
            myRigidbody.velocity = new Vector2 (0f, myRigidbody.velocity.y);
        }

        
        if(isDamaged)
        {
            isDamagedCounter -= Time.deltaTime;

            if(isDamagedCounter <= 0)
            isDamaged = false;
            /*myRigidbody.velocity = new Vector2 (0f, myRigidbody.velocity.y); //stop if in scene
            if(myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                isDamaged = false;
            }*/
        }
        

        if(Time.timeScale == 0f || isBroken) { return; }
        Run();
        Jump();
        Dodge();
        UpdateStagger();
        StanceSwap(-1);
        SoulSwap(-1);
    }

    void Run()
    {
        if(isInvincible && myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Dodge")) //if dodging
        {
            myRigidbody.velocity = new Vector2 (dodgeDirection * myAnimator.GetFloat("attackAdvance"), myRigidbody.velocity.y);
            FlipSides();   
        }

        else if(myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement") && knockback != 0)
        {
            myRigidbody.velocity = new Vector2 (knockback, myRigidbody.velocity.y);

            if(knockback > 0)
            {
                knockback -= Time.deltaTime * 25;
                if(knockback < 1)
                knockback = 0;
            }
            else
            {
                knockback += Time.deltaTime * 25;
                if(knockback > -1)
                knockback = 0;
            }
        }

        else if(myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement") && knockback == 0)
            myRigidbody.velocity = new Vector2 (transform.localScale.x * myAnimator.GetFloat("attackAdvance"), myRigidbody.velocity.y);

        else
        {
            myRigidbody.velocity = new Vector2 (Input.GetAxisRaw("Horizontal") * moveSpeed, myRigidbody.velocity.y);
            FlipSides();
        }

        myAnimator.SetFloat("speed", Mathf.Abs(Input.GetAxisRaw("Horizontal") * moveSpeed));
    }

    void FlipSides()
    {
        if(myRigidbody.velocity.x < 0)
        {
            transform.localScale = new Vector3 (-1f, 1f, 1f);
        }

        else if(myRigidbody.velocity.x > 0)
        {
            transform.localScale = Vector3.one; //creates a vector 3 of all 1s
        }
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && isOnGround && !myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement"))
        {
            myAnimator.SetTrigger("hasJumped");
            myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpForce);
        }
    }

    void Dodge() //update to use stored input later
    {
        if(Input.GetButton("DodgeRun") && isOnGround)
        moveSpeed = baseMoveSpeed + sprintMod;

        else
        moveSpeed = baseMoveSpeed;

        if(Input.GetButtonDown("DodgeRun") && isOnGround)
        {
            dodgeTimer = dodgeTimerBase;
        }

        if(dodgeTimer > 0f)
        dodgeTimer -= Time.deltaTime;

        if(Input.GetButtonUp("DodgeRun") && isOnGround && !isInvincible && dodgeTimer > 0f)
        {
            dodgeTimeAtAnim = Time.time;

            if(Input.GetAxisRaw("Horizontal") != 0f)
            dodgeDirection = Mathf.Sign(Input.GetAxisRaw("Horizontal"));

            else
            dodgeDirection = transform.localScale.x;
            
            isDodging = true;
            dodgeTimer = 0f;
            //myAnimator.SetTrigger("hasDodged");
        }

        if(Time.time > dodgeTimeAtAnim + dodgeStoredInput)
        isDodging = false;

        myAnimator.SetBool("isDodging", isDodging);
        
    }

    void FireProjectile() //called through animation events to give the go-ahead for firing projectiles, read by souls/stances and than turned false again
    {
        fireProjectile = true;
    }

    void DrainEnergy(int amount) //called through the animator to drain energy, useful for soul attacks draining energy at specific times
    {
        //if eventually skills that reduce or change energy costs are added, instead of passing in int, a public variable can be defined
        //in controller then edited by souls before this function is called in animations, using the public variable instead of int amount
        PlayerTracker.instance.energy -= amount;
        UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);
    }

    void HealPlayer()
    {
        PlayerTracker.instance.health += healAmount;
        if(PlayerTracker.instance.health > PlayerTracker.instance.maxHealth)
        PlayerTracker.instance.health = PlayerTracker.instance.maxHealth;

        UIController.instance.UpdateHealth(PlayerTracker.instance.health, PlayerTracker.instance.maxHealth);
    }

    public void StanceSwap(int index) //check UpdateSoulHud / *StanceHud methods in UIController, untested, might have bugs
    {
        if(myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement")) { return; }
        if(Input.GetKeyDown(KeyCode.Alpha1) || index == 0)
        {
            if(PlayerTracker.instance.equippedStances[0] != -1)
            {
                swappingStance = true;
                Instantiate(PlayerTracker.instance.stanceObjects[PlayerTracker.instance.equippedStances[0]], transform);
                PlayerTracker.instance.activeStanceIndex = 0;
                UIController.instance.UpdateStanceHud();
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha2) || index == 1)
        {
            if(PlayerTracker.instance.equippedStances[1] != -1)
            {
                swappingStance = true;
                Instantiate(PlayerTracker.instance.stanceObjects[PlayerTracker.instance.equippedStances[1]], transform);
                PlayerTracker.instance.activeStanceIndex = 1;
                UIController.instance.UpdateStanceHud();
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha3) || index == 2)
        {
            if(PlayerTracker.instance.equippedStances[2] != -1)
            {
                swappingStance = true;
                Instantiate(PlayerTracker.instance.stanceObjects[PlayerTracker.instance.equippedStances[2]], transform);
                PlayerTracker.instance.activeStanceIndex = 2;
                UIController.instance.UpdateStanceHud();
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha4) || index == 3)
        {
            if(PlayerTracker.instance.equippedStances[3] != -1)
            {
                swappingStance = true;
                Instantiate(PlayerTracker.instance.stanceObjects[PlayerTracker.instance.equippedStances[3]], transform);
                PlayerTracker.instance.activeStanceIndex = 3;
                UIController.instance.UpdateStanceHud();
            }
        }

        myAnimator.SetInteger("lightAttackID", PlayerTracker.instance.equippedStances[PlayerTracker.instance.activeStanceIndex]);
    }

    public void SoulSwap(int index)
    {
        if(myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement")) { return; }
        if(Input.GetKeyDown(KeyCode.F1) || index == 0)
        {
            if(PlayerTracker.instance.equippedSouls[0] != -1)
            {
                swappingSoul = true;
                Instantiate(PlayerTracker.instance.soulObjects[PlayerTracker.instance.equippedSouls[0]], transform);
                PlayerTracker.instance.activeSoulIndex = 0;
                UIController.instance.UpdateSoulHud();
            }
        }

        if(Input.GetKeyDown(KeyCode.F2) || index == 1)
        {
            if(PlayerTracker.instance.equippedSouls[1] != -1)
            {
                swappingSoul = true;
                Instantiate(PlayerTracker.instance.soulObjects[PlayerTracker.instance.equippedSouls[1]], transform);
                PlayerTracker.instance.activeSoulIndex = 1;
                UIController.instance.UpdateSoulHud();
            }
        }

        if(Input.GetKeyDown(KeyCode.F3) || index == 2)
        {
            if(PlayerTracker.instance.equippedSouls[2] != -1)
            {
                swappingSoul = true;
                Instantiate(PlayerTracker.instance.soulObjects[PlayerTracker.instance.equippedSouls[2]], transform);
                PlayerTracker.instance.activeSoulIndex = 2;
                UIController.instance.UpdateSoulHud();
            }
        }

        if(Input.GetKeyDown(KeyCode.F4) || index == 3)
        {
            if(PlayerTracker.instance.equippedSouls[3] != -1)
            {
                swappingSoul = true;
                Instantiate(PlayerTracker.instance.soulObjects[PlayerTracker.instance.equippedSouls[3]], transform);
                PlayerTracker.instance.activeSoulIndex = 3;
                UIController.instance.UpdateSoulHud();
            }
        }

        myAnimator.SetInteger("soulAttackID", PlayerTracker.instance.equippedSouls[PlayerTracker.instance.activeSoulIndex]);
    }

    public void DamagePlayer(int damage, int staggerDamage, int damageType, Transform attackOrigin, bool isGrab, int enemyId, int hitSoundIndex)
    {
        if(isDamaged) { return; }
        if(myAnimator.GetBool("isBroken") && !isGrab) { return; } //grabs go into executions automatically, bypassing isBroken

        //gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");

        /*
        if(UIController.instance.travelScreen.activeSelf) //called before IconMenu as it reverts to it after closing
        UIController.instance.TravelMenu();

        if(UIController.instance.iconScreen.activeSelf) //close open menus
        UIController.instance.IconMenu();

        if(UIController.instance.dialogueScreen.activeSelf)
        UIController.instance.DialogueMenu();
        */ //player  becomes  invincible  due to  isInScene  anyways, but  still  this  can remain here as a failsafe

        if(isGrab)
        {
            if(isBroken) //prevents animation errors if grabbed while already broken
            isBroken = false;

            transform.localScale = new Vector3(-1 * Mathf.Sign(transform.position.x - attackOrigin.position.x), 1, 1);
            myAnimator.SetInteger("grabberId", enemyId);
            myAnimator.SetTrigger("hasBeenGrabbed");
        }

        AudioManager.instance.PlaySFXPitchRandomized(hitSoundIndex);

        if(damageType == 0)  //needs refactoring, change affinities into an  array, lots of code rewrite, simple but busy work
            takenDamageAffinity = PlayerTracker.instance.playerPhysicalAffinity;

        else if(damageType == 1)
            takenDamageAffinity = PlayerTracker.instance.playerFireAffinity;
        
        else if(damageType == 2)
            takenDamageAffinity = PlayerTracker.instance.playerColdAffinity;

        else if(damageType == 3)
            takenDamageAffinity = PlayerTracker.instance.playerLightningAffinity;
        
        else if(damageType == 4)
            takenDamageAffinity = PlayerTracker.instance.playerPoisonAffinity;

        else if(damageType == 5)
            takenDamageAffinity = PlayerTracker.instance.playerForceAffinity;

        else
            takenDamageAffinity = PlayerTracker.instance.playerPsychicAffinity;

        takenDamageAffinity *= 1f-(((float)PlayerTracker.instance.constitution + 1) / 200);

        if(damage*takenDamageAffinity > 0)
        BleedPlayer(Quaternion.Euler(0f, 0f, 90-(90*Mathf.Sign(gameObject.transform.position.x - attackOrigin.position.x))));
        
        PlayerTracker.instance.health -= Mathf.RoundToInt(damage*takenDamageAffinity);

        if(!isGrab) //no stagger damage on grabs, stagger code block
        {
            PlayerTracker.instance.stagger -= Mathf.RoundToInt(staggerDamage*Mathf.Max(takenDamageAffinity, 0.3f)); //minimum modifier of .3f
            staggerRegenInterrupted = true;
            currentStaggerRegenDelay = staggerRegenDelay;

            UIController.instance.UpdateStagger(PlayerTracker.instance.stagger, PlayerTracker.instance.maxStagger);

            if(PlayerTracker.instance.stagger <= -PlayerTracker.instance.maxStagger)
            {
            myAnimator.SetTrigger("hasKnockedBack");

            isDamaged = true;
            isDamagedCounter = iFrameAfterKnockback;

            knockback = (knockbackMod * 2) * Mathf.Sign(transform.position.x-attackOrigin.position.x);
            PlayerTracker.instance.stagger = PlayerTracker.instance.maxStagger;
            }

            else if(PlayerTracker.instance.stagger <= 0)
            {
            myAnimator.SetTrigger("hasStaggered");

            isDamaged = true;
            isDamagedCounter = iFrameAfterStagger;
            
            knockback = knockbackMod * Mathf.Sign(transform.position.x-attackOrigin.position.x);
            PlayerTracker.instance.stagger = PlayerTracker.instance.maxStagger;
            }
        }
        
        UIController.instance.UpdateHealth(PlayerTracker.instance.health, PlayerTracker.instance.maxHealth); //update UI

        if(PlayerTracker.instance.health <= 0 && !isGrab)  //grabs have their own executions
        {
            Instantiate(brokenEffect, transform.position, Quaternion.Euler(90, 0, 0));
            isBroken = true;
        }
        
        else if(PlayerTracker.instance.health <= 0) //if reduced <0 by grab
        {
            myAnimator.SetTrigger("hasBeenKilled");
            isAlive = false;

            RespawnController.instance.playerCorpseId = enemyId + 1; //corpse objects are structured as [...enemyExecution, enemyGrabkill,...], enemies with multiple grabs/executions can have multiple Ids, just pass in different Id values and adjust accordingly
        }
    }

    /* integrated into DamagePlayer
    public void GrabPlayer(int enemyId, int damage, int damageType, Transform attackOrigin)
    {
        transform.localScale = new Vector3(-1 * Mathf.Sign(transform.position.x - attackOrigin.position.x), 1, 1);
        myAnimator.SetInteger("grabberId", enemyId);
        myAnimator.SetTrigger("hasBeenGrabbed");

        if(damageType == 0)  //needs refactoring, change affinities into an  array, lots of code rewrite, simple but busy work
            takenDamageAffinity = PlayerTracker.instance.playerPhysicalAffinity;

        else if(damageType == 1)
            takenDamageAffinity = PlayerTracker.instance.playerFireAffinity;
        
        else if(damageType == 2)
            takenDamageAffinity = PlayerTracker.instance.playerColdAffinity;

        else if(damageType == 3)
            takenDamageAffinity = PlayerTracker.instance.playerLightningAffinity;
        
        else if(damageType == 4)
            takenDamageAffinity = PlayerTracker.instance.playerPoisonAffinity;

        else if(damageType == 5)
            takenDamageAffinity = PlayerTracker.instance.playerForceAffinity;

        else
            takenDamageAffinity = PlayerTracker.instance.playerPsychicAffinity;
        
        PlayerTracker.instance.health -= Mathf.RoundToInt(damage*takenDamageAffinity);

        UIController.instance.UpdateHealth(PlayerTracker.instance.health, PlayerTracker.instance.maxHealth); //update UI

        if(PlayerTracker.instance.health <= 0)
        {
            myAnimator.SetTrigger("hasBeenKilled");
            isAlive = false;
        }
    }
    */

    public void ExecutePlayer(int enemyId, Transform attackOrigin)
    {
        isBroken = false; //end broken state
        isAlive = false;
        myAnimator.SetBool("isBroken", isBroken);
        transform.localScale = new Vector3(-1 * Mathf.Sign(transform.position.x - attackOrigin.position.x), 1, 1);
        myAnimator.SetInteger("grabberId", enemyId); //maybe change to executionId later
        myAnimator.SetTrigger("hasBeenExecuted");

        RespawnController.instance.playerCorpseId = enemyId;
    }

    private void BleedPlayer(Quaternion angle)
    {
        Instantiate(bloodEffect, transform.position, angle);
    }

    void UpdateStagger()
    {
        if(staggerRegenInterrupted)
        {
            currentStaggerRegenDelay -= Time.deltaTime;

            if(currentStaggerRegenDelay <= 0)
                staggerRegenInterrupted = false;
        }

        else
        {
            if(Mathf.Abs(PlayerTracker.instance.stagger - PlayerTracker.instance.maxStagger) <= 1)
            {
                PlayerTracker.instance.stagger = PlayerTracker.instance.maxStagger;
                return;
            }

            PlayerTracker.instance.stagger += Time.deltaTime*(1 + (PlayerTracker.instance.constitution/10));
        }

        UIController.instance.UpdateStagger(PlayerTracker.instance.stagger, PlayerTracker.instance.maxStagger);
    }

    public void LoadData(GameData data) //save player position through controller, might add a FrameEnd delay if seems problematic
    {
        if(!originalObject) { return; }

        this.transform.position = data.playerPosition;
        this.isBroken = data.isBroken;
    }

    public void SaveData(ref GameData data)
    {
        if(!originalObject) { return; }

        if(!isAlive || isBroken)
        data.isBroken = true;
        
        else
        data.isBroken = false;

        data.playerPosition = this.lastOnGround;
        data.levelName = SceneManager.GetActiveScene().name;
    }

    public void PlaySoundInAnimation(int effectIndex)
    {
        AudioManager.instance.PlaySFXPitchRandomized(effectIndex);
    }
}
