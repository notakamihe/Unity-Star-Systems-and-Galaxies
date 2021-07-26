using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Belt : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject asteroidPrefab;
    public int density;
    public int seed;
    public float innerRadius;
    public float outerRadius;
    public float height;
    public bool rotatingClockwise;
    public bool initializeOnAwake = false;

    [Header("Asteroid Settings")]
    public float minOrbitSpeed;
    public float maxOrbitSpeed;
    public float minRotationSpeed;
    public float maxRotationSpeed;
    public float minDiameter;
    public float maxDiameter;

    private Vector3 localPosition;
    private Vector3 worldOffset;
    private Vector3 worldPosition;
    private float randomRadius;
    private float randomRadian;
    private float x;
    private float y;
    private float z;

    public float Thickness
    {
        get
        {
            return this.outerRadius - this.innerRadius;
        }
    }

    private void Awake()
    {
        Random.InitState(seed);

        if (initializeOnAwake)
            Initialize();
    }
   
    private void OnDrawGizmos()
    {
        Handles.color = new Color(1.0f, 1.0f, 1.0f, 0.05f);
        Handles.DrawWireDisc(this.transform.position, Vector3.up, this.innerRadius);
        Handles.DrawWireDisc(this.transform.position, Vector3.up, this.outerRadius);
    }

    public static Belt Create(Transform parent, Vector3 position, int density, float innerRadius, float thickness, float minOrbitSpeed,
        float maxOrbitSpeed, float height, float minRotationSpeed, float maxRotationSpeed, float minDiameter, float maxDiameter, 
        GameObject prefab)
    {
        GameObject obj = new GameObject("Belt");
        obj.transform.position = position;
        obj.transform.parent = parent;

        Belt belt = obj.AddComponent<Belt>();
        belt.asteroidPrefab = prefab;
        belt.density = density;
        belt.innerRadius = innerRadius;
        belt.outerRadius = innerRadius + thickness;
        belt.height = height;
        belt.minOrbitSpeed = minOrbitSpeed;
        belt.maxOrbitSpeed = maxOrbitSpeed;
        belt.minRotationSpeed = minRotationSpeed;
        belt.maxRotationSpeed = maxRotationSpeed;
        belt.minDiameter = minDiameter;
        belt.maxDiameter = maxDiameter;

        belt.Initialize();

        return belt;
    }

    public void ClearAndInitialize()
    {
        foreach (Transform child in this.transform)
        {
            Utils.Destroy(this, child.gameObject);
        }

        this.Initialize();
    }

    public void Initialize()
    {
        for (int i = 0; i < density; i++)
        {
            do
            {
                randomRadius = Random.Range(innerRadius, outerRadius);
                randomRadian = Random.Range(0, (2 * Mathf.PI));

                y = Random.Range(-(height / 2), (height / 2));
                x = randomRadius * Mathf.Cos(randomRadian);
                z = randomRadius * Mathf.Sin(randomRadian);
            }
            while (float.IsNaN(z) && float.IsNaN(x));

            localPosition = new Vector3(x, y, z);
            worldOffset = transform.rotation * localPosition;
            worldPosition = transform.position + worldOffset;

            GameObject asteroidObj = Instantiate(asteroidPrefab, worldPosition,
                Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));

            asteroidObj.AddComponent<Asteroid>().Initialize(Random.Range(minOrbitSpeed, maxOrbitSpeed),
                Random.Range(minRotationSpeed, maxRotationSpeed), transform, rotatingClockwise,
                Random.Range(minDiameter, maxDiameter));

            asteroidObj.transform.SetParent(transform);
        }
    }

    public bool Contains(Vector3 position)
    {
        float distanceFromCenter = Vector3.Distance(this.transform.position, position);
        return this.innerRadius <= distanceFromCenter && distanceFromCenter <= this.outerRadius;
    }
}