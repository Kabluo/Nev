using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpse : MonoBehaviour
{
    private Animator corpseAnimator;
    private int corpseId = 0;
    // Start is called before the first frame update
    void Awake()
    {
        corpseAnimator = GetComponent<Animator>();
        corpseId = PlayerTracker.instance.corpseTracker;
    }

    void Start()
    {
        //PlayerTracker.instance.corpseTracker++;
        gameObject.transform.localScale = new Vector3(PlayerTracker.instance.corpseRotation[corpseId], 1f, 1f);
        GetComponent<SpriteRenderer>().sortingOrder = corpseId;

        /*
        if light > 0, inst particle, destroy upon claiming light
        */

        if(PlayerTracker.instance.corpseLight[corpseId] == 0)
        GetComponent<SpriteRenderer>().color = new Color32(155, 155, 155, 255);

        else
        GetComponent<SpriteRenderer>().sortingOrder += 10000; //later change to a different layer that goes over the corpse layer, also might make corpses their own seperate layer as well
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" || other.tag == "Enemy")
        {
            corpseAnimator.SetTrigger("isSteppedOn");
        }

        if(other.tag == "EnemyAttack")
        {
            corpseAnimator.SetTrigger("isHit");
        }

        if(other.tag == "PlayerAttack")
        {
            corpseAnimator.SetTrigger("isHit");
            if(PlayerTracker.instance.corpseLight[corpseId] > 0)
            {
                Instantiate(PlayerController.instance.brokenEffect, transform.position, Quaternion.Euler(90, 0, 0)); //inst player broken effect
                PlayerTracker.instance.lightAmount += PlayerTracker.instance.corpseLight[corpseId];
                PlayerTracker.instance.corpseLight[corpseId] = 0;
                GetComponent<SpriteRenderer>().color = new Color32(155, 155, 155, 255);
                UIController.instance.UpdateLightHud();
            }
        }
    }
}
