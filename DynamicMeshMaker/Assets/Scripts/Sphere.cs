using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Sphere : MonoBehaviour {
    private MeshRenderer _renderer;
    private MeshRenderer Renderer => _renderer != null ? _renderer : (_renderer = GetComponent<MeshRenderer>());

    private MeshFilter _filter;
    private MeshFilter Filter => _filter != null ? _filter : (_filter = GetComponent<MeshFilter>());

    private Mesh _mesh;

    //メッシュ貼り付け用変数
    public float fov = 200;  //魚眼カメラの視野角（仕様上は200度？）
    public int width = 1920;  //映像の横解像度
    public int height = 1080;  //映像の縦解像度
    public int radiusPixels = 500;  //魚眼部分の半径ピクセル数（調整が必要だがとりあえず500くらいで）
    public Vector2 centerPixelPos = new Vector2(960, 540);  //UV座標の中心となるピクセル座標（とりあえずど真ん中の（960, 540）に設定しておく）
    public float enlarge = 0f;  //拡大率パーセント（拡大率20％なら、20を入れる）

    //Materialを保持するようにする
    [SerializeField]
    private Material _mat;

    //カメラから入力した画像を表示する
    int fps = 30;  //FPS
    WebCamTexture webcamTexture;

    public Vector2Int divide;  //横の分割数、縦の分割数を入れる構造体
    public float sphereSize;  //球のサイズ

    //左右反転と上下反転を操作する
    public bool rightLeftInvert = false;
    public bool upDownInvert = false;

    void Update() {
        Create();
    }

    [ContextMenu("Create")]
    void Create() {
        int divideX = divide.x;
        int divideY = divide.y;
        float size = sphereSize;

        MeshData data = CreateSphere(divideX, divideY, sphereSize);
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
        //分割数は最低でもそれぞれ4にする
        divideX = divideX < 4 ? 4 : divideX;
        divideY = divideY < 4 ? 4 : divideY;

        //
        //頂点座標作成
        //

        //半径
        float r = size * 0.5f;
        //カウンタ
        int cnt_vert = 0;
        //頂点の数（天面と底面合わせて）
        int vertCount;
        if (fov == 360) {
            vertCount = divideX * (divideY - 1) + 2;
        } else {
            vertCount = divideX * divideY + 1;
        }
        
        //頂点の配列を生成
        Vector3[] vertices = new Vector3[vertCount];

        //中心角の単位
        float centerRadianX = 2f * Mathf.PI / (float)divideX;  //360度を横分割数で割ったもの
        float centerRadianY = (fov / 2) * Mathf.Deg2Rad / (float)divideY;  //FOVの半分の値を縦分割数で割ったもの

        //
        //☆UV座標の設定（同時に行う）
        //
        Vector2[] uvs = new Vector2[vertCount];
        //カウンタ
        int cnt_uvs = 0;

        //天面の頂点座標
        vertices[cnt_vert] = new Vector3(0, r, 0);
        cnt_vert++;

        //☆天面のUV座標
        uvs[cnt_uvs] = new Vector2(centerPixelPos.x / width, centerPixelPos.y / height);
        cnt_uvs++;

        //側面
        for (int vy = 0; vy < fov; vy++) {
            float yRadian = (float)(vy + 1) * centerRadianY;

            //中心軸からの距離
            float tmpLen = r * Mathf.Sin(yRadian);

            for (int vx = 0; vx < divideX; vx++) {
                vertices[cnt_vert] = new Vector3(tmpLen * Mathf.Sin((float)vx * centerRadianX),
                                            r * Mathf.Cos(yRadian),
                                            tmpLen * Mathf.Cos((float)vx * centerRadianX)
                                            );

                if (cnt_vert < vertCount - 1) {
                    cnt_vert++;
                }

                //☆UV座標の設定
                //左右反転用インデックス
                int rightLeftInvertIndex;
                if (rightLeftInvert) {
                    rightLeftInvertIndex = 1;
                } else {
                    rightLeftInvertIndex = -1;
                }
                //上下反転用インデックス
                int upDownInvertIndex;
                if (upDownInvert) {
                    upDownInvertIndex = 1;
                } else {
                    upDownInvertIndex = -1;
                }

                uvs[cnt_uvs] = uvs[0] + new Vector2(rightLeftInvertIndex * ((float)radiusPixels / ((float)fov * (1 + enlarge / 100) / 2)) * (vy + 1) * Mathf.Sin(vx * centerRadianX) / width,
                                                    upDownInvertIndex * ((float)radiusPixels / ((float)fov * (1 + enlarge / 100) / 2)) * (vy + 1) * Mathf.Cos(vx * centerRadianX) / height);

                if (cnt_uvs < vertCount - 1) {
                    cnt_uvs++;
                }
            }
        }

        //最後の頂点座標
        if (fov != 360) {
            vertices[vertCount - 1] = vertices[vertCount - divideX];
        }

        //底面
        if (fov == 360) {
            vertices[cnt_vert] = new Vector3(0, -r, 0);
        }

        //
        //頂点インデックス情報
        //

        //天面と底面の三角形の数
        int topAndBottomTriCount;
        if (fov == 360) {
            topAndBottomTriCount = divideX * 2;
        } else {
            topAndBottomTriCount = divideX;
        }

        //天面と底面以外の三角形の数
        int aspectTriCount;
        if (fov == 360) {
            aspectTriCount = divideX * (divideY - 2) * 2;
        } else {
            aspectTriCount = divideX * (divideY - 1) * 2;
        }

        //頂点インデックスの配列を生成
        int[] indices = new int[(topAndBottomTriCount + aspectTriCount) * 3];

        //天面
        int offsetIndex = 0;
        cnt_vert = 0;
        for (int i = 0; i < divideX * 3; i++) {
            if (i % 3 == 0) {
                //天面の出っ張り
                indices[cnt_vert] = 0;
                cnt_vert++;
            } else if (i % 3 == 1) {
                int index = 2 + offsetIndex;
                if (index > divideX) {
                    index = indices[2];
                }
                indices[cnt_vert] = index;
                cnt_vert++;
            } else if (i % 3 == 2) {
                indices[cnt_vert] = 1 + offsetIndex;
                cnt_vert++;
                offsetIndex++;
            }
        }

        //側面
        //開始Index番号　
        int startIndex = indices[2];

        //天面と底面以外のIndex要素数
        int sideIndexLen = aspectTriCount * 3;

        //ループ時に使用するIndex
        int loop1stIndex = 0;
        int loop2ndIndex = 0;

        //ピンクの帯一周分のIndex要素数
        int lapDiv = divideX * 2 * 3;

        //何本目の帯に取り掛かっているのか（一本目を１とする）
        int createSquareFaceCount = 0;

        for (int i = 0; i < sideIndexLen; i++) {
            //一周の帯を終了したら次の帯に移動する
            if (i % lapDiv == 0) {
                loop1stIndex = startIndex;
                loop2ndIndex = startIndex + divideX;
                createSquareFaceCount++;
            }

            if (i % 6 == 0) {
                indices[cnt_vert] = startIndex;
                cnt_vert++;
            } else if (i % 6 == 1) {
                if (i > 0 && i % (lapDiv * createSquareFaceCount - 5) == 0) {
                    //一周したときのループ処理
                    indices[cnt_vert] = loop2ndIndex;
                    cnt_vert++;
                } else {
                    indices[cnt_vert] = startIndex + divideX + 1;
                    cnt_vert++;
                }
            } else if (i % 6 == 2) {
                indices[cnt_vert] = startIndex + divideX;
                cnt_vert++;
            } else if (i % 6 == 3) {
                indices[cnt_vert] = startIndex;
                cnt_vert++;
            } else if (i % 6 == 4) {
                if (i > 0 && i % (lapDiv * createSquareFaceCount - 2) == 0) {
                    //一周したときのループ処理
                    indices[cnt_vert] = loop1stIndex;
                    cnt_vert++;
                } else {
                    indices[cnt_vert] = startIndex + 1;
                    cnt_vert++;
                }
            } else if (i % 6 == 5) {
                if (i > 0 && i % (lapDiv * createSquareFaceCount - 1) == 0) {
                    //一周したときのループ処理
                    indices[cnt_vert] = loop2ndIndex;
                    cnt_vert++;
                } else {
                    indices[cnt_vert] = startIndex + divideX + 1;
                    cnt_vert++;
                }

                //開始Indexの更新
                startIndex++;
            }
        }

        //底面
        if (fov == 360) {
            offsetIndex = vertices.Length - 1 - divideX;
            int loopIndex = offsetIndex;

            for (int i = 0; i < divideX * 3; i++) {
                if (i % 3 == 0) {
                    //底面の先頂点
                    indices[cnt_vert] = vertices.Length - 1;
                    cnt_vert++;
                } else if (i % 3 == 1) {
                    indices[cnt_vert] = offsetIndex;
                    cnt_vert++;
                } else if (i % 3 == 2) {
                    int value = 1 + offsetIndex;
                    if (value >= vertices.Length - 1) {
                        value = loopIndex;
                    }
                    indices[cnt_vert] = value;
                    cnt_vert++;
                    offsetIndex++;
                }
            }
        }

        return new MeshData() {
            indices = indices,
            vertices = vertices,
            uvs = uvs,
        };
    }
}