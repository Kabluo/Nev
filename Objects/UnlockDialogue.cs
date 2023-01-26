using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDialogue : MonoBehaviour
{
    [SerializeField] GameObject interactablePopup;
    private Animator animator;
    [SerializeField] int unlockType = 0; //0 = stance, 1 = soul, might expand later like 011 = axe stance first skill (0, stanceId, stanceSkillId)
    [SerializeField] int unlockId = 0;
    [SerializeField] int unlockCost = 80;
    private int unlockSoundIndex;

    [SerializeField] List<Sprite> portrait = new List<Sprite>(); //list of images present in dialogue
    [SerializeField] List<int> dialoguePortraitIndex = new List<int>(); //index of image to be received at certain index points
    [SerializeField] List<string> lines = new List<string>();
    [SerializeField] float textSpeed;

    [SerializeField] ParticleSystem unlockEffect;

    private int index;
    private bool listening;
    private bool inDialogue;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        if(unlockType != 100) //anvils are shared unlock objects for every stance
        {
            StartCoroutine(CheckIfUnlocked());
            unlockSoundIndex = 42; //might change to unique sounds per-unlock later on
        }

        else
        unlockSoundIndex = 26;
    }

    void Update()
    {
        if(listening)
        {
            listening = UIController.instance.dialogueScreen.activeSelf;
            if(UIController.instance.listeningForUnlock)
            {
                UIController.instance.listeningForUnlock = false;

                /*  implement later, player unlocking animation
                if(unlockType == 0)
                PlayerController.instance.myAnimator.SetTrigger("hasUnlockedStance");

                if(unlockType == 1)
                PlayerController.instance.myAnimator.SetTrigger("hasUnlockedSoul");
                */

                Debug.Log(this.unlockType);

                if(this.unlockType != 100)
                {
                    TriggerUnlock();
                }

                else
                {
                    animator.SetTrigger("isActivated");
                }

                TriggerSound();
            }
        }

        if(!inDialogue) { return; }

        else //while in dialogue, listen if gameobject is disabled through esc key or button press
        {
            inDialogue = UIController.instance.dialogueScreen.activeSelf;
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            inDialogue = false;
            UIController.instance.DialogueMenu();
        }

        if(Input.GetMouseButtonDown(0) && !listening)
        {
            if(UIController.instance.textField.text == lines[index])
            NextLine();

            else
            {
                StopAllCoroutines();
                UIController.instance.textField.text = lines[index];
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        interactablePopup.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        interactablePopup.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player" && !PlayerController.instance.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("PreventMovement") && !PlayerController.instance.isInScene)
        {
            if(Input.GetKey(KeyCode.E)) //getkeydown is somewhat unresponsive
            {
                UIController.instance.textField.text = string.Empty;
                PlayerController.instance.myAnimator.SetFloat("speed", 0f); //stops walking/running animation
                UIController.instance.DialogueMenu();
                inDialogue = true;

                if(unlockType == 100) //if is a stance skill
                UIController.instance.unlockType = this.unlockType + PlayerTracker.instance.equippedStances[PlayerTracker.instance.activeStanceIndex];
                
                //else if(unlockType == 200)
                //UIController.instance.unlockType = unlockType += PlayerTracker.instance.equippedSouls[PlayerTracker.instance.activeSoulIndex];

                else
                UIController.instance.unlockType = this.unlockType;

                UIController.instance.unlockId = this.unlockId;
                UIController.instance.unlockCost = this.unlockCost;
                StartDialogue();
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(StartDialogueCO());
    }

    IEnumerator StartDialogueCO()
    {
        UIController.instance.portraitContainer.sprite = portrait[dialoguePortraitIndex[index]];
        foreach(char c in lines[index].ToCharArray())
        {
            UIController.instance.textField.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if(index < lines.Count-1)
        {
            index++;
            UIController.instance.textField.text = string.Empty;
            StartCoroutine(StartDialogueCO());
        }
        else
        {
            UIController.instance.dialogueUnlockContainer.SetActive(true); //activate choices
            listening = true;
            /*
            inDialogue = false;
            UIController.instance.DialogueMenu();
            */
        }
    }

    IEnumerator CheckIfUnlocked() //also should scale with unlocktype
    {
        yield return new WaitForSeconds(.1f);

        if(unlockType == 0 && PlayerTracker.instance.unlockedStances[unlockId]) //stance
        TriggerUnlock();

        if(unlockType == 1 && PlayerTracker.instance.unlockedSouls[unlockId]) //soul
        TriggerUnlock();

        if(unlockType == 200 && PlayerTracker.instance.eldritchSoulUnlocks[unlockId]) //eldritch soul
        TriggerUnlock();

        if(unlockType == 201 && PlayerTracker.instance.mushroomSoulUnlocks[unlockId]) //mushroom soul
        TriggerUnlock();
        
    }

    private void TriggerUnlock()
    {
        animator.SetTrigger("isUnlocked");
        Instantiate(unlockEffect, transform.position, Quaternion.identity);
        interactablePopup.SetActive(false);
        Destroy(this); //destroy script component
    }

    private void TriggerSound()
    {
        AudioManager.instance.PlaySFXPitchRandomized(unlockSoundIndex);
    }
}
