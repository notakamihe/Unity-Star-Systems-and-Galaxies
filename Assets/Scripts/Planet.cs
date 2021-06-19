using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Planet : World
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void setMaterial(Material material)
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.sharedMaterial = material;
    }
}