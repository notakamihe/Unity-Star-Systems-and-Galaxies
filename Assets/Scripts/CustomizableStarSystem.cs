using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class CustomizableStarSystem : StarSystem
{
    [Range(1000.0f, 100000.0f)] public float starSize = 1000.0f;
    [Range(0.0f, 50.0f)] public float starGrowthSpeed = 5.0f;
    public bool isProgradeClockwise = false;
    public Material starMat;
    public string starName = "My Star";
    public Color starColor = Color.yellow;
    [Range(1.0f, 200.0f)] public float starLuminosity = 1.0f;
    public float starMass = 100000000.0f;
    [Range(2000.0f, 40000.0f)] public float starTemperature = 5000.0f;
    public StarType starType;
    public Remnant stellarRemnant;
    
    public float starDeathSize = 10000.0f;
    public float starDeathTemperature = 4000.0f;
    public float starDeathLuminosity = 100.0f * Units.SOLAR_LUMINOSITY;
    public float starDeathMass = 1.0f * Units.SOLAR_MASS;

    [Space(10)]
    public List<CustomizablePlanet> planets = new List<CustomizablePlanet>();
    public List<CustomizableBelt> belts = new List<CustomizableBelt>();
    public List<CustomizableDwarfPlanet> dwarfPlanets = new List<CustomizableDwarfPlanet>();

    List<CustomizablePlanet> prevPlanets = new List<CustomizablePlanet>();
    List<CustomizableBelt> prevBelts = new List<CustomizableBelt>();
    List<CustomizableDwarfPlanet> prevDwarfPlanets = new List<CustomizableDwarfPlanet>();

    bool initialized = false;

    private void Start()
    {
        if (PrefabUtility.GetPrefabInstanceHandle(this.gameObject) != null)
            PrefabUtility.UnpackPrefabInstance(this.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        List<CustomizablePlanet> newPlanets = new List<CustomizablePlanet>();
        List<CustomizableBelt> newBelts = new List<CustomizableBelt>();
        List<CustomizableDwarfPlanet> newDwarfPlanets = new List<CustomizableDwarfPlanet>();

        foreach (Planet planet in this.GetComponentsInChildren(typeof(Planet)))
        {
            Orbit orbit = planet.GetComponentInParent<Orbit>();
            CustomizablePlanet cp = null;

            if (planet.GetType().Equals(typeof(Planet)))
            {
                cp = new CustomizablePlanet(orbit, planet, planet.axialTilt, planet.day, planet.diameter,
                    this.star.DistanceFromSurface(orbit.transform.position), planet.atmosphere, orbit.inclination, 
                    planet.mass, planet.name, orbit.period, planet.temperature, planet.Material, planet.GetComponent<Ring>());

                newPlanets.Add(cp);
            }
            else if (planet.GetType().Equals(typeof(DwarfPlanet)))
            {
                cp = new CustomizableDwarfPlanet(orbit, planet, planet.axialTilt, planet.day, planet.diameter,
                    this.star.DistanceFromSurface(orbit.transform.position), planet.atmosphere, orbit.inclination, 
                    planet.mass, planet.name, orbit.period, planet.temperature, planet.Material, planet.GetComponent<Ring>());

                newDwarfPlanets.Add((CustomizableDwarfPlanet)cp);
            }

            List<CustomizableMoon> newMoons = new List<CustomizableMoon>();

            foreach (Moon moon in planet.GetComponentsInChildren<Moon>())
            {
                Orbit moonOrbit = moon.GetComponentInParent<Orbit>();
                CustomizableMoon cm = new CustomizableMoon(moonOrbit, moon, moon.axialTilt, moon.day, moon.diameter,
                    planet.DistanceFromSurface(moon.transform.position), moon.atmosphere, orbit.inclination, moon.isTidallyLocked, 
                    moon.mass, moon.name, orbit.period, moon.temperature, moon.Material, planet);

                newMoons.Add(cm);
            }

            cp.prevMoons = newMoons;
        }

        foreach (Belt belt in this.GetComponentsInChildren(typeof(Belt)))
        {
            CustomizableBelt cb = new CustomizableBelt(belt, belt.gameObject.name, belt.asteroidPrefab, belt.density, belt.innerRadius, 
                belt.Thickness, belt.height, belt.rotatingClockwise, belt.minOrbitSpeed, belt.maxOrbitSpeed, belt.minDiameter, belt.maxDiameter);

            newBelts.Add(cb);
        }

        this.prevPlanets = newPlanets;
        this.prevBelts = newBelts;
        this.prevDwarfPlanets = newDwarfPlanets;

        this.UpdateStar();

        this.initialized = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Star star = this.GetComponentInChildren<Star>();

        if (star)
            this.star = star;
        else
            this.star = this.CreateStar(Star.GeneratedName + " (Star)");

        this.starName = this.star.name;
        this.star.starSystem = this;
        this.SetSize();
    }

    private void OnValidate()
    {
        if (this.initialized)
            this.UpdateSystem();
    }

    public override void Clear(Transform target = null)
    {
        foreach (Transform child in this.transform)
        {
            if (child != target) Utils.Destroy(this, child.gameObject);
        }
    }

    protected Star CreateStar(string name)
    {
        star = Star.Create(name, this, this.starMat);
        star.transform.localPosition = Vector3.zero;
        this.UpdateSystem();

        return star;
    }

    protected override void GetNearestPlanets()
    {
        if (this.star && SpaceProbe.probe)
        {
            Vector3 probePosFlat = SpaceProbe.probe.transform.position;
            Vector3 starPosFlat = this.star.transform.position;

            probePosFlat.y = 0.0f;
            starPosFlat.y = 0.0f;

            float distanceFlat = Vector3.Distance(probePosFlat, starPosFlat) - this.star.Radius;
            List<CustomizablePlanet> planets = this.planets.Concat(this.dwarfPlanets).ToList();

            Customizable prevPlanet = planets.Where(c => c.distance < distanceFlat).OrderByDescending(c => c.distance).FirstOrDefault();
            Customizable nextPlanet = planets.Where(c => c.distance > distanceFlat).OrderBy(c => c.distance).FirstOrDefault();

            if (prevPlanet != null)
            {
                try
                {
                    float distanceFromPrev = ((CustomizablePlanet)prevPlanet).world.DistanceFromSurface(SpaceProbe.probe.transform.position);
                    SpaceProbe.probe.probeCamera.ui.precedingPlanet.text =
                        $"▲ {prevPlanet.name.ToUpper()} IN {Units.ToDistanceUnit(distanceFromPrev)}";
                }
                catch (Exception)
                {
                }
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
                    float distanceFromNext = ((CustomizablePlanet)nextPlanet).world.DistanceFromSurface(SpaceProbe.probe.transform.position);
                    SpaceProbe.probe.probeCamera.ui.succeedingPlanet.text =
                        $"▼ {nextPlanet.name.ToUpper()} IN {Units.ToDistanceUnit(distanceFromNext)}";
                }
                catch (Exception)
                {
                }
            }
            else
            {
                SpaceProbe.probe.probeCamera.ui.succeedingPlanet.text =
                    $"▼ EDGE OF SYSTEM IN {Units.ToDistanceUnit(this.size - distanceFlat)}";
            }

            foreach (CustomizableBelt cb in this.belts)
            {
                if (!SpaceProbe.probe.probeCamera.isLookingAtSomething && cb.belt.Contains(SpaceProbe.probe.transform.position))
                {
                    SpaceProbeUI ui = SpaceProbe.probe.probeCamera.ui;

                    ui.SetUI(ui.beltUI);
                    ui.beltUI.name.text = cb.name.ToUpper();
                    ui.beltUI.distanceFromStar.text = $"{Units.ToDistanceUnit(cb.distance - this.star.Radius)} AWAY FROM STAR";
                    ui.beltUI.thickness.text = $"{Units.ToDistanceUnit(cb.thickness)} THICK";

                    break;
                }
            }
        }
    }

    protected override void SetSize()
    {
        CustomizablePlanet farthestPlanet = this.planets.OrderByDescending(p => p.distance).FirstOrDefault();
        CustomizableBelt farthestBelt = this.belts.OrderByDescending(b => b.distance + b.thickness).FirstOrDefault();
        CustomizableDwarfPlanet farthestDwarfPlanet = this.dwarfPlanets.OrderByDescending(dp => dp.distance).FirstOrDefault();

        float newSize = this.star.diameter;

        if (farthestPlanet != null && farthestPlanet.distance > newSize)
        {
            newSize = farthestPlanet.distance;
        }

        if (farthestDwarfPlanet != null && farthestDwarfPlanet.distance > newSize)
        {
            newSize = farthestDwarfPlanet.distance;
        }

        if (farthestBelt != null && farthestBelt.distance + farthestBelt.thickness > newSize)
        {
            newSize = farthestBelt.distance + farthestBelt.thickness;
        }

        this.size = newSize * 1.25f + this.star.Radius;
        this.collider.radius = this.size;
    }

    protected override void SetSystem()
    {
        this.UpdateCelestialBodies(ref this.planets, ref this.prevPlanets);
        this.UpdateCelestialBodies(ref this.belts, ref this.prevBelts);
        this.UpdateCelestialBodies(ref this.dwarfPlanets, ref this.prevDwarfPlanets);
    }

    public void ReduceCelestialBodies<T>(ref List<T> curr, ref List<T> prev) where T : Customizable
    {
        for (int i = 0; i < prev.Count; i++)
        {
            if (i >= curr.Count)
            {
                if (typeof(T).IsSubclassOf(typeof(CustomizableWorld)))
                {
                    CustomizableWorld cw = prev[i] as CustomizableWorld;

                    if (cw.orbit)
                        Utils.Destroy(this, cw.orbit.gameObject);
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
                    float distance = i * Units.AU + 100.0f;
                    Vector3 orbitPos = this.star.transform.position + this.star.transform.forward * (distance + this.star.Radius);

                    Orbit orbit = Orbit.Create(name, orbitPos, this.transform, this.star.transform, 100.0f, Vector3.up, 
                        this.isProgradeClockwise, 0.0f);
                    Planet planet = World.Create<Planet>(name, orbit.transform, 50.0f, 0.0f, 20.0f, 100.0f);

                    currPlanets[i].orbit = orbit;
                    currPlanets[i].world = planet;

                    currPlanets[i].SetTilt(planet.axialTilt);
                    currPlanets[i].SetDayLength(planet.day);
                    currPlanets[i].SetDiameter(planet.diameter);
                    currPlanets[i].SetDistance(distance, this.star, this.star.transform.forward);
                    currPlanets[i].SetHasAtmosphere(false);
                    currPlanets[i].SetInclination(orbit.inclination);
                    currPlanets[i].SetMass(planet.mass);
                    currPlanets[i].SetName(planet.name);
                    currPlanets[i].SetOrbitalPeriod(orbit.period);
                    currPlanets[i].SetTemperature(planet.temperature);
                    currPlanets[i].SetRing(false);

                    currPlanets[i].UpdateMoons(this);
                    currPlanets[i].world.transform.localPosition = Vector3.zero;
                }
                else if (typeof(T).Equals(typeof(CustomizableMoon)))
                {
                    List<CustomizableMoon> currMoons = curr.OfType<CustomizableMoon>().ToList();

                    CustomizableMoon cm = currMoons[i];

                    string name = World.GeneratedName;
                    float distance = i * 15.0f + 10.0f;
                    Vector3 orbitPos = cm.parent.transform.position + cm.parent.transform.forward * (distance + cm.parent.Radius);

                    Orbit orbit = Orbit.Create(name, orbitPos, cm.parent.transform.parent, cm.parent.transform, 10.0f, cm.parent.transform.up,
                        this.isProgradeClockwise, 0.0f);
                    Moon moon = World.Create<Moon>(name, orbit.transform, 1.0f, 0.0f, 20.0f, 10.0f);

                    cm.orbit = orbit;
                    cm.world = moon;

                    cm.SetTilt(moon.axialTilt);
                    cm.SetDayLength(moon.day);
                    cm.SetDiameter(moon.diameter);
                    cm.SetDistance(distance, cm.parent, cm.parent.transform.forward);
                    cm.SetHasAtmosphere(false);
                    cm.SetInclination(orbit.inclination);
                    cm.SetIsTidallyLocked(moon.isTidallyLocked);
                    cm.SetMass(moon.mass);
                    cm.SetName(moon.name);
                    cm.SetOrbitalPeriod(orbit.period);
                    cm.SetTemperature(moon.temperature);

                    cm.world.transform.localPosition = Vector3.zero;
                }
                else if (typeof(T).Equals(typeof(CustomizableBelt)))
                {
                    List<CustomizableBelt> currBelts = curr.OfType<CustomizableBelt>().ToList();

                    float distance = i * Units.AU + 100.0f + this.star.Radius;

                    Belt belt = Belt.Create(this.transform, this.transform.position, 10, distance, 200.0f, 5.0f, 15.0f, 
                        15.0f, 0.0f, 100.0f, 0.1f, 0.5f, Singleton.Instance.asteroidPrefabs[0]);

                    currBelts[i].belt = belt;
                    currBelts[i].SetDensity(belt.density);
                    currBelts[i].SetDistance(distance, this.star);
                    currBelts[i].SetHeight(belt.height);
                    currBelts[i].SetIsClockwise(belt.rotatingClockwise);
                    currBelts[i].SetMinOrbitSpeed(belt.minOrbitSpeed);
                    currBelts[i].SetMaxOrbitSpeed(belt.maxOrbitSpeed);
                    currBelts[i].SetMinDiameter(belt.minDiameter);
                    currBelts[i].SetMaxDiameter(belt.maxDiameter);
                    currBelts[i].SetName(belt.name);
                    currBelts[i].SetPrefab(belt.asteroidPrefab);
                    currBelts[i].SetThickness(belt.Thickness);
                }
                else if (typeof(T).Equals(typeof(CustomizableDwarfPlanet)))
                {
                    List<CustomizableDwarfPlanet> currDwarfs = curr.OfType<CustomizableDwarfPlanet>().ToList();

                    string name = World.GeneratedName;
                    float distance = i * Units.AU + 100.0f + (Units.AU * 0.5f);
                    Vector3 orbitPos = this.star.transform.position + this.star.transform.forward * (distance + this.star.Radius);

                    Orbit orbit = Orbit.Create(name, orbitPos, this.transform, this.star.transform, 100.0f, Vector3.up,
                        this.isProgradeClockwise, 0.0f);
                    DwarfPlanet dwarfPlanet = World.Create<DwarfPlanet>(name, orbit.transform, 1.0f, 0.0f, 20.0f, 10.0f);

                    currDwarfs[i].orbit = orbit;
                    currDwarfs[i].world = dwarfPlanet;

                    currDwarfs[i].SetTilt(dwarfPlanet.axialTilt);
                    currDwarfs[i].SetDayLength(dwarfPlanet.day);
                    currDwarfs[i].SetDiameter(dwarfPlanet.diameter);
                    currDwarfs[i].SetDistance(distance, this.star, this.star.transform.forward);
                    currDwarfs[i].SetHasAtmosphere(false);
                    currDwarfs[i].SetInclination(orbit.inclination);
                    currDwarfs[i].SetMass(dwarfPlanet.mass);
                    currDwarfs[i].SetName(dwarfPlanet.name);
                    currDwarfs[i].SetOrbitalPeriod(orbit.period);
                    currDwarfs[i].SetTemperature(dwarfPlanet.temperature);
                    currDwarfs[i].SetRing(false);

                    currDwarfs[i].UpdateMoons(this);
                    currDwarfs[i].world.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    void UpdateCelestialBodies<T>(ref List<T> curr, ref List<T> prev) where T : Customizable
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
                if (typeof(CustomizablePlanet).IsAssignableFrom(typeof(T)))
                {
                    List<CustomizablePlanet> currPlanets = curr.OfType<CustomizablePlanet>().ToList();

                    foreach (CustomizablePlanet cp in currPlanets)
                    {
                        cp.orbit.isClockwise = this.isProgradeClockwise;

                        cp.SetTilt(cp.axialTilt);
                        cp.SetDayLength(cp.dayLength);
                        cp.SetDiameter(cp.diameter);
                        cp.SetDistance(cp.distance, this.star, (cp.orbit.transform.position - this.star.transform.position).normalized);
                        cp.SetHasAtmosphere(cp.hasAtmosphere);
                        cp.SetInclination(cp.inclination);
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
                        cb.SetDistance(cb.distance, this.star);
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

                    curr = newPlanets.OfType<T>().ToList();
                }
                else if (typeof(T).Equals(typeof(CustomizableBelt)))
                {
                    List<CustomizableBelt> currBelts = curr.OfType<CustomizableBelt>().ToList();

                    foreach (CustomizableBelt belt in currBelts)
                    {
                        if (belt.belt == null)
                        {
                            curr.Remove((T)(object)belt);
                        }
                    }
                }
            }
        }

        prev = curr.ToList();
    }

    void UpdateStar()
    {
        if (this.starMat != star.GetComponent<Renderer>().sharedMaterial)
            this.star.SetMat(this.starMat);

        if (this.starDeathSize < this.star.diameter)
            this.starDeathSize = this.star.diameter * 1.25f;

        this.star.SetName(this.starName);
        this.star.SetDiameter(this.starSize);
        this.star.SetColor(this.starColor);
        this.star.SetLuminosity(this.starLuminosity);
        this.star.SetMass(this.starMass);
        this.star.temperature = this.starTemperature;
        this.star.UpdateStar();
        this.star.growthSpeed = this.starGrowthSpeed;
        this.star.deathSize = this.starDeathSize;
        this.star.type = this.starType;
        this.star.remnant = this.stellarRemnant;
        this.star.UpdateOtherProperties();
    }

    public override void UpdateStarProperties()
    {
        this.star.temperature = Mathf.Lerp(this.starTemperature, this.starDeathTemperature, this.star.LifetimePercentage);
        this.star.luminosity = Mathf.Lerp(this.starLuminosity, this.starDeathLuminosity, this.star.LifetimePercentage);
        this.star.mass = Mathf.Lerp(this.starMass, this.starDeathMass, this.star.LifetimePercentage);

        if (this.star.LifetimePercentage >= 0.8f && (this.star.type != StarType.RedSupergiant && this.star.type != StarType.Giant))
            this.star.type = this.star.IsMassive ? StarType.RedSupergiant : StarType.Giant;
    }

    protected override void UpdateSystem()
    {
        if (this.star)
        {
            this.UpdateStar();
            this.SetSystem();
            this.SetSize();
        }
    }
}

public abstract class Customizable {
    public string name;
    public float distance;

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
    public bool hasAtmosphere = false;
    public float inclination;
    public float mass;
    public Material mat;
    public float orbitalPeriod;
    [Range(0f, 7000.0f)] public float temperature;

    [HideInInspector] public Orbit orbit;
    [HideInInspector] public World world;

    public abstract void SetDiameter(float diameter);
    public abstract void SetDistance(float distance, CelestialBody parent, Vector3 direction);

    public CustomizableWorld(Orbit orbit, World world, float axialTilt, float dayLength, float diameter, float distance, 
        bool hasAtmosphere, float inclination, float mass, string name, float orbitalPeriod, float temperature, Material mat)
    {
        this.orbit = orbit;
        this.world = world;

        this.SetTilt(axialTilt);
        this.SetDayLength(dayLength);
        this.SetDiameter(diameter);
        this.distance = distance;
        this.SetHasAtmosphere(hasAtmosphere);
        this.SetInclination(inclination);
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
        this.world.day = dayLength;
    }

    public void SetHasAtmosphere(bool hasAtmosphere)
    {
        this.hasAtmosphere = hasAtmosphere;

        if (hasAtmosphere && !this.world.atmosphere)
        {
            this.world.AddAtmosphere(1.0f, Color.white, 0.3f);
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
        this.orbit.name = "Orbit of " + name;
        this.world.SetName(name);
    }

    public virtual void SetInclination(float inclination)
    {
        this.inclination = inclination;
        this.orbit.SetInclination(inclination);
    }

    public void SetOrbitalPeriod(float orbitalPeriod)
    {
        this.orbitalPeriod = orbitalPeriod;
        this.orbit.period = orbitalPeriod;
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
    public bool hasRing;
    public List<CustomizableMoon> moons = new List<CustomizableMoon>();

    [HideInInspector] public List<CustomizableMoon> prevMoons = new List<CustomizableMoon>();

    public CustomizablePlanet(Orbit orbit, Planet planet, float axialTilt, float dayLength, float diameter, float distance, 
        bool hasAtmosphere, float inclination, float mass, string name, float orbitalPeriod, float temperature, Material mat, bool hasRing) : 
        base (orbit, planet, axialTilt, dayLength, diameter, distance, hasAtmosphere, inclination, mass, name, orbitalPeriod, temperature, mat)
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
        Vector3 orbitPos = parent.transform.position + direction * (clampedDistance + parent.Radius);

        this.distance = clampedDistance;
        this.orbit.transform.position = orbitPos;
    }

    public override void SetInclination(float inclination)
    {
        this.inclination = 0;
        this.orbit.SetInclination(0);
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
            Vector3 newOrbitPos = this.world.transform.position + this.world.transform.forward * (moon.distance + this.world.Radius);

            if (moon.orbit)
            {
                moon.orbit.transform.position = newOrbitPos;
                moon.orbit.tiltAxis = this.world.transform.up;
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
                cm.orbit.isClockwise = customizableStarSystem.isProgradeClockwise;

                cm.SetTilt(cm.axialTilt);
                cm.SetDayLength(cm.dayLength);
                cm.SetDiameter(cm.diameter);
                cm.SetDistance(cm.distance, cm.parent, (cm.orbit.transform.position - cm.parent.transform.position).normalized);
                cm.SetHasAtmosphere(cm.hasAtmosphere);
                cm.SetInclination(cm.inclination);
                cm.SetIsTidallyLocked(cm.isTidallyLocked);
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
    public CustomizableDwarfPlanet(Orbit orbit, Planet planet, float axialTilt, float dayLength, float diameter, float distance, 
        bool hasAtmosphere,float inclination, float mass, string name, float orbitalPeriod, float temperature, Material mat, bool hasRing) : 
        base(orbit, planet, axialTilt, dayLength, diameter, distance, hasAtmosphere, inclination, mass, name, orbitalPeriod, temperature, 
            mat, hasRing)
    {
    }

    public override void SetDiameter(float diameter)
    {
        float clampedDiameter = Mathf.Clamp(diameter, 0.1f, 2.99f);

        this.diameter = clampedDiameter;
        this.world.SetDiameter(clampedDiameter);
    }

    public override void SetInclination(float inclination)
    {
        this.inclination = inclination;
        this.orbit.SetInclination(inclination);
    }
}

[System.Serializable]
public class CustomizableMoon : CustomizableWorld
{
    public bool isTidallyLocked = true;
    [HideInInspector] public Planet parent;

    public CustomizableMoon(Orbit orbit, World world, float axialTilt, float dayLength, float diameter, float distance, bool hasAtmosphere, 
        float inclination, bool isTidallyLocked, float mass, string name, float orbitalPeriod, float temperature, Material mat, Planet parent) : 
        base(orbit, world, axialTilt, dayLength, diameter, distance, hasAtmosphere, inclination, mass, name, orbitalPeriod, temperature, mat)
    {
        this.parent = parent;
        this.SetIsTidallyLocked(isTidallyLocked);
    }

    public CustomizableMoon(CustomizableMoon other) : base(other.orbit, other.world, other.axialTilt, other.dayLength, other.diameter, 
        other.distance, other.hasAtmosphere, other.inclination, other.mass, other.name, other.orbitalPeriod, 
        other.temperature, other.mat)
    {
        this.parent = other.parent;
        this.SetIsTidallyLocked(other.isTidallyLocked);
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
        Vector3 orbitPos = parent.transform.position + direction * (clampedDistance + parent.Radius);

        this.distance = clampedDistance;
        this.orbit.transform.position = orbitPos;
    }

    public void SetIsTidallyLocked(bool isTidallyLocked)
    {
        this.isTidallyLocked = isTidallyLocked;
        ((Moon)this.world).isTidallyLocked = isTidallyLocked;
    }
}

[System.Serializable]
public class CustomizableBelt : Customizable
{
    public GameObject prefab;
    [Range(10, 500)] public int density;
    public float thickness;
    public float height;
    public bool isClockwise;
    public float minOrbitSpeed;
    public float maxOrbitSpeed;
    [Range(0.05f, 2.0f)] public float minDiameter;
    [Range(0.05f, 2.0f)] public float maxDiameter;

    [HideInInspector] public Belt belt;

    public CustomizableBelt(Belt belt, string name, GameObject prefab, int density, float distance, float thickness, float height, bool isClockwise, 
        float minOrbitSpeed, float maxOrbitSpeed, float minDiameter, float maxDiameter)
    {
        this.belt = belt;

        this.SetPrefab(prefab);
        this.SetName(name);
        this.SetDensity(density);
        this.distance = distance;
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

    public void SetDistance(float distance, Star star)
    {
        this.distance = distance;
        this.belt.innerRadius = distance + star.Radius;
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