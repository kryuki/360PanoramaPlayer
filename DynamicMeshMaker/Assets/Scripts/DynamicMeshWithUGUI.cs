using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicMeshWithUGUI : Graphic {
    protected override void OnPopulateMesh(VertexHelper vh) {
        //頂点の順番
        vh.AddTriangle(0, 1, 2);

        //UIVertex:各頂点の情報
        UIVertex v0 = new UIVertex();
        v0.position = new Vector3(-100f, -100f);
        UIVertex v1 = new UIVertex();
        v1.position = new Vector3(0, 100f);
        UIVertex v2 = new UIVertex();
        v2.position = new Vector3(100f, -100f);

        //頂点情報を渡す
        vh.AddVert(v0);
        vh.AddVert(v1);
        vh.AddVert(v2);
    }

}
