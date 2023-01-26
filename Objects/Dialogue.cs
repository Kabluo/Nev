using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI textField;
    //[SerializeField] Image portraitContainer;
    [SerializeField] List<Sprite> portrait = new List<Sprite>(); //list of images present in dialogue
    [SerializeField] List<int> dialoguePortraitIndex = new List<int>(); //index of image to be received at certain index points
    [SerializeField] List<string> lines = new List<string>();
    [SerializeField] float textSpeed;
    
    private int index;

    /*
    [SerializeField] int dialogueId; - might have a  system  later  where the progress of some  dialogue might be stored in savedata  through  player tracker,
    would work like this:
    if(id == -1) return
    else check a bool or integer list (if dialogue has multiple states) in PlayerTracker
    update the list through dialogue or other methods (if id != -1)
    save said  list  in savegame
    not needed now
    */

    private bool inDialogue;

    void Update()
    {
        if(!inDialogue) { return; }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            inDialogue = false;
            UIController.instance.DialogueMenu();
        }

        if(Input.GetMouseButtonDown(0))
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
            inDialogue = false;
            UIController.instance.DialogueMenu();
        }
    }
}
