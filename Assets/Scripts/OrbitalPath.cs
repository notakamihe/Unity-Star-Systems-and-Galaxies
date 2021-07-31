using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class OrbitalPath : MonoBehaviour
{
    private Transform target;
    public int segments;
    public float radius;
    LineRenderer line;

    void Start()
    {

        target = transform.GetComponentInParent<Orbit>().transform;
        line = gameObject.GetComponent<LineRenderer>();

        transform.position = target.position;
        radius = Vector2.Distance(target.position, transform.parent.position);

        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        CreatePoints();
    }


    //void CreatePoints ()
    void CreatePoints()
    {

        float x;
        float y;
        float z = 0f;

        float angle = 180f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = (Mathf.Sin(Mathf.Deg2Rad * angle) * radius);
            y = (Mathf.Cos(Mathf.Deg2Rad * angle) * radius);

            line.SetPosition(i, new Vector3(x, y, z));

            angle += (-180f / segments);
        }

    }
}
