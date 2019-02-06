using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//GameSceneのGameDirectorにアタッチ(迷路生成)
public class GameDirector : MonoBehaviour {

    public static Map gameMap = new Map();  //マップ

    private new GameObject[,] gameObject = new GameObject[Map.MAX_HEIGHT, Map.MAX_WIDTH];   //オブジェクト

    private void Awake () {
        //gameMap.Generate();        //迷路生成
        gameMap.CreateMazeWALL();
    }

    // Use this for initialization
    private void Start() {
        MazeDisplay();             //迷路の表示
        //gameMap.TestDisplay();   //マップやそのコストの確認用
    }

    // Update is called once per frame
    private void Update () {
        
    }

    private void MazeDisplay() //迷路を表示する
    {
        for (int y = 0; y < gameMap.HEIGHT; y++)
        {
            for (int x = 0; x < gameMap.WIDTH; x++)
            {
                if (gameMap.map[y, x] == (int)GameManager.MapType.ROAD)          //通路の床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Road");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if (gameMap.map[y, x] == (int)GameManager.MapType.WALL)     //壁
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Wall");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.0f), Quaternion.identity);
                }
                else if (gameMap.map[y, x] == (int)GameManager.MapType.START)    //スタートの床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Start");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if (gameMap.map[y, x] == (int)GameManager.MapType.GOAL)    //ゴールの床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Goal");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if (gameMap.map[y, x] == (int)GameManager.MapType.TRAP)      //罠の床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Trap");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if (gameMap.map[y, x] == (int)GameManager.MapType.RECOVERY)  //回復床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Recovery");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
            }
        }
    }

}

//MAPクラス
public class Map
{
    public const int MAX_HEIGHT = 30;  //迷路の最大の縦幅
    public const int MAX_WIDTH = 30;   //迷路の最大の横幅

    private const int INF = (int)1e5;  //初期化用

    private enum RETURN
    {
        NONE = -1,      //なし
        MADE =  0,      //生成した
        NOT_MADE = 1,   //生成していない
    }

    private readonly int[] dx = new int[] { 1, 0, -1, 0 };    //上下左右4方向のx
    private readonly int[] dy = new int[] { 0, 1, 0, -1 };    //上下左右4方向のy

    public int HEIGHT { get; private set; }           //迷路の縦幅
    public int WIDTH { get; private set; }            //迷路の横幅

    public int[,] map = new int[MAX_HEIGHT, MAX_WIDTH];     //マップ
    private int[,] dist = new int[MAX_HEIGHT, MAX_WIDTH];   //コスト

    public int[] startPos = new int[2];  //スタート
    public int[] goalPos = new int[2];   //ゴール

    private List<Vector2> node = new List<Vector2>();   //壁が伸びていない柱

    private List<Vector2> path = new List<Vector2>();   //探索済の柱

    private List<Vector2> dir = new List<Vector2>()     //方向
    {
            new Vector2(2, 0),
            new Vector2(0, 2),
            new Vector2(-2, 0),
            new Vector2(0, -2),
    };

