using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class WhiteDwarf : CompactStar
{
    Behaviour halo;

    public static WhiteDwarf Create(Transform parent, Vector3 position, float diameter, float mass)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        obj.transform.parent = parent;
        obj.transform.position = position;

        WhiteDwarf whiteDwarf = obj.AddComponent<WhiteDwarf>();
        whiteDwarf.SetName("White Dwarf");
        whiteDwarf.SetDiameter(diameter);
        whiteDwarf.SetMass(mass);
        whiteDwarf.SetMat(Singleton.Instance.whiteDwarfMat);
        whiteDwarf.temperature = 100000.0f;
        whiteDwarf.halo = whiteDwarf.CreateHalo(diameter * 10.0f, Color.white);

        return whiteDwarf;
    }

    private void Update()
    {
        this.Cool();
    }

    void Cool()
    {
        this.temperature = Mathf.Max(this.temperature - 1.0f * Time.deltaTime * Singleton.Instance.timeScale, 0.0f);
        this.Darken();
    }

    void Darken()
    {
        float t = Mathf.InverseLerp(-100000.0f, 0.0f, -this.temperature);
        this.renderer.material.color = Color.Lerp(Color.white, Color.black, t) * 2.0f;

        SerializedObject haloComponent = new SerializedObject(this.halo);
        haloComponent.FindProperty("m_Size").floatValue = Mathf.Lerp(-this.diameter * 10.0f, 0.0f, t) * -1.0f;
        haloComponent.ApplyModifiedProperties();
    }
}