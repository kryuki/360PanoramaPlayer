using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Windows.Forms;
using UnityEngine.UI;

public class test : MonoBehaviour {
    public InputField input_field_path;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OpenExistFile() {
        OpenFileDialog open_file_dialog = new OpenFileDialog();

        //InputFieldの初期値を代入しておく
        open_file_dialog.FileName = input_field_path.text;

        //txtファイルを開くことを指定する
        open_file_dialog.Filter = "txtファイル|*.txt";

        //ファイルが存在しない場合は警告を出す（true）、ファイルが存在する場合は警告を出す（false）
        open_file_dialog.CheckFileExists = false;

        //ダイアログを出す
        open_file_dialog.ShowDialog();

        //取得したファイル名をInputFieldに代入する
        input_field_path.text = open_file_dialog.FileName;
    }
}
