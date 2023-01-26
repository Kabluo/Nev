using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string newGameScene;
    [SerializeField] Button newGameButton;
    [SerializeField] Button continueGameButton;
    [SerializeField] Button loadGameButton;

    [SerializeField] SaveSlotsMenu saveSlotsMenu;

    [SerializeField] GameObject mainMenuScreen;
    [SerializeField] GameObject optionsButton;
    [SerializeField] GameObject optionsScreen;
    [SerializeField] GameObject backButton;

    void Start()
    {
        optionsScreen.GetComponent<OptionsMenu>().LoadPrefs();
        Time.timeScale = 1f; //reset timescale
        AudioManager.instance.PlayMusic(0);

        if(!DataPersistanceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
            continueGameButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(100, 100, 100, 255);
            loadGameButton.interactable = false;
            loadGameButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(100, 100, 100, 255);
        }

        DataPersistanceManager.instance.firstTimeLoading = true; //reset firsttimeloading upon returning to menu
    }

    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu(false);
    }

    public void OnLoadGameClicked()
    {
        saveSlotsMenu.ActivateMenu(true);
    }

    public void OnContinueGameClicked()
    {
        SceneManager.LoadSceneAsync(DataPersistanceManager.instance.GetProfileLevelName());
    }

    public void MainOptionsSwap()
    {
        if(mainMenuScreen.activeSelf)
        {
            mainMenuScreen.SetActive(false);
            optionsScreen.SetActive(true);
            SetHighlightedOnNewMenu(backButton);
        }
        else
        {
            mainMenuScreen.SetActive(true);
            optionsScreen.SetActive(false);
            SetHighlightedOnNewMenu(optionsButton);
        }
    }

    void SetHighlightedOnNewMenu(GameObject gameObject)
    {
        StartCoroutine(SetHighlightedOnNewMenuCO(gameObject));
    }

    IEnumerator SetHighlightedOnNewMenuCO(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void QuitGame()
    {
        Application.Quit(); //gets caught in editor

        Debug.Log("Game Quit");
    }
}
