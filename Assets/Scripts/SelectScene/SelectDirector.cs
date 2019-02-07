using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SelectSceneのSelectDirectorにアタッチ
public class SelectDirector : MonoBehaviour
{
    private void Awake()
    {
        SoundManager.Instance.SelectBGM();   //BGM開始
    }

    // Use this for initialization
    private void Start()
    {
        GameManager.Instance.SetPlayerHp(GameManager.maxPlayerHp);
        GameManager.Instance.SetGoalCount(0);   //ゴール数の初期化
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
