using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushgrowthAnimationBridge : MonoBehaviour
{
    [SerializeField] MushgrowthBehaviour behaviourScript;

    public void GraspingVine()
    {
        behaviourScript.SummonVine();
    }

    public void SummonSpike()
    {
        behaviourScript.SummonSpike();
    }
}
