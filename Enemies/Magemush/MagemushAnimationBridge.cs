using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagemushAnimationBridge : MonoBehaviour
{
    [SerializeField] MagemushBehaviour behaviourScript;

    public void Firebolt()
    {
        behaviourScript.InstFireball();
    }

    public void Lightningstrike()
    {
        behaviourScript.CallLightning();
    }

    public void Teleport()
    {
        behaviourScript.PerformTeleport();
    }
}
