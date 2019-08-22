using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadGenerator : MonoBehaviour {
    public Material mat;

	// Use this for initialization
	void Start () {
        Mesh m = new Mesh();
        //頂点座標を格納
        m.vertices = new Vector3[] {
            new Vector3(-1.2f, -1.2f, 0),
            new Vector3(-1.2f, 1.2f, 0),
            new Vector3(1.2f, -1.2f, 0),
            new Vector3(1.2f, 1.2f, 0),
        };
        //UV座標を格納（頂点座標と対応したUV座標）
        m.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(1, 1),
        };

        m.triangles = new int[] {
            0, 1, 2,
            1, 3, 2,
        };

        GetComponent<MeshFilter>().sharedMesh = m;
        GetComponent<MeshRenderer>().material = mat;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}