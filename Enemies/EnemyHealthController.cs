using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] Animator animator;
    public bool triggeredByDamage = false;
    //get unlock  array  from  player tracker using a getter method at start

    [Header("Enemy Info")]
    public int enemyId = 0;
    [SerializeField] int maxHealth = 10;
    public int currentHealth;
    [SerializeField] int staggerThreshold = 10;
    public int currentStagger;
    public float knockback = 0f;
    [SerializeField] float knockbackMod = 5f;
    public bool knockedBack;
    [SerializeField] float[] damageAffinities = {1f, 1f, 1f, 1f, 1f, 1f, 1f};

    [Header("On Death")]
    public bool isBroken = false;
    [SerializeField] float deathTimer = 3f;
    [SerializeField] int healOnKill = 15;
    [SerializeField] int lightReward = 8; //light given to player
    [SerializeField] ParticleSystem brokenEffect;
    [SerializeField] ParticleSystem deathEffect; //only on default death
    //[SerializeField] string soulUnlockMessage = "Soul Unlocked";
    //[SerializeField] string abilityUnlockMessage  = "Skill Unlocked";
    
    
    //affinities by index 0-6
    //[SerializeField] float physicalAffinity = 1f;
    //[SerializeField] float fireAffinity = 1f;
    //[SerializeField] float coldAffinity = 1f;
    //[SerializeField] float lightningAffinity = 1f;
    //[SerializeField] float poisonAffinity = 1f;
    //[SerializeField] float forceAffinity = 1f;
    //[SerializeField] float psychicAffinity = 1f;

    void Start()
    {
        currentHealth = maxHealth;
        currentStagger = staggerThreshold;
    }

    void Update()
    {
        if(Time.timeScale == 0f) { return; }

        if(isBroken)
        {
            animator.SetBool("isBroken", isBroken);

            deathTimer -= Time.deltaTime;

            if(deathTimer <= 0)
            {
                Death();
            }
        }

        if(knockback == 0) { return ;}
        if(knockback > 0) //change knockback over time depending on knockback location
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

    public void DamageEnemy(float damageAmount, int staggerDamage, int damageType, Vector3 sourceLocation, int hitSoundIndex)
    {
        if(animator.GetBool("isBroken"))
        {
            return;
        }

        if(!triggeredByDamage)
        triggeredByDamage = true;

        AudioManager.instance.PlaySFXPitchRandomized(hitSoundIndex);

        currentHealth -= Mathf.RoundToInt(damageAmount*damageAffinities[damageType]);
        currentStagger -= Mathf.RoundToInt(staggerDamage*damageAffinities[damageType]);

        if(currentStagger <= -staggerThreshold)
        {
            animator.SetTrigger("hasKnockedBack");
            knockback = (knockbackMod * 2) * Mathf.Sign(transform.position.x-sourceLocation.x);
            currentStagger = staggerThreshold;
        }

        else if(currentStagger <= 0)
        {
            animator.SetTrigger("hasStaggered");
            knockback = knockbackMod * Mathf.Sign(transform.position.x-sourceLocation.x);
            currentStagger = staggerThreshold;
        }

        if(currentHealth <= 0)
        {
            Instantiate(brokenEffect, transform.position, Quaternion.Euler(90, 0, 0));
            isBroken = true;
        }

    }

    void Death()
    {
        PlayerTracker.instance.health += healOnKill;
        if(PlayerTracker.instance.health > PlayerTracker.instance.maxHealth)
        PlayerTracker.instance.health = PlayerTracker.instance.maxHealth;

        UIController.instance.UpdateHealth(PlayerTracker.instance.health, PlayerTracker.instance.maxHealth);
        PlayerTracker.instance.lightAmount += lightReward;
        UIController.instance.UpdateLightHud();

        Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    public void Execute()
    {
        /*
        if(!PlayerTracker.instance.unlockedSouls[enemyId+1]) //unlock mushroom stance if not unlocked
        {
            PlayerTracker.instance.unlockedSouls[enemyId+1] = true;
            UIController.instance.ShowPopup(soulUnlockMessage);
        }
        */

        //if(!PlayerTracker.instance.mushroomSoulUnlocks[0]) //unlock mushroom stance skill 0 if not unlocked
        //{
        //tracker.mushroomSoulUnlocks[0] = true;
        //UIController.instance.ShowPopup(abilityUnlockMessage);   !!!  USE  A  GETTER METHOD TO GET  THE  ASSOCIATED  UNLOCK TREE  LIKE  ONMOUSEOVER  STUFF, THEN  CHECK FOR THAT ARRAY
        //}

        PlayerTracker.instance.health += 2*healOnKill;
        if(PlayerTracker.instance.health > PlayerTracker.instance.maxHealth)
        PlayerTracker.instance.health = PlayerTracker.instance.maxHealth;

        PlayerTracker.instance.energy += healOnKill;
        if(PlayerTracker.instance.energy > PlayerTracker.instance.maxEnergy)
        PlayerTracker.instance.energy = PlayerTracker.instance.maxEnergy;

        UIController.instance.UpdateHealth(PlayerTracker.instance.health, PlayerTracker.instance.maxHealth);
        UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);
        PlayerTracker.instance.lightAmount += lightReward;
        UIController.instance.UpdateLightHud();

        Destroy(gameObject);
    }

    public void PlaySoundInAnimation(int sfxIndex)
    {
        AudioManager.instance.PlaySFXPitchRandomized(sfxIndex);
    }
}