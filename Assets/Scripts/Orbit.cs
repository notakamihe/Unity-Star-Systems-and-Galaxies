using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Orbit : MonoBehaviour
{
    public Transform parent;
    public float orbitalPeriod = 1.0f;
    public float angle = 0.0f;
    public Vector3 tilt = Vector3.up;
    public bool isClockwise = false;

    Vector3 spawnLocation;

    private void Awake()
    {
        spawnLocation = this.transform.localPosition;
    }

    private void Update()
    {
        transform.position = Utils.RotatePointAroundPivot(transform.position, parent.position, 
            Quaternion.Euler(tilt * ((isClockwise ? 1 : -1) * orbitalPeriod) * Time.deltaTime));
    }

    public static Orbit Create(string name, Vector3 position, Transform parent, float orbitalPeriod, Vector3 tilt, bool isClockwise)
    {
        GameObject orbitObj = new GameObject("Orbit of " + name);
        orbitObj.transform.position = position;
        orbitObj.transform.parent = parent;

        Orbit orbit = orbitObj.AddComponent<Orbit>();
        orbit.parent = parent;
        orbit.orbitalPeriod = orbitalPeriod;
        orbit.tilt = tilt;
        orbit.isClockwise = isClockwise;

        return orbit;
    }
}