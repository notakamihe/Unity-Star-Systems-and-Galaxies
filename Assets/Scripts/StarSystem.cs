using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class StarSystem : MonoBehaviour
{
    [Range(1000, 5000)] public float starSize = 1000.0f;
    [Range(0, 9)] public int numPlanets = 1;
    [Range(1.35f, 1.65f)] public float scale = 1.5f;
    [Range(0.0f, 50.0f)] public float starGrowthSpeed = 5.0f;
    public bool isProgradeClockwise = false;
    public Material starMat;

    GameObject[] planets;
    List<GameObject> belts = new List<GameObject>();
    List<GameObject> dwarfPlanets = new List<GameObject>();

    Star star;

    private void Awake()
    {
        this.Clear();
        this.CreateStar();
    }

    private void OnValidate()
    {
        if (!this.star)
        {
            this.CreateStar();
        }
        else
        {
            this.Clear(this.star.transform);
            this.UpdateSystem();
        }
    }

    Atmosphere AddAtmosphere(World world, float distance)
    {
        float atmosphereByDiameter = Math.Max(-Mathf.Pow(1.3f, -world.diameter + 12.5f) + 7.5f, Random.Range(0.0f, 0.5f));
        float distanceModifier = distance / 1000.0f < 1.0f ? Mathf.Max(0, 1.583f * (distance / 1000.0f) - 0.633f) : Random.Range(0.9f, 1.25f);
        float thickness = atmosphereByDiameter * distanceModifier;
        float hueRangeByTemperature = world.temperature > 100.0f ? Random.Range(0.0f, 0.2f) : Random.Range(0.0f, 1.0f);
        Color atmosphereColor = Color.HSVToRGB(hueRangeByTemperature, Random.Range(0.0f, 1f), Random.Range(0.75f, 1f));

        world.temperature += Mathf.Pow(1.95f, thickness);

        Atmosphere atmosphere = world.AddAtmosphere(thickness, atmosphereColor);
        return atmosphere;
    }

    public void Clear(Transform target = null)
    {
        foreach (Transform child in this.transform)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (child != target)
                    DestroyImmediate(child?.gameObject);
            };
        }

        this.belts.Clear();
        this.dwarfPlanets.Clear();
    }

    void CreateMoons(Planet planet, float distanceFromStar, float distanceScale = 1.0f)
    {
        int numMoons = Mathf.FloorToInt((0.405f * (planet.mass / 1000.0f) + 0.6f) *
            -(Mathf.Pow(10.0f, -distanceFromStar / 1000.0f) - 1.05f));

        for (int i = 0; i < numMoons; i++)
        {
            float distance = distanceScale * (Mathf.Pow(1.15f, i + 30) - 50.0f);
            float regularMoonZone = planet.diameter * Random.Range(0.5f, 0.75f);

            if (planet.TryGetComponent(out Ring ring))
            {
                regularMoonZone += ring.innerRadius * planet.diameter + ring.thickness * planet.diameter;
                distance += ring.innerRadius * planet.diameter + ring.thickness * planet.diameter;
            }

            string name = World.GeneratedName;
            bool isIrregular = distance > regularMoonZone;
            float diameter;
            Vector3 orbitalAngleAxis;

            if (isIrregular)
            {
                diameter = Mathf.Min(Random.Range(0.2f, 5f), planet.diameter * 0.25f);
                orbitalAngleAxis = Random.insideUnitSphere.normalized;
            }
            else
            {
                diameter = Mathf.Min(Random.Range(0.2f, 15f), planet.diameter * 0.25f);
                orbitalAngleAxis = planet.transform.up;
            }

            float mass = 149.495f * diameter + 151.515f;

            Moon moon = planet.AddMoon(distance, Mathf.Pow(1 / 1.6f, i - 11), name, diameter, Random.Range(0.0f, 180.0f),
                Random.Range(0.0001f, 100.0f), mass + Random.Range(-mass * 0.25f, mass * 0.25f), orbitalAngleAxis, isIrregular);
            float moonDistanceFromStar = this.star.DistanceFromSurface(moon.transform.position);

            this.SetWorldTemperature(moon, moonDistanceFromStar);

            if (Random.Range(1, 51) == 1)
                this.AddAtmosphere(moon, moonDistanceFromStar);
        }
    }

    void CreateStar()
    {
        this.star = Star.Create("Star", this, this.starMat);
        this.star.transform.localPosition = Vector3.zero;
        this.UpdateSystem();
    }

    void CreateSystem()
    {
        this.planets = new GameObject[this.numPlanets];

        System.Random random = new System.Random();
        float distanceOffset = Random.Range(1100.0f, 1300.0f);
        float beltOffset = 0.0f;

        for (int i = 0; i < this.numPlanets; i++)
        {
            float distance = Mathf.Pow(scale, i) * CelestialBody.AU - distanceOffset + beltOffset;
            float size = Random.Range(0.0f, 1.0f) <= 0.35f ? Random.Range(3, 30) : Random.Range(31, 300);
            float mass = 149.495f * size + 151.515f;
            float orbitSpeed = 2 * Mathf.Pow(1 / Random.Range(1.2f, 1.6f), 4 * i - 15) + 1;
            Vector3 orbitSpawnLocation = this.star.transform.position + this.star.transform.forward * (this.star.Radius + distance);
            string name = World.GeneratedName;

            Orbit orbit = Orbit.Create(name, orbitSpawnLocation, this.star.transform.parent, orbitSpeed, Vector3.up, this.isProgradeClockwise);
            planets[i] = orbit.gameObject;

            Planet planet = World.Create<Planet>(name, orbit.transform, size, Random.Range(0.0f, 180.0f),
                Random.Range(0.0001f, 100.0f), mass + Random.Range(-mass * 0.25f, mass * 0.25f));
            planet.transform.localPosition = Vector3.zero;

            SetWorldTemperature(planet, distance);
            SetPlanetTexture(planet);

            float moonDistanceScale = 1.0f;

            if (planet.diameter > 30.0f)
            {
                if (Random.Range(1, 16) == 1)
                    planet.AddRing(Random.Range(0.25f, 1.5f), Random.Range(0.5f, 2.0f), Utils.RandomChoose(Singleton.Instance?.ringMats));
            }
            else
            {
                moonDistanceScale = 0.021f * size - 0.039f;
                this.AddAtmosphere(planet, distance);
            }

            this.CreateMoons(planet, distance, moonDistanceScale);

            if (random.Next(1, 13) == 1 && this.belts.Count < 2)
            {
                float thickness = i >= numPlanets / 4 * 3 ? Random.Range(10000.0f, 25000.0f) : Random.Range(100.0f, 5000.0f);
                float minOrbitSpeed = orbitSpeed * 0.1f, maxOrbitSpeed = orbitSpeed * 1.5f;
                GameObject prefab = Utils.RandomChoose(Singleton.Instance.asteroidPrefabs);

                Belt belt = Belt.Create(this.transform, this.transform.position, Random.Range(100, 200),
                    (this.star.Radius + distance * 1.1f), thickness, minOrbitSpeed, maxOrbitSpeed, Random.Range(5.0f, 50.0f), 0.01f, 50.0f, 
                    0.1f, 1.0f, prefab);

                beltOffset += thickness * 1.1f;
                distance += thickness * 1.1f;
                this.belts.Add(belt.gameObject);

                int numDwarfPlanets = Random.Range(0, 7);
                float minDistance = belt.innerRadius;

                for (int j = 0; j < numDwarfPlanets; j++)
                {
                    float dpDistance = Random.Range(minDistance, belt.outerRadius);
                    minDistance = dpDistance;

                    string dpName = World.GeneratedName;
                    float dpDiameter = Random.Range(1.0f, 3.0f);
                    float dpMass = 149.495f * dpDiameter + 151.515f;
                    Vector3 dpOrbitSpawnLocation = this.star.transform.position + this.star.transform.forward * dpDistance;

                    Orbit dpOrbit = Orbit.Create(dpName, dpOrbitSpawnLocation, this.transform, orbitSpeed * (-0.033f * i + 0.95f),
                        Vector3.up + Vector3.right * Random.Range(0.0f, 0.5f), this.isProgradeClockwise); ;

                    DwarfPlanet dp = World.Create<DwarfPlanet>(dpName, dpOrbit.transform, dpDiameter, Random.Range(0.0f, 180.0f),
                        Random.Range(0.0001f, 100.0f), dpMass + Random.Range(-dpMass * 0.25f, dpMass * 2.0f));
                    dp.transform.localPosition = Vector3.zero;

                    this.dwarfPlanets.Add(dpOrbit.gameObject);

                    this.SetWorldTemperature(dp, dpDistance);
                    this.SetPlanetTexture(dp);
                    this.CreateMoons(dp, dpDistance, 0.075f);
                    this.AddAtmosphere(dp, dpDistance);
                }
            }

            if (i == planets.Length - 1)
            {
                Boundary boundary = Boundary.Create(this.transform, this.transform.position,
                    (this.star.Radius + distance) * 1.25f, 0.274f * distance + 808.0f, Color.HSVToRGB(Random.Range(0.0f, 1f), 
                    Random.Range(0.0f, 1f), Random.Range(0.5f, 1f)));
            }
        }
    }

    void SetWorldTemperature(World world, float distance)
    {
        if (distance <= 0.5f * CelestialBody.AU)
            world.temperature = -775.0f * distance / 1000.0f + 800.0f;
        else if (distance <= 1.6f * CelestialBody.AU)
            world.temperature = Mathf.Pow(1.0f / 3.0f, distance / 1000.0f - 4.82f) - 50.0f;
        else
            world.temperature = Mathf.Pow(1.0f / 1.03f, distance / 1000.0f - 185.0f) - 250.0f;

        if (world.temperature > 0)
            world.temperature *= Mathf.Max(1.03f, this.star.temperature / 1000.0f - 2.0f);
    }

    void SetPlanetTexture(Planet planet)
    {
        if (planet.diameter > 30.0f)
        {
            planet.SetMat(Utils.RandomChoose(Singleton.Instance.gasGiantMats));
        }
        else
        {
            if (planet.temperature >= 100.0f)
                planet.SetMat(Utils.RandomChoose(Singleton.Instance.terrestrialMats.hot));
            else if (planet.temperature > 0.0f)
                planet.SetMat(Utils.RandomChoose(Singleton.Instance.terrestrialMats.habitable));
            else
                planet.SetMat(Utils.RandomChoose(Singleton.Instance.terrestrialMats.cold));
        }
    }

    void UpdateSystem()
    {
        this.star.SetDiameter(this.starSize);
        this.star.SetColor(1000.0f, 5000.0f);
        this.star.UpdateStar();
        this.star.growthSpeed = this.starGrowthSpeed;

        float deathSize = 0.75f * this.star.diameter + 6250.0f;
        this.star.deathSize = deathSize + Random.Range(-deathSize * 0.1f, deathSize * 0.1f);

        this.CreateSystem();
    }
}