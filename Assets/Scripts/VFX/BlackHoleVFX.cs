using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BlackHoleVFX : MonoBehaviour
{
    public ParticleSystem accretionDisk;
    public ParticleSystem[] jets;

    public void SetAccretionDisk(float size)
    {
        this.accretionDisk.transform.localScale = Vector3.one * size;
    }

    public void SetJets(float size)
    {
        foreach (ParticleSystem jet in jets)
        {
            jet.transform.localScale = new Vector3(1.0f, 5.0f, 1.0f) * size;
        }
    }
}