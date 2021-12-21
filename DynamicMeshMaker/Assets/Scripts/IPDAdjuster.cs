using UnityEngine;

//Binocular stereoscopic
public class IPDAdjuster : MonoBehaviour {
    public float IPD = 65.0f;  //in millimeter（1mm = 0.001m）

    [SerializeField] GameObject leftCam;  //For left eye
    [SerializeField] GameObject rightCam;  //For right eye
    
	void Update () {
        leftCam.gameObject.transform.position = new Vector3(0, 0, IPD * 0.001f / 2.0f);
        rightCam.gameObject.transform.position = new Vector3(0, 0, -IPD * 0.001f / 2.0f);
	}
}
