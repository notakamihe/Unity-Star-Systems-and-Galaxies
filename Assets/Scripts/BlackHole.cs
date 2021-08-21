using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackHole : CompactStar, Attractor
{
    public float rotationSpeed = 0.75f * Units.SPEED_OF_LIGHT;

    Behaviour halo;

    protected override void OnEnable()
    {
        base.OnEnable();

        this.rb.isKinematic = true;
    }

    public static BlackHole Create(Transform parent, Vector3 position, float diameter, float mass, float rotationSpeed)
    {
        GameObject obj = new GameObject("Black Hole");
        obj.transform.parent = parent;
        obj.transform.position = position;

        GameObject singularityObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        singularityObj.transform.parent = obj.transform;
        singularityObj.transform.localPosition = Vector3.zero;
        singularityObj.GetComponent<SphereCollider>().isTrigger = true;

        BlackHole blackHole = singularityObj.AddComponent<BlackHole>();
        blackHole.SetName("Singularity");
        blackHole.SetDiameter(diameter);
        blackHole.SetMass(mass);
        blackHole.SetMat(Singleton.Instance.blackHoleMat);
        blackHole.temperature = 0.00000006f;
        blackHole.rotationSpeed = rotationSpeed;
        blackHole.halo = blackHole.CreateHalo(blackHole.diameter, Color.white);

        GameObject vfxObj = Instantiate(Singleton.Instance.blackHoleVFX, obj.transform.position, Quaternion.identity, obj.transform);
        BlackHoleVFX vfx = vfxObj.GetComponent<BlackHoleVFX>();

        vfx.SetAccretionDisk(diameter * 2.0f);
        vfx.SetJets(diameter * 50.0f);

        obj.transform.eulerAngles = new Vector3(Random.Range(-65.0f, 65.0f), 0.0f, Random.Range(-65.0f, 65.0f));

        return blackHole;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            this.transform.Rotate(Vector3.up * this.rotationSpeed * Time.deltaTime * Singleton.Instance.timeScale);

            if (SpaceProbe.probe)
                this.Pull(SpaceProbe.probe.gameObject); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == SpaceProbe.probe.gameObject)
        {
            Utils.Destroy(this, SpaceProbe.probe.gameObject);
        }
    }

    public void Pull(GameObject other)
    {
        Rigidbody otherRb = other.GetComponent<Rigidbody>();

        Vector3 direction = this.transform.position - other.transform.position;
        float distance = Mathf.Max(0.0f, this.DistanceFromSurface(other.transform.position));
        float forceMagnitude = Mathf.Min((Units.G * rb.mass * otherRb.mass) / Mathf.Pow(distance, 3) / otherRb.mass, 1000.0f);
        Vector3 force = direction.normalized * forceMagnitude;

        Universe.Move(-force);
    }
}