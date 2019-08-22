using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

//ボタンを押したときの挙動を管理するクラス
public class ButtonManager : MonoBehaviour {
    [SerializeField] Sphere leftSphere;
    [SerializeField] Sphere rightSphere;
    [SerializeField] GameObject canvasButton;
    [SerializeField] GameObject textPercent;  //拡大率表示用
    [SerializeField] GameObject textHorizontalLeftPixel;  //中心ピクセルのX座標表示用（左）
    [SerializeField] GameObject textVerticalLeftPixel;  //中心ピクセルのY座標表示用（左）
    [SerializeField] GameObject textHorizontalRightPixel;  //中心ピクセルのX座標表示用（右）
    [SerializeField] GameObject textVerticalRightPixel;  //中心ピクセルのY座標表示用（右）

    [SerializeField] Text inputText;  //保存名が入力されたText
    [SerializeField] GameObject buttonPrefab;  //ボタンを表示する用のプレハブ
    [SerializeField] GameObject buttonParent;  //ボタンを格納するScrollView内の親玉

    [HideInInspector] public string loadFileName = "";  //ロードするファイルの名前（拡張子抜き）

    void Start() {
        //保存してあるテキストファイルをすべて読み込み、ボタンとして表示する
        string loadPath = Application.dataPath + "/../";
        string[] loadFileNames = Directory.GetFiles(loadPath);
        foreach(string loadFileName in loadFileNames) {
            if (Path.GetExtension(loadFileName) == ".txt") {
                string loadFileNameWOExtension = Path.GetFileNameWithoutExtension(loadFileName);
                //ボタンオブジェクトを生成
                MakeButton(loadFileNameWOExtension);
            }
        }
    }

    void Update() {
        //HUDのボタン表示のオンオフ
        if (Input.GetKeyDown(KeyCode.Space)) {
            canvasButton.SetActive(!canvasButton.activeSelf);
        }

        //拡大率を常に表示する（拡大率は右も左も同じなので、左から取る）
        textPercent.GetComponent<Text>().text = leftSphere.GetComponent<Sphere>().enlarge.ToString();
        //中心ピクセル座標を常に表示する
        textHorizontalLeftPixel.GetComponent<Text>().text = leftSphere.GetComponent<Sphere>().centerPixelPos.x.ToString();
        textVerticalLeftPixel.GetComponent<Text>().text = leftSphere.GetComponent<Sphere>().centerPixelPos.y.ToString();
        textHorizontalRightPixel.GetComponent<Text>().text = rightSphere.GetComponent<Sphere>().centerPixelPos.x.ToString();
        textVerticalRightPixel.GetComponent<Text>().text = rightSphere.GetComponent<Sphere>().centerPixelPos.y.ToString();
    }

    //拡大率を1％増加させる
    public void EnlargePlus() {
        leftSphere.GetComponent<Sphere>().enlarge++;
        rightSphere.GetComponent<Sphere>().enlarge++;
    }

    //拡大率を1%減少させる
    public void EnlargeMinus() {
        leftSphere.GetComponent<Sphere>().enlarge--;
        rightSphere.GetComponent<Sphere>().enlarge--;
    }

    //横中心ピクセルを1増加させる（左）
    public void HorizontalLeftPixelPlus() {
        leftSphere.GetComponent<Sphere>().centerPixelPos.x++;
    }

    //横中心ピクセルを1減少させる（左）
    public void HorizontalLeftPixelMinus() {
        leftSphere.GetComponent<Sphere>().centerPixelPos.x--;
    }

    //縦中心ピクセルを1増加させる（左）
    public void VerticalLeftPixelPlus() {
        leftSphere.GetComponent<Sphere>().centerPixelPos.y++;
    }

    //縦中心ピクセルを1減少させる（左）
    public void VerticalLeftPixelMinus() {
        leftSphere.GetComponent<Sphere>().centerPixelPos.y--;
    }



