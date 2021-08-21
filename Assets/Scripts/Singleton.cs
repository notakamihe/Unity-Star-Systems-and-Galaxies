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

    public float timeScale = 0.00001f;

    public Material starMat;
    public Material atmosphereMat;
    public Material whiteDwarfMat;
    public Material blackHoleMat;
    public Material neutronStarMat;

    public Material[] gasGiantMats;
    public TerrestrialMats terrestrialMats;
    public Material[] ringMats;

    public GameObject halo;
    public GameObject blackHoleVFX;
    public GameObject neutronStarVFX;
    public GameObject planetaryNebulaVFX;
    public GalaxyVFX galaxyVFX;

    public GameObject supernovaPrefab;
    public GameObject[] asteroidPrefabs;
}

[System.Serializable]
public struct TerrestrialMats
{
    public Material[] hot;
    public Material[] habitable;
    public Material[] cold;
}
