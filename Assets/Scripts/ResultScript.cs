using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//ResultSceneのResultDirectorにアタッチ(タイムアタックの結果表示)
public class ResultScript : MonoBehaviour {

    public Text clearTimeText;  //クリア時間の表示
    public Text bestTimeText;   //ベストタイム

    private int bestTime;   //ベストタイム

    private int minutes;    //分
    private int seconds;    //秒

	// Use this for initialization
	void Start () {
        int cleartime = PlayerController.totalTime; //PlayerControllerからクリアタイムを引き継ぐ

        minutes = cleartime / 60;
        seconds = cleartime % 60;
        clearTimeText.text = "クリア時間：" + minutes.ToString("00") + ":" + seconds.ToString("00");

        //今までのベストタイムを求める
        bestTime = PlayerPrefs.GetInt("BestTime", (int)1e5);
        if(bestTime > cleartime)
        {
            PlayerPrefs.SetInt("BestTime", cleartime);

            bestTimeText.text = "自己ベスト：" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else
        {
            int min = bestTime / 60;
            int sec = bestTime % 60;
            bestTimeText.text = "自己ベスト：" + min.ToString("00") + ":" + sec.ToString("00");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))    //押されたらSelectシーンへ
        {
            SceneManager.LoadScene("SelectScene");
        }
    }
}
