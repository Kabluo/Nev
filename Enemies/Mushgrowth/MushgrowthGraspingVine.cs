using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushgrowthGraspingVine : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerController.instance.DamagePlayer(20, 0, 4, transform, true, 104, 7);
            DespawnBullet();
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
