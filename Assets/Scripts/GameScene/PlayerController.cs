using System.Collections;
using UnityEngine;
using UnityEngine.UI;


//GameSceneのPlayerにアタッチ(プレイヤー操作)
public class PlayerController : MonoBehaviour {

    private int[] dx = new int[] { 1, -1, 0, 0 }; //右、左、上、下
    private int[] dy = new int[] { 0, 0, -1, 1 };

    private static Vector2 nowPosition = new Vector2();  //現在の位置

    private bool[] isMovable = new bool[4];  //移動可能かどうか(0:右、1:左、2:上、3:下)

    private bool[] moveDirect = new bool[4]; //動かす方向(0:右、1:左、2:上、3:下)

    //回転
    Vector3 rotatePoint = Vector3.zero; //回転中心
    Vector3 rotateAxis = Vector3.zero;  //回転軸

    private float cubeAngle = 0.0f;     //回転角度
    private float cubeSizeHalf = 0.0f;  //キューブの半分
    private bool isRotate = false;      //回転中か

    private bool isGoal = false;    //ゴールしたか

    public Text timerText;          //経過時間を知らせるテキスト

    private int minutes;            //分
    private float seconds;          //秒
    private float oldseconds;       //前の秒数の記憶

    private Vector3 StartPos;       //フリック入力の最初の位置
    private Vector3 EndPos;         //フリック入力の最後の位置

    [SerializeField]
    private GameObject hpDisplay;   //体力の表示、非表示用

    [SerializeField]
    private Healthbar hpBar;        //体力

    private bool onAnsRoute = true;         //正解の経路上にいるか
    private bool firstOnRecovery = true;    //始めて回復床に乗ったか
    private bool firstOnTrap = true;        //始めて罠の床に乗ったか

    public static Vector2 GetNowPosition()    //プレイヤーの位置の取得
    {
        return nowPosition;
    }

    private void Awake()
    {
        isGoal = false;

        if(GameManager.Instance.GetGameType() == GameManager.GameType.TIME_ATTACK) //タイムアタックなら
        {
            timerText.enabled = true;   //時間経過を表示する

            hpDisplay.SetActive(false); //hpバーを非表示にする
        }
        else //通常プレイなら
        {
            timerText.enabled = false;  //時間経過を表示しない

            hpDisplay.SetActive(true);  //hpバーを表示する
            hpBar.SetHealth(GameManager.Instance.GetPlayerHp());  //現在のhpを代入
        }

        minutes = PlayerPrefs.GetInt("Minutes", 0);
        seconds = PlayerPrefs.GetFloat("Seconds", 0);
        oldseconds = 0.0f;

        cubeSizeHalf = transform.localScale.x / 2.0f;
    }

    // Use this for initialization
    private void Start() {
        Firstpos();   //最初の位置決め
    }

    // Update is called once per frame
    private void Update() {

        if (GameManager.Instance.GetIsPause()) return;   //ポーズ中なら何もしない

        TimeCount();        //経過時間の処理

        if (isRotate) return; //回転中は何もしない

        GoalCheck();        //ゴール処理

        AnsRouteCheck();    //不正解の経路の処理

        TrapCheck();        //罠の処理

        RecoveryCheck();    //回復床の処理

        PlayerHpCheck();    //プレイヤーが生きているか

        WallSearch();       //壁を探す

        FlickScreen();      //フリックしたときの動作

        InputKey();         //入力に対する処理

        if (rotatePoint == Vector3.zero) return; //入力がない場合次の呼び出しを行わない

        StartCoroutine(MoveCube()); //ここで回転を始める
    }

    private void Firstpos()     // 最初の位置の設定
    {
        nowPosition = GameDirector.gameMap.startPos;

        transform.position = new Vector3(nowPosition.x, -nowPosition.y, 0.0f); //スタート位置に移動
    }

