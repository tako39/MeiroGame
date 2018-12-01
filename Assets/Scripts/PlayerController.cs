using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//GameSceneのPlayerにアタッチ(プレイヤー操作)
public class PlayerController : MonoBehaviour {

    public AudioSource audioSource; //回転音

    private const int HEIGHT = 30; //縦
    private const int WIDTH = 30;  //横
    private const int ROAD = 0;    //通路
    private const int WALL = 1;    //壁
    private const int START = 2;   //スタート地点
    private const int GOAL = 3;    //ゴール地点

    private int[] dx = new int[] { 1, -1, 0, 0 }; //右、左、上、下
    private int[] dy = new int[] { 0, 0, -1, 1 };

    private int[] nowPosition = new int[2]; //現在の位置
    private bool maze_made = false;         //迷路が作られたかどうか

    private bool[] can_move = new bool[4]; //移動可能かどうか(0:右、1:左、2:上、3:下)

    private bool[] move_dir = new bool[4]; //動かす方向(0:右、1:左、2:上、3:下)

    //回転
    Vector3 rotatePoint = Vector3.zero; //回転中心
    Vector3 rotateAxis = Vector3.zero;  //回転軸

    private float cubeAngle = 0.0f;     //回転角度
    private float cubeSizeHalf = 0.0f;  //キューブの半分
    private bool isRotate = false;      //回転中か

    public static bool goal = false;    //ゴールしたか
    public static int goalnum;      //ゴールした数

    public Text timerText;  //経過時間を知らせるテキスト
    public static int totalTime; //経過時間
    private int minutes;      //分
    private float seconds;    //秒
    private float oldseconds; //前の秒数の記憶

    private bool is_pause = false; //PauseScriptのpauseより一時停止しているかを取得
    private bool is_normal; //Button_Selectのis_Normalplayよりプレイモードを取得

    private Vector3 StartPos; //フリック入力の最初の位置
    private Vector3 EndPos;   //フリック入力の最後の位置

    // Use this for initialization
    void Start() {
        audioSource = gameObject.GetComponent<AudioSource>();

        goal = false;

        is_normal = Button_Select.is_Normalplay; //通常プレイかタイムアタック
        if (is_normal) //通常プレイなら
        {
            timerText.enabled = false; //時間経過を表示しない
        }
        else           //タイムアタックなら
        {
            timerText.enabled = true; //時間経過を表示する
        }

        goalnum = PlayerPrefs.GetInt("Goalnum", 0);

        minutes = PlayerPrefs.GetInt("Minutes", 0);
        seconds = PlayerPrefs.GetFloat("Seconds", 0);
        oldseconds = 0.0f;

        cubeSizeHalf = transform.localScale.x / 2.0f;
    }

    // Update is called once per frame
    void Update() {
        is_pause = PauseScript.pause; //一時停止しているかどうか
        if (is_pause) return;

        Firstpos();   //最初の位置決め

        TimeCount(); //経過時間の処理

        if (isRotate) return; //回転中は何もしない

        GoalCheck();  //ゴール処理

        WallSearch(); //壁を探す

        if (OperateMode.flickMode) //操作の種類をOperateMode.csから取得
        {
            FlickScreen();  //フリックしたときの動作
        }
        else
        {
            Acceleration(); //加速度センサーの動作
        }

        InputKey();     //入力に対する処理

        if (rotatePoint == Vector3.zero) return; //入力がない場合次の呼び出しを行わない

        StartCoroutine(MoveCube()); //ここで回転を始める
    }

    void Firstpos() // 最初の位置の設定
    {
        maze_made = GameScript.player_start; //迷路の完成を待つ

        if (maze_made)
        {
            nowPosition = GameScript.startpos_copy;

            transform.position = new Vector3(nowPosition[1], -nowPosition[0], 0.0f); //スタート位置に移動
            GameScript.player_start = false;
        }
    }

    void TimeCount() //経過時間
    {
        if (!is_normal) //タイムアタックの場合
        {
            if (!goal) seconds += Time.deltaTime;

            if (seconds >= 60.0f)
            {
                minutes++;
                seconds -= 60.0f;
            }

            if ((int)seconds != (int)oldseconds) //時間が変化したときのみテキストを変える
            {
                PlayerPrefs.SetInt("Minutes", minutes);
                PlayerPrefs.SetFloat("Seconds", seconds);

                timerText.text = minutes.ToString("00") + ":" + ((int)seconds).ToString("00");
            }

            oldseconds = seconds;
        }
    }

    void GoalCheck() //ゴール時の処理
    {
        if (GameScript.map[nowPosition[0], nowPosition[1]] == GOAL && !goal) 
        {
            goal = true;
            if (!is_normal) goalnum++;
            PlayerPrefs.SetInt("Goalnum", goalnum);

            if (goalnum >= 3) //３回ゴールした時ResultSceneへ
            {
                goalnum = 0;

                totalTime = minutes * 60 + (int)seconds;

                PlayerPrefs.SetInt("Minutes", 0);
                PlayerPrefs.SetFloat("Seconds", 0.0f);
                PlayerPrefs.SetInt("Goalnum", 0);

                SceneManager.LoadScene("ResultScene");
            }
            else
            {
                SceneManager.LoadScene("GameScene"); //繰り返し
            }
        }
    }

