using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoulOnMouseOver : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] int buttonIndex;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descText;
    private List<bool> soulUnlocks;

    // !!! THE BOTTOM PART DOESN'T SHOW UPDATED STATS, ADD A TEXT SAYING STUFF LIKE "EQUIPPED SOUL MODIFIERS" !!! //
    // MIGHT BE BUGGED, HAVEN'T TESTED YET //

    void Start()
    {
        GetSoul();
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
        if(buttonIndex >= PlayerTracker.instance.soulObjects.Count || !PlayerTracker.instance.unlockedSouls[buttonIndex]) { return; }

        for(int i = 0; i < UIController.instance.soulSkillContainers.Count; i++) //resets containers upon mouseovering on a new object, just keeps information on containers otherwise
        {
            UIController.instance.soulSkillContainers[i].SetActive(false);
        }

        int tracker = -1;
        nameText.text = UIController.instance.GetSoulSOByIndex(buttonIndex).GetSoulName();
        descText.text = UIController.instance.GetSoulSOByIndex(buttonIndex).GetSoulDescription(0);

        for(int i = 0; i < UIController.instance.soulSkillContainers.Count; i++)
        {
            if(tracker < soulUnlocks.Count)
            {
                for(int j = tracker + 1; j < soulUnlocks.Count; j++)
                {
                    if(soulUnlocks[j])
                    {
                        UIController.instance.soulSkillContainers[i].SetActive(true);
                        UIController.instance.soulSkillText[i].text = UIController.instance.GetSoulSOByIndex(buttonIndex).GetSoulDescription(j+1);
                        tracker=j;
                        break;
                    }

                    UIController.instance.soulSkillContainers[i].SetActive(false);
                }
            }
        }
    }

    void GetSoul()
    {
        if(buttonIndex == 0)
            soulUnlocks = PlayerTracker.instance.eldritchSoulUnlocks;
        
        if(buttonIndex == 1)
            soulUnlocks = PlayerTracker.instance.mushroomSoulUnlocks;
        
        if(buttonIndex == 2)
            soulUnlocks = PlayerTracker.instance.humanSoulUnlocks;

        if(buttonIndex == 3)
            soulUnlocks = PlayerTracker.instance.machineSoulUnlocks;

        if(buttonIndex == 4)
            soulUnlocks = PlayerTracker.instance.sectolSoulUnlocks;

        if(buttonIndex == 5)
            soulUnlocks = PlayerTracker.instance.forgottenSoulUnlocks;

        if(buttonIndex == 6)
            soulUnlocks = PlayerTracker.instance.queenSoulUnlocks;

        if(buttonIndex == 7)
            soulUnlocks = PlayerTracker.instance.broodmotherSoulUnlocks;

        if(buttonIndex == 8)
            soulUnlocks = PlayerTracker.instance.draconicSoulUnlocks;

        if(buttonIndex == 9)
            soulUnlocks = PlayerTracker.instance.angelicSoulUnlocks;

        if(buttonIndex == 10)
            soulUnlocks = PlayerTracker.instance.demonicSoulUnlocks;

        //add if more souls get addedd
    }
}
