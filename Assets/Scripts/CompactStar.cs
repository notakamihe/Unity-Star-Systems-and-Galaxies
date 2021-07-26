using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class CompactStar : CelestialBody
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public Behaviour CreateHalo(float size, Color color)
    {
        GameObject obj = Instantiate(Singleton.Instance.halo, this.transform.position, Quaternion.identity, this.transform);
        obj.transform.localPosition = Vector3.zero;

        Behaviour halo = (Behaviour)obj.GetComponent("Halo");
        SerializedObject haloComponent = new SerializedObject(halo);

        haloComponent.FindProperty("m_Size").floatValue = size;
        haloComponent.FindProperty("m_Color").colorValue = color;
        haloComponent.ApplyModifiedProperties();

        return halo;
    }
}