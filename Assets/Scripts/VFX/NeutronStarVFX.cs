using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class NeutronStarVFX : MonoBehaviour
{
    public ParticleSystem[] beams;

    public void SetBeams(float size)
    {
        foreach (ParticleSystem beam in beams)
        {
            beam.transform.localScale = Vector3.one * size;
        }
    }
}