    private void InitMaze() //迷路の初期化
    {
        if (GameManager.Instance.GetGameType() == GameManager.GameType.TIME_ATTACK) //タイムアタックなら13->17->21
        {
            if (GameManager.Instance.GetGoalCount() < 3)
            {
                HEIGHT = GameManager.Instance.GetMazeSize(GameManager.Instance.GetGoalCount());
                WIDTH  = GameManager.Instance.GetMazeSize(GameManager.Instance.GetGoalCount());
            }
        }
        else //通常プレイなら迷路の大きさはランダム
        {
            HEIGHT = GameManager.Instance.GetMazeSize(Random.Range(0, 3));
            WIDTH  = GameManager.Instance.GetMazeSize(Random.Range(0, 3));
        }
        
        for(int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                if(y == 0 || y == HEIGHT - 1 || x == 0 || x == WIDTH - 1)   //外周は壁
                {
                    map[y, x] = (int)GameManager.MapType.WALL;
                }
                else if((y % 2 == 0) && (x % 2 == 0))   //偶数は柱
                {
                    map[y, x] = (int)GameManager.MapType.WALL;
                    node.Add(new Vector2(x, y));
                }
                else   //それ以外は通路
                {
                    map[y, x] = (int)GameManager.MapType.ROAD;
                }
            }
        }
    }

    public void CreateMazeWALL()   //壁伸ばし法
    {
        InitMaze(); //初期化

        while(node.Count > 0)   //nodeが無くなるまで
        {
            ChooseNode(node[Random.Range(0, node.Count)]);
        }

        SetTmpStart(); //スタート地点のセット

        GoalSearch(startPos[0], startPos[1]);   //ランダムに決めたスタート地点から一番遠い点(p1)を求める
        GoalSearch(goalPos[0], goalPos[1]);     //(p1)をスタート地点とし、そこから一番遠い点(p2)をゴール地点とする

        map[startPos[0], startPos[1]] = (int)GameManager.MapType.START;  //スタート地点
        map[goalPos[0], goalPos[1]]   = (int)GameManager.MapType.GOAL;   //ゴール地点

        //if (GameManager.Instance.GetGameType() != GameManager.GameType.TIME_ATTACK)
        //{
        //    SetTrap();      //通常プレイのときは迷路に罠を追加
        //    SetRecovery();  //回復床も追加
        //}
    }

    private int SearchPath(Vector2 p)   //探索済みかどうか
    {
        for(int i = 0; i < path.Count; i++)
        {
            if(p.y == path[i].y && p.x == path[i].x)
            {
                return i;
            }
        }
        return (int)RETURN.NONE;
    }

    private int SearchNode(Vector2 p)   //未探索かどうか
    {
        for (int i = 0; i < node.Count; i++)
        {
            if (p.y == node[i].y && p.x == node[i].x)
            {
                return i;
            }
        }
        return (int)RETURN.NONE;
    }

    private int ChooseNode(Vector2 p)   //壁伸ばし法
    {
        if(SearchPath(p) != (int)RETURN.NONE) //探索済なら
        {
            return (int)RETURN.NOT_MADE;    //生成できない
        }
        else    //未探索なら
        {
            path.Add(p);    //pathに入れる

            int sNode = SearchNode(p);
            if (sNode != (int)RETURN.NONE)    //未探索のとき
            {
                node.RemoveAt(sNode);   //nodeから削除

                dir = dir.OrderBy(n => System.Guid.NewGuid()).ToList(); //シャッフル

                int r = 0;
                for (int i = 0; i < 4; i++)
                {
                    r = ChooseNode(new Vector2(p.x + (int)dir[i].x, p.y + (int)dir[i].y));
                    if(r == (int)RETURN.MADE)
                    {
                        break;
                    }
                }

                if(r == (int)RETURN.NOT_MADE)
                {
                    node.Add(path[path.Count - 1]);     //pathから戻す
                    path.RemoveAt(path.Count - 1);
                }

                return r;
            }
            else    //既に壁のとき、pathを繋げる
            {
                Vector2 p_1, p_2;
                
                p_2 = path[path.Count - 1];
                path.RemoveAt(path.Count - 1);

                do
                {
                    p_1 = p_2; 
                    p_2 = path[path.Count - 1];
                    path.RemoveAt(path.Count - 1);

                    map[(int)(p_1.y + p_2.y) / 2, (int)(p_1.x + p_2.x) / 2] = (int)GameManager.MapType.WALL;    //間を埋める
                }
                while (path.Count > 0);

                return (int)RETURN.MADE;
            }
        }
    }

    private void SetTmpStart()     //仮のスタート地点をセット
    {
        int y = MakeRandOdd(HEIGHT - 2);    //ランダムなスタート地点(奇数)を決める
        int x = MakeRandOdd(WIDTH - 2);

        startPos[0] = y;    //スタート地点としてコピー
        startPos[1] = x;
    }

    private int MakeRandOdd(int mod)   //奇数の乱数を作る
    {
        int r = Random.Range(1, mod + 1);
        if (r % 2 == 0) r++;
        if (r > mod) r -= 2;
        return r;
    }

    private void GoalSearch(int sy, int sx)    //ゴールの決定(幅優先探索で一番遠い点をゴールとする)
    {
        Queue<int> queY = new Queue<int>();
        Queue<int> queX = new Queue<int>();
        int[] tmpGoal = new int[2];

        startPos[0] = sy;
        startPos[1] = sx;

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                dist[y, x] = INF;   //コストの初期化
            }
        }

        queY.Enqueue(sy);
        queX.Enqueue(sx);
        dist[sy, sx] = 0;

        int max_dist = 0;

        while (queY.Count > 0)
        {
            int[] p = new int[2];
            p[0] = queY.Dequeue();
            p[1] = queX.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int ny = p[0] + dy[i];
                int nx = p[1] + dx[i];

                if (0 <= ny && ny < HEIGHT && 0 <= nx && nx < WIDTH)
                {
                    if (map[ny, nx] != (int)GameManager.MapType.WALL && dist[ny, nx] == INF) //通路で且つ未訪問のとき
                    {
                        queY.Enqueue(ny);
                        queX.Enqueue(nx);
                        dist[ny, nx] = dist[p[0], p[1]] + 1;

                        if (max_dist < dist[ny, nx])    //最も遠い(コストが高い)地点をゴールとする
                        {
                            max_dist = dist[ny, nx];
                            tmpGoal[0] = ny;
                            tmpGoal[1] = nx;
                        }
                    }
                }
            }
        }
        goalPos[0] = tmpGoal[0];   //コピー
        goalPos[1] = tmpGoal[1];
    }

    private void SetTrap()      //迷路に１つ罠を追加
    {
        while (true)
        {
            //ランダムにxとyを決める(0 と HEIGHT - 1 は壁)
            int randomX = Random.Range(1, HEIGHT - 2);
            int randomY = Random.Range(1, WIDTH - 2);

            //もし通路なら罠に変更する
            if (map[randomY, randomX] == (int)GameManager.MapType.ROAD)
            {
                map[randomY, randomX] = (int)GameManager.MapType.TRAP;
                break;
            }
        }
    }

    private void SetRecovery()  //迷路に１つ回復床を追加
    {
        while (true)
        {
            //ランダムにxとyを決める(0 と HEIGHT - 1 は壁)
            int randomX = Random.Range(1, HEIGHT - 2);
            int randomY = Random.Range(1, WIDTH - 2);

            //もし通路なら回復床に変更する
            if (map[randomY, randomX] == (int)GameManager.MapType.ROAD)
            {
                map[randomY, randomX] = (int)GameManager.MapType.RECOVERY;
                break;
            }
        }
    }

    public void TestDisplay()     //確認用
    {
        for (int y = HEIGHT - 1; y >= 0; y--)
        {
            string s = "";
            for (int x = 0; x < WIDTH; x++)
            {
                if (dist[y, x] == INF)
                {
                    s += "1";
                }
                else
                {
                    s += "0";
                }
            }
            Debug.Log(s);
        }
        Debug.Log(dist[startPos[0], startPos[1]]);  //スタート地点のコスト
        Debug.Log(dist[goalPos[0], goalPos[1]]);    //コール地点のコスト
    }
}