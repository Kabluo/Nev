using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class RespawnController : MonoBehaviour
{
    public static RespawnController instance;
    public int playerCorpseId;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //gameobject attached to this script is not destroyed on scene change
        }

        else //if there is already a player, destroy the new  one / don't create a new one
        Destroy(this.gameObject);
    }

    public Vector3 respawnPoint;
    public bool triggerRespawn;
    [SerializeField] List<GameObject> corpses = new List<GameObject>();
    private int numVcams;

    // Start is called before the first frame update
    void Start()
    {
        if(respawnPoint == null)
        respawnPoint = PlayerTracker.instance.iconLocation[PlayerTracker.instance.lastRested];

        numVcams = CinemachineCore.Instance.VirtualCameraCount;
        StartCoroutine(LoadCorpsesCO()); //after firing up the game for the first time / after loading data
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Backspace) && !PlayerController.instance.isAlive)
        Respawn(PlayerTracker.instance.iconSceneName[PlayerTracker.instance.lastRested], true);
    }

    public void Respawn(string sceneName, bool createCorpse)
    {
        if(createCorpse)
        CreateCorpse();

        StartCoroutine(RespawnCO(sceneName)); //might make a global variable later instead of passing around, low priority
    }

    IEnumerator RespawnCO(string sceneName)
    {
        UIController.instance.FadeToBlack();

        yield return new WaitForSeconds(1f);

        var sceneIsLoaded = SceneManager.LoadSceneAsync(sceneName);

        while(!sceneIsLoaded.isDone)
        {
            yield return null;
        }

        for (int i = 0; i < numVcams; i++)
            CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(PlayerController.instance.gameObject.transform, (respawnPoint-PlayerTracker.instance.gameObject.transform.position));

        StartCoroutine(LoadCorpsesCO());

        UIController.instance.FadeFromBlack();

        PlayerTracker.instance.gameObject.transform.position = respawnPoint;
        PlayerController.instance.lastOnGround = respawnPoint; //otherwise saving creates problems as respawn point might be slightly above ground, preventing lastonground from updating before save

        PlayerTracker.instance.health = PlayerTracker.instance.maxHealth;
        PlayerTracker.instance.stagger = PlayerTracker.instance.maxStagger;
        PlayerTracker.instance.energy = PlayerTracker.instance.maxEnergy;
        PlayerController.instance.isAlive = true;
        PlayerController.instance.gameObject.tag = "Player"; //tag is set to corpse on death, this fixes it
        PlayerTracker.instance.gameObject.GetComponent<Animator>().SetTrigger("RESET");

        UIController.instance.UpdateHealth(PlayerTracker.instance.health, PlayerTracker.instance.maxHealth);
        UIController.instance.UpdateStagger(PlayerTracker.instance.stagger, PlayerTracker.instance.maxStagger);
        UIController.instance.UpdateEnergy(PlayerTracker.instance.energy, PlayerTracker.instance.maxEnergy);

        DataPersistanceManager.instance.SaveGame(); //save after respawning / resting

        //PlayerController.instance.isInScene = false; //failsafe, might be safe to remove
    }

    public void Rest()
    {
        PlayerTracker.instance.lastRested = PlayerTracker.instance.lastTouched;
        respawnPoint = PlayerTracker.instance.iconLocation[PlayerTracker.instance.lastRested]; //get the position of last interacted icon
        Respawn(PlayerTracker.instance.iconSceneName[PlayerTracker.instance.lastRested], false); //trigger respawn
    }

    public void ChangeScene(string sceneName, Vector3 location)
    {
        StartCoroutine(ChangeSceneCO(sceneName, location));
    }

    IEnumerator ChangeSceneCO(string sceneName, Vector3 location)
    {
        UIController.instance.FadeToBlack();

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
        StartCoroutine(LoadCorpsesCO());

        for (int i = 0; i < numVcams; i++) //snap all VCs to player
        {
            CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(PlayerController.instance.gameObject.transform, (location-PlayerTracker.instance.gameObject.transform.position));
        }

        UIController.instance.FadeFromBlack();

        PlayerTracker.instance.gameObject.transform.position = location;
        
        PlayerController.instance.isInScene = false;
    }

    void CreateCorpse()
    {
        PlayerTracker.instance.corpsesByScenes.Add(SceneManager.GetActiveScene().name);
        PlayerTracker.instance.corpseIds.Add(playerCorpseId);
        PlayerTracker.instance.corpseLocation.Add(PlayerController.instance.gameObject.transform.position);
        PlayerTracker.instance.corpseRotation.Add(PlayerController.instance.gameObject.transform.localScale.x);
        PlayerTracker.instance.corpseLight.Add(PlayerTracker.instance.lightAmount);
        PlayerTracker.instance.lightAmount = 0;
        UIController.instance.UpdateLightHud(); //drop souls on death
    }

    public void LoadCorpses()
    {
        PlayerTracker.instance.corpseTracker = 0;
        for(int i = 0; i < PlayerTracker.instance.corpsesByScenes.Count; i++)
        {
            if(PlayerTracker.instance.corpsesByScenes[i] == SceneManager.GetActiveScene().name)
            {
                PlayerTracker.instance.corpseTracker = i;
                Instantiate(corpses[PlayerTracker.instance.corpseIds[i]], PlayerTracker.instance.corpseLocation[i], Quaternion.identity);
            }
        }
    }

    IEnumerator LoadCorpsesCO()
    {
        yield return new WaitForSeconds(.1f);
        LoadCorpses();
    }
}
