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

    public float fov = 200;  //FOV of the fisheye camera
    public int width = 1920;  //resolution (x)
    public int height = 1080;  //resolution (y)
    public int radiusPixels = 500;  //the number of pixels in the fisheye radius
    public Vector2 centerPixelPos = new Vector2(960, 540);  //center of the pixel coordinate
    public float enlarge = 0f;  //rate of magnification (%)

    [SerializeField] private Material _mat;

    int fps = 30;  //FPS
    WebCamTexture webcamTexture;

    public Vector2Int divide;
    public float sphereSize;

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
        //Create vertex coordinates
        //

        float r = size * 0.5f;
        int cnt_vert = 0;
        int vertCount;
        if (fov == 360) {
            vertCount = divideX * (divideY - 1) + 2;
        } else {
            vertCount = divideX * divideY + 1;
        }
        
        Vector3[] vertices = new Vector3[vertCount];

        float centerRadianX = 2f * Mathf.PI / (float)divideX;
        float centerRadianY = (fov / 2) * Mathf.Deg2Rad / (float)divideY;

        //
        //☆UV Coordinate
        //
        Vector2[] uvs = new Vector2[vertCount];
        int cnt_uvs = 0;

        //vertex coordinate at the top
        vertices[cnt_vert] = new Vector3(0, r, 0);
        cnt_vert++;

        //UV coordinate at the top
        uvs[cnt_uvs] = new Vector2(centerPixelPos.x / width, centerPixelPos.y / height);
        cnt_uvs++;

        //side
        for (int vy = 0; vy < fov; vy++) {
            float yRadian = (float)(vy + 1) * centerRadianY;

            //distance from the center axis
            float tmpLen = r * Mathf.Sin(yRadian);

            for (int vx = 0; vx < divideX; vx++) {
                vertices[cnt_vert] = new Vector3(tmpLen * Mathf.Sin((float)vx * centerRadianX),
                                            r * Mathf.Cos(yRadian),
                                            tmpLen * Mathf.Cos((float)vx * centerRadianX)
                                            );

                if (cnt_vert < vertCount - 1) {
                    cnt_vert++;
                }

                //Set UV coordinate
                int rightLeftInvertIndex;
                if (rightLeftInvert) {
                    rightLeftInvertIndex = 1;
                } else {
                    rightLeftInvertIndex = -1;
                }
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

        //Vertex coordinate at the bottom
        if (fov != 360) {
            vertices[vertCount - 1] = vertices[vertCount - divideX];
        }

        //At the bottom
        if (fov == 360) {
            vertices[cnt_vert] = new Vector3(0, -r, 0);
        }

        //
        //Vertex index
        //

        //number of triangles at the top and bottom
        int topAndBottomTriCount;
        if (fov == 360) {
            topAndBottomTriCount = divideX * 2;
        } else {
            topAndBottomTriCount = divideX;
        }

        //The number of triangles except at the top and bottom
        int aspectTriCount;
        if (fov == 360) {
            aspectTriCount = divideX * (divideY - 2) * 2;
        } else {
            aspectTriCount = divideX * (divideY - 1) * 2;
        }

        int[] indices = new int[(topAndBottomTriCount + aspectTriCount) * 3];

        //top
        int offsetIndex = 0;
        cnt_vert = 0;
        for (int i = 0; i < divideX * 3; i++) {
            if (i % 3 == 0) {
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

        //Side
        int startIndex = indices[2];

        //number of indexes except at the top and bottom
        int sideIndexLen = aspectTriCount * 3;

        int loop1stIndex = 0;
        int loop2ndIndex = 0;

        //ピンクの帯一周分のIndex要素数
        //the number of indexes for one pink band
        int lapDiv = divideX * 2 * 3;

        //On which band are we at?
        int createSquareFaceCount = 0;

        for (int i = 0; i < sideIndexLen; i++) {
            //If we finish one band, go to the next band
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
                    //Looped
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
                    //Looped
                    indices[cnt_vert] = loop1stIndex;
                    cnt_vert++;
                } else {
                    indices[cnt_vert] = startIndex + 1;
                    cnt_vert++;
                }
            } else if (i % 6 == 5) {
                if (i > 0 && i % (lapDiv * createSquareFaceCount - 1) == 0) {
                    //Looped
                    indices[cnt_vert] = loop2ndIndex;
                    cnt_vert++;
                } else {
                    indices[cnt_vert] = startIndex + divideX + 1;
                    cnt_vert++;
                }

                startIndex++;
            }
        }

        //Buttom
        if (fov == 360) {
            offsetIndex = vertices.Length - 1 - divideX;
            int loopIndex = offsetIndex;

            for (int i = 0; i < divideX * 3; i++) {
                if (i % 3 == 0) {
                    //the endpoint of the bottom surface
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