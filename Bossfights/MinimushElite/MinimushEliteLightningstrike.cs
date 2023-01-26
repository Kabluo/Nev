using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushEliteLightningstrike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerController.instance.DamagePlayer(45, 100, 4, transform, false, 101, 7);
            
            if(other.GetComponent<PlayerController>().isBroken) //if attack kills, perform execution directly
            other.GetComponent<PlayerController>().ExecutePlayer(100, transform);
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
