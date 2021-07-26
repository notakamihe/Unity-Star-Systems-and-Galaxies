using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class Orbit : MonoBehaviour
{
    public Transform parent;
    public float period = 1.0f;
    public float inclination = 0.0f;
    public Vector3 tiltAxis = Vector3.up;
    public bool isClockwise = false;

    Vector3 InclinedAxis
    {
        get
        {
            return Quaternion.AngleAxis(-this.inclination, Vector3.right) * tiltAxis;
        }
    }

    private void Update()
    {
        if (!float.IsNaN(0.0f / this.period))
        {
            float speed = (this.isClockwise ? 1 : -1) * (3000.0f / this.period);
            
            transform.position = Utils.RotatePointAroundPivot(transform.position, parent.position, 
                Quaternion.Euler(this.InclinedAxis * speed * Time.deltaTime * Singleton.Instance.timeScale));
        }
    }

    public static Orbit Create(string name, Vector3 position, Transform parent, Transform orbitParent, float orbitalPeriod, 
        Vector3 tilt, bool isClockwise, float inclination)
    {
        GameObject orbitObj = new GameObject("Orbit of " + name);
        orbitObj.transform.position = position;
        orbitObj.transform.parent = parent;

        Orbit orbit = orbitObj.AddComponent<Orbit>();
        orbit.parent = orbitParent;
        orbit.period = orbitalPeriod;
        orbit.tiltAxis = tilt;
        orbit.isClockwise = isClockwise;

        orbit.SetInclination(inclination);

        return orbit;
    }

    public void SetInclination(float inclination)
    {
        this.inclination = inclination;

        Vector3 direction = this.parent.position - this.transform.position;
        this.transform.position = Utils.RotatePointAroundPivot(this.parent.position + this.parent.forward * direction.magnitude, this.parent.position,
            Quaternion.Euler(Vector3.right * -inclination));
    }
}