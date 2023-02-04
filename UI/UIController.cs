using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //gameobject attached to this script is not destroyed on scene change
        }

        else //if there is already a player, destroy the new  one / don't create a new one
        Destroy(this.gameObject);
    } //throws a warning due to multiple eventsystems being active for a brief moment, safe to ignore

    [Header("Pause Menu Items")]
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject optionsScreen;
    [SerializeField] GameObject mainMenuButton; //for highlighting
    [SerializeField] GameObject optionsMenuBackButton;
    [SerializeField] List<GameObject> pauseMenuEquippedStances = new List<GameObject>();
    [SerializeField] List<GameObject> pauseMenuEquippedSouls = new List<GameObject>();
    [SerializeField] TextMeshProUGUI strText;
    [SerializeField] TextMeshProUGUI dexText;
    [SerializeField] TextMeshProUGUI conText;
    [SerializeField] TextMeshProUGUI magText;
    [SerializeField] TextMeshProUGUI physAffinity;
    [SerializeField] TextMeshProUGUI fireAffinity;
    [SerializeField] TextMeshProUGUI coldAffinity;
    [SerializeField] TextMeshProUGUI lgtnAffinity;
    [SerializeField] TextMeshProUGUI poisAffinity;
    [SerializeField] TextMeshProUGUI forcAffinity;
    [SerializeField] TextMeshProUGUI psycAffinity;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI energyText;
    [SerializeField] string mainMenuScene;
    private Sprite tempSprite;

    [Header("Stance Menu Items")]
    [SerializeField] GameObject stanceScreen;
    [SerializeField] List<GameObject> equippedStanceContainers = new List<GameObject>();
    [SerializeField] List<GameObject> stanceContainers = new List<GameObject>();
    public List<GameObject> stanceSkillContainers = new List<GameObject>();
    public List<TextMeshProUGUI> stanceSkillText = new List<TextMeshProUGUI>();
    [SerializeField] List<StanceSO> stanceScriptableObjects = new List<StanceSO>();
    private int swappingStanceList = -1;
    private int swappingStanceEquipped = -1;

    [Header("Soul Menu Items")]
    [SerializeField] GameObject soulScreen;
    [SerializeField] List<GameObject> equippedSoulContainers = new List<GameObject>();
    [SerializeField] List<GameObject> soulContainers = new List<GameObject>();
    public List<GameObject> soulSkillContainers = new List<GameObject>();
    public List<TextMeshProUGUI> soulSkillText = new List<TextMeshProUGUI>();
    [SerializeField] List<SoulSO> soulScriptableObjects = new List<SoulSO>();
    [SerializeField] List<TextMeshProUGUI> statMultiplierText = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> affinityMultiplierText = new List<TextMeshProUGUI>();
    private int swappingSoulList = -1;
    private int swappingSoulEquipped = -1;

    [Header("Icon Menu Items")]
    public GameObject iconScreen;
    [SerializeField] GameObject restIconMenuButton; //for highlighting
    private bool wasIcon;

    [Header("Travel Menu Items")]
    public GameObject travelScreen;
    [SerializeField] GameObject travelMenuBackButton;
    public List<IconSO> iconScriptableObjects = new List<IconSO>();
    [SerializeField] List<GameObject> areaButtons = new List<GameObject>();
    public List<GameObject> iconLists = new List<GameObject>();
    public List<GameObject> iconContainersFacility = new List<GameObject>();
    public List<TextMeshProUGUI> iconTextFacility = new List<TextMeshProUGUI>();
    public List<GameObject> iconContainersDesert = new List<GameObject>();
    public List<TextMeshProUGUI> iconTextDesert = new List<TextMeshProUGUI>();
    public GameObject iconDescriptionContainer;
    public Image iconImage;
    public TextMeshProUGUI iconDescription;

    [Header("HUD Items")]
    [SerializeField] GameObject hudScreen;
    [SerializeField] Slider healthBar;
    [SerializeField] Slider staggerBar;
    [SerializeField] Slider energyBar;
    [SerializeField] Image stanceHud;
    [SerializeField] Image soulHud;
    [SerializeField] TextMeshProUGUI lightHeldText;
    [SerializeField] List<GameObject> popupContainers = new List<GameObject>();
    private List<string> queuedPopupMessages = new List<string>();
    [SerializeField] float popupTimer = 1.5f;

    [Header("Dialogue Items")]
    public GameObject dialogueScreen;
    public TextMeshProUGUI textField;
    public Image portraitContainer;
    public GameObject dialogueUnlockContainer;
    public int unlockType = -1; //0 = stance, 1 = soul, 100-199 are stance skills, 200-299 are souls (stance/soul skills)
    public int unlockId = -1;
    public int unlockCost = -1;
    public bool listeningForUnlock = false;

    [Header("Tutorial Items")]
    [SerializeField] GameObject tutorialScreen;
    [SerializeField] TextMeshProUGUI tutorialTextField;
    [SerializeField] TextMeshProUGUI tutorialNameField;
    [SerializeField] Image tutorialImage;
    [SerializeField] List<TutorialSO> tutorialObjects = new List<TutorialSO>();
    private int tutorialIndex;
    private int tutorialTextIndex;
    
    [Header("Shared Items")]
    [SerializeField] Sprite emptyContainerImage;
    [SerializeField] Color32 activeColor;
    [SerializeField] Color32 selectedColor;
    [SerializeField] Color32 disabledColor;
    [SerializeField] Image fadeScreen;
    private bool fadeToBlack, fadeFromBlack;
    [SerializeField] float fadeTime = 1f;
    private Color32 defaultColor = new Color32(255,255,255,255);
    private int tempStorage = -1;

    void Start()
    {
        StartCoroutine(UpdateUICO());
    }

    IEnumerator UpdateUICO() //load ui after loading game
    {
        yield return new WaitForEndOfFrame();
        StanceMenuUpdate();
        SoulMenuUpdate();
        PauseMenuUpdate();
        UpdateStanceHud();
        UpdateSoulHud();
        UpdateLightHud();
        UpdateHealth(PlayerTracker.instance.health, PlayerTracker.instance.maxHealth);
        UpdateStagger(PlayerTracker.instance.stagger, PlayerTracker.instance.maxStagger);
        UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);
        optionsScreen.GetComponent<OptionsMenu>().LoadPrefs(); //load options menu parameneters on start
    }

    /*
    change hardcoded inputs to use the unity input system for controller support
    */

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Alpha7))
        //ShowPopup("Testing");
        if(tutorialScreen.activeSelf) //tutorial takes priority above other hud interactions
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                StopAllCoroutines();
                ShowTutorial(tutorialIndex);
                return;
            }

            if(Input.GetMouseButtonDown(0))
            {
                if(tutorialTextField.text == tutorialObjects[tutorialIndex].GetLineByIndex(tutorialTextIndex))
                TutorialNextLine();

                else
                {
                    StopAllCoroutines();
                    tutorialTextField.text = tutorialObjects[tutorialIndex].GetLineByIndex(tutorialTextIndex);
                }
            }

            return;
        }

        if(soulScreen.activeSelf)
        CheckStatChanges(PlayerTracker.instance.strength.ToString()); //fixes a problem with updates not happening, method only required in soulScreen, can expand to use in other UI elements if necessary

        if(Input.GetKeyDown(KeyCode.Escape) && (stanceScreen.activeSelf))
        {
            if(swappingStanceList == -1 && swappingStanceEquipped == -1)
            {
                StanceMenu();
            }
            SwappingStancesList(-1);
            SwappingStancesEquipped(-1);
        }

        else if(Input.GetKeyDown(KeyCode.Escape) && (soulScreen.activeSelf))
        {
            if(swappingSoulList == -1 && swappingSoulEquipped == -1)
            {
                SoulMenu();
            }
            SwappingSoulsList(-1);
            SwappingSoulsEquipped(-1);
        }

        else if(Input.GetKeyDown(KeyCode.Escape) && (iconScreen.activeSelf))
        {
            IconMenu();
        }

        else if(Input.GetKeyDown(KeyCode.Escape) && optionsScreen.activeSelf)
        {
            OptionsMenu();
        }

        else if(Input.GetKeyDown(KeyCode.Escape) && (travelScreen.activeSelf))
        {
            TravelMenu();
        }

        else if(Input.GetKeyDown(KeyCode.Escape) && !dialogueScreen.activeSelf)
        {
            PauseMenu();
        }

        if(fadeToBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeTime*Time.deltaTime)); //regular Color uses 0-1 range
            
            if(fadeScreen.color.a == 1)
            fadeToBlack = false;
        }

        else if (fadeFromBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeTime*Time.deltaTime)); //MoveTowards goes from value 1 to  2  at  3rd  value speed

            if(fadeScreen.color.a == 0)
            fadeFromBlack = false;
        }
    }

    void PauseMenu() //change screen relations later so esc closes any  active scene if any are open and only pauses  when no  screens are present
    {
        if(!stanceScreen.activeSelf && !soulScreen.activeSelf)
        {
            if(!pauseScreen.activeSelf)
            {
                hudScreen.SetActive(false);
                pauseScreen.SetActive(true);
                PauseMenuUpdate();
                SetHighlightedOnNewMenu(mainMenuButton); //later change to be the back button when added

                Time.timeScale = 0f; //pauses game
                //PlayerTracker.instance.isActive = false;
            }
            else
            {
                hudScreen.SetActive(true);
                pauseScreen.SetActive(false);

                Time.timeScale = 1f; //unpauses game
                //PlayerTracker.instance.isActive = true;
            }
        }
    }

    void PauseMenuUpdate()
    {
        strText.text = "STR    " + PlayerTracker.instance.strength;
        dexText.text = "DEX    " + PlayerTracker.instance.dexterity;
        conText.text = "CON    " + PlayerTracker.instance.constitution;
        magText.text = "MAG    " + PlayerTracker.instance.magic;
        strText.GetComponentInChildren<Slider>().value = PlayerTracker.instance.strength;
        dexText.GetComponentInChildren<Slider>().value = PlayerTracker.instance.dexterity;
        conText.GetComponentInChildren<Slider>().value = PlayerTracker.instance.constitution;
        magText.GetComponentInChildren<Slider>().value = PlayerTracker.instance.magic;

        physAffinity.text = "Physical Affinity: " + PlayerTracker.instance.playerPhysicalAffinity.ToString();
        fireAffinity.text = "Fire Affinity: " + PlayerTracker.instance.playerFireAffinity.ToString();
        coldAffinity.text = "Cold Affinity: " + PlayerTracker.instance.playerColdAffinity.ToString();
        lgtnAffinity.text = "Lightning Affinity: " + PlayerTracker.instance.playerLightningAffinity.ToString();
        poisAffinity.text = "Poison Affinity: " + PlayerTracker.instance.playerPoisonAffinity.ToString();
        forcAffinity.text = "Force Affinity: " + PlayerTracker.instance.playerForceAffinity.ToString();
        psycAffinity.text = "Psychic Affinity: " + PlayerTracker.instance.playerPsychicAffinity.ToString();

        healthText.text = "Health:\n" + PlayerTracker.instance.health.ToString() + "  /  " + PlayerTracker.instance.maxHealth.ToString();
        energyText.text = "Energy:\n" + PlayerTracker.instance.energy.ToString() + "  /  " + PlayerTracker.instance.maxEnergy.ToString();

        if(!PlayerController.instance.isAlive)
        mainMenuButton.GetComponent<Button>().interactable = false;

        else
        mainMenuButton.GetComponent<Button>().interactable = true;  

        //pauseMenuEquippedStances[0].GetComponent<Image>().sprite = equippedStanceContainers[PlayerTracker.instance.activeStanceIndex].GetComponent<Image>().sprite;
        //pauseMenuEquippedSouls[0].GetComponent<Image>().sprite = equippedSoulContainers[PlayerTracker.instance.activeSoulIndex].GetComponent<Image>().sprite;

        for(int i = 0; i < pauseMenuEquippedStances.Count; i++)
        {
            pauseMenuEquippedStances[i].GetComponent<Image>().sprite = equippedStanceContainers[i].GetComponent<Image>().sprite;
            pauseMenuEquippedSouls[i].GetComponent<Image>().sprite = equippedSoulContainers[i].GetComponent<Image>().sprite;
        }

        if(PlayerTracker.instance.activeStanceIndex != 0)
        {
            tempSprite = pauseMenuEquippedStances[0].GetComponent<Image>().sprite;
            pauseMenuEquippedStances[0].GetComponent<Image>().sprite = pauseMenuEquippedStances[PlayerTracker.instance.activeStanceIndex].GetComponent<Image>().sprite;
            pauseMenuEquippedStances[PlayerTracker.instance.activeStanceIndex].GetComponent<Image>().sprite = tempSprite;
        }

        if(PlayerTracker.instance.activeSoulIndex != 0)
        {
            tempSprite = pauseMenuEquippedSouls[0].GetComponent<Image>().sprite;
            pauseMenuEquippedSouls[0].GetComponent<Image>().sprite = pauseMenuEquippedSouls[PlayerTracker.instance.activeSoulIndex].GetComponent<Image>().sprite;
            pauseMenuEquippedSouls[PlayerTracker.instance.activeSoulIndex].GetComponent<Image>().sprite = tempSprite;
        }
    }

    public void OptionsMenu()
    {
        if(!optionsScreen.activeSelf)
        {
            pauseScreen.SetActive(false);
            optionsScreen.SetActive(true);
            SetHighlightedOnNewMenu(optionsMenuBackButton);
        }
        else
        {
            pauseScreen.SetActive(true);
            optionsScreen.SetActive(false);
        }
    }

    public void StanceMenu()
    {
        if(!soulScreen.activeSelf && !pauseScreen.activeSelf)
        {
            if(!stanceScreen.activeSelf)
            {
                hudScreen.SetActive(false);
                iconScreen.SetActive(false);
                stanceScreen.SetActive(true);
                StanceMenuUpdate();

                SetHighlightedOnNewMenu(equippedStanceContainers[0]); //later change to be the back button when added

                Time.timeScale = 0f; //pauses game
                //PlayerTracker.instance.isActive = false;
            }
            else
            {
                hudScreen.SetActive(true);
                iconScreen.SetActive(true);
                SetHighlightedOnNewMenu(restIconMenuButton);
                stanceScreen.SetActive(false);
                swappingStanceEquipped = -1;
                swappingStanceList = -1; //reset selections
                Time.timeScale = 1f; //unpauses game
            }
        }
    }

    void StanceMenuUpdate()
    {
        for(int i = 0; i < stanceContainers.Count; i++)
        {
            if(i < stanceScriptableObjects.Count && PlayerTracker.instance.unlockedStances[i])
            {
                stanceContainers[i].SetActive(true);
                stanceContainers[i].GetComponent<Image>().sprite = stanceScriptableObjects[i].GetStanceIcon(); //just assign images in editor later, instead of calling this
                stanceContainers[i].GetComponent<Image>().color = defaultColor;
            }
            else
            {
                stanceContainers[i].SetActive(false);
                //stanceContainers[i].GetComponent<Image>().sprite = emptyContainerImage;
            }
        }

        for(int i = 0; i < equippedStanceContainers.Count; i++)
        {
            if(PlayerTracker.instance.equippedStances[i] != -1)
            {
                equippedStanceContainers[i].GetComponent<Image>().sprite = stanceScriptableObjects[PlayerTracker.instance.equippedStances[i]].GetStanceIcon();
                equippedStanceContainers[i].GetComponent<Image>().color = defaultColor;
                stanceContainers[PlayerTracker.instance.equippedStances[i]].GetComponent<Image>().color  = disabledColor; //grays out equipped stances

                if(i == PlayerTracker.instance.activeStanceIndex)
                equippedStanceContainers[i].GetComponent<Image>().color = activeColor; //makes active stance look greener
            }
            else
            {
                equippedStanceContainers[i].GetComponent<Image>().sprite = emptyContainerImage;
                equippedStanceContainers[i].GetComponent<Image>().color = defaultColor;
            }
        }
    }

    public void SwappingStancesList(int index)
    {
        if(index != -1 && stanceContainers[index].GetComponent<Image>().color != disabledColor && index < PlayerTracker.instance.unlockedStances.Count && swappingStanceList == -1)
        {
            if(PlayerTracker.instance.unlockedStances[index])
            {
                stanceContainers[index].GetComponent<Image>().color = selectedColor;
                swappingStanceList = index;
            }
            
        }

        else if(index != -1 && stanceContainers[index].GetComponent<Image>().color != disabledColor && index < PlayerTracker.instance.unlockedStances.Count) //fix the new Color32 thing here
        {
            if(PlayerTracker.instance.unlockedStances[index])
            {
                stanceContainers[swappingStanceList].GetComponent<Image>().color = defaultColor; //resets to default color
                swappingStanceList = index;
                stanceContainers[index].GetComponent<Image>().color = selectedColor;
            }
        }

        else if(index == -1)
        {
            if(swappingStanceList != -1)
                stanceContainers[swappingStanceList].GetComponent<Image>().color = defaultColor;

            swappingStanceList = -1;
        }

        if(swappingStanceEquipped != -1 && swappingStanceList != -1)
            PerformStanceSwap();
    }

    public void SwappingStancesEquipped(int index)
    {
        if(index != -1 && swappingStanceEquipped == -1)
        {
            equippedStanceContainers[index].GetComponent<Image>().color = selectedColor;
            swappingStanceEquipped = index;
        }

        else if(index != -1 && swappingStanceList == -1)
        {
            SwapStanceBetweenEquipped(index, swappingStanceEquipped);
        }

        else if(index == -1)
        {
            if(swappingStanceEquipped != -1)
            {
                if(swappingStanceEquipped == PlayerTracker.instance.activeStanceIndex)
                    equippedStanceContainers[swappingStanceEquipped].GetComponent<Image>().color = activeColor;
            
                else
                    equippedStanceContainers[swappingStanceEquipped].GetComponent<Image>().color = defaultColor;
            }

            swappingStanceEquipped = -1;
        }

        if(swappingStanceEquipped != -1 && swappingStanceList != -1)
            PerformStanceSwap();
    }

    void SwapStanceBetweenEquipped(int val1, int val2)
    {
        tempStorage = PlayerTracker.instance.equippedStances[val1];
        PlayerTracker.instance.equippedStances[val1] = PlayerTracker.instance.equippedStances[val2];
        PlayerTracker.instance.equippedStances[val2] = tempStorage;
        swappingStanceEquipped = -1;

        if(val1 == PlayerTracker.instance.activeStanceIndex || val2 == PlayerTracker.instance.activeStanceIndex)
        {
            if(val1 == PlayerTracker.instance.activeStanceIndex)
                PlayerTracker.instance.activeStanceIndex = val2;

            else
                PlayerTracker.instance.activeStanceIndex = val1;

            GameObject.Find("Player").GetComponent<PlayerController>().StanceSwap(PlayerTracker.instance.activeStanceIndex); //might pass player to ui as a whole instead of this method
        }

        StanceMenuUpdate();
    }

    void PerformStanceSwap()
    {
        PlayerTracker.instance.equippedStances[swappingStanceEquipped] = swappingStanceList;

        if(swappingStanceEquipped == PlayerTracker.instance.activeStanceIndex)
            GameObject.Find("Player").GetComponent<PlayerController>().StanceSwap(PlayerTracker.instance.activeStanceIndex); //might pass player to ui as a whole instead of this method
        
        swappingStanceEquipped = -1;
        swappingStanceList = -1;
        
        StanceMenuUpdate();
    }

    public void SoulMenu()
    {
        if(!stanceScreen.activeSelf && !pauseScreen.activeSelf)
        {
            if(!soulScreen.activeSelf)
            {
                hudScreen.SetActive(false);
                iconScreen.SetActive(false);
                soulScreen.SetActive(true);
                SoulMenuUpdate();

                SetHighlightedOnNewMenu(equippedSoulContainers[0]); //later change to be the back button when added

                Time.timeScale = 0f; //pauses game
                //PlayerTracker.instance.isActive = false;
            }
            else
            {
                hudScreen.SetActive(true);
                iconScreen.SetActive(true);
                SetHighlightedOnNewMenu(restIconMenuButton);
                soulScreen.SetActive(false);
                swappingSoulEquipped = -1;
                swappingSoulList = -1;
                Time.timeScale = 1f; //unpauses game
                //PlayerTracker.instance.isActive = true;
            }
        }
    }

    void SoulMenuUpdate()
    {
        for(int i = 0; i < soulContainers.Count; i++)
        {
            if(i < soulScriptableObjects.Count && PlayerTracker.instance.unlockedSouls[i])
            {
                soulContainers[i].SetActive(true);
                soulContainers[i].GetComponent<Image>().sprite = soulScriptableObjects[i].GetSoulIcon();
                soulContainers[i].GetComponent<Image>().color = defaultColor;
            }
            else
            {
                soulContainers[i].SetActive(false);
                //soulContainers[i].GetComponent<Image>().sprite = emptyContainerImage;
            }
        }
        for(int i = 0; i < equippedSoulContainers.Count; i++)
        {
            if(PlayerTracker.instance.equippedSouls[i] != -1)
            {
                equippedSoulContainers[i].GetComponent<Image>().sprite = soulScriptableObjects[PlayerTracker.instance.equippedSouls[i]].GetSoulIcon();
                equippedSoulContainers[i].GetComponent<Image>().color = defaultColor;
                soulContainers[PlayerTracker.instance.equippedSouls[i]].GetComponent<Image>().color  = disabledColor; //grays out equipped stances

                if(i == PlayerTracker.instance.activeSoulIndex)
                equippedSoulContainers[i].GetComponent<Image>().color = activeColor; //makes active stance look greener
            }
            else
            {
                equippedSoulContainers[i].GetComponent<Image>().sprite = emptyContainerImage;
                equippedSoulContainers[i].GetComponent<Image>().color = defaultColor;
            }
        }
    }

    public void SwappingSoulsList(int index)
    {
        if(index != -1 && soulContainers[index].GetComponent<Image>().color != disabledColor && index < PlayerTracker.instance.unlockedSouls.Count && swappingSoulList == -1)
        {
            if(PlayerTracker.instance.unlockedSouls[index])
            {
                soulContainers[index].GetComponent<Image>().color = selectedColor;
                swappingSoulList = index;
            }
            
        }

        else if(index != -1 && soulContainers[index].GetComponent<Image>().color != disabledColor && index < PlayerTracker.instance.unlockedSouls.Count) //fix the new Color32 thing here
        {
            if(PlayerTracker.instance.unlockedSouls[index])
            {
                soulContainers[swappingSoulList].GetComponent<Image>().color = defaultColor; //resets to default color
                swappingSoulList = index;
                soulContainers[index].GetComponent<Image>().color = selectedColor;
            }
        }

        else if(index == -1)
        {
            if(swappingSoulList != -1)
                soulContainers[swappingSoulList].GetComponent<Image>().color = defaultColor;

            swappingSoulList = -1;
        }

        if(swappingSoulEquipped != -1 && swappingSoulList != -1)
            PerformSoulSwap();
    }

    public void SwappingSoulsEquipped(int index)
    {
        if(index != -1 && swappingSoulEquipped == -1)
        {
            equippedSoulContainers[index].GetComponent<Image>().color = selectedColor;
            swappingSoulEquipped = index;
        }

        else if(index != -1 && swappingSoulList == -1)
        {
            SwapSoulBetweenEquipped(index, swappingSoulEquipped);
        }

        else if(index == -1)
        {
            if(swappingSoulEquipped != -1)
            {
                if(swappingSoulEquipped == PlayerTracker.instance.activeSoulIndex)
                    equippedSoulContainers[swappingSoulEquipped].GetComponent<Image>().color = activeColor;
            
                else
                    equippedSoulContainers[swappingSoulEquipped].GetComponent<Image>().color = defaultColor;
            }

            swappingSoulEquipped = -1;
        }

        if(swappingSoulEquipped != -1 && swappingSoulList != -1)
            PerformSoulSwap();
    }

    void SwapSoulBetweenEquipped(int val1, int val2)
    {
        tempStorage = PlayerTracker.instance.equippedSouls[val1];
        PlayerTracker.instance.equippedSouls[val1] = PlayerTracker.instance.equippedSouls[val2];
        PlayerTracker.instance.equippedSouls[val2] = tempStorage;
        swappingSoulEquipped = -1;

        if(val1 == PlayerTracker.instance.activeSoulIndex || val2 == PlayerTracker.instance.activeSoulIndex)
        {
            if(val1 == PlayerTracker.instance.activeSoulIndex)
                PlayerTracker.instance.activeSoulIndex = val2;

            else
                PlayerTracker.instance.activeSoulIndex = val1;

            GameObject.Find("Player").GetComponent<PlayerController>().SoulSwap(PlayerTracker.instance.activeSoulIndex); //might pass player to ui as a whole instead of this method
        }

        SoulMenuUpdate();
    }

    void PerformSoulSwap()
    {
        PlayerTracker.instance.equippedSouls[swappingSoulEquipped] = swappingSoulList;

        if(swappingSoulEquipped == PlayerTracker.instance.activeSoulIndex)
            GameObject.Find("Player").GetComponent<PlayerController>().SoulSwap(PlayerTracker.instance.activeSoulIndex); //might pass player to ui as a whole instead of this method
        
        swappingSoulEquipped = -1;
        swappingSoulList = -1;

        SoulMenuUpdate();
    }

    public StanceSO GetStanceSOByIndex(int index)
    {
        return stanceScriptableObjects[index];
    }

    public SoulSO GetSoulSOByIndex(int index)
    {
        return soulScriptableObjects[index];
    }

    private void CheckStatChanges(string checker) //this method was used as gui update was happening earlier, causing the update to not happen immediately
    {
        if(checker == statMultiplierText[0].text) { return; }
        UpdateStatsOnChange();
    }

    private void UpdateStatsOnChange()
    {
        statMultiplierText[0].text = PlayerTracker.instance.strength.ToString(); //displays stats, might change to display modifiers later
        statMultiplierText[1].text = PlayerTracker.instance.dexterity.ToString(); //can obtain modifiers by dividing this value by the base stat value
        statMultiplierText[2].text = PlayerTracker.instance.constitution.ToString();
        statMultiplierText[3].text = PlayerTracker.instance.magic.ToString();

        affinityMultiplierText[0].text = PlayerTracker.instance.playerPhysicalAffinity.ToString();
        affinityMultiplierText[1].text = PlayerTracker.instance.playerFireAffinity.ToString();
        affinityMultiplierText[2].text = PlayerTracker.instance.playerColdAffinity.ToString();
        affinityMultiplierText[3].text = PlayerTracker.instance.playerLightningAffinity.ToString();
        affinityMultiplierText[4].text = PlayerTracker.instance.playerPoisonAffinity.ToString();
        affinityMultiplierText[5].text = PlayerTracker.instance.playerForceAffinity.ToString();
        affinityMultiplierText[6].text = PlayerTracker.instance.playerPsychicAffinity.ToString();
    }

    public void FadeToBlack()
    {
        fadeToBlack = true;
        fadeFromBlack = false;
    }

    public void FadeFromBlack()
    {
        fadeToBlack = false;
        fadeFromBlack = true;
    }

    public void UpdateHealth(int currentHealth, int maxHealth) //might refactor to remove function inputs, getting values from tracker directly, like UpdateLightHud
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void UpdateStagger(float currentStagger, int maxStagger)
    {
        staggerBar.maxValue = maxStagger;
        staggerBar.value = currentStagger;
    }

    public void UpdateEnergy(int currentEnergy, int maxEnergy)
    {
        energyBar.maxValue = maxEnergy;
        energyBar.value = currentEnergy;
    }

    public void UpdateStanceHud()
    {
        stanceHud.sprite = stanceScriptableObjects[PlayerTracker.instance.equippedStances[PlayerTracker.instance.activeStanceIndex]].GetStanceIcon();
    }

    public void UpdateSoulHud()
    {
        soulHud.sprite = soulScriptableObjects[PlayerTracker.instance.equippedSouls[PlayerTracker.instance.activeSoulIndex]].GetSoulIcon();
    }

    public void UpdateLightHud()
    {
        lightHeldText.text = PlayerTracker.instance.lightAmount.ToString();
    }

    public void IconMenu()
    {
            if(!iconScreen.activeSelf)
            {
                iconScreen.SetActive(true);
                SetHighlightedOnNewMenu(restIconMenuButton);
                PlayerController.instance.isInScene = true;
            }
            else
            {
                iconScreen.SetActive(false);
                PlayerController.instance.isInScene = false;
            }
    }

    public void TravelMenu()  //look over again later
    {
        if(!travelScreen.activeSelf)
            {
                travelScreen.SetActive(true); //no need to manip isInScene as this is used
                iconScreen.SetActive(false); //through iconMenu
                TravelMenuUpdate();
                SetHighlightedOnNewMenu(travelMenuBackButton); //for highlighting
            }
            else
            {
                travelScreen.SetActive(false);
                iconScreen.SetActive(true);
                SetHighlightedOnNewMenu(restIconMenuButton);
            }
    }

    void TravelMenuUpdate()
    {
        SetHighlightedOnNewMenu(travelMenuBackButton);

        if(iconDescriptionContainer.activeSelf) //start disabled by default
        {
            iconDescriptionContainer.SetActive(false);
        }

        if(!areaButtons[0].activeSelf)
        {
            for(int i = 0; i < 12; i++) //check first 12 icons
            {
                if(PlayerTracker.instance.iconIsActive[i])
                {
                    areaButtons[0].SetActive(true);
                    break;
                }
            }
        }

        if(!areaButtons[1].activeSelf)
        {
            for(int i = 12; i < 24; i++) //check icons 12-23
            {
                if(PlayerTracker.instance.iconIsActive[i])
                {
                    areaButtons[1].SetActive(true);
                    break;
                }
            }
        }

        for(int i = 0; i < 12; i++)
        {
            if(PlayerTracker.instance.iconIsActive[i])
            {
                iconContainersFacility[i].SetActive(true);
                iconTextFacility[i].text = iconScriptableObjects[i].GetIconName();
            }
            //else                  otherwise set icon buttons to false to prevent showing empty buttons, not needed now as containers start disabled by default
            //iconContainersFacility[i].SetActive(false);
        }

        for(int i = 0; i < 12; i++)
        {
            if(PlayerTracker.instance.iconIsActive[i + 12])
            {
                iconContainersDesert[i].SetActive(true);
                iconTextDesert[i].text = iconScriptableObjects[i + 12].name;
            }
        }

        if(PlayerTracker.instance.lastTouched < 12)
        ShowFacilityIcons();

        else //add more clauses as more zones get added
        ShowDesertIcons();
    }

    public void ShowFacilityIcons()
    {
        iconLists[0].SetActive(true);

        for(int i = 0; i < iconLists.Count; i++) //disable other iconLists
        {
            if(i != 0)
            iconLists[i].SetActive(false);
        }
    }

    public void ShowDesertIcons()
    {
        iconLists[1].SetActive(true);

        for(int i = 0; i < iconLists.Count; i++)
        {
            if(i != 1)
            iconLists[i].SetActive(false);
        }
    }

    public void WarpByIndex(int index) //triggers a rest at index icon, might change to a unique function later on
    {
        iconScreen.SetActive(false);
        travelScreen.SetActive(false);
        PlayerController.instance.isInScene = false;
        PlayerTracker.instance.lastTouched = index;
        RespawnController.instance.Rest();
    }

    public void ShowPopup(string message)
    {
        for(int i = 0; i < popupContainers.Count; i++)
        {
            if(!popupContainers[i].activeSelf)
            {
                popupContainers[i].GetComponentInChildren<TextMeshProUGUI>().text = message;
                popupContainers[i].SetActive(true);
                StartCoroutine(DisablePopupCO(popupContainers[i]));
                return;
            }
        }

        queuedPopupMessages.Add(message); //if no container satisfied the check
    }

    IEnumerator DisablePopupCO(GameObject container)
    {
        yield return new WaitForSeconds(popupTimer);
        container.SetActive(false);

        if(queuedPopupMessages.Count != 0)
        {
            ShowPopup(queuedPopupMessages[0]);
            queuedPopupMessages.RemoveAt(0);
        }
    }

    public void ReturnToMainMenu()
    {
        DataPersistanceManager.instance.SaveGame(); //save game
        
        SceneManager.LoadScene(mainMenuScene);
        Destroy(PlayerController.instance.gameObject);
        PlayerController.instance = null;
        PlayerTracker.instance = null;

        Destroy(Cameras.instance.gameObject);
        Cameras.instance = null;

        Destroy(RespawnController.instance.gameObject);
        RespawnController.instance = null;

        instance = null;
        Destroy(gameObject);
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

    public void DialogueMenu()
    {
        if(!dialogueScreen.activeSelf)
            {
                dialogueUnlockContainer.SetActive(false); //disabled by default
                dialogueScreen.SetActive(true);
                PlayerController.instance.isInScene = true;
            }
            else
            {
                unlockId = -1;
                unlockType = -1;
                dialogueUnlockContainer.SetActive(false); //for safety, probably safe to remove
                dialogueScreen.SetActive(false);
                StartCoroutine(DialogueMenuCO());
            }
    }

    IEnumerator DialogueMenuCO() //prevents autoattack after leaving menu
    {
        yield return new WaitForEndOfFrame();
        PlayerController.instance.isInScene = false;
    }

    public void PerformUnlock()
    {
        if(PlayerTracker.instance.lightAmount < unlockCost)
        {
            textField.color = new Color32(255, 0, 0, 255); //hardcoded, maybe change later
            StartCoroutine(ResetDialogueColor());
        }

        else
        {
            if(unlockType == 0) //stance
            {
                PlayerTracker.instance.unlockedStances[unlockId] = true;
                PlayerTracker.instance.lightAmount -= unlockCost;

                for(int i = 0; i < PlayerTracker.instance.equippedStances.Count; i++) //equip if empty slots are present
                {
                    if(PlayerTracker.instance.equippedStances[i] == -1)
                    {
                        PlayerTracker.instance.equippedStances[i] = unlockId;
                        StanceMenuUpdate();
                        break;
                    }
                }
            }

            if(unlockType == 1) //soul
            {
                PlayerTracker.instance.unlockedSouls[unlockId] = true;
                PlayerTracker.instance.lightAmount -= unlockCost;

                for(int i = 0; i < PlayerTracker.instance.equippedSouls.Count; i++)
                {
                    if(PlayerTracker.instance.equippedSouls[i] == -1)
                    {
                        PlayerTracker.instance.equippedSouls[i] = unlockId;
                        SoulMenuUpdate();
                        break;
                    }
                }
            }

            if(unlockType == 100) //scimitar stance
            {
                if(PlayerTracker.instance.scimitarStanceUnlocks[unlockId])
                {
                    textField.color = new Color32(255, 0, 0, 255); //hardcoded, maybe change later
                    textField.text = "Stance already upgraded by this anvil.";
                    StartCoroutine(ResetDialogueColor());
                    return; //prevents from reaching dialogue close line
                }

                else
                {
                    PlayerTracker.instance.scimitarStanceUnlocks[unlockId] = true;
                    PlayerTracker.instance.lightAmount -= unlockCost;
                }
            }

            if(unlockType == 101) //axe stance
            {
                if(PlayerTracker.instance.axeStanceUnlocks[unlockId])
                {
                    textField.color = new Color32(255, 0, 0, 255); //hardcoded, maybe change later
                    textField.text = "Stance already upgraded by this anvil.";
                    StartCoroutine(ResetDialogueColor());
                    return;
                }

                else
                {
                    PlayerTracker.instance.axeStanceUnlocks[unlockId] = true;
                    PlayerTracker.instance.lightAmount -= unlockCost;
                }
            }

            //other stances

            if(unlockType == 200) //eldritch soul
            {
                PlayerTracker.instance.eldritchSoulUnlocks[unlockId] = true;
                PlayerTracker.instance.lightAmount -= unlockCost;
            }

            if(unlockType == 201) //mushroom soul
            {
                PlayerTracker.instance.mushroomSoulUnlocks[unlockId] = true;
                PlayerTracker.instance.lightAmount -= unlockCost;
            }

            UpdateLightHud();
            DialogueMenu();
            listeningForUnlock = true; //listened by unlock script, script checks for the bool, performs action when true, changes this back to false
        }
        
    }

    IEnumerator ResetDialogueColor()
    {
        yield return new WaitForSeconds(.5f);
        textField.color = new Color32(255, 255, 255, 255);
    }

    public void ShowTutorial(int index)
    {
        if(!tutorialScreen.activeSelf)
        {
            tutorialTextIndex = 0;
            tutorialIndex = index;

            tutorialNameField.text = tutorialObjects[tutorialIndex].GetTutorialName();
            tutorialScreen.SetActive(true);
            StartTutorial();
            
            Time.timeScale = 0f;
            PlayerController.instance.isInScene = true;
        }
        else
        {
            tutorialScreen.SetActive(false);
            Time.timeScale = 1f;
            StartCoroutine(DialogueMenuCO()); //disables isInScene at the end of frame
        }
    }

    void StartTutorial()
    {
        tutorialImage.sprite = tutorialObjects[tutorialIndex].GetImageByIndex(tutorialTextIndex);
        tutorialTextField.text = tutorialObjects[tutorialIndex].GetLineByIndex(tutorialTextIndex);
    }

    void TutorialNextLine()
    {
        if(tutorialTextIndex < tutorialObjects[tutorialIndex].GetTutorialLength()-1)
        {
            tutorialTextIndex++;
            tutorialTextField.text = string.Empty;
            StartTutorial();
        }
        else
        {
            ShowTutorial(tutorialIndex);
        }
    }
}
