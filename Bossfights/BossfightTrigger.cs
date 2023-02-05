using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossfightTrigger : MonoBehaviour
{
    [SerializeField] int bossFightIndex = 0;
    [SerializeField] int bossFightMusicIndex;
    [SerializeField] GameObject bossfightObject;
    [SerializeField] GameObject afterBossObject;

    void Start()
    {
        StartCoroutine(CheckIfDefeatedCO());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" || other.tag == "PlayerInvincible")
        {
            bossfightObject.SetActive(true);
            AudioManager.instance.PlayMusic(bossFightMusicIndex);
            gameObject.SetActive(false);
        }
    }

    IEnumerator CheckIfDefeatedCO()
    {
        yield return new WaitForEndOfFrame();
        if(PlayerTracker.instance.bossesDefeated[bossFightIndex])
        {
            gameObject.SetActive(false);
        }

        else //disable this / these gameobjects if boss is alive
        afterBossObject.SetActive(false);
    }
}