    private void TimeCount()    //経過時間
    {
        if (GameManager.Instance.GetGameType() == GameManager.GameType.TIME_ATTACK) //タイムアタックの場合
        {
            if (!isGoal) seconds += Time.deltaTime;

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

    private void GoalCheck()    //ゴール時の処理
    {
        if (GameDirector.gameMap.map[(int)nowPosition.y, (int)nowPosition.x] == (int)GameManager.MapType.GOAL && !isGoal) 
        {
            isGoal = true;
            GameManager.Instance.AddGoalCount();    //ゴールした数を増やす

            //タイムアタックで3回ゴールした時はResultシーンへ
            if ((GameManager.Instance.GetGameType() == GameManager.GameType.TIME_ATTACK) && 
                (GameManager.Instance.GetGoalCount() >= 3))
            {
                GameManager.Instance.SetTotalTime(minutes * 60 + (int)seconds);     //クリア時間を保存

                PlayerPrefs.SetInt("Minutes", 0);
                PlayerPrefs.SetFloat("Seconds", 0.0f);

                StartCoroutine(GameManager.Instance.LoadSceneAsync("TimeAttackEndScene"));
            }
            else
            {
                StartCoroutine(GameManager.Instance.LoadSceneAsync("GameScene"));   //繰り返し
            }
        }
    }

    private void AnsRouteCheck()    //不正解の経路を歩いた時の処理
    {
        //現在地が正解の経路かどうか
        if(GameDirector.gameMap.ansRoute[(int)nowPosition.y, (int)nowPosition.x] == (int)GameManager.MapType.ANS_ROUTE)
        {
            if (!onAnsRoute)    //不正解経路から正解経路に復帰したとき
            {
                SoundManager.Instance.IncorrectSound();         //不正解音
                hpBar.TakeDamage(GameManager.incorrectDamage);  //ダメージを食らう
                GameManager.Instance.PlayerDamaged(GameManager.incorrectDamage);

                onAnsRoute = true;
            }
        }
        else
        {
            onAnsRoute = false;
        }
    }

    private void TrapCheck()        //罠に乗った時の処理
    {
        if (GameDirector.gameMap.map[(int)nowPosition.y, (int)nowPosition.x] == (int)GameManager.MapType.TRAP)
        {
            if (firstOnTrap)        //罠に乗った瞬間
            {
                SoundManager.Instance.DamageSound();        //ダメージ音
                hpBar.TakeDamage(GameManager.trapDamage);   //ダメージを食らう
                GameManager.Instance.PlayerDamaged(GameManager.trapDamage);

                firstOnTrap = false;
            }
        }
        else
        {
            firstOnTrap = true;
        }
    }

    private void RecoveryCheck()    //回復床に乗った時の処理
    {
        if (GameDirector.gameMap.map[(int)nowPosition.y, (int)nowPosition.x] == (int)GameManager.MapType.RECOVERY)
        {
            if (firstOnRecovery)    //回復床に乗った瞬間
            {
                SoundManager.Instance.RecoverySound();         //回復音
                hpBar.GainHealth(GameManager.recoveryAmount);  //回復する
                GameManager.Instance.PlayerRecovered(GameManager.recoveryAmount);
                
                GameDirector.gameMap.map[(int)nowPosition.y, (int)nowPosition.x] = (int)GameManager.MapType.ROAD;

                firstOnRecovery = false;
            }
        }
        else
        {
            firstOnRecovery = true;
        }
    }

    private void PlayerHpCheck()    //プレイヤーの生存処理
    {
        if (GameManager.Instance.GetPlayerHp() <= GameManager.minPlayerHp)
        {
            StartCoroutine(GameManager.Instance.LoadSceneAsync("NormalEndScene"));  //ゲーム終了
        }
    }

    private void WallSearch()       //現在地から上下左右の壁を探す
    {
        for (int i = 0; i < 4; i++)
        {
            isMovable[i] = false;   //動くことができる方向
        }

        for (int i = 0; i < 4; i++) //4方向に対して通路を探す
        {
            int ny = (int)nowPosition.y + dy[i];
            int nx = (int)nowPosition.x + dx[i];

            if (0 <= nx && nx < GameDirector.gameMap.WIDTH && 0 <= ny && ny < GameDirector.gameMap.HEIGHT && 
                GameDirector.gameMap.map[ny, nx] != (int)GameManager.MapType.WALL)
            {
                isMovable[i] = true; //通路があるときは動かすことができる
            }
        }
    }

    private void FlickScreen()      //フリック入力での移動
    {
        for (int i = 0; i < 4; i++)
        {
            moveDirect[i] = false;
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

    private void GetDirection()     //フリック入力で取得する向き
    {
        float dirX = EndPos.x - StartPos.x; //x方向の移動
        float dirY = EndPos.y - StartPos.y; //y方向の移動

        if (Mathf.Abs(dirY) < Mathf.Abs(dirX))
        {
            if (30.0f < dirX)
            {
                moveDirect[0] = true; //右方向のフリック
            }
            else if (-30.0f > dirX)
            {
                moveDirect[1] = true; //左方向のフリック
            }
        }
        else if (Mathf.Abs(dirX) < Mathf.Abs(dirY)) {
            if (30.0f < dirY) {
                moveDirect[2] = true; //上方向のフリック
            }
            else if (-30.0f > dirY) {
                moveDirect[3] = true; //下方向のフリック
            }
        }
        else {
            //タッチした場合
        }
    }

    private void InputKey()     //入力を受け付ける
    {
        //入力に応じて回転する位置と軸を決める
        if ((Input.GetKeyDown(KeyCode.RightArrow) || moveDirect[0]) && isMovable[0]) //右
        {
            rotatePoint = transform.position + new Vector3(cubeSizeHalf, 0.0f, cubeSizeHalf); //回転の中心点
            rotateAxis = new Vector3(0.0f, -1.0f, 0.0f); //回転軸
            nowPosition.x++;
        }
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || moveDirect[1]) && isMovable[1])  //左
        {
            rotatePoint = transform.position + new Vector3(-cubeSizeHalf, 0.0f, cubeSizeHalf);
            rotateAxis = new Vector3(0.0f, 1.0f, 0.0f);
            nowPosition.x--;
        }
        if ((Input.GetKeyDown(KeyCode.UpArrow) || moveDirect[2]) && isMovable[2])    //上
        {
            rotatePoint = transform.position + new Vector3(0.0f, cubeSizeHalf, cubeSizeHalf);
            rotateAxis = new Vector3(1.0f, 0.0f, 0.0f);
            nowPosition.y--;
        }
        if ((Input.GetKeyDown(KeyCode.DownArrow) || moveDirect[3]) && isMovable[3])  //下
        {
            rotatePoint = transform.position + new Vector3(0.0f, -cubeSizeHalf, cubeSizeHalf);
            rotateAxis = new Vector3(-1.0f, 0.0f, 0.0f);
            nowPosition.y++;
        }
    }

    private IEnumerator MoveCube()  //回転処理
    {
        SoundManager.Instance.RotateSound(); //回転音
        isRotate = true;         //回転中

        float sumAngle = 0.0f;
        while(sumAngle < 90.0f)  //90度になるまで回す
        {
            cubeAngle = 18.0f;   //回転速度
            sumAngle += cubeAngle;

            if(sumAngle > 90.0f) //回しすぎないように調整
            {
                cubeAngle -= sumAngle - 90.0f;
            }
            transform.RotateAround(rotatePoint, rotateAxis, cubeAngle);

            yield return null;
        }

        isRotate = false;        //回転終了
        rotatePoint = Vector3.zero;
        rotateAxis = Vector3.zero;

        yield break;
    }
}
