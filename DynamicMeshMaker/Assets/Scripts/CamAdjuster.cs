using UnityEngine;

public class CamAdjuster : MonoBehaviour {
    public Vector3 basePos = Vector3.zero;

    public bool gyro = true;

    public GameObject[] GyroKill;

    void Start() {
        if (basePos == Vector3.zero) {
            basePos = transform.position;
        }
    }

    void Update() {
        //Get HMD position from VR.InputTracking
        var trackingPos = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.CenterEye);

        var scale = transform.localScale;
        trackingPos = new Vector3(
            trackingPos.x * scale.x,
            trackingPos.y * scale.y,
            trackingPos.z * scale.z
        );

        trackingPos = transform.rotation * trackingPos;

        //Deactivate HMD movement by subtracting the fixed position
        transform.position = basePos - trackingPos;

        //Kill HMD gyro
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