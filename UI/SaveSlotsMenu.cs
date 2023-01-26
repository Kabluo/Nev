using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotsMenu : MonoBehaviour
{
    [SerializeField] string newGameScene = "Test 0";
    private SaveSlot[] saveSlots;
    private bool isLoadingGame = false;

    void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        DataPersistanceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if(!isLoadingGame)
        {
            DataPersistanceManager.instance.NewGame();
            SceneManager.LoadSceneAsync(newGameScene);
            return;
        }

        else
        SceneManager.LoadSceneAsync(DataPersistanceManager.instance.GetProfileLevelName());
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        this.gameObject.SetActive(true);

        this.isLoadingGame = isLoadingGame;

        Dictionary<string, GameData> profilesGameData = DataPersistanceManager.instance.GetAllProfilesGameData();

        foreach(SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if(profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }
}
