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

    public Moon AddMoon(float distance, float orbitalPeriod, string name, float diameter, float axialTilt, float dayLength,
        float mass, Vector3 orbitalAngleAxis, bool isRetrograde)
    {
        Vector3 orbitPos = this.transform.position + this.transform.forward * (this.Radius + distance);
        Orbit orbit = Orbit.Create(name, orbitPos, this.transform.parent, this.transform, orbitalPeriod, orbitalAngleAxis, isRetrograde);
        Moon moon = World.Create<Moon>(name, orbit.transform, diameter, axialTilt, dayLength, mass);

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
        ring.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = material;
        ring.enabled = true;

        return ring;
    }
}