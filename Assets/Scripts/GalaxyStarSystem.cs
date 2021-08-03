using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GalaxyStarSystem : ProceduralStarSystem
{
    protected override void OnEnable()
    {
        base.OnEnable();
        this.ToggleBodies(false);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.gameObject == SpaceProbe.probe?.gameObject)
        {
            this.ToggleBodies(true);
            SpaceProbe.probe.probeCamera.ui.nearestStar.gameObject.SetActive(false);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.gameObject == SpaceProbe.probe?.gameObject)
        {
            this.ToggleBodies(false);
            SpaceProbe.probe.probeCamera.ui.nearestStar.gameObject.SetActive(true);
        }

    }

    public static GalaxyStarSystem Create(Transform parent, Vector3 pos)
    {
        GameObject gameObject = new GameObject("Galaxy");
        gameObject.transform.position = pos;
        gameObject.transform.parent = parent;

        GalaxyStarSystem galaxyStarSystem = gameObject.AddComponent<GalaxyStarSystem>();
        return galaxyStarSystem;
    }

    void ToggleBodies(bool show)
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject != this.star.gameObject)
            {
                child.gameObject.SetActive(show);
            }
        }
    }
}