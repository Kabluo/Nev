using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistanceManager : MonoBehaviour
{
    public static DataPersistanceManager instance {get;  private  set;}
    
    [Header("File Storage Config")]
    [SerializeField] string fileName;
    [SerializeField] bool useEncryption;

    private GameData gameData;
    private List<IDataPersistance> dataPersistanceObjects;
    private FileDataHandler dataHandler;
    private string selectedProfileId = "";

    public bool isNewGame = false;
    public bool firstTimeLoading = true; //default true

    void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        GetDataById(selectedProfileId);
    }

    public void ChangeSelectedProfileId(string newProfileId) //later refactor other scripts to use the "this." notation, easier to read and understand, low priority
    {
        this.selectedProfileId = newProfileId;
    }

    public void NewGame()
    {
        this.isNewGame = true;
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.dataPersistanceObjects = FindAllDataPersistanceObjects(); //get all objects to load
        this.gameData = dataHandler.Load(selectedProfileId);
        
        if(this.gameData == null)
        {
            NewGame();
        }

        //push data  to other scripts
        foreach(IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        this.dataPersistanceObjects = FindAllDataPersistanceObjects();
        //push data to other scripts to save
        foreach(IDataPersistance dataPersistanceObj in dataPersistanceObjects)
        {
            dataPersistanceObj.SaveData(ref gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary(); //timestamp save file
        //save data  to file
        dataHandler.Save(gameData, selectedProfileId);
    }

    List<IDataPersistance> FindAllDataPersistanceObjects()
    {
        IEnumerable<IDataPersistance> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>()
        .OfType<IDataPersistance>();

        return new List<IDataPersistance>(dataPersistanceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    public string GetProfileLevelName()
    {
        return dataHandler.Load(this.selectedProfileId).levelName;
    }

    private void GetDataById(string profileId)
    {
        this.gameData = dataHandler.Load(profileId);
    }

    public void IsNewGame(bool isNewGame)
    {
        this.isNewGame = isNewGame;
    }

    /*
    public void GetDataPersistentObjects()
    {
        dataPersistanceObjects = FindAllDataPersistanceObjects();
    }
    */
}
