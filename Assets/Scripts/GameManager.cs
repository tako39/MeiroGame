using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//ゲームシーンなどの管理を行うクラス
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int[] mazeSize = new int[3];   //迷路の大きさ

    private bool isNormalPlay = false;     //通常プレイかどうか
    private int playerHp = 100;            //プレイヤーのHP

    private bool isPause = false;          //ポーズ中かどうか

    private int goalCount = 0;             //ゴールした数
    private int totalTime = 0;             //タイムアタックでゴールした時間

    public const int ROAD = 0;             //通路
    public const int WALL = 1;             //壁
    public const int START = 2;            //スタート地点
    public const int GOAL = 3;             //ゴール地点
    public const int TRAP = 4;             //トラップ

    public const int EASY = 0;             //易しい
    public const int NORMAL = 1;           //普通
    public const int DIFFICULT = 2;        //難しい
    public const int TIME_ATTACK = 3;      //タイムアタック

    public const int trapDamage = 20;      //罠によるダメージ

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

    public void SetMazeSize(int difficulty)  //難易度に応じた迷路の設定
    {
        if (difficulty == EASY)
        {
            mazeSize = new int[] { 11, 13, 15 };
        }
        else if (difficulty == NORMAL)
        {
            mazeSize = new int[] { 15, 17, 19 };
        }
        else if (difficulty == DIFFICULT)
        {
            mazeSize = new int[] { 19, 21, 23 };
        }
        else if (difficulty == TIME_ATTACK)
        {
            mazeSize = new int[] { 13, 17, 21 };
        }
    }

    public int[] GetMazeSize()               //迷路の大きさの取得
    {
        return mazeSize;
    }

    public void SetPlayerHp(int hp)          //hpをセット
    {
        playerHp = hp;
    }

    public int GetPlayerHp()                 //hpの取得
    {
        return playerHp;
    }

    public void PlayerDamaged(int amount)    //ダメージを受ける
    {
        playerHp -= amount;
    }

    public void SetIsNormalPlay(bool normalPlay)   //通常プレイかどうかをセット
    {
        isNormalPlay = normalPlay;
    }

    public bool GetIsNormalPlay()                  //通常プレイかどうかを取得
    {
        return isNormalPlay;
    }

    public void SetIsPause(bool pause)   //ポーズ中かどうかをセット
    {
        isPause = pause;
    }

    public bool GetIsPause()             //ポーズ中かどうかを取得
    {
        return isPause;
    }

    public void SetGoalCount(int num)    //ゴール数をセットする
    {
        goalCount = num;
    }

    public int GetGoalCount()            //ゴール数を取得
    {
        return goalCount;
    }

    public void AddGoalCount()           //ゴール数を増やす
    {
        goalCount++;
    }

    public void SetTotalTime(int time)   //ゴール時間をセットする
    {
        totalTime = time;
    }

    public int GetTotalTime()            //ゴール時間を取得する
    {
        return totalTime;
    }
}
