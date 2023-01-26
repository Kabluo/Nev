using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushEliteLightningstrikeVariant : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().DamagePlayer(85, 0, 4, transform, true, 2, 7);
            FindObjectOfType<MinimushEliteBehaviour>().grabbedNev = true;
            FindObjectOfType<MinimushEliteBehaviour>().ManageStandby();
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
