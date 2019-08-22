using UnityEngine;
using System.Collections;

public class CamAdjuster : MonoBehaviour {
    public Vector3 basePos = Vector3.zero;

    //ジャイロを有効にするかどうか
    public bool gyro = true;

    public GameObject[] GyroKill;

    void Start() {
        if (basePos == Vector3.zero) {
            basePos = transform.position;
        }
    }

    void Update() {
        // VR.InputTracking から hmd の位置を取得
        var trackingPos = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.CenterEye);

        var scale = transform.localScale;
        trackingPos = new Vector3(
            trackingPos.x * scale.x,
            trackingPos.y * scale.y,
            trackingPos.z * scale.z
        );

        // 回転
        trackingPos = transform.rotation * trackingPos;

        // 固定したい位置から hmd の位置を
        // 差し引いて実質 hmd の移動を無効化する
        transform.position = basePos - trackingPos;

        // 子のカメラの座標がbasePosと同じ値になるかを確認する
        // Debug.Log(transform.GetChild(0).position);

        //（チェックを入れた場合）HMDのジャイロ回転を殺す
        if (gyro == false) {
            var trackingRot = UnityEngine.XR.InputTracking.GetLocalRotation(UnityEngine.XR.XRNode.CenterEye);
            if (GyroKill != null) {
                foreach (GameObject gk in GyroKill) {
                    Vector3 rotationAngle = gk.gameObject.transform.rotation.eulerAngles;
                    rotationAngle = new Vector3(-trackingRot.eulerAngles.z, 0, 0);
                    gk.gameObject.transform.rotation = Quaternion.Euler(rotationAngle);
                }
            }
        }
    }
}