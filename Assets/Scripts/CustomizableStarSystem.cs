using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CustomizableStarSystem : StarSystem
{
    [Range(7000, 20000)] public float starDeathSize = 10000.0f;
    public Remnant stellarRemnant;

    public List<CustomizablePlanet> planets = new List<CustomizablePlanet>();
    public List<CustomizableBelt> belts = new List<CustomizableBelt>();
    public List<CustomizableDwarfPlanet> dwarfPlanets = new List<CustomizableDwarfPlanet>();
    public CustomizableBoundary boundary;

    List<CustomizablePlanet> prevPlanets = new List<CustomizablePlanet>();
    List<CustomizableBelt> prevBelts = new List<CustomizableBelt>();
    List<CustomizableDwarfPlanet> prevDwarfPlanets = new List<CustomizableDwarfPlanet>();

    private void Start()
    {
        List<CustomizablePlanet> newPlanets = new List<CustomizablePlanet>();
        List<CustomizableBelt> newBelts = new List<CustomizableBelt>();
        List<CustomizableDwarfPlanet> newDwarfPlanets = new List<CustomizableDwarfPlanet>();

        foreach (Planet planet in this.GetComponentsInChildren(typeof(Planet)))
        {
            Orbit orbit = planet.GetComponentInParent<Orbit>();
            CustomizablePlanet cp = null;

            if (planet.GetType().Equals(typeof(Planet)))
            {
                cp = new CustomizablePlanet(orbit, planet, planet.axialTilt, planet.dayLength, planet.diameter,
                    this.star.DistanceFromSurface(orbit.transform.position), planet.atmosphere, planet.mass, planet.name, orbit.orbitalPeriod,
                    planet.temperature, planet.Material, planet.GetComponent<Ring>());

                newPlanets.Add(cp);
            }
            else if (planet.GetType().Equals(typeof(DwarfPlanet)))
            {
                cp = new CustomizableDwarfPlanet(orbit, planet, planet.axialTilt, planet.dayLength, planet.diameter,
                    this.star.DistanceFromSurface(orbit.transform.position), planet.atmosphere, planet.mass, planet.name, orbit.orbitalPeriod,
                    planet.temperature, planet.Material, planet.GetComponent<Ring>(), orbit.tilt.x * 45.0f);

                newDwarfPlanets.Add((CustomizableDwarfPlanet)cp);
            }

            List<CustomizableMoon> newMoons = new List<CustomizableMoon>();

            foreach (Moon moon in this.GetComponentsInChildren(typeof(Moon)))
            {
                Orbit moonOrbit = moon.GetComponentInParent<Orbit>();
                CustomizableMoon cm = new CustomizableMoon(moonOrbit, moon, moon.axialTilt, moon.dayLength, moon.diameter,
                    planet.DistanceFromSurface(moon.transform.position), planet.GetComponent<Atmosphere>(), moon.mass, moon.name, orbit.orbitalPeriod, moon.temperature,
                    moon.Material, planet);

                newMoons.Add(cm);
            }

            cp.prevMoons = newMoons;
        }

        foreach (Belt belt in this.GetComponentsInChildren(typeof(Belt)))
        {
            CustomizableBelt cb = new CustomizableBelt(belt, belt.gameObject.name, belt.density, belt.innerRadius, belt.Thickness,
                belt.height, belt.rotatingClockwise, belt.minOrbitSpeed, belt.maxOrbitSpeed, belt.minDiameter, belt.maxDiameter);

            newBelts.Add(cb);
        }

        Boundary boundary = this.GetComponentInChildren<Boundary>();

        if (boundary != null)
        {
            this.boundary = new CustomizableBoundary(boundary, boundary.name, boundary.radius, boundary.particleSize, boundary.color);
        }

        this.prevPlanets = newPlanets;
        this.prevBelts = newBelts;
        this.prevDwarfPlanets = newDwarfPlanets;

        this.UpdateStar();
    }

    private void OnEnable()
    {
        Star star = this.GetComponentInChildren<Star>();

        if (star)
            this.star = star;
        else
            this.star = this.CreateStar(Star.GeneratedName + " (Star)");
    }

    private void OnValidate()
    {
        this.UpdateSystem();
    }

    public override void Clear(Transform target = null)
    {
        foreach (Transform child in this.transform)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (child != target)
                    DestroyImmediate(child?.gameObject);
            };
        }
    }

    protected override void SetSystem()
    {
        this.UpdateCelestialBodies(ref this.planets, ref this.prevPlanets);
        this.UpdateCelestialBodies(ref this.belts, ref this.prevBelts);
        this.UpdateCelestialBodies(ref this.dwarfPlanets, ref this.prevDwarfPlanets);
        this.UpdateBoundary();
    }

    public void ReduceCelestialBodies<T>(ref List<T> curr, ref List<T> prev)
    {
        for (int i = 0; i < prev.Count; i++)
        {
            if (i >= curr.Count)
            {
                if (typeof(T).IsSubclassOf(typeof(CustomizableWorld)))
                {
                    CustomizableWorld world = (CustomizableWorld)(object)prev[i];

                    if (world.orbit)
                        Utils.Destroy(this, world.orbit.gameObject);
                }
                else if (typeof(T).Equals(typeof(CustomizableBelt)))
                {
                    CustomizableBelt cb = (CustomizableBelt)(object)prev[i];

                    if (cb.belt)
                        Utils.Destroy(this, cb.belt.gameObject);
                }
            }
        }
    }

    public void AddCelestialBodies<T>(ref List<T> curr, ref List<T> prev)
    {
        for (int i = 0; i < curr.Count; i++)
        {
            if (i > prev.Count - 1)
            {
                if (typeof(T).Equals(typeof(CustomizablePlanet)))
                {
                    List<CustomizablePlanet> currPlanets = curr.OfType<CustomizablePlanet>().ToList();

                    string name = World.GeneratedName;
                    float distance = i * CelestialBody.AU + 100.0f + this.star.Radius;
                    Vector3 orbitPos = this.star.transform.position + this.star.transform.forward * distance;

                    Orbit orbit = Orbit.Create(name, orbitPos, this.transform, this.star.transform, 10.0f, Vector3.up, this.isProgradeClockwise);
                    Planet planet = World.Create<Planet>(name, orbit.transform, 50.0f, 0.0f, 10.0f, 5000.0f);

                    currPlanets[i].orbit = orbit;
                    currPlanets[i].world = planet;

                    currPlanets[i].SetTilt(planet.axialTilt);
                    currPlanets[i].SetDayLength(planet.dayLength);
                    currPlanets[i].SetDiameter(planet.diameter);
                    currPlanets[i].SetDistance(distance, this.star, this.star.transform.forward);
                    currPlanets[i].SetHasAtmosphere(false);
                    currPlanets[i].SetMass(planet.mass);
                    currPlanets[i].SetName(planet.name);
                    currPlanets[i].SetOrbitalPeriod(orbit.orbitalPeriod);
                    currPlanets[i].SetRing(false);
                    currPlanets[i].SetTemperature(planet.temperature);
                    currPlanets[i].UpdateMoons(this);

                    planet.transform.localPosition = Vector3.zero;
                }
                else if (typeof(T).Equals(typeof(CustomizableMoon)))
                {
                    List<CustomizableMoon> currMoons = curr.OfType<CustomizableMoon>().ToList();

                    CustomizableMoon cm = currMoons[i];

                    string name = World.GeneratedName;
                    float distance = i * 15.0f + 10.0f + cm.parent.Radius;
                    Vector3 orbitPos = cm.parent.transform.position + cm.parent.transform.forward * distance;

                    Orbit orbit = Orbit.Create(name, orbitPos, cm.parent.transform.parent, cm.parent.transform, 50.0f, cm.parent.transform.up,
                        false);
                    Moon moon = World.Create<Moon>(name, orbit.transform, 1.0f, 0.0f, 20.0f, 2000.0f);

                    cm.orbit = orbit;
                    cm.world = moon;

                    cm.SetTilt(moon.axialTilt);
                    cm.SetDayLength(moon.dayLength);
                    cm.SetDiameter(moon.diameter);
                    cm.SetDistance(distance, cm.parent, cm.parent.transform.forward);
                    cm.SetHasAtmosphere(false);
                    cm.SetMass(moon.mass);
                    cm.SetName(moon.name);
                    cm.SetOrbitalPeriod(orbit.orbitalPeriod);
                    cm.SetTemperature(moon.temperature);

                    moon.transform.localPosition = Vector3.zero;
                }
                else if (typeof(T).Equals(typeof(CustomizableBelt)))
                {
                    List<CustomizableBelt> currBelts = curr.OfType<CustomizableBelt>().ToList();

                    float distance = i * CelestialBody.AU + 100.0f + this.star.Radius;

                    Belt belt = Belt.Create(this.transform, this.transform.position, 100, distance, 200.0f, 5.0f, 15.0f, 15.0f, 0.0f, 100.0f, 0.1f, 0.5f,
                        Singleton.Instance.asteroidPrefabs[0]);

                    currBelts[i].belt = belt;

                    currBelts[i].SetName("Belt");
                    currBelts[i].SetPrefab(belt.asteroidPrefab);
                    currBelts[i].SetDensity(belt.density);
                    currBelts[i].SetDistance(distance);
                    currBelts[i].SetThickness(belt.outerRadius - belt.innerRadius);
                    currBelts[i].SetHeight(belt.height);
                    currBelts[i].SetIsClockwise(belt.rotatingClockwise);
                    currBelts[i].SetMinOrbitSpeed(belt.minOrbitSpeed);
                    currBelts[i].SetMaxOrbitSpeed(belt.maxOrbitSpeed);
                    currBelts[i].SetMinDiameter(belt.minDiameter);
                    currBelts[i].SetMaxDiameter(belt.maxDiameter);
                }
                else if (typeof(T).Equals(typeof(CustomizableDwarfPlanet)))
                {
                    List<CustomizableDwarfPlanet> currDwarfs = curr.OfType<CustomizableDwarfPlanet>().ToList();

                    string name = World.GeneratedName;
                    float distance = i * CelestialBody.AU + 100.0f + this.star.Radius + (CelestialBody.AU * 0.5f);
                    Vector3 orbitPos = this.star.transform.position + this.star.transform.forward * distance;

                    Orbit orbit = Orbit.Create(name, orbitPos, this.transform, this.star.transform, 10.0f, Vector3.up,
                        this.isProgradeClockwise);
                    DwarfPlanet dwarfPlanet = World.Create<DwarfPlanet>(name, orbit.transform, 1.0f, 0.0f, 10.0f, 2000.0f);

                    currDwarfs[i].orbit = orbit;
                    currDwarfs[i].world = dwarfPlanet;

                    currDwarfs[i].SetTilt(dwarfPlanet.axialTilt);
                    currDwarfs[i].SetDayLength(dwarfPlanet.dayLength);
                    currDwarfs[i].SetDiameter(dwarfPlanet.diameter);
                    currDwarfs[i].SetDistance(distance, this.star, this.star.transform.forward);
                    currDwarfs[i].SetHasAtmosphere(false);
                    currDwarfs[i].SetMass(dwarfPlanet.mass);
                    currDwarfs[i].SetName(dwarfPlanet.name);
                    currDwarfs[i].SetOrbitalPeriod(orbit.orbitalPeriod);
                    currDwarfs[i].SetRing(false);
                    currDwarfs[i].SetTemperature(dwarfPlanet.temperature);
                    currDwarfs[i].SetInclination(orbit.tilt.x);
                    currDwarfs[i].UpdateMoons(this);

                    dwarfPlanet.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    void UpdateBoundary()
    {
        if (this.boundary == null || this.boundary.boundary == null)
        {
            Boundary boundary = Boundary.Create(this.transform, this.transform.position, 80000.0f, 2000.0f, Color.white);
            this.boundary = new CustomizableBoundary(boundary, boundary.name, boundary.radius, boundary.particleSize, boundary.color);
        }
        else
        {
            this.boundary.SetDistance(this.boundary.distance);
            this.boundary.SetSize(this.boundary.size);
            this.boundary.SetColor(this.boundary.color);
            this.boundary.SetName(this.boundary.name);
        }
    }

    void UpdateCelestialBodies<T>(ref List<T> curr, ref List<T> prev)
    {
        try
        {
            if (curr.Count < prev.Count)
            {
                this.ReduceCelestialBodies(ref curr, ref prev);
            }
            else if (curr.Count > prev.Count)
            {
                this.AddCelestialBodies(ref curr, ref prev);
            }
            else
            {
                if (typeof(T).IsAssignableFrom(typeof(CustomizablePlanet)))
                {
                    List<CustomizablePlanet> currPlanets = curr.OfType<CustomizablePlanet>().ToList();

                    foreach (CustomizablePlanet cp in currPlanets)
                    {
                        cp.SetTilt(cp.axialTilt);
                        cp.SetDayLength(cp.dayLength);
                        cp.SetDiameter(cp.diameter);
                        cp.SetDistance(cp.distance, this.star, (cp.orbit.transform.position - this.star.transform.position).normalized);
                        cp.SetHasAtmosphere(cp.hasAtmosphere);
                        cp.SetMass(cp.mass);
                        cp.SetMat(cp.mat);
                        cp.SetName(cp.name);
                        cp.SetOrbitalPeriod(cp.orbitalPeriod);
                        cp.SetRing(cp.hasRing);
                        cp.SetTemperature(cp.temperature);
                        cp.UpdateMoons(this);
                    }
                }
                else if (typeof(T).Equals(typeof(CustomizableBelt)))
                {
                    List<CustomizableBelt> currBelts = curr.OfType<CustomizableBelt>().ToList();

                    foreach (CustomizableBelt cb in currBelts)
                    {
                        cb.SetName(cb.name);
                        cb.SetPrefab(cb.prefab);
                        cb.SetDensity(cb.density);
                        cb.SetDistance(cb.distance);
                        cb.SetThickness(cb.thickness);
                        cb.SetHeight(cb.height);
                        cb.SetIsClockwise(cb.isClockwise);
                        cb.SetMinOrbitSpeed(cb.minOrbitSpeed);
                        cb.SetMaxOrbitSpeed(cb.maxOrbitSpeed);
                        cb.SetMinDiameter(cb.minDiameter);
                        cb.SetMaxDiameter(cb.maxDiameter);

                        cb.belt.ClearAndInitialize();
                    }
                }
                else if (typeof(T).Equals(typeof(CustomizableDwarfPlanet)))
                {
                    List<CustomizableDwarfPlanet> currDwarfs = curr.OfType<CustomizableDwarfPlanet>().ToList();

                    foreach (CustomizableDwarfPlanet cdp in currDwarfs)
                    {
                        cdp.SetTilt(cdp.axialTilt);
                        cdp.SetDayLength(cdp.dayLength);
                        cdp.SetDiameter(cdp.diameter);
                        cdp.SetDistance(cdp.distance, this.star, (cdp.orbit.transform.position - this.star.transform.position).normalized);
                        cdp.SetHasAtmosphere(cdp.hasAtmosphere);
                        cdp.SetMass(cdp.mass);
                        cdp.SetMat(cdp.mat);
                        cdp.SetName(cdp.name);
                        cdp.SetOrbitalPeriod(cdp.orbitalPeriod);
                        cdp.SetRing(cdp.hasRing);
                        cdp.SetTemperature(cdp.temperature);
                        cdp.SetInclination(cdp.orbitInclination);
                        cdp.UpdateMoons(this);
                    }
                }
            }
        }
        catch (NullReferenceException)
        {
            if (curr.Count > 0)
            {
                if (typeof(T).IsSubclassOf(typeof(CustomizableWorld)))
                {
                    List<CustomizableWorld> newPlanets = new List<CustomizableWorld>();
                    List<CustomizableWorld> currPlanets = curr.OfType<CustomizableWorld>().ToList();

                    for (int i = 0; i < currPlanets.Count; i++)
                    {
                        if (currPlanets[i].orbit != null)
                        {
                            if (currPlanets[i].world == null)
                            {
                                Utils.Destroy(this, currPlanets[i].orbit.gameObject);
                            }
                            else
                            {
                                newPlanets.Add(currPlanets[i]);
                            }
                        }
                    }

                    currPlanets = newPlanets;
                }
            }
        }

        prev = curr.ToList();
    }

    void UpdateStar()
    {
        if (this.starMat != star.GetComponent<Renderer>().sharedMaterial)
            this.star.SetMat(this.starMat);

        this.star.SetDiameter(this.starSize);
        this.star.SetColor(1000.0f, 5000.0f);
        this.star.UpdateStar();
        this.star.growthSpeed = this.starGrowthSpeed;
        this.star.deathSize = this.starDeathSize;
        this.star.remnant = this.stellarRemnant;
    }

    protected override void UpdateSystem()
    {
        if (this.star)
        {
            this.UpdateStar();
            this.SetSystem();
        }
    }
}

public abstract class Customizable {
    public string name;

    public abstract void SetName(string name);

    public override string ToString()
    {
        return $"Name: {this.name}";
    }
}

public abstract class CustomizableWorld : Customizable
{
    [Range(0.0f, 180.0f)] public float axialTilt;
    public float dayLength;
    public float diameter;
    public float distance;
    public float mass;
    public float orbitalPeriod;
    [Range(-273.0f, 20000.0f)] public float temperature;
    public bool hasAtmosphere = false;
    public Material mat;

    [HideInInspector] public Orbit orbit;
    [HideInInspector] public World world;

    public abstract void SetDiameter(float diameter);
    public abstract void SetDistance(float distance, CelestialBody parent, Vector3 direction);

    public CustomizableWorld(Orbit orbit, World world, float axialTilt, float dayLength, float diameter, float distance, bool hasAtmosphere,
        float mass, string name, float orbitalPeriod, float temperature, Material mat)
    {
        this.orbit = orbit;
        this.world = world;

        this.SetTilt(axialTilt);
        this.SetDayLength(dayLength);
        this.SetDiameter(diameter);
        this.distance = distance;
        this.SetHasAtmosphere(hasAtmosphere);
        this.SetMass(mass);
        this.SetName(name);
        this.SetOrbitalPeriod(orbitalPeriod);
        this.SetTemperature(temperature);
        this.SetMat(mat);
    }

    public virtual void SetTilt(float axialTilt)
    {
        this.axialTilt = axialTilt;
        this.world.SetTilt(axialTilt);
    }

    public void SetDayLength(float dayLength)
    {
        this.dayLength = dayLength;
        this.world.dayLength = dayLength;
    }

    public void SetHasAtmosphere(bool hasAtmosphere)
    {
        this.hasAtmosphere = hasAtmosphere;

        if (hasAtmosphere && !this.world.atmosphere)
        {
            this.world.AddAtmosphere(1.0f, Color.white);
        }
        else if (!hasAtmosphere && this.world.atmosphere)
        {
            this.world.RemoveAtmosphere();
        }
    }

    public void SetMass(float mass)
    {
        this.mass = mass;
        this.world.SetMass(mass);
    }

    public void SetMat(Material mat)
    {
        this.mat = mat;
        this.world.SetMat(mat);
    }

    public override void SetName(string name)
    {
        this.name = name;
        this.world.SetName(name);
    }

    public void SetOrbitalPeriod(float orbitalPeriod)
    {
        this.orbitalPeriod = orbitalPeriod;
        this.orbit.orbitalPeriod = orbitalPeriod;
    }

    public void SetTemperature(float temperature)
    {
        this.temperature = temperature;
        this.world.temperature = temperature;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Axial Tilt: {this.axialTilt}, Day Length: {this.dayLength}, Diameter: {this.diameter}, " +
            $"Distance: {this.distance}, " + $"Mass: {this.mass}, Orbital Period: {this.orbitalPeriod}, Temperature: {this.temperature}";
    }
}

[System.Serializable]
public class CustomizablePlanet : CustomizableWorld
{
    public List<CustomizableMoon> moons = new List<CustomizableMoon>();
    public bool hasRing;

    [HideInInspector] public List<CustomizableMoon> prevMoons = new List<CustomizableMoon>();

    public CustomizablePlanet(Orbit orbit, Planet planet, float axialTilt, float dayLength, float diameter, float distance, 
        bool hasAtmosphere, float mass, string name, float orbitalPeriod, float temperature, Material mat, bool hasRing) : 
        base (orbit, planet, axialTilt, dayLength, diameter, distance, hasAtmosphere, mass, name, orbitalPeriod, temperature, mat)
    {
        this.SetRing(hasRing);
    }

    public override void SetDiameter(float diameter)
    {
        float clampedDiameter = Mathf.Clamp(diameter, 3.0f, 300.0f);

        this.diameter = clampedDiameter;
        this.world.SetDiameter(clampedDiameter);
    }

    public override void SetDistance(float distance, CelestialBody parent, Vector3 direction)
    {
        float clampedDistance = Mathf.Clamp(distance, 0.0f, 100000.0f);
        Vector3 orbitPos = parent.transform.position + direction * clampedDistance;

        this.distance = clampedDistance;
        this.orbit.transform.position = orbitPos;
    }

    public void SetRing(bool hasRing)
    {
        this.hasRing = hasRing;

        if (hasRing && !this.world.GetComponent<Ring>())
        {
            ((Planet)this.world).AddRing(1.15f, 1, null);
        }
        else if (!hasRing && this.world.GetComponent<Ring>())
        {
            this.world.RemoveRing();
        }
    }

    public override void SetTilt(float axialTilt)
    {
        base.SetTilt(axialTilt);

        foreach (CustomizableMoon moon in this.moons)
        {
            Vector3 newOrbitPos = this.world.transform.position + this.world.transform.forward * (moon.distance);

            if (moon.orbit)
            {
                moon.orbit.transform.position = newOrbitPos;
                moon.orbit.tilt = this.world.transform.up;
            }
        }
    }

    public void UpdateMoons(CustomizableStarSystem customizableStarSystem)
    {
        foreach (CustomizableMoon moon in this.moons)
        {
            moon.parent = (Planet) this.world;
        }

        if (this.moons.Count == this.prevMoons.Count)
        {
            foreach (CustomizableMoon cm in this.moons)
            {
                cm.SetTilt(cm.axialTilt);
                cm.SetDayLength(cm.dayLength);
                cm.SetDiameter(cm.diameter);
                cm.SetDistance(cm.distance, cm.parent, (cm.orbit.transform.position - cm.parent.transform.position).normalized);
                cm.SetHasAtmosphere(cm.hasAtmosphere);
                cm.SetMass(cm.mass);
                cm.SetMat(cm.mat);
                cm.SetName(cm.name);
                cm.SetOrbitalPeriod(cm.orbitalPeriod);
                cm.SetTemperature(cm.temperature);
            }
        }
        else if (this.moons.Count < this.prevMoons.Count)
        {
            customizableStarSystem.ReduceCelestialBodies(ref this.moons, ref this.prevMoons);
            this.prevMoons = this.moons.Select(m => new CustomizableMoon(m)).ToList();
        }
        else
        {
            customizableStarSystem.AddCelestialBodies(ref this.moons, ref this.prevMoons);
            this.prevMoons = this.moons.Select(m => new CustomizableMoon(m)).ToList();
        }
    }
}

[System.Serializable]
public class CustomizableDwarfPlanet : CustomizablePlanet
{
    public float orbitInclination;

    public CustomizableDwarfPlanet(Orbit orbit, Planet planet, float axialTilt, float dayLength, float diameter, float distance, 
        bool hasAtmosphere, float mass, string name, float orbitalPeriod, float temperature, Material mat, bool hasRing, 
        float orbitInclination) : 
        base(orbit, planet, axialTilt, dayLength, diameter, distance, hasAtmosphere, mass, name, orbitalPeriod, temperature, mat, hasRing)
    {
        this.SetInclination(orbitInclination);
    }

    public override void SetDiameter(float diameter)
    {
        float clampedDiameter = Mathf.Clamp(diameter, 0.1f, 2.99f);

        this.diameter = clampedDiameter;
        this.world.SetDiameter(clampedDiameter);
    }

    public void SetInclination(float orbitInclination)
    {
        this.orbitInclination = orbitInclination;
        this.orbit.tilt = new Vector3(Mathf.InverseLerp(0.0f, 45.0f, orbitInclination), 1.0f, 0.0f);
    }
}

[System.Serializable]
public class CustomizableMoon : CustomizableWorld
{
    [HideInInspector] public Planet parent;

    public CustomizableMoon(Orbit orbit, World world, float axialTilt, float dayLength, float diameter, float distance, bool hasAtmosphere,
        float mass, string name, float orbitalPeriod, float temperature, Material mat, Planet parent) : base(orbit, world, axialTilt, 
            dayLength, diameter, distance, hasAtmosphere, mass, name, orbitalPeriod, temperature, mat)
    {
        this.parent = parent;
    }

    public CustomizableMoon(CustomizableMoon other) : base(other.orbit, other.world, other.axialTilt, other.dayLength, other.diameter, 
        other.distance, other.hasAtmosphere, other.mass, other.name, other.orbitalPeriod, other.temperature, other.mat)
    {
        this.parent = other.parent;
    }

    public override void SetDiameter(float diameter)
    {
        float clampedDiameter = Mathf.Clamp(diameter, 0.1f, 15.0f);

        this.diameter = clampedDiameter;
        this.world.SetDiameter(clampedDiameter);
    }

    public override void SetDistance(float distance, CelestialBody parent, Vector3 direction)
    {
        float clampedDistance = Mathf.Clamp(distance, 0.0f, 5000.0f);
        Vector3 orbitPos = parent.transform.position + direction * clampedDistance;

        this.distance = clampedDistance;
        this.orbit.transform.position = orbitPos;
    }
}

[System.Serializable]
public class CustomizableBelt : Customizable
{
    public GameObject prefab;
    [Range(10, 500)] public int density;
    [Range(0.0f, 100000.0f)] public float distance;
    public float thickness;
    public float height;
    public bool isClockwise;
    public float minOrbitSpeed;
    public float maxOrbitSpeed;
    [Range(0.05f, 2.0f)] public float minDiameter;
    [Range(0.05f, 2.0f)] public float maxDiameter;

    [HideInInspector] public Belt belt;

    public CustomizableBelt(Belt belt, string name, int density, float distance, float thickness, float height, bool isClockwise, 
        float minOrbitSpeed, float maxOrbitSpeed, float minDiameter, float maxDiameter)
    {
        this.belt = belt;

        this.SetName(name);
        this.SetDensity(density);
        this.SetDistance(distance);
        this.SetThickness(thickness);
        this.SetHeight(height);
        this.SetIsClockwise(isClockwise);
        this.SetMinOrbitSpeed(minOrbitSpeed);
        this.SetMaxOrbitSpeed(maxOrbitSpeed);
        this.SetMinDiameter(minDiameter);
        this.SetMaxDiameter(maxDiameter);
    }

    public override void SetName(string name)
    {
        this.name = name;
        this.belt.gameObject.name = name;
    }

    public void SetPrefab(GameObject prefab)
    {
        this.prefab = prefab;
        this.belt.asteroidPrefab = prefab;
    }

    public void SetDensity(int density)
    {
        this.density = density;
        this.belt.density = density;
    }

    public void SetDistance(float distance)
    {
        this.distance = distance;
        this.belt.innerRadius = distance;
    }

    public void SetThickness(float thickness)
    {
        this.thickness = thickness;
        this.belt.outerRadius = this.belt.innerRadius + thickness;
    }

    public void SetHeight(float height)
    {
        this.height = height;
        this.belt.height = height;
    }
    
    public void SetIsClockwise(bool isClockwise)
    {
        this.isClockwise = isClockwise;
        this.belt.rotatingClockwise = isClockwise;
    }

    public void SetMinOrbitSpeed(float minOrbitSpeed)
    {
        this.minOrbitSpeed = minOrbitSpeed;
        this.belt.minOrbitSpeed = minOrbitSpeed;
    }

    public void SetMaxOrbitSpeed(float maxOrbitSpeed)
    {
        this.maxOrbitSpeed = maxOrbitSpeed;
        this.belt.maxOrbitSpeed = maxOrbitSpeed;
    }

    public void SetMinDiameter(float minDiameter)
    {
        this.minDiameter = minDiameter;
        this.belt.minDiameter = minDiameter;
    }

    public void SetMaxDiameter(float maxDiameter)
    {
        this.maxDiameter = maxDiameter;
        this.belt.maxDiameter = maxDiameter;
    }
}

[System.Serializable]
public class CustomizableBoundary : Customizable
{
    [Range(100.0f, 100000.0f)] public float distance;
    public float size;
    public Color color;

    [HideInInspector] public Boundary boundary;

    public CustomizableBoundary(Boundary boundary, string name, float distance, float size, Color color)
    {
        this.boundary = boundary;

        this.SetName(name);
        this.SetDistance(distance);
        this.SetSize(size);
        this.SetColor(color);
    }

    public override void SetName(string name)
    {
        this.name = name;
        this.boundary.gameObject.name = name;
    }

    public void SetDistance(float distance)
    {
        this.distance = distance;
        this.boundary.SetRadius(distance);
    }

    public void SetSize(float size)
    {
        this.size = size;
        this.boundary.SetSize(size);
    }

    public void SetColor(Color color)
    {
        this.color = color;
        this.boundary.SetColor(color);
    }
}