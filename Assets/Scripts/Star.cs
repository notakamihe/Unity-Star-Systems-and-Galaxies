using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
[RequireComponent(typeof(Light))]
public class Star : CelestialBody
{
    public int numPlanets = 1;

    Renderer renderer;
    Light light;

    GameObject[] planetOrbits;
    List<GameObject> belts = new List<GameObject>();
    List<GameObject> moonOrbits = new List<GameObject>();
    List<GameObject> boundary = new List<GameObject>(); 

    int orbitDirection;

    protected override void OnEnable()
    {
        base.OnEnable();

        renderer = GetComponent<Renderer>();

        light = GetComponent<Light>();
        light.type = LightType.Point;

        rb.useGravity = false;
        rb.isKinematic = true;

        orbitDirection = Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;

        SetDiameter(diameter);
    }

    public void Clear()
    {
        if (planetOrbits != null)
        {
            foreach (GameObject planet in this.planetOrbits)
            {
                UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(planet);
            }
        }

        foreach (GameObject moon in this.moonOrbits)
        {
            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(moon);
        }

        foreach (GameObject belt in this.belts)
        {
            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(belt);
        }

        foreach (GameObject boundary in this.boundary)
        {
            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(boundary);
        }

        this.moonOrbits.Clear();
        this.belts.Clear();
        this.boundary.Clear();
    }

    Belt CreateBelt(Transform parent, float distance, float thickness, float minOrbitSpeed, float maxOrbitSpeed, float minDiameter, float maxDiameter)
    {
        GameObject obj = new GameObject("Belt");
        obj.transform.position = this.transform.position;
        obj.transform.parent = parent;

        Belt belt = obj.AddComponent<Belt>();
        belt.asteroidPrefab = Singleton.instance.asteroidPrefab;
        belt.cubeDensity = 150;
        belt.innerRadius = distance;
        belt.outerRadius = distance + thickness;
        belt.height = 15.0f;
        belt.minOrbitSpeed = minOrbitSpeed;
        belt.maxOrbitSpeed = maxOrbitSpeed;
        belt.minDiameter = minDiameter;
        belt.maxDiameter = maxDiameter;

        belt.Initialize();

        return belt;
    }

    ParticleSystem CreateBoundary(float distance, float size)
    {
        GameObject obj = new GameObject("Boundary");
        obj.transform.position = this.transform.position;
        obj.transform.parent = this.transform.parent;

        Vector3 newEuler = new Vector3(-90.0f, 0.0f, 0.0f);
        obj.transform.eulerAngles = newEuler;

        ParticleSystem particleSystem = obj.AddComponent<ParticleSystem>();

        ParticleSystem.MainModule main = particleSystem.main;
        Color boundaryDustColor = Color.HSVToRGB(Random.Range(0.0f, 1f), Random.Range(0.0f, 1f), Random.Range(0.5f, 1f));
        boundaryDustColor.a = 0.25f;
        main.startLifetime = 3.0f;
        main.startSize = size;
        main.startColor = boundaryDustColor;

        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.rateOverTime = 500.0f;

        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = distance;
        shape.radiusThickness = 0.0f;

        ParticleSystem.NoiseModule noise = particleSystem.noise;
        noise.enabled = true;

        ParticleSystemRenderer particleSystemRenderer = obj.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.sharedMaterial = Singleton.instance.boundaryMat;

        return particleSystem;
    }

    public Moon CreateMoon(string name, Transform parent, float size, float axialTilt, float dayLength)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        obj.name = name;
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;

        Moon satellite = obj.AddComponent<Moon>();
        satellite.SetDiameter(size);
        satellite.SetTilt(axialTilt);
        satellite.dayLength = dayLength;
        satellite.rb.drag = 100.0f;

