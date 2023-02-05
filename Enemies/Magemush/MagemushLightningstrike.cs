using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagemushLightningstrike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(PlayerController.instance.isBroken)
            {
                PlayerController.instance.ExecutePlayer(102, transform);
            }

            else
                PlayerController.instance.DamagePlayer(45, 65, 4, transform, false, 102, 7);
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
