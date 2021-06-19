using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Singleton : MonoBehaviour
{
    public static Singleton instance;

    private void OnEnable()
    {
        instance = this;
    }

    public Material[] gasGiantMats;
    public Material[] waterMats;
    public TerrestrialMats terrestrialMats;
    public Material[] ringMats;

    public GameObject asteroidPrefab;

    public Material boundaryMat;
}

[System.Serializable]
public struct TerrestrialMats
{
    public Material[] hot;
    public Material[] habitable;
    public Material[] cold;
}
