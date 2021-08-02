using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum GalaxyShape
{
    Spiral,
    Elliptical
}

[RequireComponent(typeof(SphereCollider))]
public class Galaxy : MonoBehaviour
{
    public new string name;
    public GalaxyShape shape;
    [Range(1.0f, 100.0f)] public float diameterInKPC = 1.0f;
    [Range(0.0f, 1.0f)] public float centerDiameterPercentage = 0.2f;
    [Range(2, 7)] public int numArms = 2;
    public Color color = Color.white;
    [Range(1, 100)] public int numStars = 8;

    GalaxyVFX vfx;
    SphereCollider collider;
    List<StarSystem> starSystems = new List<StarSystem>();
    bool probeEntered = false;

    public static string GeneratedName
    { 
        get { return Utils.SelectNameFromFile("Assets/Scripts/Resources/GalaxyNames.txt") + " Galaxy"; }
    }

    public float Diameter
    {
        get { return this.diameterInKPC * Units.KPC; }
    }

    public float Radius
    { 
        get { return this.Diameter / 2.0f; }
    }

    public static Galaxy Create (Transform parent, Vector3 pos, string name, GalaxyShape shape, float diamterInKPC, 
        float centerDiameterPercentage, int numArms, int numStars, Color color)
    {
        GameObject gameObject = new GameObject();
        gameObject.transform.position = pos;
        gameObject.transform.parent = parent;

        Galaxy galaxy = gameObject.AddComponent<Galaxy>();
        galaxy.name = name;
        galaxy.shape = shape;
        galaxy.color = color;
        galaxy.diameterInKPC = diamterInKPC;
        galaxy.centerDiameterPercentage = centerDiameterPercentage;
        galaxy.numArms = numArms;
        galaxy.numStars = numStars;

        return galaxy;
    }

    private void Start()
    {
        this.collider = this.GetComponent<SphereCollider>();
        this.collider.radius = this.Radius;

        this.gameObject.name = this.name;

        switch (this.shape)
        {
            case GalaxyShape.Spiral:
                vfx = Instantiate(Singleton.Instance.spiralGalaxyVFX, this.transform.position, 
                    Singleton.Instance.spiralGalaxyVFX.transform.rotation);
                vfx.transform.localScale = Vector3.one * this.Diameter / 1333.0f;
                break;
            case GalaxyShape.Elliptical:
                vfx = Instantiate(Singleton.Instance.ellipticalGalaxyVFX, this.transform.position,
                    Singleton.Instance.spiralGalaxyVFX.transform.rotation);
                vfx.transform.localScale = new Vector3(1.0f, 0.6f, 1.0f) * this.Diameter / 1333.0f;
                break;
        }

        vfx.SetArms(numArms);
        vfx.SetCenterSizePercentage(this.centerDiameterPercentage);
        vfx.SetColor(this.color);
        vfx.transform.parent = this.transform;

        for (int i = 0; i < numStars; i++)
        {
            Vector3 pos;
            float ang = Utils.NextFloat(0.0f, 360.0f);

            pos.x = this.transform.position.x + Utils.NextFloat(this.centerDiameterPercentage * this.Radius, this.Radius) * 
                Mathf.Sin(ang * Mathf.Deg2Rad);
            pos.y = this.transform.position.y + Utils.NextFloat(-this.Diameter * 0.1f, this.Diameter * 0.1f);
            pos.z = this.transform.position.z + Utils.NextFloat(this.centerDiameterPercentage * this.Radius, this.Radius) * 
                Mathf.Cos(ang * Mathf.Deg2Rad);

            Orbit orbit = Orbit.Create("Orbit of ", pos, this.transform, this.transform, Utils.NextFloat(100000000.0f, 1000000000.0f), 
                this.transform.up, Utils.random.Next(0, 2) == 0, Utils.NextFloat(0.0f, 60.0f));
            GalaxyStarSystem galaxyStarSystem = GalaxyStarSystem.Create(orbit.transform, pos);

            Vector3 direction = pos - this.transform.position;
            direction.y = 0.0f;
            float angle = Vector3.Angle(this.transform.forward, direction.normalized);

            orbit.name = "Orbit of " + galaxyStarSystem.name;
            orbit.transform.position = Utils.RotatePointAroundPivot(pos, this.transform.position, 
                Quaternion.Euler(Quaternion.AngleAxis(angle, this.transform.up) * this.transform.up));
            
            galaxyStarSystem.transform.localPosition = Vector3.zero;
            this.starSystems.Add(galaxyStarSystem);
        }

        this.ToggleStarSystems(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        this.probeEntered = true;

        if (SpaceProbe.probe && other.gameObject == SpaceProbe.probe.gameObject)
        {
            this.ToggleStarSystems(true);
            SpaceProbe.probe.probeCamera.ui.nearestStar.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        this.probeEntered = false;

        if (SpaceProbe.probe && other.gameObject == SpaceProbe.probe.gameObject)
        {
            this.ToggleStarSystems(false);
            SpaceProbe.probe.probeCamera.ui.nearestStar.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (this.probeEntered && SpaceProbe.probe)
        {
            Star nearestStar = this.starSystems.OrderBy(ss => ss.star.DistanceFromSurface(SpaceProbe.probe.transform.position))
                .FirstOrDefault()?.star;

            if (nearestStar)
            {
                SpaceProbe.probe.probeCamera.ui.nearestStar.text = $"NEAREST STAR: {nearestStar.name.ToUpper()} IN " +
                    $"{Units.ToDistanceUnit(nearestStar.DistanceFromSurface(SpaceProbe.probe.transform.position))}";
            }
        }
    }

    void ToggleStarSystems(bool show)
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject != this.vfx.gameObject)
            {
                child.gameObject.SetActive(show);
            }
        }
    }
}