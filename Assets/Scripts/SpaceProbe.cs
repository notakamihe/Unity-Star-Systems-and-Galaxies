using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpaceProbe : MonoBehaviour
{
    public static SpaceProbe probe;

    public float speed = 1000.0f;
    [HideInInspector] public bool escape = false;
    [HideInInspector] public Vector3 escapeDir = Vector3.zero;

    Rigidbody rb;

    float forward;
    float horizontal;
    float accelerationScale;

    private void Start()
    {
        probe = this;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        accelerationScale = Input.GetKey(KeyCode.LeftShift) ? 10 : 1;

        rb.AddForce((transform.up * forward + transform.right * horizontal) * accelerationScale);

        if (Input.GetKeyDown(KeyCode.Q) && escape)
        {
            Escape();
        }
    }

    private void Escape()
    {
        escape = false;
        rb.AddForce(escapeDir * 7500000 * Time.deltaTime);
    }
}