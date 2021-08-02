using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpaceProbeCamera : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public float verticalTiltMaxAngle = 180.0f;
    public float zoomSpeed = 10.0f;

    [HideInInspector] public bool isLookingAtSomething;

    [SerializeField] Transform cursor;
    [HideInInspector] public new Camera camera;
    [HideInInspector] public SpaceProbeUI ui;

    private void Awake()
    {
        this.camera = this.GetComponentInChildren<Camera>();
        this.ui = this.GetComponentInChildren<SpaceProbeUI>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * camera.fieldOfView * (this.mouseSensitivity / 30.0f) * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * camera.fieldOfView * (this.mouseSensitivity / 30.0f) * Time.deltaTime;
        float scrollY = Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;

        transform.parent.Rotate(Vector3.up * mouseX + Vector3.right * -mouseY);
        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView - scrollY, 9.0f, 60.0f);

        if (Physics.Raycast(this.camera.ScreenPointToRay(this.cursor.position), out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Probe"),
            QueryTriggerInteraction.UseGlobal))
        {
            this.isLookingAtSomething = true;

            if (hit.transform.gameObject.TryGetComponent(out Star star))
            {
                this.ui.SetUI(this.ui.starUI);
                this.ui.starUI.name.text = star.name.ToUpper();
                this.ui.starUI.distance.text = $"{Units.ToDistanceUnit(star.DistanceFromSurface(this.transform.position))}";
                this.ui.starUI.temperature.text = $"{Mathf.RoundToInt(star.temperature)}° K";
                this.ui.starUI.radius.text = $"{Units.ToSolarRadius(star.Radius).ToString("0.##")} R☉";
                this.ui.starUI.mass.text = $"{Units.ToSolarMass(star.mass).ToString("0.####")} M☉";
                this.ui.starUI.luminosity.text = $"{Units.ToSolarLuminosity(star.luminosity).ToString("0.##")} L☉";
                this.ui.starUI.SetType(star.type);
            }
            else if (hit.transform.gameObject.TryGetComponent(out Planet planet))
            {
                Orbit orbit = planet.GetComponentInParent<Orbit>();

                this.ui.SetUI(this.ui.planetUI);
                this.ui.planetUI.name.text = planet.name.ToUpper();
                this.ui.planetUI.distance.text = $"{Units.ToDistanceUnit(planet.DistanceFromSurface(this.transform.position))}";
                this.ui.planetUI.temperature.text = $"{Mathf.RoundToInt(planet.temperature)}° K";
                this.ui.planetUI.radius.text = Units.ToPlanetRadiusUnit(planet.Radius);
                this.ui.planetUI.mass.text = Units.ToPlanetMassUnit(planet.mass);
                this.ui.planetUI.axialTilt.text = $"{planet.axialTilt.ToString("0.#")}°";
                this.ui.planetUI.habitablity.text = planet.IsHabitable ? "HABITABLE" : "";
                this.ui.planetUI.orbit.text = Units.ToTimeUnit(orbit.period);
            }
            else if (hit.transform.gameObject.TryGetComponent(out Moon moon))
            {
                Orbit orbit = moon.GetComponentInParent<Orbit>();

                this.ui.SetUI(this.ui.moonUI);
                this.ui.moonUI.name.text = moon.name.ToUpper();
                this.ui.moonUI.distance.text = $"{Units.ToDistanceUnit(moon.DistanceFromSurface(this.transform.position))}";
                this.ui.moonUI.temperature.text = $"{Mathf.RoundToInt(moon.temperature)}° K";
                this.ui.moonUI.radius.text = $"{Units.ToLunarRadius(moon.Radius).ToString("0.##")} R☽";
                this.ui.moonUI.mass.text = $"{Units.ToLunarMass(moon.mass).ToString("0.####")} M☽";
                this.ui.moonUI.inclination.text = $"INCLINED {orbit.inclination.ToString("0.#")}°";
                this.ui.moonUI.habitablity.text = moon.IsHabitable ? "HABITABLE" : "";
                this.ui.moonUI.orbit.text = Units.ToTimeUnit(orbit.period);
            }
            else if (hit.transform.gameObject.TryGetComponent(out StarSystem starSystem))
            {
                this.ui.SetUI(this.ui.starSystemUI);
                this.ui.starSystemUI.name.text = starSystem.name.ToUpper();
                this.ui.starSystemUI.radius.text = $"RADIUS: {Units.ToAU(starSystem.size).ToString("0.#")} AU";
                this.ui.starSystemUI.distance.text = Units.ToDistanceUnit(Utils.DistanceFromSurface(
                    starSystem.transform.position, this.transform.position, starSystem.size));

                if (starSystem is CustomizableStarSystem)
                {
                    CustomizableStarSystem customizableStarSystem = (CustomizableStarSystem)starSystem;

                    this.ui.starSystemUI.numPlanets.text = customizableStarSystem.planets.Count.ToString() + " PLANETS";
                    this.ui.starSystemUI.numDwarfPlanets.text = customizableStarSystem.dwarfPlanets.Count.ToString() + " DWARFS";
                }
                else if (starSystem is ProceduralStarSystem)
                {
                    ProceduralStarSystem randomStarSystem = (ProceduralStarSystem)starSystem;
                    
                    this.ui.starSystemUI.numPlanets.text = randomStarSystem.planets.Count.ToString() + " PLANETS";
                    this.ui.starSystemUI.numDwarfPlanets.text = randomStarSystem.dwarfPlanets.Count.ToString() + " DWARFS";
                }
            }
            else if (hit.transform.gameObject.TryGetComponent(out WhiteDwarf whiteDwarf))
            {
                this.ui.SetUI(this.ui.whiteDwarfUI);
                this.ui.whiteDwarfUI.distance.text = $"{Units.ToDistanceUnit(whiteDwarf.DistanceFromSurface(this.transform.position))}";
                this.ui.whiteDwarfUI.temperature.text = $"{Mathf.RoundToInt(whiteDwarf.temperature)}° K";
                this.ui.whiteDwarfUI.mass.text = $"{Units.ToSolarMass(whiteDwarf.mass).ToString("0.####")} M☉";
            }
            else if (hit.transform.gameObject.TryGetComponent(out NeutronStar neutronStar))
            {
                this.ui.SetUI(this.ui.neutronStarUI);
                this.ui.neutronStarUI.distance.text = $"{Units.ToDistanceUnit(neutronStar.DistanceFromSurface(this.transform.position))}";
                this.ui.neutronStarUI.temperature.text = $"{Mathf.RoundToInt(neutronStar.temperature)}° K";
                this.ui.neutronStarUI.mass.text = $"{Units.ToSolarMass(neutronStar.mass).ToString("0.####")} M☉";
                this.ui.neutronStarUI.hertz.text = $"{Units.ToHertz(neutronStar.rotationSpeed).ToString("0")} HZ";
            }
            else if (hit.transform.gameObject.TryGetComponent(out BlackHole blackHole))
            {
                this.ui.SetUI(this.ui.blackHoleUI);
                this.ui.blackHoleUI.distance.text = $"{Units.ToDistanceUnit(blackHole.DistanceFromSurface(this.transform.position))}";
                this.ui.blackHoleUI.mass.text = $"{Units.ToSolarMass(blackHole.mass).ToString("0.####")} M☉";
                this.ui.blackHoleUI.hertz.text = $"{Units.ToHertz(blackHole.rotationSpeed).ToString("0")} HZ";
            }
            else if (hit.transform.gameObject.TryGetComponent(out Galaxy galaxy))
            {
                this.ui.SetUI(this.ui.galaxyUI);
                this.ui.galaxyUI.name.text = galaxy.name?.ToUpper();
                this.ui.galaxyUI.distance.text = Units.ToPC(Utils.DistanceFromSurface(galaxy.transform.position, 
                    this.transform.position, galaxy.Radius));
                this.ui.galaxyUI.numStars.text = $"{galaxy.numStars} STARS";
                this.ui.galaxyUI.SetShape(galaxy.shape);
                this.ui.galaxyUI.diameter.text = $"{Units.ToKPC(galaxy.Diameter)} KPC";
            }
        }
        else
        {
            this.isLookingAtSomething = false;
            this.ui.SetUI(null);
        }
    }
}