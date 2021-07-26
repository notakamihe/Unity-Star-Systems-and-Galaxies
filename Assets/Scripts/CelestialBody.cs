using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
[ExecuteInEditMode]
public abstract class CelestialBody : MonoBehaviour
{
    public Rigidbody rb;

    public new string name;
    public float diameter = 100.0f;
    public float temperature = 100.0f;
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

    public virtual void SetDiameter(float diameter)
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
