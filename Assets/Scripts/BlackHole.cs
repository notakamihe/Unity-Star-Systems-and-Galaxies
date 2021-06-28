using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackHole : Exotic
{
    public float rotationSpeed = 0.75f * CelestialBody.SPEED_OF_LIGHT;

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

        BlackHole blackHole = singularityObj.AddComponent<BlackHole>();
        blackHole.SetName("Singularity");
        blackHole.SetDiameter(diameter);
        blackHole.SetMass(mass);
        blackHole.SetMat(Singleton.Instance.blackHoleMat);
        blackHole.temperature = 0.00000006f;
        blackHole.rotationSpeed = rotationSpeed;
        blackHole.halo = blackHole.CreateHalo(blackHole.diameter + 1.0f, Color.white);

        GameObject vfxObj = Instantiate(Singleton.Instance.blackHoleVFX, obj.transform.position, Quaternion.identity, obj.transform);
        BlackHoleVFX vfx = vfxObj.GetComponent<BlackHoleVFX>();

        vfx.SetAccretionDisk(diameter * 0.05f);
        vfx.SetJets(diameter * 0.05f);

        obj.transform.eulerAngles = new Vector3(Random.Range(-65.0f, 65.0f), 0.0f, Random.Range(-65.0f, 65.0f));

        return blackHole;
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            this.transform.Rotate(Vector3.up * this.rotationSpeed * Time.deltaTime);
        }
    }
}