using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Moon : World
{
    public bool isTidallyLocked = true;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Spin()
    {
        if (this.isTidallyLocked)
        {
            Orbit orbit = this.GetComponentInParent<Orbit>();

            if (orbit)
            {
                this.SetTilt(this.axialTilt);

                if (this.transform.position - orbit.parent.position != Vector3.zero)
                    this.transform.rotation = Quaternion.LookRotation(this.transform.position - orbit.parent.position);
            }
        }
        else
            base.Spin();
    }
}