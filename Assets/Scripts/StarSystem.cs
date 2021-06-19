using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class StarSystem : MonoBehaviour
{
    [Range(1000, 5000)]
    public float starSize = 1000.0f;
    [Range(0, 9)]
    public int numPlanets = 1;
    [Range(1.35f, 1.65f)]
    public float scale = 1.5f;

    public Material starMat;

    Star star;

    private void Awake()
    {
        foreach (Transform child in this.transform)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (child)
                    DestroyImmediate(child?.gameObject);
            };
        }

        CreateStar();
    }

    private void OnDestroy()
    {
        foreach (Transform child in this.transform)
            child.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (star != null)
        {
            UpdateStar();
        } else
        {
            CreateStar();
        }
    }

    void CreateStar()
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = this.transform;
        obj.name = "Star";
        obj.GetComponent<Renderer>().sharedMaterial = starMat;

        star = obj.AddComponent<Star>();
        UpdateStar();
    }

    void UpdateStar()
    {
        if (starSize != star.diameter)
        {
            star.SetDiameter(starSize);
            star.SetColor(1000, 5000);
        }

        star.SetPlanets(numPlanets, scale);

        star.UpdateStar();
    }
}