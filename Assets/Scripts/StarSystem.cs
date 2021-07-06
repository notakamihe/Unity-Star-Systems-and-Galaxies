using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

[ExecuteInEditMode]
public abstract class StarSystem : MonoBehaviour
{
    [Range(1000, 5000)] public float starSize = 1000.0f;
    [Range(0.0f, 50.0f)] public float starGrowthSpeed = 5.0f;
    public bool isProgradeClockwise = false;
    public Material starMat;

    protected Star star;

    public abstract void Clear(Transform target = null);
    protected abstract void SetSystem();
    protected abstract void UpdateSystem();
    
    protected Star CreateStar(string name)
    {
        star = Star.Create(name, this, this.starMat);
        star.transform.localPosition = Vector3.zero;
        this.UpdateSystem();

        return star;
    }

}