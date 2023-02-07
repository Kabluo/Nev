using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushgrowthSpike : MonoBehaviour
{
    private Animator anim;
    private int spikeType = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        spikeType = Random.Range(0, 2);
        anim.SetInteger("spikeType", spikeType);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(105, transform);
                DespawnBullet();
            }

            else
                PlayerController.instance.DamagePlayer(45, 65, 0, transform, false, 105, 7);
        }
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
