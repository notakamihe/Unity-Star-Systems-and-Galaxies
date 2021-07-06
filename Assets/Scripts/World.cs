using UnityEngine;


public class World : CelestialBody
{
    public float dayLength = 1.0f;
    public float axialTilt = 0.0f;

    [HideInInspector] public Atmosphere atmosphere;

    public float Gravity
    {
        get
        {
            float gravitationalFieldStrength = (G * this.mass) / Mathf.Pow(this.diameter * 0.5f, 2);
            return gravitationalFieldStrength * Mathf.Pow(1.25f, this.mass / 1000.0f - 20.0f) * 0.5f;
        }
    }
    
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
            return 0.0f <= this.temperature && this.temperature <= 100.0f;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        this.rb.isKinematic = true;
        this.SetDiameter(diameter);
        this.SetTilt(axialTilt);

        this.atmosphere = this.GetComponent<Atmosphere>() ?? null;
    }

    protected override void FixedUpdate()
    {
        transform.Rotate(0, -dayLength, 0);
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
        world.dayLength = dayLength;

        return world;
    }

    public Atmosphere AddAtmosphere(float thickness, Color color)
    {
        this.atmosphere = this.gameObject.AddComponent<Atmosphere>();
        this.atmosphere.SetThiccness(thickness);
        this.atmosphere.SetColor(color);

        return atmosphere;
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

        Vector3 newTilt = new Vector3(axialTilt, 0, 0);
        transform.localEulerAngles = newTilt;
    }
}