using UnityEngine;


public class World : CelestialBody
{
    public float day = 100.0f;
    public float axialTilt = 0.0f;

    [HideInInspector] public Atmosphere atmosphere;

    public static string GeneratedName
    {
        get
        {
            return Utils.SelectNameFromFile("Assets/Scripts/Resources/PlanetNames.txt");
        }
    }

    public bool IsHabitable
    {
        get
        {
            return 273.15f <= this.temperature && this.temperature <= 373.15f && this.diameter < 30.0f && atmosphere?.thickness > 0.15f;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        this.rb.isKinematic = true;
        this.GetComponent<Collider>().isTrigger = false;
        this.SetDiameter(diameter);
        this.SetTilt(axialTilt);

        this.atmosphere = this.GetComponent<Atmosphere>() ?? null;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        this.Spin();
    }
    
    public static T Create<T>(string name, Transform parent, float diameter, float axialTilt, float dayLength, float mass) where T : World
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = parent;

        T world = obj.AddComponent<T>();
        world.SetName(name);
        world.SetDiameter(diameter);
        world.SetTilt(axialTilt);
        world.SetMass(mass);
        world.day = dayLength;

        return world;
    }

    public Atmosphere AddAtmosphere(float thickness, Color color, float ambience)
    {
        this.atmosphere = this.gameObject.AddComponent<Atmosphere>();
        this.atmosphere.SetThickness(thickness);
        this.atmosphere.SetColor(color);
        this.atmosphere.SetAmbience(ambience);

        return atmosphere;
    }

    protected virtual void Spin()
    {
        if (!float.IsNaN(0.0f / this.day))
        {
            transform.Rotate(Vector3.up * -(73500.0f / this.day) * Time.deltaTime * Singleton.Instance.timeScale);
        }
    }

    public void RemoveAtmosphere()
    {
        this.atmosphere?.Destroy();
        this.atmosphere = null;
    }

    public void RemoveRing()
    {
        if (this.TryGetComponent(out Ring ring))
            ring.Destroy();
    }

    public void SetTilt(float axialTilt)
    {
        this.axialTilt = axialTilt;
        this.transform.localEulerAngles = Vector3.right * axialTilt;
    }
}