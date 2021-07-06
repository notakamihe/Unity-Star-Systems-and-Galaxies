using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Ring : MonoBehaviour
{

	//manual settings
	[Range(3, 360)]
	public int segments = 3;
	public float innerRadius = 0.7f;
	public float thickness = 0.5f;
	public Material ringMat;

	//cached references
	GameObject ring;
	Mesh ringMesh;
	MeshFilter ringMF;
	MeshRenderer ringMR;

	void OnEnable()
	{
		if (ring == null || ringMesh == null)
		{
			SetUpRing();
		}
		BuildRingMesh();
	}

	void OnValidate()
	{
		if (ring == null || ringMesh == null)
		{
			SetUpRing();
		}
		BuildRingMesh();
	}

    public void Destroy()
    {
		Utils.Destroy(this, this.ring);
		Utils.Destroy(this, this);
    }

    void SetUpRing()
	{
		foreach (Transform child in this.transform)
        {
			if (child.CompareTag("Ring"))
            {
				ring = child.gameObject;
				ringMF = ring.GetComponent<MeshFilter>();
				ringMR = ring.GetComponent<MeshRenderer>();
				ringMesh = new Mesh();
				ringMF.sharedMesh = ringMesh;
				return;
			}
        }

		//check if ring is null and there are no children
		if (ring == null)
		{
			//create ring object
			ring = new GameObject(name + " Ring");
			ring.tag = "Ring";
			ring.transform.parent = transform;
			ring.transform.SetAsFirstSibling();
			ring.transform.localScale = Vector3.one;
			ring.transform.localPosition = Vector3.zero;
			ring.transform.localRotation = Quaternion.identity;
			ringMF = ring.AddComponent<MeshFilter>();
			ringMR = ring.AddComponent<MeshRenderer>();
			ringMR.material = ringMat;
		}
		else
		{
			ring = transform.GetChild(0).gameObject;
			ringMF = ring.GetComponent<MeshFilter>();
			ringMR = ring.GetComponent<MeshRenderer>();
		}

		ringMesh = new Mesh();
		ringMF.sharedMesh = ringMesh;
	}

	void BuildRingMesh()
	{
		//build ring mesh
		Vector3[] vertices = new Vector3[(segments + 1) * 2 * 2];
		int[] triangles = new int[segments * 6 * 2];
		Vector2[] uv = new Vector2[(segments + 1) * 2 * 2];
		int halfway = (segments + 1) * 2;

		for (int i = 0; i < segments + 1; i++)
		{
			float progress = (float)i / (float)segments;
			float angle = Mathf.Deg2Rad * progress * 360;
			float x = Mathf.Sin(angle);
			float z = Mathf.Cos(angle);

			vertices[i * 2] = vertices[i * 2 + halfway] = new Vector3(x, 0f, z) * (innerRadius + thickness);
			vertices[i * 2 + 1] = vertices[i * 2 + 1 + halfway] = new Vector3(x, 0f, z) * innerRadius;
			uv[i * 2] = uv[i * 2 + halfway] = new Vector2(progress, 0f);
			uv[i * 2 + 1] = uv[i * 2 + 1 + halfway] = new Vector2(progress, 1f);

			if (i != segments)
			{
				triangles[i * 12] = i * 2;
				triangles[i * 12 + 1] = triangles[i * 12 + 4] = (i + 1) * 2;
				triangles[i * 12 + 2] = triangles[i * 12 + 3] = i * 2 + 1;
				triangles[i * 12 + 5] = (i + 1) * 2 + 1;

				triangles[i * 12 + 6] = i * 2 + halfway;
				triangles[i * 12 + 7] = triangles[i * 12 + 10] = i * 2 + 1 + halfway;
				triangles[i * 12 + 8] = triangles[i * 12 + 9] = (i + 1) * 2 + halfway;
				triangles[i * 12 + 11] = (i + 1) * 2 + 1 + halfway;
			}

		}

		if (vertices.Length < ringMesh.vertices.Length)
		{
			ringMesh.triangles = triangles;
			ringMesh.vertices = vertices;
		}
		else
		{
			ringMesh.vertices = vertices;
			ringMesh.triangles = triangles;
		}
		ringMesh.uv = uv;
		ringMesh.RecalculateNormals();

		ringMR.sharedMaterial = ringMat;
	}
}