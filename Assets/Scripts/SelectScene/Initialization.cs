using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SelectSceneの初期化ボタンにアタッチ
public class Initialization : MonoBehaviour {
    
    public GameObject initial;
    private int pushCount;

	// Use this for initialization
	void Start () {
        pushCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void PushCount()
    {
        pushCount++;

        if (pushCount > 5)
        {
            pushCount = 0;
            initial.SetActive(true);
        }
    }

    public void PushYesButton() //はいを押したとき
    {
        PlayerPrefs.DeleteKey("Minutes");
        PlayerPrefs.DeleteKey("Seconds");
        PlayerPrefs.DeleteKey("Goalnum");
        PlayerPrefs.DeleteKey("BestTime");
        initial.SetActive(false);
    }

    public void PushBackButton() //ｘを押したとき
    {
        pushCount = 0;
        initial.SetActive(false);
    }
}
