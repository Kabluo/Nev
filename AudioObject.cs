using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    //just calls the specified music on starting a level
    public int musicIndex;
    void Start()
    {
        AudioManager.instance.PlayMusic(musicIndex);
    }
}
