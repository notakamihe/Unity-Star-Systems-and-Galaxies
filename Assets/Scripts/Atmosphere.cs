using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Atmosphere : MonoBehaviour
{
    [Range(0.01f, 2.0f)] public float thickness = 0.25f;
    [Range(0.0f, 2.0f)] public float ambience = 1.0f;
    public Color color = Color.blue;

    GameObject obj;
    World world;

    private void OnValidate()
    {
        if (this.obj)
        {
            this.SetThickness(this.thickness);
            this.SetColor(this.color);
            this.SetAmbience(this.ambience);
        }
    }

    private void Awake()
    {
        this.world = GetComponent<World>();

        if (this.world)
        {
            foreach (Transform child in this.transform)
            {
                if (child.CompareTag("Atmosphere"))
                {
                    this.obj = child.gameObject;
                    return;
                }
            }

            obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.parent = this.world.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.GetComponent<Renderer>().material = Singleton.Instance.atmosphereMat;
            obj.tag = "Atmosphere";

            this.SetThickness(this.thickness);
            this.SetColor(color);
        }
    }

    public void Destroy()
    {
        Utils.Destroy(this, this.obj);
        Utils.Destroy(this, this);
    }

    public void SetAmbience(float ambience)
    {
        this.ambience = ambience;

        Renderer renderer = obj?.GetComponent<Renderer>();

        if (renderer && renderer.sharedMaterial)
        {
            var tempMaterial = new Material(renderer.sharedMaterial);
            tempMaterial.SetFloat("_Ambient", ambience);
            renderer.sharedMaterial = tempMaterial;
        }
    }

    public void SetColor (Color color)
    {
        this.color = color;

        Renderer renderer = obj?.GetComponent<Renderer>();

        if (renderer && renderer.sharedMaterial)
        { 
            var tempMaterial = new Material(renderer.sharedMaterial);
            tempMaterial.SetColor("_Color", color);
            renderer.sharedMaterial = tempMaterial;
        }
    }

    public void SetThickness(float thickness)
    {
        this.thickness = thickness;

        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer && renderer.sharedMaterial)
        {
            var tempMaterial = new Material(renderer.sharedMaterial);
            tempMaterial.SetFloat("_Thickness", thickness);
            renderer.sharedMaterial = tempMaterial;
        }
    }

}