        return satellite;
    }

    void CreateMoons(Planet planet)
    {
        int numMoons = Mathf.FloorToInt(0.405f * (planet.mass / 1000.0f) - 1.216f);

        for (int i = 0; i < numMoons; i++)
        {
            float distance = Mathf.Pow(1.15f, i + 30) - 50.0f;
            Vector3 orbitSpawnLocation;
            
            if (planet.TryGetComponent(out Ring ring))
            {
                orbitSpawnLocation = planet.transform.position + planet.transform.forward * 
                    (ring.thickness * planet.diameter + ring.innerRadius * planet.diameter) + planet.transform.forward * distance;
            } else
            {
                orbitSpawnLocation = planet.transform.position + planet.transform.forward * ((planet.diameter / 2) + distance);
            }

            string name = GenerateWorldName();
            float size = Mathf.Min(Random.Range(0.2f, 15f), planet.transform.localScale.x * 0.25f);
            bool isRetrograde = distance > Random.Range(0.5f, 0.75f) * planet.diameter;

            Orbit orbit = CreateOrbit(name, orbitSpawnLocation, planet.transform, Mathf.Pow(1 / 1.6f, i - 11), planet.transform.up, 
                isRetrograde);
            Moon satellite = CreateMoon(name, orbit.transform, size, Random.Range(0.0f, 180.0f), 
                Random.Range(0.0001f, 100.0f));
        }
    }

    public Orbit CreateOrbit(string name, Vector3 pos, Transform parent, float orbitalPeriod, Vector3 tilt, bool isClockwise)
    {
        GameObject orbitObj = new GameObject(name + "Orbit");
        orbitObj.transform.position = pos;
        orbitObj.transform.parent = parent.parent;

        Orbit orbit = orbitObj.AddComponent<Orbit>();
        orbit.parent = parent;
        orbit.orbitalPeriod = orbitalPeriod;
        orbit.tilt = tilt;
        orbit.isClockwise = isClockwise;

        return orbit;
    }

    public Planet CreatePlanet(string name, Transform parent, float size, float axialTilt, float dayLength, float mass)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        obj.name = name;
        obj.transform.localScale = new Vector3(size, size, size);
        obj.transform.parent = parent;

        Planet planet = obj.AddComponent<Planet>();
        planet.SetDiameter(size);
        planet.SetTilt(axialTilt);
        planet.SetMass(mass);
        planet.dayLength = dayLength;

        return planet;
    }

    Ring CreateRing(Planet planet)
    {
        planet.gameObject.SetActive(false);

        Ring ring = planet.gameObject.AddComponent<Ring>();

        ring.segments = 100;
        ring.innerRadius = Random.Range(0.25f, 1.5f);
        ring.thickness = Random.Range(0.5f, 2.0f);
        SetRingTexture(ring);

        planet.gameObject.SetActive(true);

        return ring;
    }

    string GenerateWorldName ()
    {
        string[] names = File.ReadAllLines("Assets/Scripts/PlanetNames.txt");
        return names[Random.Range(0, names.Length)];
    }

    public void SetPlanets(int numPlanets, float scale)
    {
        Clear();

        this.numPlanets = numPlanets;
        planetOrbits = new GameObject[this.numPlanets];

        float distanceOffset = Random.Range(1100.0f, 1300.0f);
        System.Random random = new System.Random();
         
        for (int i = 0; i < numPlanets; i++)
        {
            float distance = Mathf.Pow(scale, i) * AU - distanceOffset;
            float size = Random.Range(0.0f, 1.0f) <= 0.35f ? Random.Range(3, 30) : Random.Range(31, 300);
            float mass = 149.495f * size + 151.515f;
            Vector3 orbitSpawnLocation = transform.position + transform.forward * (this.diameter * 0.5f + distance);
            string name = GenerateWorldName();

            Orbit orbit = CreateOrbit(name, orbitSpawnLocation, transform,
                2 * Mathf.Pow(1 / Random.Range(1.2f, 1.6f), 4 * i - 15) + 1, Vector3.up, this.orbitDirection == 1);
            planetOrbits[i] = orbit.gameObject;

            Planet planet = CreatePlanet(name, orbit.transform, size, Random.Range(0.0f, 180.0f),
                Random.Range(0.0001f, 100.0f), mass + Random.Range(-mass * 0.25f, mass * 0.25f));
            planet.transform.localPosition = Vector3.zero;

            SetPlanetTemperature(planet, distance);
            SetPlanetTexture(planet);

            if (Random.Range(1, 16) == 1 && planet.diameter > 30.0f)
            {
                Ring ring = CreateRing(planet);

                if (planet.transform.eulerAngles.x <= 90.0f || planet.transform.eulerAngles.x >= 180.0f)
                    planet.SetTilt(-1 * planet.axialTilt);
            }

            CreateMoons(planet);

            if (random.Next(1, 13) == 1 && this.belts.Count < 2)
            {
                float thickness = 300.0f;

                Belt belt = this.CreateBelt(this.transform.parent, (this.diameter * 0.5f + distance) * 1.1f, thickness, 20.0f, 30.0f, 0.1f, 5.0f);

                belts.Add(belt.gameObject);
            }

            if (i == planetOrbits.Length - 1)
            {
                ParticleSystem boundary = CreateBoundary((this.diameter * 0.5f + distance) * 1.5f, 0.274f * distance + 808.0f);
                this.boundary.Add(boundary.gameObject);
            }
        }
    }

    public void SetColor(float min, float max)
    {
        Material mat = renderer.sharedMaterial;
        float t = Mathf.InverseLerp(min, max, diameter);

        if (t < 0.5)
            mat.SetColor("_EmissionColor", Color.Lerp(Color.blue, Color.yellow, t * 2));
        else
            mat.SetColor("_EmissionColor", Color.Lerp(Color.yellow, Color.red, (t - 0.5f) * 2));
    }

    void SetPlanetTemperature (Planet planet, float distance)
    {
        if (distance <= 0.5f * AU)
            planet.temp = -775.0f * distance / 1000.0f + 800.0f;
        else if (distance <= 1.6f * AU)
            planet.temp = Mathf.Pow(1.0f / 3.0f, distance / 1000.0f - 4.82f) - 50.0f;
        else
            planet.temp = Mathf.Pow(1.0f / 1.03f, distance / 1000.0f - 185.0f) - 250.0f;

        planet.temp *= Mathf.Max(1.0f, this.temp / 10000.0f);
    }

    void SetPlanetTexture(Planet planet)
    {
        if (Random.Range(1, 8) == 1)
        {
            planet.SetTexture(Utils.RandomChoose(Singleton.instance?.waterMats));
            return;
        }

        if (planet.diameter > 30.0f)
        {
            planet.SetTexture(Utils.RandomChoose(Singleton.instance?.gasGiantMats));
        }
        else
        {
            if (planet.temp >= 100.0f)
                planet.SetTexture(Utils.RandomChoose(Singleton.instance?.terrestrialMats.hot));
            else if (planet.temp > 0.0f)
                planet.SetTexture(Utils.RandomChoose(Singleton.instance?.terrestrialMats.habitable));
            else
                planet.SetTexture(Utils.RandomChoose(Singleton.instance?.terrestrialMats.cold));
        }
    }

    void SetRingTexture(Ring ring)
    {
        Renderer renderer = ring.transform.GetChild(0).GetComponent<Renderer>();
        renderer.sharedMaterial = Utils.RandomChoose(Singleton.instance?.ringMats);
    }

    public void UpdateStar()
    {
        light.range = -37.5f * diameter + 287500.0f;
        rb.mass = 200.0f * diameter;
        this.temp = -9.25f * diameter + 49250.0f;
    }

    private void OnDestroy()
    {
        Clear();
    }
}
