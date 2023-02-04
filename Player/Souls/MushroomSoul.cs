using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSoul : MonoBehaviour
{
    //later refactor and move stat/affinity information to SOs
    private float soulStrength = 1.2f;
    private float soulDexterity = 0.7f;
    private float soulConstitution = 1.5f;
    private float soulMagic = 0.2f;

    private float soulPhysicalAffinity = 0.7f; //takes *0.7 damage from physical attacks
    private float soulFireAffinity = 1.5f;
    private float soulColdAffinity = 0.9f;
    private float soulLightningAffinity = 0.5f;
    private float soulPoisonAffinity = 0f;
    private float soulForceAffinity = 1.5f;
    private float soulPsychicAffinity = 2f;

    [SerializeField] int attackEnergyCost = 1;

    private float timeAtAnim;
    private float damage;
    [SerializeField] float storedInput = 0.45f;
    [SerializeField] BoxCollider2D hitBox;
    [SerializeField] ParticleSystem hitEffect;
    

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

        SoulAttack();
        SoulAttackFrames();
    }

    void SoulAttack()
    {
        if(Input.GetButtonDown("soulAttack") && PlayerController.instance.isOnGround && !PlayerController.instance.isLightAttacking && PlayerTracker.instance.energy > attackEnergyCost)
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

    void SoulAttackFrames()
    {
        if(PlayerController.instance.soulAttack1)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0.75f, 0f);
            hitBox.size = new Vector2(1f, 2f);
        }

        else if(PlayerController.instance.soulAttack2)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0.75f, 0f);
            hitBox.size = new Vector2(1f, 2f);
        }

        else if(PlayerController.instance.soulAttack3)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0.75f, 0f);
            hitBox.size = new Vector2(1f, 2f);
        }

        else
        {
            hitBox.enabled = false;
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            Instantiate(hitEffect, other.transform.position, Quaternion.Euler(0f, 0f, 90+(90*Mathf.Sign(gameObject.transform.position.x - other.transform.position.x))));

            if(PlayerController.instance.soulAttack1)
            {
                damage = 5f + (2f * PlayerTracker.instance.strength); //2 damage per str
                other.GetComponent<EnemyHealthController>().DamageEnemy(damage, 10, 0, transform.position, 13); //stagger damage is static
            }
            
            else if(PlayerController.instance.soulAttack2)
            {
                damage = 5f + (2f * PlayerTracker.instance.strength); //2 damage per str
                other.GetComponent<EnemyHealthController>().DamageEnemy(damage, 10, 0, transform.position, 14); //stagger damage is static
            }
            
            else if(PlayerController.instance.soulAttack3)
            {
                damage = 5f + (2f * PlayerTracker.instance.strength); //2 damage per str
                other.GetComponent<EnemyHealthController>().DamageEnemy(damage, 10, 0, transform.position, 15); //stagger damage is static
            }
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
