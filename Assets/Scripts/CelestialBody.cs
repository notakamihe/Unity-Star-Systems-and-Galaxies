using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[ExecuteInEditMode]
public class CelestialBody : MonoBehaviour
{
    public const float G = 6.67f;
    public const float AU = 1500;

    public Rigidbody rb;

    public float diameter = 100.0f;
    public float temp = 0.0f;
    public float mass = 1000.0f;

    // Start is called before the first frame update
    protected virtual void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        this.SetDiameter(diameter);
        this.SetMass(mass);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if (SpaceProbe.probe != null)
            Pull(SpaceProbe.probe.gameObject);
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

    public void SetMass (float mass)
    {
        this.mass = mass;
        this.rb.mass = mass;
    }

    public void SetDiameter(float diameter)
    {
        this.diameter = diameter;
        transform.localScale = new Vector3(diameter, diameter, diameter);
    }
}
