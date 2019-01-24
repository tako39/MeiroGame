using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//GameSceneのOperateModeButtonにアタッチ(ジャイロとフリックの選択)
public class OperateMode : MonoBehaviour {

    public Text modeText;                   //モード変更するテキスト
    private int clickCount;                 //ボタンを押した回数
    public static bool flickMode = true;    //フリック入力かジャイロを使用するか

	// Use this for initialization
	void Start () {
        clickCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OperateButton_Click()   //ボタンを押したとき
    {
        clickCount++;

        if (clickCount % 2 == 1)    //ジャイロ機能を使用
        {
            flickMode = false;
            modeText.text = "ジャイロ";
        }
        else　                      //フリック操作を使用
        {
            flickMode = true;
            modeText.text = "フリック";
        }
    }
}
