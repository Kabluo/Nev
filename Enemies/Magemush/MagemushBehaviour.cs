using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagemushBehaviour : MonoBehaviour
{
    [SerializeField] EnemyHealthController healthController;
    [SerializeField] EnemyController controller;
    [SerializeField] BoxCollider2D hitBox;
    [SerializeField] float teleportCooldown = 20f;
    [SerializeField] Transform[] teleportPoints;
    private float teleportTimer = 0f;
    [SerializeField] GameObject lightningStrike;
    [SerializeField] float lightningStrikeCooldown = 15f;
    private float lightningStrikeTimer = 0f;
    [SerializeField] GameObject fireball;
    [SerializeField] float fireballCooldown = 3f;
    private float fireballTimer;
    [SerializeField] float fireballAngleLimit = 1f;
    private int attackId;
    public bool attackSmash;
    public bool attackFirebolt;
    public bool attackLightningstrike;
    public bool attackTeleport; //does force stagger damage
    //[SerializeField] Vector2 grabAttackOffset;

    void Start()
    {
        foreach(Transform tPoint in teleportPoints)
            tPoint.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0 || healthController.isBroken) { return; }

        if(teleportTimer > 0)
            teleportTimer -= Time.deltaTime;
        
        if(lightningStrikeTimer > 0)
            lightningStrikeTimer -= Time.deltaTime;

        if(fireballTimer > 0)
            fireballTimer -= Time.deltaTime;

        if(controller.onStandby)
        {
            EndStandby();
            return;
        }
        
        Attack();
        AttackFrames();
    }

    void Attack()
    {
        if(controller.inRange)
        {
            if(teleportTimer <= 0)
            {
                attackId = 3;
                controller.animator.SetInteger("attackId", attackId);
                controller.animator.SetBool("isAttacking", true);  
            }

            else
            {
                attackId = 0; //smash attack
                controller.animator.SetInteger("attackId", attackId);
                controller.animator.SetBool("isAttacking", true);  
            } 
        }
        
        else if(!Physics2D.Linecast(transform.position, PlayerController.instance.transform.position, controller.whatIsGround)
        && Mathf.Abs(PlayerController.instance.transform.position.y - transform.position.y) <= fireballAngleLimit && controller.isChasing
        && fireballTimer <= 0)
        {
            if(transform.position.x < PlayerController.instance.gameObject.transform.position.x) //turn when casting
            {
                controller.gameObject.transform.localScale = new Vector3 (-1f, 1f, 1f);
            }
            else
                controller.gameObject.transform.localScale = Vector3.one;

            attackId = 1; //fireball
            controller.animator.SetInteger("attackId", attackId);
            controller.animator.SetBool("isAttacking", true);
        }

        else if(lightningStrikeTimer <= 0 && controller.isChasing)
        {
            attackId = 2; //lightning strike
            controller.animator.SetInteger("attackId", attackId);
            controller.animator.SetBool("isAttacking", true);
        }

        else
            controller.animator.SetBool("isAttacking", false);
    }

    void AttackFrames()
    {
        if(attackSmash)
        {
            hitBox.offset = new Vector2(-1.4f, 0.3f);
            hitBox.size = new Vector2(2.3f, 2.7f);
            hitBox.enabled = true;
        }

        else if(attackFirebolt)
        {
            fireballTimer = fireballCooldown;
            hitBox.offset = new Vector2(-0.8f, 0.1f);
            hitBox.size = new Vector2(1.8f, 1.5f);
            hitBox.enabled = true;
        }

        else if(attackLightningstrike)
        {
            lightningStrikeTimer = lightningStrikeCooldown;

            hitBox.offset = new Vector2(-0.4f, 2.5f);
            hitBox.size = new Vector2(2.4f, 7f);
            hitBox.enabled = true;
        }

        else if(attackTeleport)
        {
            teleportTimer = teleportCooldown;
            
            hitBox.offset = new Vector2(-0.15f, 0.2f);
            hitBox.size = new Vector2(1.6f, 2.5f);
            hitBox.enabled = true;
        }

        else
        {
            hitBox.enabled = false;
            return;
        }
    }

    void EndStandby()
    {
        if(PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && controller.animator.GetCurrentAnimatorStateInfo(0).IsName("Standby"))
        {
            controller.animator.SetBool("onStandby", false);
            controller.gameObject.layer = LayerMask.NameToLayer("Enemy"); //just doing gameObject updates the child's layer
            controller.onStandby = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && attackSmash)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(4, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(20, 60, 5, controller.gameObject.transform, false, 4, 7);
        }

        else if(other.tag == "Player" && attackFirebolt)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(4, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(20, 40, 1, controller.gameObject.transform, false, 4, 44);
        }

        else if(other.tag == "Player" && attackLightningstrike)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(4, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(20, 60, 3, controller.gameObject.transform, false, 4, 37);
        }

        else if(other.tag == "Player" && attackTeleport)
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(4, controller.gameObject.transform);
                controller.ManageStandby(new Vector2(0f, 0f));
            }

            else
                PlayerController.instance.DamagePlayer(0, 50, 5, controller.gameObject.transform, false, 4, 40);
        }
    }

    //called through animator
    public void InstFireball()
    {
        Instantiate(fireball, transform.position, transform.rotation);
    }

    public void CallLightning()
    {
        Instantiate(lightningStrike, PlayerController.instance.lastOnGround, Quaternion.identity);
    }

    public void PerformTeleport()
    {
        Vector2 pointToTeleport;

        if(teleportPoints.Length == 0)
        {
            pointToTeleport = transform.position;
        }

        else
        {
            pointToTeleport = teleportPoints[0].position;

            for(int i = 1; i < teleportPoints.Length; i++)
            {
                if(Vector2.Distance(teleportPoints[i].position, PlayerController.instance.transform.position) > Vector2.Distance(pointToTeleport, PlayerController.instance.transform.position))
                    pointToTeleport = teleportPoints[i].position;
            }

            controller.gameObject.transform.position = pointToTeleport;
        }
    }
}
