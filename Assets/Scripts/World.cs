using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class World : CelestialBody
{
    public float dayLength = 1.0f;
    public float axialTilt = 0.0f;

    public float Gravity
    {
        get
        {
            float gravitationalFieldStrength = (G * this.mass) / Mathf.Pow(this.diameter * 0.5f, 2);
            return gravitationalFieldStrength * Mathf.Pow(1.25f, this.mass / 1000.0f - 20.0f) * 0.5f;
        }
    }

    public bool IsHabitable
    {
        get
        {
            return 0.0f <= this.temp && this.temp <= 100.0f;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.SetDiameter(diameter);
        this.SetTilt(axialTilt);
    }

    protected override void FixedUpdate()
    {
        transform.Rotate(0, -dayLength, 0);
    }

    public void SetTilt(float axialTilt)
    {
        this.axialTilt = axialTilt;

        Vector3 newTilt = new Vector3(axialTilt, 0, 0);
        transform.localEulerAngles = newTilt;
    }

    public void SetTexture(Material material)
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.sharedMaterial = material;
    }
}