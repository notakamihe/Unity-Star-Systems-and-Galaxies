using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public enum Remnant { Nebula, NeutronStar, BlackHole };

public enum StarType {
    RedDwarf,
    YellowDwarf,
    AMainSequence,
    BMainSequence,
    Giant,
    OMainSequence,
    BlueSupergiant,
    RedSupergiant
}

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class Star : CelestialBody
{
    [HideInInspector] public StarSystem starSystem;
    public float growthSpeed = 5.0f;
    public float deathSize = 20000.0f;
    public StarType type;
    public Remnant remnant;
    public Color color;
    public float luminosity = 1.0f;

    Light light;
    Behaviour halo;

    bool isDead = false;
    float startDiameter;

    public static string GeneratedName
    {
        get
        {
            return Utils.SelectNameFromFile("Assets/Scripts/Resources/StarNames.txt");
        }
    }

    Color CurrentColor
    {
        get
        {
            return Color.Lerp(this.color, Color.red, this.LifetimePercentage);
        }
    }

    public bool IsMassive
    {
        get
        {
            return this.type >= StarType.BMainSequence && this.type != StarType.Giant;
        }
    }

    public float LifetimePercentage
    {
        get
        {
            return Mathf.InverseLerp(this.startDiameter, this.deathSize, this.diameter);
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

    protected override void FixedUpdate()
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

    public static Star Create(string name, StarSystem starSystem, Material mat, float diameter, float mass, float temperature, float luminosity, 
        Color color, float growthSpeed, float deathSize, StarType type, Remnant remnant)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = starSystem.transform;
        obj.GetComponent<SphereCollider>().isTrigger = true;

        Star star = obj.AddComponent<Star>();
        star.starSystem = starSystem;
        star.SetName(name);
        star.SetMat(mat);
        star.SetDiameter(diameter);
        star.SetMass(mass);
        star.SetLuminosity(luminosity);
        star.SetColor(color);

        star.growthSpeed = growthSpeed;
        star.deathSize = deathSize;
        star.type = type;
        star.remnant = remnant;
        star.temperature = temperature;

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

        switch (this.remnant)
        {
            case Remnant.Nebula:
                PlanetaryNebula nebula = PlanetaryNebula.Create(this.transform.parent, this.transform.position, this.Radius,
                    new Color((float)Utils.random.NextDouble(), (float)Utils.random.NextDouble(), (float)Utils.random.NextDouble(),
                        (float)Utils.random.NextDouble() * 0.25f),
                    new Color((float)Utils.random.NextDouble(), (float) Utils.random.NextDouble(), (float) Utils.random.NextDouble(), 
                        (float) Utils.random.NextDouble() * 0.25f));
                this.starSystem.Die(nebula.transform.parent);
                break;
            default:
                Supernova supernova = Supernova.Create(this, this.transform.parent, remnant);
                this.starSystem.Die(supernova.transform);
                break;
        }
    }

    void Expand()
    {
        this.SetDiameter(this.diameter + this.growthSpeed * Time.deltaTime * Singleton.Instance.timeScale);
        this.UpdateColor();
        this.UpdateOtherProperties();
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

    public void SetColor(Color color)
    {
        this.color = color;
        this.renderer.material.SetColor("_EmissionColor", color * 2.0f);
    }

    void SetGlow(Color color)
    {
        SerializedObject haloComponent = new SerializedObject(this.halo);
        haloComponent.FindProperty("m_Color").colorValue = color;
        haloComponent.ApplyModifiedProperties();
    }

    public void SetLuminosity(float luminosity)
    {
        this.luminosity = Mathf.Max(1.0f, luminosity);

        float sizeByLuminosity = -Mathf.Pow(1.0000652f, -luminosity / Units.SOLAR_LUMINOSITY + 21000.0f) + 5.0f;

        SerializedObject haloComponent = new SerializedObject(this.halo);
        haloComponent.FindProperty("m_Size").floatValue = Mathf.Max(this.diameter * sizeByLuminosity, this.diameter);
        haloComponent.ApplyModifiedProperties();

        this.SetGlow(this.CurrentColor);
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

    void UpdateColor()
    {
        this.SetGlow(this.CurrentColor);

        if (this.renderer.sharedMaterial)
        {
            this.renderer.material.SetColor("_EmissionColor", this.CurrentColor * 2.0f);
        }
    }

    public void UpdateStar()
    {
        this.startDiameter = this.diameter;
        this.SetGlow(this.CurrentColor);

        if (this.light)
            this.light.range = Mathf.Min(25.0f * this.diameter + 225000.0f, 400000.0f) * 2.0f;
    }
    
    public void UpdateOtherProperties()
    {
        this.starSystem.UpdateStarProperties();
    }
}
