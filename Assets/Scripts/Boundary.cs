using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class Boundary : MonoBehaviour
{
    public float diamater = 10000.0f;
    public Color color;
    public float particleSize = 10.0f;

    public ParticleSystem particleSystem;

    private void OnEnable()
    {
        this.particleSystem = GetComponent<ParticleSystem>();

        Vector3 newEuler = new Vector3(-90.0f, 0.0f, 0.0f);
        this.transform.eulerAngles = newEuler;
    }

    private void Update()
    {

    }

    public static Boundary Create(Transform parent, Vector3 position, float diameter, float size, Color color)
    {
        GameObject obj = new GameObject("Boundary");
        obj.transform.parent = parent;
        obj.transform.position = position;

        Boundary boundary = obj.AddComponent<Boundary>();
        boundary.diamater = diameter;
        boundary.particleSize = size;
        color.a = 0.1f;
        boundary.color = color;

        boundary.Initialize();

        return boundary;
    }

    void Initialize()
    {
        ParticleSystem.MainModule main = this.particleSystem.main;
        main.startLifetime = 3.0f;
        main.startSize = this.particleSize;
        main.startColor = this.color;

        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = 500.0f;

        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = this.diamater;
        shape.radiusThickness = 0.0f;

        ParticleSystem.NoiseModule noise = particleSystem.noise;
        noise.enabled = true;

        ParticleSystem.TextureSheetAnimationModule tsa = particleSystem.textureSheetAnimation;
        tsa.enabled = true;
        tsa.numTilesX = 4;
        tsa.numTilesY = 4;
        tsa.cycleCount = 15;

        ParticleSystemRenderer particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.sharedMaterial = Singleton.Instance.boundaryMat;
    }
}