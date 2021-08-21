using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class ProceduralStarSystem : StarSystem
{
    [HideInInspector] public List<Planet> planets = new List<Planet>();
    [HideInInspector] public List<Belt> belts = new List<Belt>();
    [HideInInspector] public List<DwarfPlanet> dwarfPlanets = new List<DwarfPlanet>();

    float starStartTemperature;
    float starStartLuminosity;
    float starStartMass;
    float starEndTemperature = Utils.NextFloat(2000.0f, 4000.0f);
    float starEndLuminosity;
    float starEndMass;

    float farthestDistance = 100000.0f;

    bool initialized = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!this.initialized)
        {
            this.initialized = true;

            this.Clear();
            this.star = this.CreateStar();
            this.UpdateSystem();
            this.SetSize();
        }
    }

    Atmosphere AddAtmosphere(World world, float distance)
    {
        float atmosphereByDiameter = Mathf.Max((-Mathf.Pow(1.12f, -world.diameter + 18.0f) + 6.5f) * 0.1f, Utils.NextFloat(0.0f, 0.05f));
        float distanceModifier = distance / Units.AU < 1.0f ? Mathf.Pow(2.0f, distance / Units.AU) + 1.0f : Utils.NextFloat(0.9f, 1.25f);
        float thickness = atmosphereByDiameter * distanceModifier;
        float hueRangeByTemperature = world.temperature > 373.15f ? Utils.NextFloat(0.0f, 0.2f) : Utils.NextFloat(0.0f, 1.0f);
        Color atmosphereColor = Color.HSVToRGB(hueRangeByTemperature, Utils.NextFloat(0.0f, 1f), Utils.NextFloat(0.75f, 1f));
        
        Atmosphere atmosphere = world.AddAtmosphere(thickness, atmosphereColor, Utils.NextFloat(0.1f, 1.0f));

        world.temperature += Mathf.Max(Mathf.Pow(1.95f, thickness * 10.0f) * 0.25f, 100.0f);

        if (world.TryGetComponent(out Planet planet))
            this.SetPlanetTexture(planet);

        return atmosphere;
    }

    public override void Clear(Transform target = null)
    {
        foreach (Transform child in this.transform)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (child && child != target)
                    DestroyImmediate(child?.gameObject);
            };
        }

        this.planets.Clear();
        this.belts.Clear();
        this.dwarfPlanets.Clear();
    }

    void CreateMoons(Planet planet, float distanceFromStar, float distanceScale = 1.0f)
    {
        int numMoons;

        if (planet.mass >= Units.JUPITER_MASS)
            numMoons = Mathf.RoundToInt(Mathf.Pow(1.13f, planet.mass / Units.JUPITER_MASS) + 9.0f);
        else if (planet.mass >= Units.EARTH_MASS)
            numMoons = Mathf.RoundToInt(-Mathf.Pow(0.927f, planet.mass / Units.EARTH_MASS - 30.0f) + 10.0f);
        else
            numMoons = Mathf.RoundToInt(-Mathf.Pow(1000.0f, -planet.mass / Units.EARTH_MASS) + 1.0f);

        numMoons = (int) ((float) numMoons * (-Mathf.Pow(1000.0f, 0.5f - distanceFromStar / Units.AU) + Utils.NextFloat(1.0f, 2.0f)));

        float prevDistance = Utils.NextFloat(1.0f, 40.0f);
        float regularMoonZone = planet.diameter * Random.Range(1.0f, 3.0f);

        for (int i = 0; i < numMoons; i++)
        {
            float distance = prevDistance * distanceScale;
            float additionalDistance = 0.0f;

            prevDistance += 15.0f * Utils.NextFloat(1.0f, 3.0f);

            if (planet.TryGetComponent(out Ring ring))
            {
                additionalDistance = ring.innerRadius * planet.diameter + ring.thickness * planet.diameter;
                regularMoonZone += additionalDistance;
            }

            string name = $"{planet.name} {Utils.ToRoman(i+1)}";
            bool isIrregular = distance + additionalDistance > regularMoonZone;
            float diameter;
            float inclination;

            if (isIrregular)
            {
                diameter = Mathf.Min(Random.Range(0.2f, 5f), planet.diameter * 0.25f);
                inclination = Utils.NextFloat(0.0f, 180.0f);
            }
            else
            {
                diameter = Mathf.Min(Random.Range(0.2f, 15f), planet.diameter * 0.25f);
                inclination = Utils.NextFloat(0.0f, 5.0f);
            }

            distance += diameter / 2.0f;

            float mass = Mathf.Pow(1.5f, diameter - 13.0f) * Units.EARTH_MASS * Utils.NextFloat(0.5f, 1.5f);
            float day = Utils.NextFloat(0.0f, 1.0f) < 0.75f ? Utils.NextFloat(8.0f, 100.0f) : Utils.NextFloat(101.0f, 10000.0f);

            float gm = (6.67f * Mathf.Pow(10.0f, -11.0f)) * (planet.mass * Mathf.Pow(10.0f, 22.0f));
            float distanceInMetersCubed = Mathf.Pow((distance + additionalDistance) * Mathf.Pow(10.0f, 4.0f) * 1000.0f, 3.0f);
            float secondsSquared = ((4.0f * Mathf.Pow(Mathf.PI, 2.0f)) / gm) * distanceInMetersCubed;
            float days = Mathf.Sqrt(secondsSquared) / 86400.0f;

            Moon moon = planet.AddMoon(distance + additionalDistance, days, name, diameter, Utils.NextFloat(0.0f, 5.0f), day, mass, 
                planet.transform.up, isIrregular, inclination, Utils.NextFloat(0.0f, 1.0f) <= 0.9f);
            float moonDistanceFromStar = this.star.DistanceFromSurface(moon.transform.position);

            this.SetWorldTemperature(moon, moonDistanceFromStar);

            if (Utils.random.Next(1, 51) == 1)
                this.AddAtmosphere(moon, moonDistanceFromStar);
        }
    }

    public Star CreateStar()
    {
        string name = Star.GeneratedName;
        float diameter;
        StarType type;
        Color color;
        float mass;
        float luminosity;
        Remnant remnant;
        float growthSpeed;
        float deathSize;
        float temperature;

        float percentage = Utils.NextFloat(0.0f, 1.0f);

        if (percentage <= 0.01f)
        {
            diameter = Utils.NextFloat(150000.0f, 200000.0f);
            type = StarType.RedSupergiant;
            color = SpectralType.Interpolate(SpectralType.K, SpectralType.M);
            mass = Utils.NextFloat(10.0f, 40.0f) * Units.SOLAR_MASS;
            luminosity = Utils.NextFloat(10000.0f, 100000.0f) * Units.SOLAR_LUMINOSITY;
            remnant = (Remnant)Utils.random.Next(1, 3);
            growthSpeed = Utils.NextFloat(9.0f, 10.0f);
            temperature = Utils.NextFloat(3500.0f, 4000.0f);
        }
        else if (percentage <= 0.02f)
        {
            diameter = Utils.NextFloat(125000.0f, 150000.0f);
            type = StarType.BlueSupergiant;
            color = SpectralType.Interpolate(SpectralType.O, SpectralType.A);
            mass = Utils.NextFloat(10.0f, 20.0f) * Units.SOLAR_MASS;
            luminosity = Utils.NextFloat(10000.0f, 100000.0f) * Units.SOLAR_LUMINOSITY;
            remnant = (Remnant)Utils.random.Next(1, 3);
            growthSpeed = Utils.NextFloat(8.0f, 9.0f);
            temperature = Utils.NextFloat(20000.0f, 50000.0f);
        }
        else if (percentage <= 0.05f)
        {
            diameter = Utils.NextFloat(100000.0f, 125000.0f);
            type = StarType.OMainSequence;
            color = SpectralType.Interpolate(SpectralType.O, SpectralType.B);
            mass = Utils.NextFloat(15.0f, 90.0f) * Units.SOLAR_MASS;
            luminosity = Utils.NextFloat(10000.0f, 100000.0f) * Units.SOLAR_LUMINOSITY;
            remnant = (Remnant)Utils.random.Next(1, 3);
            growthSpeed = Utils.NextFloat(8.0f, 9.0f);
            temperature = Utils.NextFloat(30000.0f, 50000.0f);
        }
        else if (percentage <= 0.09f)
        {
            diameter = Utils.NextFloat(75000.0f, 100000.0f);
            type = StarType.Giant;
            color = SpectralType.Interpolate(SpectralType.G, SpectralType.M);
            mass = Utils.NextFloat(0.3f, 8.0f) * Units.SOLAR_MASS;
            luminosity = Utils.NextFloat(100.0f, 10000.0f) * Units.SOLAR_LUMINOSITY;
            remnant = Remnant.Nebula;
            growthSpeed = Utils.NextFloat(5.0f, 8.0f);
            temperature = Utils.NextFloat(2000.0f, 3000.0f);
        }
        else if (percentage <= 0.11f)
        {
            diameter = Utils.NextFloat(50000.0f, 75000.0f);
            type = StarType.BMainSequence;
            color = SpectralType.Interpolate(SpectralType.B, SpectralType.A);
            mass = Utils.NextFloat(2.0f, 16.0f) * Units.SOLAR_MASS;
            luminosity = Utils.NextFloat(100.0f, 10000.0f) * Units.SOLAR_LUMINOSITY;
            remnant = Remnant.NeutronStar;
            growthSpeed = Utils.NextFloat(6.0f, 8.0f);
            temperature = Utils.NextFloat(10000.0f, 30000.0f);
        }
        else if (percentage <= 0.13f)
        {
            diameter = Utils.NextFloat(15000.0f, 50000.0f);
            type = StarType.AMainSequence;
            color = SpectralType.Interpolate(SpectralType.A, SpectralType.F);
            mass = Utils.NextFloat(1.4f, 2.1f) * Units.SOLAR_MASS;
            luminosity = Utils.NextFloat(10.0f, 1000.0f) * Units.SOLAR_LUMINOSITY;
            remnant = Remnant.Nebula;
            growthSpeed = Utils.NextFloat(4.0f, 5.0f);
            temperature = Utils.NextFloat(7000.0f, 10000.0f);
        }
        else if (percentage <= 0.30f)
        {
            diameter = Utils.NextFloat(4000.0f, 15000.0f);
            type = StarType.YellowDwarf;
            color = SpectralType.Interpolate(SpectralType.F, SpectralType.K);
            mass = Utils.NextFloat(0.8f, 1.4f) * Units.SOLAR_MASS;
            luminosity = Utils.NextFloat(2.0f, 0.5f) * Units.SOLAR_LUMINOSITY;
            remnant = Remnant.Nebula;
            growthSpeed = Utils.NextFloat(2.0f, 3.0f);
            temperature = Utils.NextFloat(5000.0f, 7000.0f);
        }
        else
        {
            diameter = Utils.NextFloat(1000.0f, 4000.0f);
            type = StarType.RedDwarf;
            color = SpectralType.Interpolate(SpectralType.K, SpectralType.M);
            mass = Utils.NextFloat(0.08f, 0.5f) * Units.SOLAR_MASS;
            luminosity = Utils.NextFloat(0.1f, 0.0001f) * Units.SOLAR_LUMINOSITY;
            remnant = Remnant.Nebula;
            growthSpeed = Utils.NextFloat(0.01f, 0.5f);
            temperature = Utils.NextFloat(2000.0f, 3500.0f);
        }

        if (diameter >= 50000.0f && type != StarType.Giant)
        {
            deathSize = Utils.NextFloat(225000.0f, 300000.0f);
            this.starEndLuminosity = Utils.NextFloat(10000.0f, 100000.0f) * Units.SOLAR_LUMINOSITY;
            this.starEndMass = Utils.NextFloat(10.0f, 40.0f) * Units.SOLAR_MASS;
        }
        else
        {
            deathSize = Utils.NextFloat(100000.0f, 125000.0f);
            this.starEndLuminosity = Utils.NextFloat(100.0f, 10000.0f) * Units.SOLAR_LUMINOSITY;
            this.starEndMass = Utils.NextFloat(0.3f, 8.0f) * Units.SOLAR_MASS;
        }

        Star star = Star.Create(name, this, Singleton.Instance.starMat, diameter, mass, temperature, luminosity, color, 
            growthSpeed, deathSize, type, remnant);
        star.transform.localPosition = Vector3.zero;
        this.name = name + " System";

        this.starStartTemperature = temperature;
        this.starStartLuminosity = luminosity;
        this.starStartMass = mass;

        return star;
    }

    protected override void GetNearestPlanets()
    {
        try
        {
            Vector3 probePosFlat = SpaceProbe.probe.transform.position;
            Vector3 starPosFlat = this.star.transform.position;

            probePosFlat.y = 0.0f;
            starPosFlat.y = 0.0f;

            float distanceFlat = Vector3.Distance(probePosFlat, starPosFlat) - this.star.Radius;
            Planet[] allPlanets = this.planets.Concat(this.dwarfPlanets).ToArray();

            Planet prevPlanet = allPlanets.Where(c => this.star.DistanceFromSurface(c.transform.position) < distanceFlat)
                .OrderByDescending(c => this.star.DistanceFromSurface(c.transform.position)).FirstOrDefault();
            Planet nextPlanet = allPlanets.Where(c => this.star.DistanceFromSurface(c.transform.position) > distanceFlat)
                .OrderBy(c => this.star.DistanceFromSurface(c.transform.position)).FirstOrDefault();

            if (prevPlanet != null)
            {
                try
                {
                    float distanceFromPrev = prevPlanet.DistanceFromSurface(SpaceProbe.probe.transform.position);
                    SpaceProbe.probe.probeCamera.ui.precedingPlanet.text =
                        $"▲ {prevPlanet.name.ToUpper()} IN {Units.ToDistanceUnit(distanceFromPrev)}";
                }
                catch (Exception) { }
            }
            else
            {
                SpaceProbe.probe.probeCamera.ui.precedingPlanet.text =
                    $"▲ {this.star.name.ToUpper()} IN {Units.ToDistanceUnit(this.star.DistanceFromSurface(SpaceProbe.probe.transform.position))}";
            }

            if (nextPlanet != null)
            {
                try
                {
                    float distanceFromNext = nextPlanet.DistanceFromSurface(SpaceProbe.probe.transform.position);
                    SpaceProbe.probe.probeCamera.ui.succeedingPlanet.text =
                        $"▼ {nextPlanet.name.ToUpper()} IN {Units.ToDistanceUnit(distanceFromNext)}";
                }
                catch (Exception) { }
            }
            else
            {
                SpaceProbe.probe.probeCamera.ui.succeedingPlanet.text =
                    $"▼ EDGE OF SYSTEM IN {Units.ToDistanceUnit(this.size - distanceFlat)}";
            }

            foreach (Belt belt in this.belts)
            {
                if (!SpaceProbe.probe.probeCamera.isLookingAtSomething && belt.Contains(SpaceProbe.probe.transform.position))
                {
                    SpaceProbeUI ui = SpaceProbe.probe.probeCamera.ui;

                    ui.SetUI(ui.beltUI);
                    ui.beltUI.name.text = belt.name.ToUpper();
                    ui.beltUI.distanceFromStar.text = $"{Units.ToDistanceUnit(belt.innerRadius - this.star.Radius)} AWAY FROM STAR";
                    ui.beltUI.thickness.text = $"{Units.ToDistanceUnit(belt.Thickness)} THICK";

                    break;
                }
            }
        }
        catch (Exception e)
        {
            this.planets = this.planets.Where(p => p != null).ToList();
            this.dwarfPlanets = this.dwarfPlanets.Where(dp => dp != null).ToList();
            this. belts = this. belts.Where(b => b != null).ToList();
        }
    }

    void SetPlanetTexture(Planet planet)
    {
        if (planet.diameter > 30.0f)
        {
            planet.SetMat(Utils.RandomChoose(Singleton.Instance.gasGiantMats));
        }
        else
        {
            if (planet.IsHabitable)
            {
                planet.SetMat(Utils.RandomChoose(Singleton.Instance.terrestrialMats.habitable));
            }
            else if (planet.temperature > 373.15f)
            {
                planet.SetMat(Utils.RandomChoose(Singleton.Instance.terrestrialMats.hot));
            }
            else
            {
                planet.SetMat(Utils.RandomChoose(Singleton.Instance.terrestrialMats.cold));
            }
        }
    }

    protected override void SetSize()
    {
        if (this.collider)
        {
            this.size = this.star.Radius + this.farthestDistance * 1.25f;
            this.collider.radius = this.size;
        }
    }

    protected override void SetSystem()
    {
        int numPlanets = Utils.random.Next(1, 10);
        int numDwarfs = 0;
        float beltOffset = 0.0f;
        float prevDistance = Utils.NextFloat(0.01f, 0.4f) * Units.AU;
        float orbitalPeriodModifier = Utils.NextFloat(0.25f, 1.25f);
        bool isProgradeClockwise = Utils.random.Next(0, 2) == 0;

        this.farthestDistance = this.star.Radius;

        for (int i = 0; i < numPlanets; i++)
        {
            float distance = prevDistance;
            float diameter = Utils.NextFloat(0.0f, 1.0f) <= 0.35f ? Utils.NextFloat(3.0f, 30.0f) : Utils.NextFloat(31.0f, 300.0f);

            prevDistance += 300.0f + Utils.NextFloat(0.1f, 15.0f) * Units.AU * 
                Mathf.Min(1.0f, (Mathf.Pow(1.2f, distance / Units.AU - 7.0f) - 0.1f)) + beltOffset;

            float mass;
            float orbitalPeriod;

            if (diameter <= Units.JUPITER_RADIUS)
                mass = (Mathf.Pow(1.0185f, 3.0f * diameter - 15.0f) - 0.5f) * Units.EARTH_MASS * Utils.NextFloat(0.5f, 1.5f);
            else
                mass = Mathf.Pow(1.017f, diameter - 110.0f) * Units.JUPITER_MASS * Utils.NextFloat(0.5f, 1.5f);

            orbitalPeriod = Mathf.Sqrt(Mathf.Pow(distance / Units.AU, 3)) * 365.0f;

            distance += diameter * 0.5f;

            Vector3 orbitPos = this.star.transform.position + this.star.transform.forward * (this.star.Radius + distance);
            string name = Planet.NameByStar(this.star, i);
            float day = Utils.NextFloat(0.0f, 1.0f) < 0.75f ? Utils.NextFloat(8.0f, 100.0f) : Utils.NextFloat(101.0f, 10000.0f);

            Orbit orbit = Orbit.Create(name, orbitPos, this.transform, this.star.transform, orbitalPeriod, Vector3.up, isProgradeClockwise, 0.0f);
            Planet planet = World.Create<Planet>(name, orbit.transform, diameter, Utils.NextFloat(0.0f, 180.0f), day, mass);
            
            this.planets.Add(planet);
            planet.transform.localPosition = Vector3.zero;

            this.SetWorldTemperature(planet, distance);
            this.SetPlanetTexture(planet);

            float moonDistanceScale = 1.0f;

            if (Random.Range(1, 25) == 1)
                planet.AddRing(Random.Range(0.25f, 1.5f), Random.Range(0.05f, 2.0f), Utils.RandomChoose(Singleton.Instance?.ringMats));

            if (planet.diameter <= 30.0f)
            {
                moonDistanceScale = 0.031f * planet.diameter + 0.085f;
                this.AddAtmosphere(planet, distance);
            }

            this.CreateMoons(planet, distance, moonDistanceScale);
            this.farthestDistance = distance;

            if (Utils.random.Next(1, 13) == 1 && this.belts.Count < 2)
            {
                float thickness = i >= numPlanets / 4 * 3 ? Utils.NextFloat(3.0f, 30.0f) * Units.AU : Random.Range(100.0f, 5000.0f);
                float minOrbitSpeed = orbitalPeriod * 0.1f, maxOrbitSpeed = orbitalPeriod * 1.5f;
                float beltDistance = distance * 1.1f;
                GameObject prefab = Utils.RandomChoose(Singleton.Instance.asteroidPrefabs);

                Belt belt = Belt.Create(this.transform, this.transform.position, Random.Range(100, 200),
                    (this.star.Radius + beltDistance), thickness, minOrbitSpeed, maxOrbitSpeed, Random.Range(5.0f, 50.0f), 0.01f, 50.0f,
                    0.1f, 1.0f, prefab);

                beltOffset += thickness * 1.1f;
                this.belts.Add(belt);
                this.farthestDistance = beltDistance + thickness;

                int numDwarfPlanets = Utils.random.Next(0, 7);
                float minDistance = belt.innerRadius;

                for (int j = 0; j < numDwarfPlanets; j++)
                {
                    numDwarfs++;

                    float dpDistance = Utils.NextFloat(minDistance, belt.outerRadius);
                    string dpName = $"{star.name} Dwarf {numDwarfs}"; ;
                    float dpDiameter = Utils.NextFloat(1.0f, 3.0f);
                    float dpMass = (Mathf.Pow(1.0185f, 3.0f * dpDiameter - 15.0f) - 0.5f) * Units.EARTH_MASS * Utils.NextFloat(0.5f, 1.5f);
                    Vector3 dpOrbitPos = this.star.transform.position + this.star.transform.forward * dpDistance;
                    float dpOrbitalPeriod;
                    float dpDay = Utils.NextFloat(0.0f, 1.0f) < 0.75f ? Utils.NextFloat(8.0f, 100.0f) : Utils.NextFloat(101.0f, 10000.0f);

                    if (distance >= 5.0f * Units.AU)
                        dpOrbitalPeriod = Mathf.Pow(1.038f, dpDistance / Units.AU + 65.0f) * 365.0f;
                    else
                        dpOrbitalPeriod = (Mathf.Pow(1.67f, dpDistance / Units.AU) - 1.0f + ((dpDistance / Units.AU) / 3.0f)) * 365.0f * orbitalPeriodModifier;

                    minDistance = dpDistance;

                    Orbit dpOrbit = Orbit.Create(dpName, dpOrbitPos, this.transform, this.star.transform, dpOrbitalPeriod, Vector3.up,
                        isProgradeClockwise, Utils.NextFloat(0.0f, 60.0f));
                    DwarfPlanet dp = World.Create<DwarfPlanet>(dpName, dpOrbit.transform, dpDiameter, Utils.NextFloat(0.0f, 180.0f), dpDay, dpMass);
                    
                    dp.transform.localPosition = Vector3.zero;

                    this.dwarfPlanets.Add(dp);

                    this.SetWorldTemperature(dp, dpDistance);
                    this.SetPlanetTexture(dp);
                    this.CreateMoons(dp, dpDistance, 0.075f);
                    this.AddAtmosphere(dp, dpDistance);

                    this.farthestDistance = dpDistance;
                }
            }
        }
    }

    void SetWorldTemperature(World world, float distance)
    {
        if (distance < 0.5 * Units.AU)
            world.temperature = Mathf.Pow(0.004f, distance / Units.AU - 1.55f);
        else if (distance < 1.5f * Units.AU) 
            world.temperature = -150.0f * (distance / Units.AU) + 438.0f; 
        else
            world.temperature = Mathf.Pow(0.92f, distance / Units.AU - 64.0f) + 30.0f;

        float temperatureMutiplierByStarSize = Mathf.Pow(1.01f, this.star.diameter / 1000.0f - 50.0f) + 0.3f;
        world.temperature += (-Mathf.Pow(1.18f, -this.star.temperature / 1000.0f + 30.5f) + 150.0f) * temperatureMutiplierByStarSize;
    }

    protected override void UpdateSystem()
    {
        this.SetSystem();
    }

    public override void UpdateStarProperties()
    {
        if (this.starStartTemperature > this.starEndTemperature)
            this.star.temperature = Mathf.Lerp(this.starStartTemperature, this.starEndTemperature, this.star.LifetimePercentage);

        if (this.starStartLuminosity <= this.starEndLuminosity)
            this.star.luminosity = Mathf.Lerp(this.starStartLuminosity, this.starEndLuminosity, this.star.LifetimePercentage);

        this.star.mass = Mathf.Lerp(this.starStartMass, this.starEndMass, this.star.LifetimePercentage);

        if (this.star.IsMassive)
        {
            if (this.star.diameter >= 150000.0f && this.star.type != StarType.RedSupergiant)
                this.star.type = StarType.RedSupergiant;
        }
        else
        {
            if (this.star.diameter >= 75000.0f && this.star.type != StarType.Giant)
                this.star.type = StarType.Giant;
        }
    }
}