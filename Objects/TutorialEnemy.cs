using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    [SerializeField] int tutorialId;
    [SerializeField] EnemyHealthController healthController;

    void Update()
    {
        if(healthController.isBroken && !PlayerTracker.instance.tutorialSeen[tutorialId])
        {
            PlayerTracker.instance.tutorialSeen[tutorialId] = true;
            UIController.instance.ShowTutorial(tutorialId);
        }
    }
}
