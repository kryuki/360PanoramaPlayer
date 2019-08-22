using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicCreateMesh : MonoBehaviour {
    //Materialを保持するようにする
    [SerializeField]
    private Material _mat;

	// Use this for initialization
	void Start () {
        Mesh mesh = new Mesh();
        //頂点座標配列
        mesh.vertices = new Vector3[] {
            new Vector3(0, 1f),
            new Vector3(1f, -1f),
            new Vector3(-1f, -1f),
        };
        //頂点を結ぶ順番配列
        mesh.triangles = new int[] {
            0, 1, 2
        };

        //各頂点に対してUV座標を設定する
        mesh.uv = new Vector2[] {
            new Vector2(0.5f, 1f),
            new Vector2(1f, 0),
            new Vector2(0, 0),
        };
        //法線ベクトルの再計算処理（特に理由がない場合は必ず呼ぶ）
        mesh.RecalculateNormals();
        MeshFilter filter = GetComponent<MeshFilter>();
        filter.sharedMesh = mesh;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = _mat;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
