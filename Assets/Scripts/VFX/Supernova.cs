using UnityEngine;


[RequireComponent(typeof(ParticleSystem))]
public class Supernova : MonoBehaviour
{
    public ParticleSystem particleSystem;

    Remnant remnant;
    float startTime;

    void Awake()
    {
        this.particleSystem = GetComponent<ParticleSystem>();
        this.startTime = Time.time;

        if (this.particleSystem)
            this.particleSystem.playOnAwake = true;
    }

    void Update()
    {
        if (Time.time - this.startTime > particleSystem.main.duration * 0.75f)
        {
            this.transform.localScale -= 0.5f * this.transform.localScale * Time.deltaTime;
        }

        if (Time.time - this.startTime > particleSystem.main.duration)
        {
            switch (remnant)
            {
                case Remnant.BlackHole:
                    BlackHole.Create(this.transform.parent, this.transform.position, Random.Range(10.0f, 60.0f * CelestialBody.AU),
                    Random.Range(3.0f * CelestialBody.SOLAR_MASS, 1000000000 * CelestialBody.SOLAR_MASS),
                    Random.Range(0.1f, 0.9f) * CelestialBody.SPEED_OF_LIGHT);
                    break;
                case Remnant.NeutronStar:
                    NeutronStar.Create(this.transform.parent, this.transform.position, Random.Range(10.0f, 20.0f),
                    Random.Range(1.1f * CelestialBody.SOLAR_MASS, 2.14f * CelestialBody.SOLAR_MASS), Random.Range(1000000.0f, 7000000.0f));
                    break;
            }

            DestroyImmediate(this.gameObject);
        }
    }

    public static Supernova Create(Transform parent, Vector3 position, Remnant remnant)
    {
        GameObject obj = Instantiate(Singleton.Instance.supernovaPrefab, position, Quaternion.identity, parent);
        obj.transform.localScale = Vector3.one * 2.0f;

        Supernova supernova = obj.AddComponent<Supernova>();
        supernova.remnant = remnant;
        return supernova;
    }
}