﻿using UnityEngine;
using UnityEngine.UI;

//タイムアタックの結果表示
public class TimeAttackEndDirector : MonoBehaviour {

    [SerializeField]
    private Text clearTimeText;         //クリア時間の表示
    [SerializeField]
    private Text bestTimeText;          //ベストタイムの表示
    [SerializeField]
    private Text newRecodeText;         //新記録かどうか

    private void Awake()
    {
        SoundManager.Instance.EndBGM(); //BGM開始

        int clearTime = GameManager.Instance.GetTotalTime();    //クリア時間を取得

        int minutes = clearTime / 60;   //分
        int seconds = clearTime % 60;   //秒
        clearTimeText.text = "クリア時間：" + minutes.ToString("00") + ":" + seconds.ToString("00");

        //今までのベストタイムを求める
        int bestTime = PlayerPrefs.GetInt("BestTime", (int)1e9);
        if(bestTime > clearTime)
        {
            PlayerPrefs.SetInt("BestTime", clearTime);

            bestTimeText.text = "自己ベスト：" + minutes.ToString("00") + ":" + seconds.ToString("00");
            if (!newRecodeText.enabled) newRecodeText.enabled = true;
        }
        else
        {
            int min = bestTime / 60;
            int sec = bestTime % 60;
            bestTimeText.text = "自己ベスト：" + min.ToString("00") + ":" + sec.ToString("00");

            if (newRecodeText.enabled)  newRecodeText.enabled = false;
        }
	}
	
	// Update is called once per frame
	private void Update () {
        if (Input.GetMouseButtonDown(0))    //押されたらSelectシーンへ
        {
            StartCoroutine(GameManager.Instance.LoadSceneAsync("SelectScene"));
        }
    }
}
