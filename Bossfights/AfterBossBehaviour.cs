using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterBossBehaviour : MonoBehaviour
{
    [SerializeField] int bossId = 0;
    [SerializeField] GameObject objectToTrigger;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerTracker.instance.bossesDefeated[bossId])
        objectToTrigger.SetActive(true);
    }
}
