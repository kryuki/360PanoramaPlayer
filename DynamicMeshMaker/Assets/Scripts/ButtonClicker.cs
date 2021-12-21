using UnityEngine;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour {
    public bool buttonFlag = false;
    Color buttonNormalColor;
    [SerializeField] Color buttonHilightedColor = Color.blue;
    Image image;

    [SerializeField] GameObject contentParent;  //The parent of button (one level up)

    private void Awake() {
        image = this.gameObject.GetComponent<Image>();
        buttonNormalColor = image.color;
        contentParent = GameObject.Find("ContentLoad");
    }
	
	void Update () {
        if (buttonFlag) {
            image.color = buttonHilightedColor;
        } else {
            image.color = buttonNormalColor;
        }
    }

    //When you press button, the button will be activated
    public void SelectButton() {
        bool currentFlag = buttonFlag;

        foreach(Transform child in contentParent.transform) {
            child.GetComponent<ButtonClicker>().buttonFlag = false;
        }

        buttonFlag = !currentFlag;

        if (!currentFlag) {
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().loadFileName = this.gameObject.name;
        } else {
            GameObject.Find("ButtonManager").GetComponent<ButtonManager>().loadFileName = "";
        }
    }
}