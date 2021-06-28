using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;


public class World : CelestialBody
{
    public float dayLength = 1.0f;
    public float axialTilt = 0.0f;

    public float Gravity
    {
        get
        {
            float gravitationalFieldStrength = (G * this.mass) / Mathf.Pow(this.diameter * 0.5f, 2);
            return gravitationalFieldStrength * Mathf.Pow(1.25f, this.mass / 1000.0f - 20.0f) * 0.5f;
        }
    }
    
    public static string GeneratedName
    {
        get
        {
            string[] names = File.ReadAllLines("Assets/Scripts/PlanetNames.txt");
            return Utils.RandomChoose(names);
        }
    }

    public bool IsHabitable
    {
        get
        {
            return 0.0f <= this.temperature && this.temperature <= 100.0f;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        rb.isKinematic = true;
        this.SetDiameter(diameter);
        this.SetTilt(axialTilt);
    }

    protected override void FixedUpdate()
    {
        transform.Rotate(0, -dayLength, 0);
    }
    
    public static T Create<T>(string name, Transform parent, float diameter, float axialTilt, float dayLength, float mass) where T : World
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = parent;

        T world = obj.AddComponent<T>();
        world.SetName(name);
        world.SetDiameter(diameter);
        world.SetTilt(axialTilt);
        world.SetMass(mass);
        world.dayLength = dayLength;

        return world;
    }

    public Atmosphere AddAtmosphere(float thickness, Color color)
    {
        Atmosphere atmosphere = this.gameObject.AddComponent<Atmosphere>();
        atmosphere.SetThiccness(thickness);
        atmosphere.SetColor(color);

        return atmosphere;
    }

    public void SetTilt(float axialTilt)
    {
        this.axialTilt = axialTilt;

        Vector3 newTilt = new Vector3(axialTilt, 0, 0);
        transform.localEulerAngles = newTilt;
    }
}