    void WallSearch() //現在地から上下左右の壁を探す
    {
        for (int i = 0; i < 4; i++)
        {
            can_move[i] = false; //動くことができる方向
        }

        for (int i = 0; i < 4; i++) //4方向に対して通路を探す
        {
            int ny = nowPosition[0] + dy[i];
            int nx = nowPosition[1] + dx[i];

            if (nx >= 0 && ny >= 0 && nx < WIDTH && ny < HEIGHT && GameScript.map[ny, nx] != 1)
            {
                can_move[i] = true; //通路があるときは動かすことができる
            }
        }
    }

    void FlickScreen() //フリック入力での移動
    {
        for (int i = 0; i < 4; i++)
        {
            move_dir[i] = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) //押したとき
        {
            StartPos = new Vector3(Input.mousePosition.x,
                                        Input.mousePosition.y,
                                        Input.mousePosition.z);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))   //離したとき
        {
            EndPos = new Vector3(Input.mousePosition.x,
                                      Input.mousePosition.y,
                                      Input.mousePosition.z);
            GetDirection();
        }
    }

    void GetDirection() //フリック入力で取得する向き
    {
        float dirX = EndPos.x - StartPos.x; //x方向の移動
        float dirY = EndPos.y - StartPos.y; //y方向の移動

        if (Mathf.Abs(dirY) < Mathf.Abs(dirX))
        {
            if (30 < dirX)
            {
                move_dir[0] = true; //右方向のフリック
            }
            else if (-30 > dirX)
            {
                move_dir[1] = true; //左方向のフリック
            }
        }
        else if (Mathf.Abs(dirX) < Mathf.Abs(dirY)) {
            if (30 < dirY) {
                move_dir[2] = true; //上方向のフリック
            }
            else if (-30 > dirY) {
                move_dir[3] = true; //下方向のフリック
            }
        }
        else {
            //タッチした場合
        }
    }

    void Acceleration() //加速度センサーに応じた移動
    {
        for(int i = 0; i < 4; i++)
        {
            move_dir[i] = false;
        }

        float x = Input.acceleration.x;
        float y = Input.acceleration.y;
        float z = Input.acceleration.z;

        if (x > 0.15f && z > -0.85f)       //右
        {
            move_dir[0] = true;
        }
        else if (x < -0.15f && z > -0.85f) //左
        {
            move_dir[1] = true;
        }
        else if (y > 0.15f && z > -0.85f)  //上
        {
            move_dir[2] = true;
        }
        else if (y < -0.15f && z > -0.85f) //下
        {
            move_dir[3] = true;
        }   
    }

    void InputKey() //入力を受け付ける
    {
        //入力に応じて回転する位置と軸を決める
        if ((Input.GetKeyDown(KeyCode.RightArrow) || move_dir[0]) && can_move[0]) //右
        {
            rotatePoint = transform.position + new Vector3(cubeSizeHalf, 0.0f, cubeSizeHalf); //回転の中心点
            rotateAxis = new Vector3(0, -1, 0); //回転軸
            nowPosition[1]++;
        }
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || move_dir[1]) && can_move[1])  //左
        {
            rotatePoint = transform.position + new Vector3(-cubeSizeHalf, 0.0f, cubeSizeHalf);
            rotateAxis = new Vector3(0, 1, 0);
            nowPosition[1]--;
        }
        if ((Input.GetKeyDown(KeyCode.UpArrow) || move_dir[2]) && can_move[2])    //上
        {
            rotatePoint = transform.position + new Vector3(0.0f, cubeSizeHalf, cubeSizeHalf);
            rotateAxis = new Vector3(1, 0, 0);
            nowPosition[0]--;
        }
        if ((Input.GetKeyDown(KeyCode.DownArrow) || move_dir[3]) && can_move[3])  //下
        {
            rotatePoint = transform.position + new Vector3(0.0f, -cubeSizeHalf, cubeSizeHalf);
            rotateAxis = new Vector3(-1, 0, 0);
            nowPosition[0]++;
        }
    }

    IEnumerator MoveCube() //回転処理
    {
        audioSource.Play();
        isRotate = true; //回転中

        float sumAngle = 0.0f;
        while(sumAngle < 90.0f) //90度になるまで回す
        {
            cubeAngle = 18.0f;  //回転速度
            sumAngle += cubeAngle;

            if(sumAngle > 90.0f) //回しすぎないように調整
            {
                cubeAngle -= sumAngle - 90.0f;
            }
            transform.RotateAround(rotatePoint, rotateAxis, cubeAngle);

            yield return null;
        }

        isRotate = false; //回転終了
        rotatePoint = Vector3.zero;
        rotateAxis = Vector3.zero;

        yield break;
    }
}
