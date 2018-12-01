using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//SelectSceneの初期化ボタンにアタッチ
public class DeleteScript : MonoBehaviour {
    
    public GameObject initial;
    private int pushCount;

	// Use this for initialization
	void Start () {
        pushCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void Push_Count()
    {
        pushCount++;

        if (pushCount > 5)
        {
            pushCount = 0;
            initial.SetActive(true);
        }
    }

    public void Delete_Click() //はいを押したとき
    {
        PlayerPrefs.DeleteKey("Minutes");
        PlayerPrefs.DeleteKey("Seconds");
        PlayerPrefs.DeleteKey("Goalnum");
        PlayerPrefs.DeleteKey("BestTime");
        initial.SetActive(false);
    }

    public void Back_Click() //ｘを押したとき
    {
        pushCount = 0;
        initial.SetActive(false);
    }
}
