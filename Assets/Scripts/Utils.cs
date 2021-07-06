using System;
using System.IO;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;


public static class Utils
{
    public static void Destroy(MonoBehaviour instance, Object obj)
    {
        instance.StartCoroutine(WaitAndDestroy(obj));
    }

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
    
    static IEnumerator WaitAndDestroy(Object obj)
    {
        yield return new WaitForEndOfFrame();
        Object.DestroyImmediate(obj);
    }

    public static string SelectNameFromFile (string path)
    {
        string[] names = File.ReadAllLines(path);
        return RandomChoose(names);
    }
}