using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Planet : World
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public static string NameByStar(Star star, int index)
    {
        string alphabet = "abcdefghijklmnopqrstuvwxyz";
        return $"{star.name} {alphabet[index % 26]}";
    }

    public Moon AddMoon(float distance, float orbitalPeriod, string name, float diameter, float axialTilt, float dayLength,
        float mass, Vector3 tiltAxis, bool isRetrograde, float inclination, bool isTidallyLocked)
    {
        Vector3 orbitPos = this.transform.position + this.transform.forward * (this.Radius + distance);
        Orbit orbit = Orbit.Create(name, orbitPos, this.transform.parent, this.transform, orbitalPeriod, tiltAxis, isRetrograde, inclination);
        Moon moon = World.Create<Moon>(name, orbit.transform, diameter, axialTilt, dayLength, mass);

        moon.isTidallyLocked = isTidallyLocked;
        moon.transform.localPosition = Vector3.zero;

        return moon;
    }

    public Ring AddRing(float innerRadius, float thickness, Material material)
    {
        Ring ring = this.gameObject.AddComponent<Ring>();
        ring.enabled = false;
        ring.segments = 100;
        ring.innerRadius = innerRadius;
        ring.thickness = thickness;
        ring.ringMat = material;
        ring.enabled = true;

        return ring;
    }
}