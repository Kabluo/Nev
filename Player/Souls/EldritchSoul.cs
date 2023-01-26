using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldritchSoul : MonoBehaviour
{
    private float soulStrength = 0.7f;
    private float soulDexterity = 1f;
    private float soulConstitution = 1.5f;
    private float soulMagic = 2f;

    private float soulPhysicalAffinity = 1f; //takes *0.7 damage from physical attacks
    private float soulFireAffinity = 1f;
    private float soulColdAffinity = 0.5f;
    private float soulLightningAffinity = 1.5f;
    private float soulPoisonAffinity = 0.7f;
    private float soulForceAffinity = 1f;
    private float soulPsychicAffinity = 0f;
    
    [SerializeField] int attackEnergyCost = 5;

    private float timeAtAnim;
    [SerializeField] float storedInput = 0.45f;
    [SerializeField] private GameObject baseBullet;

    private bool hasDoubleJumped = false;
    [SerializeField] float doubleJumpForce = 8f;

    void Start()
    {
        PlayerController.instance.swappingSoul = false;
        UpdateStats();
    }

    void Update()
    {
        if(PlayerController.instance.swappingSoul)
        Destroy(gameObject);
        
        if(Time.timeScale == 0f || !PlayerController.instance.isAlive || PlayerController.instance.isInScene) { return; }

        if(PlayerTracker.instance.eldritchSoulUnlocks[0]) //if doublejumps is unlocked
        {
            DoubleJump();

            if(PlayerController.instance.isOnGround && hasDoubleJumped) //reset doublejump upon touching floor
            hasDoubleJumped = false;
        }

        if(PlayerTracker.instance.eldritchSoulUnlocks[1]) //if heal is unlocked
        {
            Heal();
        }

        SoulAttack();
        InstProjectile(); //might need to figure out a better way to handle this later on

    }

    void SoulAttack()
    {
        if(PlayerTracker.instance.energy < attackEnergyCost) { return; }

        if(Input.GetButtonDown("soulAttack") && PlayerController.instance.isOnGround && !PlayerController.instance.isLightAttacking)
        {
            timeAtAnim = Time.time;
            PlayerController.instance.isSoulAttacking = true;
        }
        
        if(Time.time > timeAtAnim + storedInput)
        {
            PlayerController.instance.isSoulAttacking = false;
        }

        PlayerController.instance.myAnimator.SetBool("isSoulAttacking", PlayerController.instance.isSoulAttacking);
    }

    void InstProjectile() //this structure ensures that only a single bullet gets fired at a  time
    {
        if(PlayerController.instance.fireProjectile)
        {
            PlayerTracker.instance.energy -= attackEnergyCost;
            UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);
            
            Instantiate(baseBullet, GameObject.Find("Shot Point").transform.position, transform.rotation);
            PlayerController.instance.fireProjectile = false;
        }
    }

    void DoubleJump()
    {
        if(Input.GetButtonDown("Jump") && !PlayerController.instance.isOnGround && !PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement") && !hasDoubleJumped)
        {
            PlayerController.instance.myAnimator.SetTrigger("eldritchSoulDoubleJumped");
            PlayerController.instance.myRigidbody.velocity = new Vector2 (PlayerController.instance.myRigidbody.velocity.x, doubleJumpForce);
            hasDoubleJumped = true;
        }
    }

    void Heal()
    {
        //cannot heal while running
        if(PlayerTracker.instance.energy < 33) { return; } //costs 33 energy, if refactored into changing DrainEnergy, instead set energy cost public variable in playercontroller.instance
        if(Input.GetKeyDown(KeyCode.R) && PlayerController.instance.isOnGround && !PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement") && PlayerController.instance.myAnimator.GetFloat("speed") < 4.1f)
        {
            PlayerController.instance.myAnimator.SetTrigger("soulSkill1");
            PlayerController.instance.healAmount = PlayerTracker.instance.maxHealth/2 + PlayerTracker.instance.magic * 2;
        }
    }

    void UpdateStats()
    {
        PlayerTracker.instance.strength = 1 + Mathf.FloorToInt(soulStrength * PlayerTracker.instance.baseStrength);
        PlayerTracker.instance.dexterity = 1 + Mathf.FloorToInt(soulDexterity * PlayerTracker.instance.baseDexterity);
        PlayerTracker.instance.constitution = 1 + Mathf.FloorToInt(soulConstitution * PlayerTracker.instance.baseConstitution);
        PlayerTracker.instance.magic = 1 + Mathf.FloorToInt(soulMagic * PlayerTracker.instance.baseMagic);

        PlayerTracker.instance.playerPhysicalAffinity = soulPhysicalAffinity;
        PlayerTracker.instance.playerFireAffinity = soulFireAffinity;
        PlayerTracker.instance.playerColdAffinity = soulColdAffinity;
        PlayerTracker.instance.playerLightningAffinity = soulLightningAffinity;
        PlayerTracker.instance.playerPoisonAffinity = soulPoisonAffinity;
        PlayerTracker.instance.playerForceAffinity = soulForceAffinity;
        PlayerTracker.instance.playerPsychicAffinity = soulPsychicAffinity;
    }
}
