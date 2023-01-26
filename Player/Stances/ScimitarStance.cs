using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScimitarStance : MonoBehaviour
{
    private float timeAtAnim;
    private float damage;
    [SerializeField] float storedInput = 0.45f;
    [SerializeField] BoxCollider2D hitBox;
    [SerializeField] ParticleSystem hitEffect;

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
            hitBox.offset = new Vector2(-0.3f, 0f);
            hitBox.size = new Vector2(3.5f, 2f);
        }

        else if(PlayerController.instance.lightAttack2)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0.1f, 0.45f);
            hitBox.size = new Vector2(3.5f, 2f);
        }

        else if(PlayerController.instance.lightAttack3)
        {
            hitBox.enabled = true;
            hitBox.offset = new Vector2(0.5f, 0.6f);
            hitBox.size = new Vector2(3f, 2.8f);
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
                PlayerTracker.instance.energy += 2;
                if(PlayerTracker.instance.energy > PlayerTracker.instance.maxEnergy)
                PlayerTracker.instance.energy = PlayerTracker.instance.maxEnergy;

                UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);

                damage = 10f + (2f * PlayerTracker.instance.dexterity); //2 damage per dex

                if(PlayerTracker.instance.scimitarStanceUnlocks[0]) //1 added damage per dex if unlocked
                damage += 1f * PlayerTracker.instance.dexterity;

                other.GetComponent<EnemyHealthController>().DamageEnemy(damage, 10, 0, transform.position, 4); //stagger damage is static
            }
            
            else if(PlayerController.instance.lightAttack2)
            {
                PlayerTracker.instance.energy += 2;
                if(PlayerTracker.instance.energy > PlayerTracker.instance.maxEnergy)
                PlayerTracker.instance.energy = PlayerTracker.instance.maxEnergy;

                UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);

                damage = 10f + (2f * PlayerTracker.instance.dexterity);

                if(PlayerTracker.instance.scimitarStanceUnlocks[0]) //1 added damage per dex if unlocked
                damage += 1f * PlayerTracker.instance.dexterity;

                other.GetComponent<EnemyHealthController>().DamageEnemy(damage, 10, 0, transform.position, 4);
            }
            
            else if(PlayerController.instance.lightAttack3)
            {
                PlayerTracker.instance.energy += 3;
                if(PlayerTracker.instance.energy > PlayerTracker.instance.maxEnergy)
                PlayerTracker.instance.energy = PlayerTracker.instance.maxEnergy;

                UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);
                
                damage = 15f + (2.5f * PlayerTracker.instance.dexterity); //2.5 damage per dex

                if(PlayerTracker.instance.scimitarStanceUnlocks[0]) //1.5 added damage per dex if unlocked
                damage += 1.5f * PlayerTracker.instance.dexterity;

                other.GetComponent<EnemyHealthController>().DamageEnemy(damage, 15, 0, transform.position, 4);
            }
        }
    }
}
