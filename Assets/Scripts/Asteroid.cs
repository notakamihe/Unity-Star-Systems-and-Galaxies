using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random; 


public class Asteroid : CelestialBody
{
    public float orbitSpeed;
    public float rotationSpeed;
    public Transform parent;
    public bool isClockwise;

    Vector3 rotationDirection;

    public void Initialize(float orbitSpeed, float rotationSpeed, Transform parent, bool isClockwise, float diameter)
    {
        this.orbitSpeed = orbitSpeed;
        this.rotationSpeed = rotationSpeed;
        this.parent = parent;
        this.isClockwise = isClockwise;
        this.rotationDirection = rotationDirection = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

        this.SetDiameter(diameter);
    }

    private void Update()
    {
        if (isClockwise)
        {
            transform.RotateAround(parent.position, parent.transform.up, orbitSpeed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(parent.position, -parent.transform.up, orbitSpeed * Time.deltaTime);
        }

        transform.Rotate(rotationDirection, rotationSpeed * Time.deltaTime);
    }
}