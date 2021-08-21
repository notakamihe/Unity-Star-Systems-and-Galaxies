using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BlackHoleVFX : MonoBehaviour
{
    public ParticleSystem accretionDisk;
    public Transform[] jetPivots;

    public void SetAccretionDisk(float size)
    {
        this.accretionDisk.transform.localScale = Vector3.one * size;
    }

    public void SetJets(float size)
    {
        foreach (Transform pivot in this.jetPivots)
        {
           pivot.transform.localScale = Vector3.one * size;
           pivot.localPosition = Vector3.zero;
        }
    }
}