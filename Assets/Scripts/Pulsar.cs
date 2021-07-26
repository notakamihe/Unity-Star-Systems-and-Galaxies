using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Pulsar : MonoBehaviour
{
    public float rotationSpeed = 100.0f;

    private void Update()
    {
        this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * Singleton.Instance.timeScale);
    }
}