using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Universe : MonoBehaviour
{
    public static bool shouldMove = true;
    public static Universe universe;

    void OnEnable()
    {
        universe = this;
    }

    public static void Move(Vector3 movement)
    {
        universe.transform.Translate(movement * Time.deltaTime);
    }
}