using System;
using System.IO;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;


public static class Utils
{
    public static System.Random random = new System.Random();

    public static Quaternion RandomRotation
    {
        get { return Quaternion.Euler(NextFloat(0.0f, 360.0f), NextFloat(0.0f, 360.0f), NextFloat(0.0f, 360.0f)); }
    }

    public static void Destroy(MonoBehaviour instance, Object obj)
    {
        instance.StartCoroutine(WaitAndDestroy(obj));
    }

    public static float DistanceFromSurface(Vector3 pos, Vector3 otherPos, float radius)
    {
        return Vector3.Distance(pos, otherPos) - radius;
    }

    public static Behaviour GetHaloInChildren(Transform transform)
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent("Halo"))
                return (Behaviour)child.GetComponent("Halo");
        }

        return null;
    }

    public static float NextFloat(float min, float max)
    {
        return (float) (random.NextDouble() * (max - min) + min);
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }

    public static T RandomChoose<T>(T[] arr)
    {
        try
        {
            return arr[random.Next(0, arr.Length)];
        } catch (Exception)
        {
            return default;
        }
    }

    public static string ToRoman(int number)
    {
        if (number >= 1000) 
            return "M" + ToRoman(number - 1000);
        if (number >= 900) 
            return "CM" + ToRoman(number - 900);
        if (number >= 500) 
            return "D" + ToRoman(number - 500);
        if (number >= 400) 
            return "CD" + ToRoman(number - 400);
        if (number >= 100) 
            return "C" + ToRoman(number - 100);
        if (number >= 90) 
            return "XC" + ToRoman(number - 90);
        if (number >= 50) 
            return "L" + ToRoman(number - 50);
        if (number >= 40) 
            return "XL" + ToRoman(number - 40);
        if (number >= 10) 
            return "X" + ToRoman(number - 10);
        if (number >= 9) 
            return "IX" + ToRoman(number - 9);
        if (number >= 5) 
            return "V" + ToRoman(number - 5);
        if (number >= 4) 
            return "IV" + ToRoman(number - 4);
        if (number >= 1) 
            return "I" + ToRoman(number - 1);
        else 
            return string.Empty;
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