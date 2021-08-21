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
    public float intergalacticSpeed = 1000000000.0f;

    [HideInInspector] public SpaceProbeCamera probeCamera;

    [HideInInspector] public bool movePlayer = false;

    float maxInterstellarSpeed;
    float timeSpaceTabReleased;

    bool tabPressed;
    float timeTabPressed;

    private void OnEnable()
    {
        probe = this;
        this.probeCamera = this.GetComponentInChildren<SpaceProbeCamera>();
        this.maxInterstellarSpeed = this.interstellarSpeed * 100.0f;
    }

    private void Update()
    {
        if (this.transform.position.x > 50000.0f || this.transform.position.y > 50000.0f || this.transform.position.z > 50000.0f)
        {
            this.transform.position = Vector3.zero;
            this.movePlayer = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Singleton.Instance.timeScale = 0.000001f;
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

        float forward = 0.0f, horizontal = 0.0f, vertical = 0.0f;

        if (Input.GetKey(KeyCode.Tab))
        {
            if (!this.tabPressed)
            {
                this.tabPressed = true;
                this.timeTabPressed = Time.time;
            }

            if (!movePlayer)
            {
                if (Time.time - this.timeTabPressed >= 3.0f)
                {
                    this.probeCamera.ui.galaticSpeedProgressBar.gameObject.SetActive(false);

                    forward = Input.GetAxis("Forward") * this.intergalacticSpeed;
                    horizontal = Input.GetAxis("Horizontal") * this.intergalacticSpeed;
                    vertical = Input.GetAxis("Vertical") * this.intergalacticSpeed;
                }
                else
                {
                    Vector2 newDimensions = this.probeCamera.ui.galaticSpeedProgressBar.rectTransform.sizeDelta;
                    newDimensions.x = (Time.time - this.timeTabPressed) / 3.0f * 21.0f;

                    this.probeCamera.ui.galaticSpeedProgressBar.gameObject.SetActive(true);
                    this.probeCamera.ui.galaticSpeedProgressBar.rectTransform.sizeDelta = newDimensions;
                }
            }
        }
        else
        {
            this.probeCamera.ui.galaticSpeedProgressBar.gameObject.SetActive(false);
            this.tabPressed = false;

            if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetAxis("Forward") == 0 && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
                {
                    this.timeSpaceTabReleased = Time.time;
                }

                float speed = Mathf.Lerp(this.interstellarSpeed, this.maxInterstellarSpeed, (Time.time - this.timeSpaceTabReleased) / 15.0f);

                forward = Input.GetAxis("Forward") * speed;
                horizontal = Input.GetAxis("Horizontal") * speed;
                vertical = Input.GetAxis("Vertical") * speed;
            }
            else
            {
                this.timeSpaceTabReleased = Time.time;

                if (Input.GetKey(KeyCode.Space))
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
            }
        }

        Vector3 movement = this.transform.forward * forward + this.transform.right * horizontal + this.transform.up * vertical;

        if (!movePlayer)
            Universe.Move(-movement);
        else
        {
            this.Move(movement);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent(typeof(CelestialBody)))
        {
            this.probeCamera.camera.nearClipPlane = 0.03f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        this.probeCamera.camera.nearClipPlane = 7.0f;
    }

    public void Move(Vector3 direction)
    {
        this.transform.Translate(direction * Time.deltaTime, Space.World);
    }
}