using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour {
    public bool buttonFlag = false;  //ボタンが押されたか
    Color buttonNormalColor;  //ボタンの平常時の色
    [SerializeField] Color buttonHilightedColor = Color.blue;  //選択状態の時のボタンの色
    Image image;

    [SerializeField] GameObject contentParent;  //ボタンの一階層上にある親

    private void Awake() {
        image = this.gameObject.GetComponent<Image>();
        buttonNormalColor = image.color;
        contentParent = GameObject.Find("ContentLoad");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (buttonFlag) {
            image.color = buttonHilightedColor;
        } else {
            image.color = buttonNormalColor;
        }
    }

    //ボタンを押したら、そのボタンだけ選択状態にする
    public void SelectButton() {
        //現在のボタンのフラグを一時保存
        bool currentFlag = buttonFlag;

        //すべてのボタンを非選択状態に戻す
        foreach(Transform child in contentParent.transform) {
            child.GetComponent<ButtonClicker>().buttonFlag = false;
        }

        buttonFlag = !currentFlag;

        //currentFlagがfalseだった場合
        //クリックしたボタンの名前を、ButtonManagerに渡す
        if (!currentFlag) {
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().loadFileName = this.gameObject.name;
        } else {
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().loadFileName = "";
        }
    }
}