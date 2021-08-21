using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class Supernova : MonoBehaviour
{
    public ParticleSystem vfx;

    Remnant remnant;
    float collapseSize = 1000.0f;

    void Awake()
    {
        this.vfx = GetComponent<ParticleSystem>();

        if (this.vfx)
            this.vfx.playOnAwake = true;
    }

    void Update()
    {
        this.Expand();
    }

    public static Supernova Create(Star star, Transform parent, Remnant remnant)
    {
        GameObject obj = Instantiate(Singleton.Instance.supernovaPrefab, star.transform.position, Quaternion.identity, parent);
        float size = star.diameter / 700.0f;

        obj.transform.localScale = Vector3.one * size;

        Supernova supernova = obj.AddComponent<Supernova>();
        supernova.remnant = remnant;
        supernova.collapseSize = size * 2.0f;
        return supernova;
    }

    void Collapse()
    {
        switch (this.remnant)
        {
            case Remnant.BlackHole:
                BlackHole.Create(this.transform.parent, this.transform.position, Utils.NextFloat(100.0f, 200.0f),
                Utils.NextFloat(5.0f, 100.0f) * Units.SOLAR_MASS, Utils.NextFloat(0.1f, 0.9f) * Units.SPEED_OF_LIGHT);
                break;
            case Remnant.NeutronStar:
                NeutronStar.Create(this.transform.parent, this.transform.position, Utils.NextFloat(5.0f, 20.0f),
                Utils.NextFloat(1.1f, 2.14f) * Units.SOLAR_MASS, Utils.NextFloat(0.03f, 0.24f) * Units.SPEED_OF_LIGHT);
                break;
        }

        Utils.Destroy(this, this.gameObject);
    }

    void Expand()
    {
       this.transform.localScale += Vector3.one * 100.0f * Time.deltaTime;

        if (this.transform.localScale.x >= this.collapseSize)
        {
            this.Collapse();
        }
    }
}