    //横中心ピクセルを1増加させる（右）
    public void HorizontalRightPixelPlus() {
        rightSphere.GetComponent<Sphere>().centerPixelPos.x++;
    }

    //横中心ピクセルを1減少させる（右）
    public void HorizontalRightPixelMinus() {
        rightSphere.GetComponent<Sphere>().centerPixelPos.x--;
    }

    //縦中心ピクセルを1増加させる（右）
    public void VerticalRightPixelPlus() {
        rightSphere.GetComponent<Sphere>().centerPixelPos.y++;
    }

    //縦中心ピクセルを1減少させる（右）
    public void VerticalRightPixelMinus() {
        rightSphere.GetComponent<Sphere>().centerPixelPos.y--;
    }

    //読み込みボタンを押した
    public void LoadData() {
        //ボタンを何も選択していない場合、スルーする
        if (loadFileName == "") {
            return;
        }

        //選択したファイルまでのパスを取得
        string loadPath = Application.dataPath + "/../" + loadFileName + ".txt";
        StreamReader reader = new StreamReader(loadPath);

        //テキストファイルの毎行を読み込んでいく
        while (true) {
            string read = reader.ReadLine();
            if (read == null) break;

            string[] splits = read.Split(':');
            if (splits.Length != 2) continue;

            int tryInt;
            int.TryParse(splits[1], out tryInt);

            switch (splits[0]) {
                case "Enlargement":
                    leftSphere.GetComponent<Sphere>().enlarge = tryInt;
                    rightSphere.GetComponent<Sphere>().enlarge = tryInt;
                    break;
                case "CenterPixelPosLeftX":
                    leftSphere.GetComponent<Sphere>().centerPixelPos.x = tryInt;
                    break;
                case "CenterPixelPosLeftY":
                    leftSphere.GetComponent<Sphere>().centerPixelPos.y = tryInt;
                    break;
                case "CenterPixelPosRightX":
                    rightSphere.GetComponent<Sphere>().centerPixelPos.x = tryInt;
                    break;
                case "CenterPixelPosRightY":
                    rightSphere.GetComponent<Sphere>().centerPixelPos.y = tryInt;
                    break;
                default:
                    break;
            }
        }
        reader.Close();
    }

    //保存ボタンを押した
    public void SaveData() {
        //押した瞬間のInputFieldに入っている文字列を取得
        string saveName = inputText.text;

        //何も入力していなければ、スルー
        if (saveName == "") {
            return;
        }

        string savePath = Application.dataPath + "/../" + saveName + ".txt";

        //ファイルの存在チェック
        StreamWriter writer;
        //ファイルが存在するとき
        if (File.Exists(savePath)) {
            writer = new StreamWriter(savePath);
        //ファイルが存在しないとき
        } else {
            writer = File.CreateText(savePath);

            //新しく保存したテキストファイルの時のみ、新しくボタンに追加する
            MakeButton(saveName);
        }

        writer.WriteLine("Enlargement:" + leftSphere.GetComponent<Sphere>().enlarge.ToString());
        writer.WriteLine("CenterPixelPosLeftX:" + leftSphere.GetComponent<Sphere>().centerPixelPos.x.ToString());
        writer.WriteLine("CenterPixelPosLeftY:" + leftSphere.GetComponent<Sphere>().centerPixelPos.y.ToString());
        writer.WriteLine("CenterPixelPosRightX:" + rightSphere.GetComponent<Sphere>().centerPixelPos.x.ToString());
        writer.Write("CenterPixelPosRightY:" + rightSphere.GetComponent<Sphere>().centerPixelPos.y.ToString());

        writer.Close();
    }

    //ボタンを生成する
    void MakeButton(string _buttonName) {
        GameObject buttonGo = Instantiate(buttonPrefab);
        buttonGo.transform.SetParent(buttonParent.transform, false);  //親子付け
        buttonGo.GetComponentInChildren<Text>().text = _buttonName;  //ボタンにファイル名を表示
        buttonGo.name = _buttonName;  //ヒエラルキー上の名前を設定
    }
}