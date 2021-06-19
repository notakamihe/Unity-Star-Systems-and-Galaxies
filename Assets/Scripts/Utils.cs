using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;


public class Utils
{
    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }

    public static T RandomChoose<T>(T[] arr)
    {
        try
        {
            return arr[Random.Range(0, arr.Length)];
        } catch (Exception)
        {
            return default;
        }
    }
}