using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Belt : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject asteroidPrefab;
    public int cubeDensity;
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

    private void Awake()
    {
        Random.InitState(seed);

        if (initializeOnAwake)
            Initialize();
    }

    public void Initialize()
    {
        for (int i = 0; i < cubeDensity; i++)
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
}