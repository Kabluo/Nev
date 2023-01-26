using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossfightHealthbar : MonoBehaviour
{
    [SerializeField] GameObject bossfight;
    [SerializeField] EnemyHealthController bossHealthController;
    [SerializeField] BossfightReward rewardController;
    [SerializeField] Slider healthbar;
    private int checkForChange;

    void Start()
    {
        healthbar.maxValue = bossHealthController.currentHealth; //might change to maxhealth later, shouldn't be necessary however
        healthbar.value = bossHealthController.currentHealth;
        checkForChange = bossHealthController.currentHealth;
    }

    void Update()
    {
        if(checkForChange != bossHealthController.currentHealth)
        {
            checkForChange = bossHealthController.currentHealth;
            healthbar.value = checkForChange;

            if(checkForChange <= 0) //executing isn't required, breaking the boss ends encounter
            {
                rewardController.GiveReward();
                AudioManager.instance.PlayMusic(FindObjectOfType<AudioObject>().musicIndex); //play old music
                Destroy(bossfight);
            }
        }
    }
}
