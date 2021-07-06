using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
[ExecuteInEditMode]
public class CelestialBody : MonoBehaviour
{
    public const float G = 6.67f;
    public const float AU = 1500.0f;
    public const float LIGHT_YEAR = 200000.0f;
    public const float SOLAR_MASS = 500000.0f;
    public const float SPEED_OF_LIGHT = 299792458.0f;

    public Rigidbody rb;

    public string name;
    public float diameter = 100.0f;
    public float temperature = 0.0f;
    public float mass = 1000.0f;

    protected Renderer renderer;

    public Material Material
    {
        get
        {
            return this.renderer.sharedMaterial;
        }
    }

    public float Radius
    {
        get
        {
            return this.diameter * 0.5f;
        }
    }

    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        renderer = GetComponent<Renderer>();

        this.SetDiameter(diameter);
        this.SetMass(mass);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (SpaceProbe.probe != null)
            Pull(SpaceProbe.probe.gameObject);
    }

    private void OnValidate()
    {
        this.SetDiameter(this.diameter);
        this.SetMass(this.mass);
        this.SetName(this.name);
    }

    public float DistanceFromSurface (Vector3 otherPos)
    {
        return Vector3.Distance(this.transform.position, otherPos) - this.Radius;
    }

    void Pull (GameObject other)
    {
        Rigidbody otherRb = other.GetComponent<Rigidbody>();

        Vector3 direction = rb.position - otherRb.position;
        float distance = direction.magnitude;

        float forceMagnitude = Mathf.Min(4000.0f, G * 100.0f * (rb.mass * otherRb.mass) / Mathf.Pow(distance, 2));
        Vector3 force = direction.normalized * forceMagnitude;

        if (forceMagnitude > 3500.0f)
        {
            if (other = SpaceProbe.probe.gameObject)
            {
                SpaceProbe.probe.escape = true;
                SpaceProbe.probe.escapeDir = -direction.normalized;
            }
        }

        otherRb.AddForce(force);
    }

    public void SetDiameter(float diameter)
    {
        this.diameter = diameter;
        transform.localScale = new Vector3(diameter, diameter, diameter);
    }

    public void SetMass (float mass)
    {
        this.mass = mass;
        this.rb.mass = mass;
    }

    public void SetMat(Material mat)
    {
        renderer.sharedMaterial = mat;
    }

    public void SetName(string name)
    {
        this.name = name;
        this.gameObject.name = name;
    }
}
