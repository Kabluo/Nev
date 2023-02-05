using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushEliteLightningstrike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(100, transform);
            }

            else
                PlayerController.instance.DamagePlayer(45, 100, 4, transform, false, 100, 7);
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
