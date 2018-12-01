using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//CountSceneのCountDirectorにアタッチ
public class CountScript : MonoBehaviour {

    public AudioSource audioSource; //カウントダウン音

    private float sumTime; //時間の計測
    private int seconds;   //カウントダウン用

    public Text countText;

    // Use this for initialization
    void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
        sumTime = 2.0f; //2 -> 1 -> 0
	}
	
	// Update is called once per frame
	void Update () {
        sumTime -= Time.deltaTime; //カウントダウンする
        seconds = (int)sumTime;

        if (seconds < 0)
        {
            countText.text = "0";
            SceneManager.LoadScene("GameScene");
        }
        else if (seconds == 0)
        {
            countText.text = "1";
        }
        else if (seconds == 1)
        {
            countText.text = "2";
        }
    }
}
