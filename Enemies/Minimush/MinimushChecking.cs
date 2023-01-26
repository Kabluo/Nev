using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimushChecking : MonoBehaviour
{
    private MinimushController controller;

    void Update()
    {
        if(GetComponentInParent<EnemyHealthController>().triggeredByDamage)
        ActivateEnemy();
    }

    void Start()
    {
        controller = GetComponentInParent<MinimushController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            ActivateEnemy();
        }
    }

    void ActivateEnemy()
    {
        controller.isChasing = true;
        controller.minChaseTime = controller.minChaseTimeBase;
        this.gameObject.SetActive(false); //disable self afterwards
    }
}
