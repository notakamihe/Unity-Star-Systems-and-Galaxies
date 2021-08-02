using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ProbeMovementAdjuster : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (SpaceProbe.probe && other.GetComponent(typeof(World)) && !SpaceProbe.probe.movePlayer)
        {
            SpaceProbe.probe.movePlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (SpaceProbe.probe && other.GetComponent(typeof(World)))
        {
            SpaceProbe.probe.movePlayer = false;
        }
    }
}