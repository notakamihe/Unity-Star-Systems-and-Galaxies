using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SpaceProbe : MonoBehaviour
{
    public static SpaceProbe probe;

    public float speed = 100.0f;
    public float boostSpeed = 500.0f;
    public float slowSpeed = 5f;
    public float interstellarSpeed = 100000.0f;

    [HideInInspector] public SpaceProbeCamera probeCamera;

    private void Start()
    {
        probe = this;
        this.probeCamera = this.GetComponentInChildren<SpaceProbeCamera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Singleton.Instance.timeScale = 0.00001f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Singleton.Instance.timeScale = 0.001f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Singleton.Instance.timeScale = 0.01f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Singleton.Instance.timeScale = 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Singleton.Instance.timeScale = 1.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Singleton.Instance.timeScale = 2.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Singleton.Instance.timeScale = 4.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Singleton.Instance.timeScale = 5.0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Singleton.Instance.timeScale = 10.0f;
        }

        float forward, horizontal, vertical;

        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
        {
            forward = Input.GetAxis("Forward") * this.interstellarSpeed;
            horizontal = Input.GetAxis("Horizontal") * this.interstellarSpeed;
            vertical = Input.GetAxis("Vertical") * this.interstellarSpeed;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            forward = Input.GetAxis("Forward") * this.boostSpeed;
            horizontal = Input.GetAxis("Horizontal") * this.boostSpeed;
            vertical = Input.GetAxis("Vertical") * this.boostSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            forward = Input.GetAxis("Forward") * this.slowSpeed;
            horizontal = Input.GetAxis("Horizontal") * this.slowSpeed;
            vertical = Input.GetAxis("Vertical") * this.slowSpeed;
        }
        else
        {
            forward = Input.GetAxis("Forward") * this.speed;
            horizontal = Input.GetAxis("Horizontal") * this.speed;
            vertical = Input.GetAxis("Vertical") * this.speed;
        }

        Vector3 movement = this.transform.forward * forward + this.transform.right * horizontal + this.transform.up * vertical;
        Universe.Move(-movement);

        this.transform.position = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(typeof(CelestialBody)))
        {
            this.probeCamera.camera.nearClipPlane = 0.03f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        this.probeCamera.camera.nearClipPlane = 10.0f;
    }
}