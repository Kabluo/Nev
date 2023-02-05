using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushEliteLightningTrap : MonoBehaviour
{
    private bool isActivated = false;
    [SerializeField] float lifeTime = 8f; //8 seconds lifetime by default
    private bool playerInArea = false;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(!isActivated)
        lifeTime -= Time.deltaTime;

        if(lifeTime <= 0)
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            isActivated = true; //stops countdown too
            anim.SetTrigger("isActivated");
            playerInArea = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            playerInArea = false;
        }
    }

    public void DamagePlayerIfInArea() //called through animator at specific frames
    {
        if(!playerInArea) { return; }

        if(PlayerController.instance.isBroken)
            PlayerController.instance.ExecutePlayer(101, transform);

        else
            PlayerController.instance.DamagePlayer(45, 100, 4, transform, false, 101, 7);
    }

    public void DespawnBullet()
    {
        Destroy(gameObject);
    }

    void PlaySoundInAnimation(int index)
    {
        AudioManager.instance.PlaySFXPitchRandomized(index);
    }
}
