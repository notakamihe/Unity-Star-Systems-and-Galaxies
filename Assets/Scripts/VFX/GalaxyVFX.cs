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
    [Range(2, 7)] public int numArms = 2;
    public bool isSpiral = true;

    public ParticleSystem vfx;
    public ParticleSystem dust;
    public ParticleSystem colorDust;
    public ParticleSystem center;

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
            this.StartCoroutine(ChangeGalaxySpeed(5.0f * 1.1f, 0.001f));
        }
        else if ((!this.vfx.isEmitting || this.vfx.time == 0.0f) && this.played)
        {
            this.played = false;
            this.StartCoroutine(ChangeGalaxySpeed(0.0f, 1.0f));
        }
    }

    private void OnValidate()
    {
        if (this.gameObject.active)
            this.StartCoroutine(ChangeGalaxySpeed(0.0f, 1.0f));

        this.played = false;

        this.SetArms(this.numArms);
        this.SetColor(this.color);
    }

    public void SetArms(int numArms)
    {
        ParticleSystem.ShapeModule shape = this.vfx.shape;
        ParticleSystem.ShapeModule dustShape = this.dust.shape;
        ParticleSystem.ShapeModule colorShape = this.colorDust.shape;
        ParticleSystem.ShapeModule centerShape = this.center.shape;
        
        shape.arcSpread = this.isSpiral ? (1.0f / (float)numArms) : 0.0f;
        dustShape.arcSpread = this.isSpiral ? (1.0f / (float)numArms) : 0.0f;
        colorShape.arcSpread = this.isSpiral ? (1.0f / (float)numArms) : 0.0f;
        centerShape.arcSpread = this.isSpiral ? (1.0f / (float)numArms) : 0.0f;
    }

    public void SetColor(Color color)
    {
        color.a = 0.066f;

        this.color = color;
        this.colorDust.startColor = color;
    }

    IEnumerator ChangeGalaxySpeed(float delay, float speed)
    {
        yield return new WaitForSeconds(delay);

        ParticleSystem.MainModule main = this.vfx.main;
        ParticleSystem.MainModule dustMain = this.dust.main;
        ParticleSystem.MainModule colorDustMain = this.colorDust.main;
        ParticleSystem.MainModule centerMain = this.center.main;

        main.simulationSpeed = speed;
        dustMain.simulationSpeed = speed;
        colorDustMain.simulationSpeed = speed;
        centerMain.simulationSpeed = speed;
    }
}