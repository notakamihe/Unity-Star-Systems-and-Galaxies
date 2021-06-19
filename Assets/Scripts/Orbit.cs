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
    [HideInInspector]
    public bool constrainOrbit = false;

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
}