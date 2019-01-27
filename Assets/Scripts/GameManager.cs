using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private static int[] mazeSize = new int[3];     //迷路の大きさ

    public const int EASY = 0;           //易しい
    public const int NORMAL = 1;         //普通
    public const int DIFFICULT = 2;      //難しい
    public const int TIME_ATTACK = 3;    //タイムアタック

    public static int trapDamage = 20;   //罠によるダメージ

    private static bool isNormalPlay = false;    //通常プレイかどうか
    private static int playerHp = 100;     //プレイヤーのHP

    public const int maxPlayerHp = 100;    //プレイヤーの最大HP
    public const int minPlayerHp = 0;      //プレイヤーの最小HP

    public IEnumerator LoadSceneAsync(string sceneName ,bool autoChangeSceneAfterLoading = true)
    {
        // シーン名がないときはエラーを出す
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("シーン名を渡す必要があります");
            yield break;
        }

        var loadTask = SceneManager.LoadSceneAsync(sceneName);
        loadTask.allowSceneActivation = autoChangeSceneAfterLoading;
        while (!loadTask.isDone)
        {
            yield return null;
        }
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    public static void SetMazeSize(int difficulty)  //難易度に応じた迷路の設定
    {
        if(difficulty == EASY)
        {
            mazeSize = new int[] { 11, 13, 15 };
        }
        else if(difficulty == NORMAL)
        {
            mazeSize = new int[] { 15, 17, 19 };
        }
        else if(difficulty == DIFFICULT)
        {
            mazeSize = new int[] { 19, 21, 23 };
        }
        else if(difficulty == TIME_ATTACK)
        {
            mazeSize = new int[] { 13, 17, 21 };
        }
    }

    public static int[] GetMazeSize()  //迷路の大きさの取得
    {
        return mazeSize;
    }

    public static void SetPlayerHp(int hp)
    {
        playerHp = hp;
    }

    public static int GetPlayerHp() //hpの取得
    {
        return playerHp;
    }

    public static void PlayerDamaged(int amount) //ダメージを受ける
    {
        playerHp -= amount;
    }

    public static void SetIsNormalPlay(bool isNormal)   //通常プレイかどうかをセット
    {
        isNormalPlay = isNormal;
    }

    public static bool GetIsNormalPlay()    //通常プレイかどうかを取得
    {
        return isNormalPlay;
    }
}
