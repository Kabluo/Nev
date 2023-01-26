using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossfightReward : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int bossId;

    public void GiveReward()
    {
        PlayerTracker.instance.bossesDefeated[bossId] = true;
        
        if(bossId == 0) //minimush elite
        {
            PlayerTracker.instance.lightAmount += 440; //gives 440 light
            //unlock souls, stances, skills or whatnot through here
        }
    }
}
