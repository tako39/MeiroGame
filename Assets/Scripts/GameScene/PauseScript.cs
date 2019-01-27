using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//GameSceneのPauseButtonにアタッチ
public class PauseScript : MonoBehaviour {

    public GameObject pauseButton;  //画像の切り替え用
    public GameObject startButton;

    public GameObject ChangeDir;    //向き変更ボタン

    public GameObject pauseScreen;  //ポーズ画面
    public GameObject endButton;    //やめるボタン


    private int clickCount;         //何回押したか
    public static bool pause;       //一時停止しているか

    // Use this for initialization
    void Start () {
        clickCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PushPauseButton() //ポーズボタンを押したとき
    {
        clickCount++;

        if (clickCount % 2 == 1)
        {
            pause = true;   //ポーズ中の処理

            pauseButton.SetActive(false);
            startButton.SetActive(true);
            ChangeDir.SetActive(false);
            pauseScreen.SetActive(true);
            endButton.SetActive(true);
        }
        else
        {
            pause = false;  //プレイ中の処理

            pauseButton.SetActive(true);
            startButton.SetActive(false);
            ChangeDir.SetActive(true);
            pauseScreen.SetActive(false);
            endButton.SetActive(false);
        }
    }

    public void PushEndButton() //やめるボタンを押したとき
    {
        pause = false;

        PlayerPrefs.SetInt("Minutes", 0);
        PlayerPrefs.SetFloat("Seconds", 0.0f);
        PlayerPrefs.SetInt("Goalnum", 0);

        SceneManager.LoadScene("SelectScene");  //Selectシーンへ
    }
}
