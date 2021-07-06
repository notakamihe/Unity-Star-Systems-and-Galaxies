using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public enum Remnant { NeutronStar, BlackHole };

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class Star : CelestialBody
{
    [HideInInspector] public StarSystem starSystem;
    public float growthSpeed = 5.0f;
    public float deathSize = 7000.0f;
    public Remnant remnant;

    Light light;
    Behaviour halo;

    float minSize;
    float maxSize;

    bool isDead = false;

    public static string GeneratedName
    {
        get
        {
            return Utils.SelectNameFromFile("Assets/Scripts/Resources/StarNames.txt");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        renderer = GetComponent<Renderer>();

        light = GetComponent<Light>();
        light.type = LightType.Point;

        this.InitializeHalo();

        rb.useGravity = false;
        rb.isKinematic = true;

        SetDiameter(diameter);
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            this.Expand();

            if (this.diameter >= this.deathSize && !isDead)
            {
                this.Die();
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out World world))
        {
            this.Swallow(world);
        }

        if (collider.TryGetComponent(out Asteroid asteroid))
        {
            this.Swallow(asteroid);
        }
    }

    public static Star Create(string name, StarSystem starSystem, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = starSystem.transform;
        obj.GetComponent<SphereCollider>().isTrigger = true;

        Star star = obj.AddComponent<Star>();
        star.starSystem = starSystem;
        star.SetName(name);
        star.SetMat(mat);

        star.UpdateStar();

        return star;
    }

    Behaviour CreateHalo()
    {
        GameObject obj = Instantiate(Singleton.Instance.halo, this.transform.position, Quaternion.identity, this.transform);
        obj.transform.localPosition = Vector3.zero;

        Behaviour halo = (Behaviour)obj.GetComponent("Halo");
        return halo;
    }

    void Die()
    {
        this.isDead = true;
        Supernova supernova = Supernova.Create(this.transform.parent, this.transform.position, remnant);
        this.starSystem.Clear(supernova.transform);
    }

    void Expand()
    {
        this.SetDiameter(this.diameter + this.growthSpeed * Time.deltaTime);
        this.UpdateStar();
        this.SetColor(this.minSize, this.maxSize);
    }

    void InitializeHalo()
    {
        foreach (Transform child in this.transform)
        {
            if (child.GetComponent("Halo"))
            {
                this.halo = (Behaviour)child.GetComponent("Halo");
                return;
            }
        }

        this.halo = this.CreateHalo();
    }

    public void SetColor(float min, float max)
    {
        this.minSize = min;
        this.maxSize = max;

        float t = Mathf.InverseLerp(min, max, diameter);
        Color color;

        if (t < 0.5)
            color = Color.Lerp(Color.blue, Color.yellow, t * 2);
        else
            color = Color.Lerp(Color.yellow, Color.red, (t - 0.5f) * 2);

        if (this.renderer.sharedMaterial)
        {
            var tempMaterial = new Material(this.renderer.sharedMaterial);
            tempMaterial.SetColor("_EmissionColor", color * 2.0f);
            renderer.sharedMaterial = tempMaterial;

            SetGlow(-2.25f * this.diameter + 17250.0f, color);
        }
    }

    public void SetGlow(float size, Color color)
    {
        SerializedObject haloComponent = new SerializedObject(this.halo);
        haloComponent.FindProperty("m_Size").floatValue = size;
        haloComponent.FindProperty("m_Color").colorValue = color;
        haloComponent.ApplyModifiedProperties();
    }  

    void Swallow(CelestialBody body)
    {
        if (body)
        {
            if (body is Planet)
            {
                Utils.Destroy(this, body.transform.parent.gameObject);
                return;
            }
            
            Utils.Destroy(this, body.gameObject);
        }
    }

    public void UpdateStar()
    {
        this.light.range = Math.Min(25.0f * this.diameter + 225000.0f, 400000.0f);
        this.SetMass(200.0f * this.diameter);
        this.temperature = Math.Max(-9.25f * diameter + 49250.0f, 2900.0f);
    }
}
