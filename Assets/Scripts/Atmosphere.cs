using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Atmosphere : MonoBehaviour
{
    public float thiccness = 4f;
    public Color color = Color.blue;

    GameObject obj;
    World world;

    private void Awake()
    {
        this.world = GetComponent<World>();

        if (this.world)
        {
            obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.parent = this.world.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one * 1.15f;
            obj.GetComponent<Renderer>().material = Singleton.Instance.atmosphereMat;

            this.SetThiccness(this.thiccness);
            this.SetColor(color);
        }
    }

    private void OnValidate()
    {
        this.SetThiccness(this.thiccness);
        this.SetColor(this.color);
    }

    public void SetThiccness(float thiccness)
    {
        this.thiccness = thiccness;

        Renderer renderer = obj?.GetComponent<Renderer>();

        if (renderer)
        {
            var tempMaterial = new Material(renderer.sharedMaterial);
            tempMaterial.SetFloat("_Thickness", thiccness);
            renderer.sharedMaterial = tempMaterial;
        }
    }

    public void SetColor (Color color)
    {
        this.color = color;

        Renderer renderer = obj?.GetComponent<Renderer>();

        if (renderer)
        { 
            var tempMaterial = new Material(renderer.sharedMaterial);
            tempMaterial.SetColor("_Color", color);
            renderer.sharedMaterial = tempMaterial;
        }
    }
}