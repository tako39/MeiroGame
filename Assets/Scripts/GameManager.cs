using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//ゲームシーンなどの管理を行うクラス
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int[] mazeSize = new int[3];                //迷路の大きさ
    private GameType gameType = GameType.TIME_ATTACK;   //ゲームの種類

    private int playerHp = 100;            //プレイヤーのHP

    private bool isPause = false;          //ポーズ中かどうか

    private int goalCount = 0;             //ゴールした数
    private int totalTime = 0;             //タイムアタックでゴールした時間

    public enum MapType
    {
        ROAD      = 0,   //通路
        WALL      = 1,   //壁
        START     = 2,   //スタート地点
        GOAL      = 3,   //ゴール地点
        TRAP      = 4,   //トラップ
        RECOVERY  = 5,   //回復床
        ANS_ROUTE = 6,   //正解の経路
    }

    public enum GameType
    {
        EASY        = 0,    //易しい
        NORMAL      = 1,    //普通
        DIFFICULT   = 2,    //難しい
        TIME_ATTACK = 3,    //タイムアタック
    }

    public const int trapDamage = 20;      //罠によるダメージ
    public const int recoveryAmount = 10;  //回復床による回復

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

    public void SetGameType(GameType type)        //ゲームの種類をセット
    {
        gameType = type;
    }

    public GameType GetGameType()                 //ゲームの種類を取得
    {
        return gameType;
    }

    public void SetMazeSize()                //難易度に応じた迷路の設定
    {
        if (gameType == GameType.EASY)
        {
            mazeSize = new int[] { 11, 13, 15 };
        }
        else if (gameType == GameType.NORMAL)
        {
            mazeSize = new int[] { 15, 17, 19 };
        }
        else if (gameType == GameType.DIFFICULT)
        {
            mazeSize = new int[] { 19, 21, 23 };
        }
        else if (gameType == GameType.TIME_ATTACK)
        {
            mazeSize = new int[] { 13, 17, 21 };
        }
    }

    public int GetMazeSize(int num)               //迷路の大きさの取得
    {
        return mazeSize[num];
    }

    public void SetPlayerHp(int hp)          //hpをセット
    {
        playerHp = hp;
    }

    public int GetPlayerHp()                 //hpの取得
    {
        return playerHp;
    }

    public void PlayerDamaged()             //ダメージを受ける
    {
        playerHp -= trapDamage;
    }

    public void PlayerRecovered()           //回復する
    {
        if(playerHp + recoveryAmount <= maxPlayerHp) playerHp += recoveryAmount;
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
