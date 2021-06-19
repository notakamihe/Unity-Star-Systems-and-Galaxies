using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpaceProbeCamera : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public float verticalTiltMaxAngle = 180.0f;

    float mouseX;
    float mouseY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.parent.Rotate(-mouseY, 0, -mouseX);
    }
}