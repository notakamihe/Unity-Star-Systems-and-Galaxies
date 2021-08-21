using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class GalaxyCluster : MonoBehaviour
{
    [Range(1.0f, 10.0f)] public float diameterInMPC = 1.0f;
    [Range(1, 20)] public int numGalaxies = 10;

    float Diameter
    {
        get { return this.diameterInMPC * Units.MPC; }
    }

    float Radius
    {
        get { return this.Diameter / 2.0f; }
    }

    private void Start()
    {
        for (int i = 0; i < numGalaxies; i++)
        {
            Vector3 pos;

            pos.x = this.transform.position.x + Utils.NextFloat(0.0f, this.Radius) * Mathf.Sin(Utils.NextFloat(0.0f, 360.0f) * Mathf.Deg2Rad);
            pos.y = this.transform.position.y + Utils.NextFloat(0.0f, this.Radius) * Mathf.Sin(Utils.NextFloat(0.0f, 360.0f) * Mathf.Deg2Rad);
            pos.z = this.transform.position.z + Utils.NextFloat(0.0f, this.Radius) * Mathf.Sin(Utils.NextFloat(0.0f, 360.0f) * Mathf.Deg2Rad);

            Color color = Color.HSVToRGB(Utils.NextFloat(0.0f, 1.0f), Utils.NextFloat(0.0f, 1.0f), 1.0f);
            Galaxy galaxy = Galaxy.Create(this.transform, pos, Galaxy.GeneratedName, (GalaxyShape) Utils.random.Next(0, 2), 
                Utils.NextFloat(1.0f, 100.0f), Utils.NextFloat(0.1f, 0.3f), Utils.random.Next(2, 7), Utils.random.Next(1, 100), color);
            galaxy.transform.rotation = Utils.RandomRotation;
        }
    }
}