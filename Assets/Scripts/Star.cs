using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class Star : CelestialBody
{
    [HideInInspector] public StarSystem starSystem;
    public float growthSpeed = 5.0f;
    public float deathSize = 7000.0f;

    Light light;
    Behaviour halo;

    float minSize;
    float maxSize;

    bool isDead = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        renderer = GetComponent<Renderer>();

        light = GetComponent<Light>();
        light.type = LightType.Point;

        if (!this.halo)
            this.halo = this.CreateHalo();

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
        Supernova supernova = Supernova.Create(this.transform.parent, this.transform.position);
        this.starSystem.Clear(supernova.transform);
    }

    void Expand()
    {
        this.SetDiameter(this.diameter + this.growthSpeed * Time.deltaTime);
        this.UpdateStar();
        this.SetColor(this.minSize, this.maxSize);
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

            
        var tempMaterial = new Material(this.renderer.sharedMaterial);
        tempMaterial.SetColor("_EmissionColor", color);
        renderer.sharedMaterial = tempMaterial;

        SetGlow(-2.25f * this.diameter + 17250.0f, color);
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
        EditorApplication.delayCall += () => {
            if (body is Planet)
            {
                DestroyImmediate(body.transform.parent.gameObject);
                return;
            }

            if (body)
                DestroyImmediate(body.gameObject);
        };
    }

    public void UpdateStar()
    {
        this.light.range = Math.Min(25.0f * this.diameter + 225000.0f, 400000.0f);
        this.SetMass(200.0f * this.diameter);
        this.temperature = Math.Max(-9.25f * diameter + 49250.0f, 2900.0f);
    }
}
