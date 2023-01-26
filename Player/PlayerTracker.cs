using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour, IDataPersistance
{
    public static PlayerTracker instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //gameobject attached to this script is not destroyed on scene change
            originalObject = true;
        }

        else //if there is already a player, destroy the new  one / don't create a new one
        Destroy(this.gameObject);
    }

    void Start()
    {
        //DataPersistanceManager.instance.GetDataPersistentObjects();
        //StartCoroutine(LoadSaveFileCO());
        if(!DataPersistanceManager.instance.isNewGame)
        {
            if(DataPersistanceManager.instance.firstTimeLoading)
            {
                DataPersistanceManager.instance.LoadGame();
                DataPersistanceManager.instance.firstTimeLoading = false;
            }

            RespawnController.instance.respawnPoint = PlayerTracker.instance.iconLocation[PlayerTracker.instance.lastRested];
        }

        else
        DataPersistanceManager.instance.IsNewGame(false); //reset isNewGame afterwards to fix load errors
    }

    //IEnumerator LoadSaveFileCO()
    //{
    //    yield return new WaitForEndOfFrame();
    //    DataPersistanceManager.instance.LoadGame();
    //}

    private bool originalObject = false;

    [Header("Character Values")]
    public int maxHealth;
    public int health;

    public int maxStagger;
    public float stagger;
    
    public int maxEnergy;
    public int energy;

    //public bool isActive = true;
    //REFACTOR LATER, MAKE INTO AN INSTANCE, EASIER TO READ THAT WAY, ALSO WAY LESS REFERENCES
    //also move isBroken and isAlive here for easy reference

    //REMEMBER TO UPDATE Save and Load functions when adding and removing stuff here!

    [Header("Character Stats")]
    public int baseStrength = 1;
    public int baseDexterity = 1; //finesse weapon damage calculation
    public int baseConstitution = 1; //increased stagger resistance and health
    public int baseMagic = 1; //increased mana and spell damage

    public int strength = 1; //heavy weapon damage calculation
    public int dexterity = 1; //finesse weapon damage calculation
    public int constitution = 1; //increased stagger resistance and health
    public int magic = 1; //increased mana and spell damage

    public float playerPhysicalAffinity = 1f; //damage modifier against physical attacks
    public float playerFireAffinity = 1f;
    public float playerColdAffinity = 1f;
    public float playerLightningAffinity = 1f;
    public float playerPoisonAffinity = 1f;
    public float playerForceAffinity = 1f;
    public float playerPsychicAffinity = 1f;

    public int lightAmount = 0;

    [Header("Stance / Soul Manip")]
    public int activeStanceIndex;
    public int activeSoulIndex;
    public List<int> equippedStances = new List<int>();
    public List<int> equippedSouls = new List<int>();

    public List<bool> unlockedStances = new List<bool>();
    public List<bool> unlockedSouls = new List<bool>();

    public List<GameObject> stanceObjects = new List<GameObject>(); //array of prefabs that hold stance/soul scripts and information, created as child,
    public List<GameObject> soulObjects = new List<GameObject>(); //destroyed upon change
    
    [Header("Stance Skill Unlocks")]
    public List<bool> scimitarStanceUnlocks = new List<bool>();
    public List<bool> axeStanceUnlocks = new List<bool>();
    public List<bool> dualBladeStanceUnlocks = new List<bool>();
    public List<bool> hammerStanceUnlocks = new List<bool>();
    public List<bool> rapierStanceUnlocks = new List<bool>();
    public List<bool> greatswordStanceUnlocks = new List<bool>();
    public List<bool> katanaStanceUnlocks = new List<bool>();
    public List<bool> guardianStanceUnlocks = new List<bool>();
    public List<bool> bowStanceUnlocks = new List<bool>();
    public List<bool> beastlyStanceUnlocks = new List<bool>();
    public List<bool> monstrousStanceUnlocks = new List<bool>();

    [Header("Soul Skill Unlocks")]
    public List<bool> eldritchSoulUnlocks = new List<bool>();
    public List<bool> mushroomSoulUnlocks = new List<bool>();
    public List<bool> humanSoulUnlocks = new List<bool>();
    public List<bool> machineSoulUnlocks = new List<bool>();
    public List<bool> sectolSoulUnlocks = new List<bool>();
    public List<bool> forgottenSoulUnlocks = new List<bool>();
    public List<bool> queenSoulUnlocks = new List<bool>();
    public List<bool> broodmotherSoulUnlocks = new List<bool>();
    public List<bool> draconicSoulUnlocks = new List<bool>();
    public List<bool> angelicSoulUnlocks = new List<bool>();
    public List<bool> demonicSoulUnlocks = new List<bool>();

    [Header("Progress Tracking")]
    public List<bool> bossesDefeated = new List<bool>();
    public List<bool> strengthStatUpgrades = new List<bool>();
    public List<bool> dexterityStatUpgrades = new List<bool>();
    public List<bool> constitutionStatUpgrades = new List<bool>();
    public List<bool> magicStatUpgrades = new List<bool>();

    [Header("Icons")]
    public List<bool> iconIsActive = new List<bool>();
    public List<Vector3> iconLocation = new List<Vector3>();
    public List<string> iconSceneName = new List<string>();
    public int lastTouched = 0;
    public int lastRested = 0; //used to get sceneName, should start at 0 so that dying before getting to the first icon won't cause errors

    [Header("Corpses")]
    public List<string> corpsesByScenes = new List<string>();
    public List<Vector3> corpseLocation = new List<Vector3>();
    public List<float> corpseRotation = new List<float>();
    public List<int> corpseIds = new List<int>();
    public List<int> corpseLight = new List<int>();
    public int corpseTracker = 0;

    [Header("Tutorials")]
    public List<bool> tutorialSeen = new List<bool>();

    public void LoadData(GameData data) //might add a FrameEnd delay if seems problematic
    {
        if(!originalObject) { return; }
        
        this.maxHealth = data.maxHealth;
        this.health = data.health;
        this.maxStagger = data.maxStagger;
        this.stagger = data.stagger;
        this.maxEnergy = data.maxEnergy;
        this.energy = data.energy;

        this.baseStrength = data.baseStrength;
        this.baseDexterity = data.baseDexterity;
        this.baseConstitution = data.baseConstitution;
        this.baseMagic = data.baseMagic;

        this.corpsesByScenes = data.corpsesByScenes;
        this.corpseLocation = data.corpseLocation;
        this.corpseRotation = data.corpseRotation;
        this.corpseIds = data.corpseIds;
        this.corpseLight = data.corpseLight;

        this.lightAmount = data.lightAmount;

        this.activeStanceIndex = data.activeStanceIndex;
        this.activeSoulIndex = data.activeSoulIndex;

        this.equippedStances = data.equippedStances;
        this.equippedSouls = data.equippedSouls;

        this.unlockedStances = data.unlockedStances;
        this.unlockedSouls = data.unlockedSouls;

        this.bossesDefeated = data.bossesDefeated;
        this.strengthStatUpgrades = data.strengthStatUpgrades;
        this.dexterityStatUpgrades = data.dexterityStatUpgrades;
        this.constitutionStatUpgrades = data.constitutionStatUpgrades;
        this.magicStatUpgrades = data.magicStatUpgrades;

        this.scimitarStanceUnlocks = data.scimitarStanceUnlocks;
        this.axeStanceUnlocks = data.axeStanceUnlocks;
        this.dualBladeStanceUnlocks = data.dualBladeStanceUnlocks;
        this.hammerStanceUnlocks = data.hammerStanceUnlocks;
        this.rapierStanceUnlocks = data.rapierStanceUnlocks;
        this.greatswordStanceUnlocks = data.greatswordStanceUnlocks;
        this.katanaStanceUnlocks = data.katanaStanceUnlocks;
        this.guardianStanceUnlocks = data.guardianStanceUnlocks;
        this.bowStanceUnlocks = data.bowStanceUnlocks;
        this.beastlyStanceUnlocks = data.beastlyStanceUnlocks;
        this.monstrousStanceUnlocks = data.monstrousStanceUnlocks;

        this.eldritchSoulUnlocks = data.eldritchSoulUnlocks;
        this.mushroomSoulUnlocks = data.mushroomSoulUnlocks;
        this.humanSoulUnlocks = data.humanSoulUnlocks;
        this.machineSoulUnlocks = data.machineSoulUnlocks;
        this.sectolSoulUnlocks = data.sectolSoulUnlocks;
        this.forgottenSoulUnlocks = data.forgottenSoulUnlocks;
        this.queenSoulUnlocks = data.queenSoulUnlocks;
        this.broodmotherSoulUnlocks = data.broodmotherSoulUnlocks;
        this.draconicSoulUnlocks = data.draconicSoulUnlocks;
        this.angelicSoulUnlocks = data.angelicSoulUnlocks;
        this.demonicSoulUnlocks = data.demonicSoulUnlocks;

        this.iconIsActive = data.iconIsActive;
        this.lastTouched = data.lastTouched;
        this.lastRested = data.lastRested;

        this.tutorialSeen = data.tutorialSeen;

        //call updatesoul afterwards  to calculate active stats, this only loads base stats
        //also instantiate soul/stance object if not present
    }

    public void SaveData(ref GameData data)
    {
        if(!originalObject) { return; }

        data.maxHealth = this.maxHealth;
        data.health = this.health;
        data.maxStagger = this.maxStagger;
        data.stagger = this.stagger;
        data.maxEnergy = this.maxEnergy;
        data.energy = this.energy;

        data.baseStrength = this.baseStrength;
        data.baseDexterity = this.baseDexterity;
        data.baseConstitution = this.baseConstitution;
        data.baseMagic = this.baseMagic;

        data.corpsesByScenes = this.corpsesByScenes;
        data.corpseLocation = this.corpseLocation;
        data.corpseRotation = this.corpseRotation;
        data.corpseIds = this.corpseIds;
        data.corpseLight = this.corpseLight;

        data.lightAmount = this.lightAmount;

        data.activeStanceIndex = this.activeStanceIndex;
        data.activeSoulIndex = this.activeSoulIndex;

        data.equippedStances = this.equippedStances;
        data.equippedSouls = this.equippedSouls;

        data.unlockedStances = this.unlockedStances;
        data.unlockedSouls = this.unlockedSouls;

        data.bossesDefeated = this.bossesDefeated;
        data.strengthStatUpgrades = this.strengthStatUpgrades;
        data.dexterityStatUpgrades = this.dexterityStatUpgrades;
        data.constitutionStatUpgrades = this.constitutionStatUpgrades;
        data.magicStatUpgrades = this.magicStatUpgrades;

        data.scimitarStanceUnlocks = this.scimitarStanceUnlocks;
        data.axeStanceUnlocks = this.axeStanceUnlocks;
        data.dualBladeStanceUnlocks = this.dualBladeStanceUnlocks;
        data.hammerStanceUnlocks = this.hammerStanceUnlocks;
        data.rapierStanceUnlocks = this.rapierStanceUnlocks;
        data.greatswordStanceUnlocks = this.greatswordStanceUnlocks;
        data.katanaStanceUnlocks = this.katanaStanceUnlocks;
        data.guardianStanceUnlocks = this.guardianStanceUnlocks;
        data.bowStanceUnlocks = this.bowStanceUnlocks;
        data.beastlyStanceUnlocks = this.beastlyStanceUnlocks;
        data.monstrousStanceUnlocks = this.monstrousStanceUnlocks;

        data.eldritchSoulUnlocks = this.eldritchSoulUnlocks;
        data.mushroomSoulUnlocks = this.mushroomSoulUnlocks;
        data.humanSoulUnlocks = this.humanSoulUnlocks;
        data.machineSoulUnlocks = this.machineSoulUnlocks;
        data.sectolSoulUnlocks = this.sectolSoulUnlocks;
        data.forgottenSoulUnlocks = this.forgottenSoulUnlocks;
        data.queenSoulUnlocks = this.queenSoulUnlocks;
        data.broodmotherSoulUnlocks = this.broodmotherSoulUnlocks;
        data.draconicSoulUnlocks = this.draconicSoulUnlocks;
        data.angelicSoulUnlocks = this.angelicSoulUnlocks;
        data.demonicSoulUnlocks = this.demonicSoulUnlocks;

        data.iconIsActive = this.iconIsActive;
        data.lastTouched = this.lastTouched;
        data.lastRested = this.lastRested;

        data.tutorialSeen = this.tutorialSeen;
    }

}
