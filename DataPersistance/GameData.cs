using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public string levelName;
    public Vector3 playerPosition;
    public bool isBroken;

    public List<string> corpsesByScenes = new List<string>();
    public List<Vector3> corpseLocation = new List<Vector3>();
    public List<int> corpseIds = new List<int>();
    public List<int> corpseLight = new List<int>();
    public List<float> corpseRotation = new List<float>();

    public int maxHealth;
    public int health;

    public int maxStagger;
    public float stagger;

    public int maxEnergy;
    public int energy;

    public int baseStrength = 1;
    public int baseDexterity = 1;
    public int baseConstitution = 1;
    public int baseMagic = 1;

    public int lightAmount = 0;

    public int activeStanceIndex;
    public int activeSoulIndex;
    public List<int> equippedStances = new List<int>();
    public List<int> equippedSouls = new List<int>();

    public List<bool> unlockedStances = new List<bool>();
    public List<bool> unlockedSouls = new List<bool>();

    public List<bool> bossesDefeated = new List<bool>();
    public List<bool> strengthStatUpgrades = new List<bool>();
    public List<bool> dexterityStatUpgrades = new List<bool>();
    public List<bool> constitutionStatUpgrades = new List<bool>();
    public List<bool> magicStatUpgrades = new List<bool>();

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

    public List<bool> iconIsActive = new List<bool>();
    public int lastTouched = 0;
    public int lastRested = 0;

    public List<bool> tutorialSeen = new List<bool>();

    //default values
    public GameData()
    {
        levelName = "Test 0";
        playerPosition = new Vector3(7f, .5f, 0f);
        isBroken = false;
        this.maxHealth = 100;
        this.health = 100;

        this.maxStagger = 25;
        this.stagger = 25f;

        this.maxEnergy = 100;
        this.energy = 100;

        this.baseStrength = 1;
        this.baseDexterity = 1;
        this.baseConstitution = 1;
        this.baseMagic = 1;

        this.lightAmount = 0;

        this.activeStanceIndex = 0;
        this.activeSoulIndex = 0;
        
        this.equippedStances.Add(0);
        this.equippedStances.Add(-1);
        this.equippedStances.Add(-1);
        this.equippedStances.Add(-1);

        this.equippedSouls.Add(0);
        this.equippedSouls.Add(-1);
        this.equippedSouls.Add(-1);
        this.equippedSouls.Add(-1);

        this.unlockedStances.Add(true);
        this.unlockedSouls.Add(true);

        //update if soul  amount changes
        for(int i = 0; i < 11; i++)
        {
            this.unlockedStances.Add(false);
            this.unlockedSouls.Add(false);
        }

        for(int i = 0; i < 64; i++)
        this.bossesDefeated.Add(false);

        for(int i = 0; i < 43; i++)
        {
            this.strengthStatUpgrades.Add(false);
            this.dexterityStatUpgrades.Add(false);
            this.constitutionStatUpgrades.Add(false);
            this.magicStatUpgrades.Add(false);
        }

        for(int i = 0; i < 8; i++)
        {
            this.scimitarStanceUnlocks.Add(false);
            this.axeStanceUnlocks.Add(false);
            this.dualBladeStanceUnlocks.Add(false);
            this.hammerStanceUnlocks.Add(false);
            this.rapierStanceUnlocks.Add(false);
            this.greatswordStanceUnlocks.Add(false);
            this.katanaStanceUnlocks.Add(false);
            this.guardianStanceUnlocks.Add(false);
            this.bowStanceUnlocks.Add(false);
            this.beastlyStanceUnlocks.Add(false);
            this.monstrousStanceUnlocks.Add(false);
        }
        

        for(int i = 0; i < 8; i++)
        {
            this.eldritchSoulUnlocks.Add(false);
            this.mushroomSoulUnlocks.Add(false);
            this.humanSoulUnlocks.Add(false);
            this.machineSoulUnlocks.Add(false);
            this.sectolSoulUnlocks.Add(false);
            this.forgottenSoulUnlocks.Add(false);
            this.queenSoulUnlocks.Add(false);
            this.broodmotherSoulUnlocks.Add(false);
            this.draconicSoulUnlocks.Add(false);
            this.angelicSoulUnlocks.Add(false);
            this.demonicSoulUnlocks.Add(false);
        }

        //update when icon amount changes
        for(int i = 0; i < 24; i++)
        this.iconIsActive.Add(false);

        this.lastTouched = 0;
        this.lastRested = 0;

        //update when tutorial amount changes
        this.tutorialSeen.Add(false);
    }
}
