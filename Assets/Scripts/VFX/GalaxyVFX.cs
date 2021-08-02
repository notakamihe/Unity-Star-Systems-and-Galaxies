using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
public class GalaxyVFX : MonoBehaviour
{
    public Color color = Color.white;
    [Range(0.0f, 1.0f)] public float centerDiameterPercentage = 0.2f;
    [Range(2, 7)] public int numArms = 2;
    public bool isSpiral = true;

    public ParticleSystem vfx;
    public ParticleSystem dust;
    public Behaviour center;

    bool played;

    private void OnEnable()
    {
        this.StartCoroutine(ChangeGalaxySpeed(0.0f, 1.0f));
    }

    private void Update()
    {
        if (this.vfx.isPlaying && !this.played)
        {
            this.played = true;
            this.StartCoroutine(ChangeGalaxySpeed(this.vfx.duration * 1.1f, 0.001f));
        }
        else if ((!this.vfx.isEmitting || this.vfx.time == 0.0f) && this.played)
        {
            this.played = false;
            this.StartCoroutine(ChangeGalaxySpeed(0.0f, 1.0f));
        }
    }

    private void OnValidate()
    {
        this.StartCoroutine(ChangeGalaxySpeed(0.0f, 1.0f));
        this.played = false;

        this.SetArms(this.numArms);
        this.SetColor(this.color);
        this.SetCenterSizePercentage(this.centerDiameterPercentage);
    }

    public void SetArms(int numArms)
    {
        ParticleSystem.ShapeModule shape = this.vfx.shape;
        ParticleSystem.ShapeModule dustShape = this.dust.shape;
        
        shape.arcSpread = this.isSpiral ? (1.0f / (float)numArms) : 0.0f;
        dustShape.arcSpread = this.isSpiral ? (1.0f / (float)numArms) : 0.0f;
    }

    public void SetColor(Color color)
    {
        this.color = color;
        this.vfx.startColor = color;

        SerializedObject haloComponent = new SerializedObject(this.center);
        haloComponent.FindProperty("m_Color").colorValue = color;
        haloComponent.ApplyModifiedProperties();
    }

    public void SetCenterSizePercentage(float centerSizePercentage)
    {
        this.centerDiameterPercentage = centerSizePercentage;

        SerializedObject haloComponent = new SerializedObject(this.center);
        haloComponent.FindProperty("m_Size").floatValue = Mathf.Max(centerSizePercentage * this.vfx.transform.localScale.x * 666.0f);
        haloComponent.ApplyModifiedProperties();
    }

    IEnumerator ChangeGalaxySpeed(float delay, float speed)
    {
        yield return new WaitForSeconds(delay);

        ParticleSystem.MainModule main = this.vfx.main;
        ParticleSystem.MainModule dustMain = this.dust.main;

        main.simulationSpeed = speed;
        dustMain.simulationSpeed = speed;
    }
}