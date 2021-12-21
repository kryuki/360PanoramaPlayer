using UnityEngine;
using UnityEngine.UI;
using System.IO;

//This class manages the behavior of the button when pressed
public class ButtonManager : MonoBehaviour {
    [SerializeField] CreateSphere leftSphere;
    [SerializeField] CreateSphere rightSphere;
    [SerializeField] GameObject canvasButton;
    [SerializeField] GameObject textPercent;  //magnification
    [SerializeField] GameObject textHorizontalLeftPixel;  //Center pixel left x
    [SerializeField] GameObject textVerticalLeftPixel;  //Center pixel left y
    [SerializeField] GameObject textHorizontalRightPixel;  //Center pixel right x
    [SerializeField] GameObject textVerticalRightPixel;  //Center pixel right y

    [SerializeField] Text inputText;  //the file name to save
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] GameObject buttonParent;

    [HideInInspector] public string loadFileName = "";

    void Start() {
        //Read all the saved text files and display as buttons
        string loadPath = Application.dataPath + "/../";
        string[] loadFileNames = Directory.GetFiles(loadPath);
        foreach(string loadFileName in loadFileNames) {
            if (Path.GetExtension(loadFileName) == ".txt") {
                string loadFileNameWOExtension = Path.GetFileNameWithoutExtension(loadFileName);
                MakeButton(loadFileNameWOExtension);
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            canvasButton.SetActive(!canvasButton.activeSelf);
        }

        textPercent.GetComponent<Text>().text = leftSphere.GetComponent<CreateSphere>().enlarge.ToString();
        textHorizontalLeftPixel.GetComponent<Text>().text = leftSphere.GetComponent<CreateSphere>().centerPixelPos.x.ToString();
        textVerticalLeftPixel.GetComponent<Text>().text = leftSphere.GetComponent<CreateSphere>().centerPixelPos.y.ToString();
        textHorizontalRightPixel.GetComponent<Text>().text = rightSphere.GetComponent<CreateSphere>().centerPixelPos.x.ToString();
        textVerticalRightPixel.GetComponent<Text>().text = rightSphere.GetComponent<CreateSphere>().centerPixelPos.y.ToString();
    }

    public void EnlargePlus() {
        leftSphere.GetComponent<CreateSphere>().enlarge++;
        rightSphere.GetComponent<CreateSphere>().enlarge++;
    }

    public void EnlargeMinus() {
        leftSphere.GetComponent<CreateSphere>().enlarge--;
        rightSphere.GetComponent<CreateSphere>().enlarge--;
    }

    public void HorizontalLeftPixelPlus() {
        leftSphere.GetComponent<CreateSphere>().centerPixelPos.x++;
    }

    public void HorizontalLeftPixelMinus() {
        leftSphere.GetComponent<CreateSphere>().centerPixelPos.x--;
    }

    public void VerticalLeftPixelPlus() {
        leftSphere.GetComponent<CreateSphere>().centerPixelPos.y++;
    }

    public void VerticalLeftPixelMinus() {
        leftSphere.GetComponent<CreateSphere>().centerPixelPos.y--;
    }


    public void HorizontalRightPixelPlus() {
        rightSphere.GetComponent<CreateSphere>().centerPixelPos.x++;
    }

    public void HorizontalRightPixelMinus() {
        rightSphere.GetComponent<CreateSphere>().centerPixelPos.x--;
    }

    public void VerticalRightPixelPlus() {
        rightSphere.GetComponent<CreateSphere>().centerPixelPos.y++;
    }

    public void VerticalRightPixelMinus() {
        rightSphere.GetComponent<CreateSphere>().centerPixelPos.y--;
    }

    public void LoadData() {
        if (loadFileName == "") {
            return;
        }

        string loadPath = Application.dataPath + "/../" + loadFileName + ".txt";
        StreamReader reader = new StreamReader(loadPath);

        while (true) {
            string read = reader.ReadLine();
            if (read == null) break;

            string[] splits = read.Split(':');
            if (splits.Length != 2) continue;

            int tryInt;
            int.TryParse(splits[1], out tryInt);

            switch (splits[0]) {
                case "Enlargement":
                    leftSphere.GetComponent<CreateSphere>().enlarge = tryInt;
                    rightSphere.GetComponent<CreateSphere>().enlarge = tryInt;
                    break;
                case "CenterPixelPosLeftX":
                    leftSphere.GetComponent<CreateSphere>().centerPixelPos.x = tryInt;
                    break;
                case "CenterPixelPosLeftY":
                    leftSphere.GetComponent<CreateSphere>().centerPixelPos.y = tryInt;
                    break;
                case "CenterPixelPosRightX":
                    rightSphere.GetComponent<CreateSphere>().centerPixelPos.x = tryInt;
                    break;
                case "CenterPixelPosRightY":
                    rightSphere.GetComponent<CreateSphere>().centerPixelPos.y = tryInt;
                    break;
                default:
                    break;
            }
        }
        reader.Close();
    }

    public void SaveData() {
        string saveName = inputText.text;

        if (saveName == "") {
            return;
        }

        string savePath = Application.dataPath + "/../" + saveName + ".txt";

        StreamWriter writer;
        if (File.Exists(savePath)) {
            writer = new StreamWriter(savePath);
        } else {
            writer = File.CreateText(savePath);

            MakeButton(saveName);
        }

        writer.WriteLine("Enlargement:" + leftSphere.GetComponent<CreateSphere>().enlarge.ToString());
        writer.WriteLine("CenterPixelPosLeftX:" + leftSphere.GetComponent<CreateSphere>().centerPixelPos.x.ToString());
        writer.WriteLine("CenterPixelPosLeftY:" + leftSphere.GetComponent<CreateSphere>().centerPixelPos.y.ToString());
        writer.WriteLine("CenterPixelPosRightX:" + rightSphere.GetComponent<CreateSphere>().centerPixelPos.x.ToString());
        writer.Write("CenterPixelPosRightY:" + rightSphere.GetComponent<CreateSphere>().centerPixelPos.y.ToString());

        writer.Close();
    }

    void MakeButton(string _buttonName) {
        GameObject buttonGo = Instantiate(buttonPrefab);
        buttonGo.transform.SetParent(buttonParent.transform, false);
        buttonGo.GetComponentInChildren<Text>().text = _buttonName;
        buttonGo.name = _buttonName;
    }
}