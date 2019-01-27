using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//ResultSceneのResultDirectorにアタッチ(タイムアタックの結果表示)
public class ResultDirector : MonoBehaviour {

    public Text clearTimeText;  //クリア時間の表示
    public Text bestTimeText;   //ベストタイム

    private int bestTime;   //ベストタイム

    private int minutes;    //分
    private int seconds;    //秒

	// Use this for initialization
	void Start () {
        int clearTime = PlayerController.totalTime; //PlayerControllerからクリアタイムを引き継ぐ

        minutes = clearTime / 60;
        seconds = clearTime % 60;
        clearTimeText.text = "クリアジカン：" + minutes.ToString("00") + ":" + seconds.ToString("00");

        //今までのベストタイムを求める
        bestTime = PlayerPrefs.GetInt("BestTime", (int)1e5);
        if(bestTime > clearTime)
        {
            PlayerPrefs.SetInt("BestTime", clearTime);

            bestTimeText.text = "ジコベスト：" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else
        {
            int min = bestTime / 60;
            int sec = bestTime % 60;
            bestTimeText.text = "ジコベスト：" + min.ToString("00") + ":" + sec.ToString("00");
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
