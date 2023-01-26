using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] string profileId = "";

    [Header("Content")]
    [SerializeField] GameObject noDataContent;
    [SerializeField] GameObject hasDataContent;
    [SerializeField] TextMeshProUGUI infoText;

    private Button saveSlotButton;

    void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        if(data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            infoText.text = data.levelName + " : " + profileId;
        }
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    public void SetInteractable(bool isInteractable)
    {
        saveSlotButton.interactable = isInteractable;
    }
}
