using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeStance : MonoBehaviour
{
    private float timeAtAnim;
    private float damage;
    [SerializeField] float storedInput = 0.65f;
    [SerializeField] BoxCollider2D hitBox;
    [SerializeField] ParticleSystem hitEffect;
    //souls and stances should receive Name, Image and Descrption 1,2,3... variables for canvas display and manipulation
    //also upgrades should be made through PlayerTracker, if mushroomSoulMageSoulObtained = true, then run x function and display x description etc.
    //add energyOnHit
    void Start()
    {
        PlayerController.instance.swappingStance = false;
    }

    void Update()
    {
        if(PlayerController.instance.swappingStance)
        Destroy(gameObject);
        
        if(Time.timeScale == 0f || !PlayerController.instance.isAlive || PlayerController.instance.isInScene) { return; }

        Attack();
        AttackFrames();
    }

    void Attack()
    {
        if(Input.GetButtonDown("lightAttack") && PlayerController.instance.isOnGround && !PlayerController.instance.isSoulAttacking)
        {
            timeAtAnim = Time.time;
            PlayerController.instance.isLightAttacking = true;
        }
        
        if(Time.time > timeAtAnim + storedInput)
        {
            PlayerController.instance.isLightAttacking = false;
        }

        PlayerController.instance.myAnimator.SetBool("isLightAttacking", PlayerController.instance.isLightAttacking);
    }

    void AttackFrames()
    {
        if(PlayerController.instance.lightAttack1)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0.7f, 0.4f);
            hitBox.size = new Vector2(2.8f, 2.8f);
        }

        else if(PlayerController.instance.lightAttack2)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0.7f, 0.4f);
            hitBox.size = new Vector2(2.8f, 2.8f);
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
            //if(other.GetComponent<EnemyHealthController>().isBroken) do through interaction
            //{
            //    PlayerController.instance.myAnimator.SetTrigger("isExecuting");
            //    PlayerController.instance.myAnimator.SetInteger("executionId", other.GetComponent<EnemyHealthController>().enemyId);
            //    other.GetComponent<EnemyHealthController>().Execute();
            //}

            if(PlayerController.instance.lightAttack1)
            {
                PlayerTracker.instance.energy += 8;
                if(PlayerTracker.instance.energy > PlayerTracker.instance.maxEnergy)
                PlayerTracker.instance.energy = PlayerTracker.instance.maxEnergy;

                UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);

                damage = 40f + (3f * PlayerTracker.instance.strength);

                if(PlayerTracker.instance.axeStanceUnlocks[0])
                damage += 1f * PlayerTracker.instance.strength;

                other.GetComponent<EnemyHealthController>().DamageEnemy(damage, 30, 0, transform.position, 4); //stagger damage is static
            }
            
            else if(PlayerController.instance.lightAttack2)
            {
                PlayerTracker.instance.energy += 8;
                if(PlayerTracker.instance.energy > PlayerTracker.instance.maxEnergy)
                PlayerTracker.instance.energy = PlayerTracker.instance.maxEnergy;

                UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);

                damage = 40f + (3f * PlayerTracker.instance.strength);

                if(PlayerTracker.instance.axeStanceUnlocks[0])
                damage += 1f * PlayerTracker.instance.strength;

                other.GetComponent<EnemyHealthController>().DamageEnemy(damage, 30, 0, transform.position, 4);
            }
        }
    }
}
