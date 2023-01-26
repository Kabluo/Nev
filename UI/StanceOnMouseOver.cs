using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StanceOnMouseOver : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] int buttonIndex;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descText;
    private List<bool> stanceUnlocks;

    void Start()
    {
        GetStance();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DisplayInfo();
    }

    public void OnSelect(BaseEventData eventData)
    {
        DisplayInfo();
    }

    void DisplayInfo()
    {
        if(buttonIndex >= PlayerTracker.instance.stanceObjects.Count || !PlayerTracker.instance.unlockedStances[buttonIndex]) { return; }

        for(int i = 0; i < UIController.instance.stanceSkillContainers.Count; i++) //resets containers upon mouseovering on a new object, just keeps information on containers otherwise
        {
            UIController.instance.stanceSkillContainers[i].SetActive(false);
        }

        int tracker = -1;
        nameText.text = UIController.instance.GetStanceSOByIndex(buttonIndex).GetStanceName();
        descText.text = UIController.instance.GetStanceSOByIndex(buttonIndex).GetStanceDescription(0);

        for(int i = 0; i < UIController.instance.stanceSkillContainers.Count; i++)
        {
            if(tracker < stanceUnlocks.Count)
            {
                for(int j = tracker + 1; j < stanceUnlocks.Count; j++)
                {
                    if(stanceUnlocks[j])
                    {
                        UIController.instance.stanceSkillContainers[i].SetActive(true);
                        UIController.instance.stanceSkillText[i].text = UIController.instance.GetStanceSOByIndex(buttonIndex).GetStanceDescription(j+1);
                        tracker=j;
                        break;
                    }

                    UIController.instance.stanceSkillContainers[i].SetActive(false);
                }
            }
        }
    }

    void GetStance()
    {
        if(buttonIndex == 0)
            stanceUnlocks = PlayerTracker.instance.scimitarStanceUnlocks;
        
        if(buttonIndex == 1)
            stanceUnlocks = PlayerTracker.instance.axeStanceUnlocks;
        
        if(buttonIndex == 2)
            stanceUnlocks = PlayerTracker.instance.dualBladeStanceUnlocks;

        if(buttonIndex == 3)
            stanceUnlocks = PlayerTracker.instance.hammerStanceUnlocks;

        if(buttonIndex == 4)
            stanceUnlocks = PlayerTracker.instance.rapierStanceUnlocks;

        if(buttonIndex == 5)
            stanceUnlocks = PlayerTracker.instance.greatswordStanceUnlocks;

        if(buttonIndex == 6)
            stanceUnlocks = PlayerTracker.instance.katanaStanceUnlocks;

        if(buttonIndex == 7)
            stanceUnlocks = PlayerTracker.instance.guardianStanceUnlocks;

        if(buttonIndex == 8)
            stanceUnlocks = PlayerTracker.instance.bowStanceUnlocks;

        if(buttonIndex == 9)
            stanceUnlocks = PlayerTracker.instance.beastlyStanceUnlocks;

        if(buttonIndex == 10)
            stanceUnlocks = PlayerTracker.instance.monstrousStanceUnlocks;

        //add if more stances get addedd
    }
}
