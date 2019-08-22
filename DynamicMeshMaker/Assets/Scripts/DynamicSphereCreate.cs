using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DynamicSphereCreate : MonoBehaviour {
    private MeshRenderer _renderer;
    private MeshRenderer Renderer => _renderer != null ? _renderer : (_renderer = GetComponent<MeshRenderer>());

    private MeshFilter _filter;
    private MeshFilter Filter => _filter != null ? _filter : (_filter = GetComponent<MeshFilter>());

    private Mesh _mesh;

    public int fov = 200;  //視野角（基本は200度で行う）
    public int width;  //横解像度（とりあえず100くらいからスタート）
    public int width_s;  //魚眼部分の左端の始まり（460とか？）
    public int width_l;  //魚眼部分の右端の終わり（1460とか？）
    public int height_s;  //魚眼部分の下端（40とか？）
    public int height_l;  //魚眼部分の上端（1040とか？）
    public int height;  //縦解像度（とりあえず100くらいからスタート）
    public int radiusFish;  //魚眼部分の半径ピクセル数（500とか？）
    public Vector2 centerUV;  //UV座標の中心となる座標（とりあえずは(0.5f, 0.5f)とか？）


    //Materialを保持するようにする
    [SerializeField]
    private Material _mat;

    // Use this for initialization
    void Start() {
    }

    private void Update() {
        Create();
    }

    public Vector2Int divide;

    [ContextMenu("Create")]
    void Create() {
        int divideX = divide.x;
        int divideY = divide.y;

        MeshData data = CreateSphere(divideX, divideY);
        if (_mesh == null) {
            _mesh = new Mesh();
        }
        _mesh.vertices = data.vertices;
        _mesh.SetIndices(data.indices, MeshTopology.Triangles, 0);
        _mesh.uv = data.uvs;
        Filter.mesh = _mesh;

        //MeshRendererからMaterialにアクセスし、Materialをセットするようにする
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = _mat;
        _mesh.RecalculateNormals();
    }

    struct MeshData {
        public Vector3[] vertices;
        public int[] indices;
        public Vector2[] uvs;
    }

    MeshData CreateSphere(int divideX, int divideY, float size = 1f) {
        divideX = divideX < 4 ? 4 : divideX;
        divideY = divideY < 4 ? 4 : divideY;

        //
        //頂点座標作成
        //

        //半径
        float r = size * 0.5f;
        int cnt = 0;
        int vertCount = divideX * fov / 2 + 1;  //頂点の数
        Vector3[] vertices = new Vector3[vertCount];

        //中心角
        float centerRadianX = 2f * Mathf.PI / (float)divideX;
        float centerRadianY = 2f * Mathf.PI / (float)divideY;

        //
        //☆UV座標作成（同時に行う）
        //
        Vector2[] uvs = new Vector2[vertCount];
        int cnt_uv = 0;

        //天面
        vertices[cnt] = new Vector3(0, r, 0);
        cnt++;

        //☆天頂のUV座標
        uvs[cnt_uv] = centerUV;
        cnt_uv++;

        //側面
        for (int vy = 0; vy < fov; vy++) {
            float yRadian = (float)(vy + 1) * centerRadianY / 2f;

            //一辺の長さ
            float tmpLen = Mathf.Abs(Mathf.Sin(yRadian));

            float y = Mathf.Cos(yRadian);
            for (int vx = 0; vx < divideX; vx++) {
                Vector3 pos = new Vector3(
                    tmpLen * Mathf.Sin((float)vx * centerRadianX),
                    y,
                    tmpLen * Mathf.Cos((float)vx * centerRadianX)
                );
                //サイズ反映
                vertices[cnt] = pos * r;
                if (cnt < vertCount - 1) {
                    cnt++;
                }

                //☆UV座標の設定
                uvs[cnt_uv] = centerUV + new Vector2((radiusFish / (fov / 2)) * (vy + 1) * Mathf.Sin(vx * centerRadianX) / width,
                                                    (radiusFish / (fov / 2)) * (vy + 1) * Mathf.Cos(vx * centerRadianX) / height);
                if (cnt_uv < vertCount - 1) {
                    cnt_uv++;
                }
            }
        }

        //底面
        //vertices[cnt++] = new Vector3(0, -r, 0);

        //
        //頂点インデックス作成
        //

        int topAndBottomTriCount = divideX;  //天面にある三角形面の数
        //側面三角形の数
        int aspectTriCount = divideX * (fov / 2 - 1) * 2;

        int[] indices = new int[(topAndBottomTriCount + aspectTriCount) * 3];

        //天面
        int offsetIndex = 0;
        cnt = 0;
        for (int i = 0; i < divideX * 3; i++) {
            if (i % 3 == 0) {
                //天面の出っ張り
                indices[cnt++] = 0;
            } else if (i % 3 == 1) {
                int index = offsetIndex + 2;
                //ループさせるためにindices[2]を使う（分割数を超えると最初に戻る）
                index = index > divideX ? indices[2] : index;
                indices[cnt] = index;
            } else if (i % 3 == 2) {
                indices[cnt] = offsetIndex + 1;
                offsetIndex++;
            }

        }

        //開始Index番号
        int startIndex = indices[2];

        //天面、底面を除いたIndex要素数
        int sideIndexLen = divideX * (fov / 2 - 1) * 2 * 3;

        int lap1stIndex = 0;
        int lap2ndIndex = 0;

        //一周したときのindex数
        int lapDiv = divideX * 2 * 3;

        int createSquareFaceCount = 0;

        for (int i = 0; i < sideIndexLen; i++) {
            //一周の頂点数を超えたら更新（初回も含む）
            if (i % lapDiv == 0) {
                lap1stIndex = startIndex;
                lap2ndIndex = startIndex + divideX;
                createSquareFaceCount++;
            }

            if (i % 6 == 0 || i % 6 == 3) {
                indices[cnt++] = startIndex;
            } else if (i % 6 == 2) {
                indices[cnt++] = startIndex + divideX;
            } else if (i % 6 == 1) {
                if (i > 0 &&
                    (i % (lapDiv * createSquareFaceCount - 1) == 0 ||
                     i % (lapDiv * createSquareFaceCount - 5) == 0)
                ) {
                    //一周したときのループ処理
                    //集会ポリゴンの最後から二番目のindex
                    indices[cnt++] = lap2ndIndex;
                } else {
                    indices[cnt++] = startIndex + divideX + 1;
                }
            } else if (i % 6 == 4) {
                if (i > 0 && i % (lapDiv * createSquareFaceCount - 2) == 0) {
                    //一周したときのループ処理
                    //周回ポリゴンの最後のIndex
                    indices[cnt++] = lap1stIndex;
                } else {
                    indices[cnt++] = startIndex + 1;
                }
            } else if (i % 6 == 5) {
                if (i > 0 &&
                    (i % (lapDiv * createSquareFaceCount - 1) == 0 ||
                     i % (lapDiv * createSquareFaceCount - 5) == 0)
                ) {
                    //一周したときのループ処理
                    //集会ポリゴンの最後から二番目のindex
                    indices[cnt++] = lap2ndIndex;
                } else {
                    indices[cnt++] = startIndex + divideX + 1;
                }

                startIndex++;
            }
        }

        //底面Index
        //offsetIndex = vertices.Length - 1 - divideX;
        //int loopIndex = offsetIndex;

        //for (int i = divideX * 3 - 1; i >= 0; i--) {
        //    if (i % 3 == 0) {
        //        //底面の先頂点
        //        indices[cnt++] = vertices.Length - 1;
        //        offsetIndex++;
        //    } else if (i % 3 == 1) {
        //        indices[cnt++] = offsetIndex;
        //    } else if (i % 3 == 2) {
        //        int value = 1 + offsetIndex;
        //        if (value >= vertices.Length - 1) {
        //            value = loopIndex;
        //        }

        //        indices[cnt++] = value;
        //    }
        //}

        

        return new MeshData() {
            indices = indices,
            vertices = vertices,
            uvs = uvs,
        };
    }
}