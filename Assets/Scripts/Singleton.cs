using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Singleton : MonoBehaviour
{
    public static Singleton instance;

    public static Singleton Instance
    {
        get
        {
            if (instance == null)
                instance = (Singleton) FindObjectOfType(typeof(Singleton));
            return instance;
        }
    }

    public GameObject halo;

    public Material[] gasGiantMats;
    public TerrestrialMats terrestrialMats;
    public Material[] ringMats;

    public GameObject[] asteroidPrefabs;

    public Material boundaryMat;

    public Material atmosphereMat;

    public GameObject supernovaPrefab;

    public Material blackHoleMat;
    public Material neutronStarMat;
    public GameObject blackHoleVFX;
    public GameObject neutronStarVFX;
}

[System.Serializable]
public struct TerrestrialMats
{
    public Material[] hot;
    public Material[] habitable;
    public Material[] cold;
}
