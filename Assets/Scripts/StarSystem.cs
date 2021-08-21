using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(SphereCollider))]
[ExecuteInEditMode]
public abstract class StarSystem : MonoBehaviour
{
    [HideInInspector] public float size = 100000.0f;

    [HideInInspector] public Star star;
    protected SphereCollider collider;

    protected bool probeEntered;
    bool showSizeGizmo;

    public abstract void Clear(Transform target = null);
    public abstract void UpdateStarProperties();

    protected abstract void GetNearestPlanets();
    protected abstract void SetSize();
    protected abstract void SetSystem();
    protected abstract void UpdateSystem();
   
    protected virtual void Update()
    {
        if (this.probeEntered)
           this.GetNearestPlanets();
    }

    protected virtual void OnEnable()
    {
        this.collider = this.GetComponent<SphereCollider>();
        this.collider.isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        if (SpaceProbe.probe && Utils.DistanceFromSurface(this.transform.position, SpaceProbe.probe.transform.position, this.size) <= Units.LIGHT_YEAR)
        {
            Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.25f);
            Handles.DrawWireDisc(this.transform.position, this.transform.up, this.size);
            Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.10f);
            Handles.DrawLine(this.transform.position, this.transform.position + this.transform.forward * this.size);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == SpaceProbe.probe.gameObject)
        {
            this.probeEntered = true;
            SpaceProbe.probe.probeCamera.ui.precedingPlanet.gameObject.SetActive(true);
            SpaceProbe.probe.probeCamera.ui.succeedingPlanet.gameObject.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject == SpaceProbe.probe.gameObject)
        {
            this.probeEntered = false;
            SpaceProbe.probe.probeCamera.ui.precedingPlanet.gameObject.SetActive(false);
            SpaceProbe.probe.probeCamera.ui.succeedingPlanet.gameObject.SetActive(false);
        }
    }

    public void Die(Transform target)
    {
        this.Clear(target);

        this.probeEntered = false;
        SpaceProbe.probe.probeCamera.ui.precedingPlanet.gameObject.SetActive(false);
        SpaceProbe.probe.probeCamera.ui.succeedingPlanet.gameObject.SetActive(false);


        this.collider.enabled = false;
        Utils.Destroy(this, this);
    }
}