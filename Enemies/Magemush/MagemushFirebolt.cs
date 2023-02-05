using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagemushFirebolt : MonoBehaviour
{
    Rigidbody2D bulletRigidbody;
    Vector3 originPosition;
    [SerializeField] float bulletSpeed = 35f;
    [SerializeField] ParticleSystem ps;
    [SerializeField] ParticleSystem hitEffect;

    private float direction;
    private float timeAtCreation;
    private float damageAmount;
    [SerializeField] float bulletLifetime = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.position;
        timeAtCreation = Time.time;
        bulletRigidbody = GetComponent<Rigidbody2D>();
        direction = Mathf.Sign(PlayerController.instance.transform.position.x - transform.position.x);
    }

    // Update is called once per frame
    void Update()
    {
        bulletRigidbody.velocity = new Vector2 (direction * bulletSpeed, bulletRigidbody.velocity.y);
        if(Time.time > timeAtCreation + bulletLifetime) //destroy if lifetime passes
        DestroyBullet();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(103, transform);
            }

            else
                PlayerController.instance.DamagePlayer(30, 40, 1, transform, false, 103, 44);
        }
        
        AudioManager.instance.PlaySFXPitchRandomized(44);

        DestroyBullet();
    }

    void DestroyBullet()
    {
        ps.transform.parent = null; //detach from parent
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting); //stop emissions, has Destroy on stop enabled on object
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
        //instigate an explosion particle effect that has destroy on stopping, maybe
    }
}
