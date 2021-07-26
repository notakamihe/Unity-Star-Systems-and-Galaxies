using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class PlanetaryNebula : MonoBehaviour
{
    public float starRadius = 1000.0f;
    public Color color1;
    public Color color2;

    public ParticleSystem particleSystem;

    private void OnEnable()
    {
        this.particleSystem = this.GetComponent<ParticleSystem>();
    }

    private void OnValidate()
    {
        this.SetRadius(this.starRadius);
        this.SetColors(this.color1, this.color2);
    }

    public static PlanetaryNebula Create(Transform parent, Vector3 position, float starRadius, Color color1, Color color2)
    {
        WhiteDwarf whiteDwarf = WhiteDwarf.Create(parent, position, Random.Range(8.0f, 20.0f), 
            Random.Range(Units.SOLAR_MASS * 0.15f, Units.SOLAR_MASS * 1.4f));

        GameObject obj = Instantiate(Singleton.Instance.planetaryNebulaVFX, position, Quaternion.identity, whiteDwarf.transform);
        PlanetaryNebula planetaryNebula = obj.GetComponent<PlanetaryNebula>();

        planetaryNebula.SetRadius(starRadius);
        planetaryNebula.SetColors(color1, color2);

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        return planetaryNebula;
    }

    public void SetRadius(float starRadius)
    {
        this.starRadius = starRadius;

        if (this.particleSystem)
        {
            ParticleSystem.ShapeModule shape = this.particleSystem.shape;
            shape.radius = starRadius;

            ParticleSystem.MainModule main = this.particleSystem.main;
            main.startSize = new ParticleSystem.MinMaxCurve(3.2f * starRadius, 5.6f * starRadius);
        }
    }

    public void SetColors(Color color1, Color color2)
    {
        this.color1 = color1;
        this.color2 = color2;

        if (this.particleSystem)
        {
            ParticleSystem.MainModule main = this.particleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient(color1, color2);
        }
    }
}
