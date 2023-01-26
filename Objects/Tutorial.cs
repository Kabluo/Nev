using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] int tutorialId;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if(PlayerTracker.instance.tutorialSeen[tutorialId])
            Destroy(gameObject);

            else
            {
                PlayerTracker.instance.tutorialSeen[tutorialId] = true;
                UIController.instance.ShowTutorial(tutorialId);
                Destroy(gameObject);
            }
            
        }
    }
}
