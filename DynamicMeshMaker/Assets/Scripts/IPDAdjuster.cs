using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//両眼立体視
public class IPDAdjuster : MonoBehaviour {
    public float IPD = 65.0f;  //ミリ単位で入力（1mm = 0.001m）

    [SerializeField] GameObject leftCam;  //左目用のカメラ
    [SerializeField] GameObject rightCam;  //右目用のカメラ
    
	// Update is called once per frame
	void Update () {
        //IPDをメートル単位に変換し、左右のカメラを目の位置に置く
        leftCam.gameObject.transform.position = new Vector3(0, 0, IPD * 0.001f / 2.0f);
        rightCam.gameObject.transform.position = new Vector3(0, 0, -IPD * 0.001f / 2.0f);
	}
}
