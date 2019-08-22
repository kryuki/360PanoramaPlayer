using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class DynamicTriangleCreate : MonoBehaviour {
    //Materialを保持するようにする
    [SerializeField]
    private Material _mat;
    void Start() {
        //Meshクラスでメッシュを作成
        Mesh m = new Mesh();
        //頂点座標配列
        m.vertices = new Vector3[] {
            new Vector3 (0, 1f),
            new Vector3 (1f, -1f),
            new Vector3 (-1f, -1f),
        };
        //頂点を結ぶ順番配列
        m.triangles = new int[] {
            0, 1, 2
        };

        //各頂点に色情報を設定
        m.colors = new Color[] {
            Color.white,
            Color.red,
            Color.green
        };

        //法線ベクトルの再計算処理（特に理由がない場合は必ず呼ぶことになる）
        m.RecalculateNormals();
        MeshFilter filter = GetComponent<MeshFilter>();
        filter.sharedMesh = m;

        //MeshRendererからMaterialにアクセスし、Materialをセットするようにする
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = _mat;
    }
}