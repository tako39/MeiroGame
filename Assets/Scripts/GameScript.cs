using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//GameSceneのGameDirectorにアタッチ(迷路生成)
public class GameScript : MonoBehaviour {

    private const int MAX_HEIGHT = 30; //迷路の最大の大きさ
    private const int MAX_WIDTH  = 30;

    private int HEIGHT; //迷路の縦幅
    private int WIDTH;  //迷路の横幅

    public static int[] msize = new int[] { 13, 17, 21 };

    private const int ROAD  = 0;   //通路
    private const int WALL  = 1;   //壁
    private const int START = 2;   //スタート地点
    private const int GOAL  = 3;   //ゴール地点

    private int[] dx = new int[]{ 1, 0, -1, 0 }; //上下左右4方向
    private int[] dy = new int[]{ 0, 1, 0, -1 };

    public static int[,] map  = new int[MAX_HEIGHT, MAX_WIDTH]; //マップ
    private int[,] dist = new int[MAX_HEIGHT, MAX_WIDTH];　　　　//コスト

    private const int INF = (int)1e5; //初期化用

    public static int[] startpos = new int[2]; //スタート
    public static int[] goalpos  = new int[2]; //ゴール

    public static int[] startpos_copy = new int[] { 1, 1 }; //PlayerController.cs用のコピー
    public static bool player_start = false; //プレイヤーの配置用

    private bool is_normalplay; //ButtonScriptから通常プレイかノーマルプレイか取得
    private int goalcount;      //PlayerControllerからゴール数を取得

    // Use this for initialization
    void Start () {
        is_normalplay = Button_Select.is_Normalplay; //通常プレイかタイムアタックか
        goalcount = PlayerController.goalnum; //ゴールした数

        init_maze(); //迷路の初期化
        maze();      //迷路の生成

        goal_search(startpos[0], startpos[1]); //ランダムに決めたスタート地点から一番遠い点(p1)を求める
        goal_search(goalpos[0], goalpos[1]);   //(p1)をスタート地点とし、そこから一番遠い点(p2)をゴール地点とする

        maze_display();   //迷路の表示
        
        //test_display(); //マップやそのコストの確認用

        startpos_copy = startpos; //PlayerController.cs用
        player_start = true;      //迷路を作り終えたらプレイヤーをスタート地点に置く
    }
	
	// Update is called once per frame
	void Update () {

	}

    void init_maze() //迷路の初期化
    {
        if (is_normalplay) //通常プレイなら迷路の大きさはランダム
        {
            HEIGHT = msize[Random.Range(0, 3)];
            WIDTH  = msize[Random.Range(0, 3)];
        }
        else //タイムアタックなら13->17->21
        {
            if (goalcount < 3)
            {
                HEIGHT = msize[goalcount];
                WIDTH  = msize[goalcount];
            }
        }

        for (int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                map[y, x] = WALL; //全て壁にする
            }
        }
    }

    void maze() //迷路の生成
    {
        int x = rand_odd(WIDTH  - 2); //ランダムなスタート地点(奇数)を決める
        int y = rand_odd(HEIGHT - 2);

        startpos[0] = y; //スタート地点としてコピー
        startpos[1] = x;

        make_maze(y, x); //(y, x)を始点として迷路を作る
    }

    int rand_odd(int mod) //奇数の乱数を作る
    {
        int r = Random.Range(1, mod + 1);
        if (r % 2 == 0) r++;
        if (r > mod) r -= 2;
        return r;
    }

    void make_maze(int y, int x) //迷路の作成(穴掘り法)
    {
        int d = Random.Range(0, 4);
        int dd = d;

        while (true) //4方向に掘り進められなくなるまで
        {
            int px = x + dx[d] * 2; //二つ先を見る
            int py = y + dy[d] * 2;

            //掘り進められないとき方向を変える
            if (px < 0 || py < 0 || px >= WIDTH || py >= HEIGHT || map[py, px] != WALL)
            {
                d++;
                if (d == 4) d = 0;
                if (d == dd) return; //4方向試し終わった時
                continue;
            }

            map[y + dy[d], x + dx[d]] = ROAD;
            map[py, px] = ROAD;

            make_maze(py, px);  //(py, px)を起点として再帰
            d = dd = Random.Range(0, 4);
        }
    }

    void goal_search(int sy, int sx) //ゴールの決定(幅優先探索で一番遠い点をゴールとする)
    {
        Queue<int> quey = new Queue<int>();
        Queue<int> quex = new Queue<int>();
        int[] goal = new int[2];

        startpos[0] = sy;
        startpos[1] = sx;

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                dist[y, x] = INF; //コストの初期化
            }
        }

        quey.Enqueue(sy);
        quex.Enqueue(sx);
        dist[sy, sx] = 0;

        int max_dist = 0;

        while (quey.Count > 0)
        {
            int[] p = new int[2];
            p[0] = quey.Dequeue();
            p[1] = quex.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nx = p[1] + dx[i];
                int ny = p[0] + dy[i];

                if (nx >= 0 && ny >= 0 && nx < WIDTH && ny < HEIGHT)
                {
                    if (map[ny, nx] != WALL && dist[ny, nx] == INF)
                    {
                        quey.Enqueue(ny);
                        quex.Enqueue(nx);
                        dist[ny, nx] = dist[p[0], p[1]] + 1;

                        if (max_dist < dist[ny, nx]) //最も遠い(コストが高い)地点をゴールとする
                        {
                            max_dist = dist[ny, nx];
                            goal[0] = ny;
                            goal[1] = nx;
                        }
                    }
                }
            }
        }
        goalpos[0] = goal[0]; //コピー
        goalpos[1] = goal[1];
    }

    void maze_display() //迷路を表示する
    {
        map[startpos[0], startpos[1]] = START; //スタート地点とゴール地点の設定
        map[goalpos[0], goalpos[1]]   = GOAL;

        for (int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                if(map[y, x] == START)
                {
                    GameObject start = (GameObject)Resources.Load("Start");
                    Instantiate(start, new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if (map[y , x] == GOAL)
                {
                    GameObject goal = (GameObject)Resources.Load("Goal");
                    Instantiate(goal, new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else
                {
                    GameObject road = (GameObject)Resources.Load("Road");
                    Instantiate(road, new Vector3(x, -y, 0.5f), Quaternion.identity);
                }

                if (map[y, x] == WALL)
                {
                    GameObject wall = (GameObject)Resources.Load("Wall");
                    Instantiate(wall, new Vector3(x, -y, 0), Quaternion.identity);
                }
            }
        }
    }

    void test_display() //確認用
    {
        for(int y = HEIGHT - 1; y >= 0; y--)
        {
            string s = "";
            for (int x = 0; x < WIDTH; x++)
            {
                if(dist[y, x] == INF)
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
        Debug.Log(dist[startpos[0], startpos[1]]); //スタート地点のコスト
        Debug.Log(dist[goalpos[0], goalpos[1]]);   //コール地点のコスト
    }
}
