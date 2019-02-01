﻿using System.Collections.Generic;
using UnityEngine;

//GameSceneのGameDirectorにアタッチ(迷路生成)
public class GameDirector : MonoBehaviour {

    private const int MAX_HEIGHT = 30;  //迷路の最大の縦幅
    private const int MAX_WIDTH  = 30;  //迷路の最大の横幅

    public static int HEIGHT;           //迷路の縦幅
    public static int WIDTH;            //迷路の横幅

    private int[] mazeSize = new int[] { 13, 17, 21 };   //迷路の大きさ

    private int[] dx = new int[]{ 1, 0, -1, 0 };    //上下左右4方向のx
    private int[] dy = new int[]{ 0, 1, 0, -1 };    //上下左右4方向のy

    public static int[,] map  = new int[MAX_HEIGHT, MAX_WIDTH]; //マップ
    private int[,] dist = new int[MAX_HEIGHT, MAX_WIDTH];       //コスト

    private const int INF = (int)1e5;   //初期化用

    public static int[] startPos = new int[2];  //スタート
    public static int[] goalPos  = new int[2];  //ゴール

    public static GameObject[,] gameObject = new GameObject[MAX_HEIGHT, MAX_WIDTH];

    private void Awake () {

        mazeSize = GameManager.Instance.GetMazeSize();  //迷路の大きさを読み込む

        InitMaze();    //迷路の初期化
        CreateMaze();  //迷路の生成

        GoalSearch(startPos[0], startPos[1]);   //ランダムに決めたスタート地点から一番遠い点(p1)を求める
        GoalSearch(goalPos[0], goalPos[1]);     //(p1)をスタート地点とし、そこから一番遠い点(p2)をゴール地点とする

        map[startPos[0], startPos[1]] = GameManager.START;  //スタート地点
        map[goalPos[0], goalPos[1]] = GameManager.GOAL;     //ゴール地点

        if (GameManager.Instance.GetGameType() != GameManager.TIME_ATTACK)
        {
            SetTrap();      //通常プレイのときは迷路に罠を追加
            SetRecovery();  //回復床も追加
        }
    }

    // Use this for initialization
    private void Start() {
        DisplayMaze();     //迷路の表示

        //TestDisplay();     //マップやそのコストの確認用
    }

    // Update is called once per frame
    private void Update () {
        
	}

    private void InitMaze() //迷路の初期化
    {
        if (GameManager.Instance.GetGameType() == GameManager.TIME_ATTACK) //タイムアタックなら13->17->21
        {
            if (GameManager.Instance.GetGoalCount() < 3)
            {
                HEIGHT = mazeSize[GameManager.Instance.GetGoalCount()];
                WIDTH  = mazeSize[GameManager.Instance.GetGoalCount()];
            }
        }
        else //通常プレイなら迷路の大きさはランダム
        {
            HEIGHT = mazeSize[Random.Range(0, 3)];
            WIDTH  = mazeSize[Random.Range(0, 3)];
        }

        for (int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                map[y, x] = GameManager.WALL; //全て壁にする
            }
        }
    }

    private void CreateMaze()     //迷路の生成
    {
        int y = MakeRandOdd(HEIGHT - 2);    //ランダムなスタート地点(奇数)を決める
        int x = MakeRandOdd(WIDTH  - 2);

        startPos[0] = y;    //スタート地点としてコピー
        startPos[1] = x;

        DigMaze(y, x);      //(y, x)を始点として迷路を作る
    }

    private int MakeRandOdd(int mod)   //奇数の乱数を作る
    {
        int r = Random.Range(1, mod + 1);
        if (r % 2 == 0) r++;
        if (r > mod) r -= 2;
        return r;
    }

    private void DigMaze(int y, int x)    //迷路の作成(穴掘り法)
    {
        int d = Random.Range(0, 4);
        int dd = d;

        while (true)    //4方向に掘り進められなくなるまで
        {
            int py = y + dy[d] * 2;     //二つ先を見る
            int px = x + dx[d] * 2;

            //掘り進められないとき方向を変える
            if (px < 0 || py < 0 || px >= WIDTH || py >= HEIGHT || map[py, px] != GameManager.WALL)
            {
                d++;
                if (d == 4) d = 0;
                if (d == dd) return;    //4方向試し終わった時
                continue;
            }

            map[y + dy[d], x + dx[d]] = GameManager.ROAD;
            map[py, px] = GameManager.ROAD;

            DigMaze(py, px);           //(py, px)を起点として再帰
            d = dd = Random.Range(0, 4);
        }
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
                    if (map[ny, nx] != GameManager.WALL && dist[ny, nx] == INF) //通路で且つ未訪問のとき
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

    private void SetTrap()  //迷路に１つ罠を追加
    {
        while (true)
        {
            //ランダムにxとyを決める(0 と HEIGHT - 1 は壁)
            int randomX = Random.Range(1, HEIGHT - 2);
            int randomY = Random.Range(1, WIDTH  - 2);

            //もし通路なら罠に変更する
            if(map[randomY, randomX] == GameManager.ROAD)
            {
                map[randomY, randomX] = GameManager.TRAP;
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
            if (map[randomY, randomX] == GameManager.ROAD)
            {
                map[randomY, randomX] = GameManager.RECOVERY;
                break;
            }
        }
    }

    private void DisplayMaze()     //迷路を表示する
    {
        for (int y = 0; y < HEIGHT; y++)
        {
            for(int x = 0; x < WIDTH; x++)
            {
                if (map[y, x] == GameManager.ROAD)          //通路の床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Road");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if (map[y, x] == GameManager.WALL)     //壁
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Wall");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.0f), Quaternion.identity);
                }
                else if (map[y, x] == GameManager.START)    //スタートの床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Start");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if (map[y , x] == GameManager.GOAL)    //ゴールの床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Goal");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if(map[y, x] == GameManager.TRAP)      //罠の床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Trap");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
                else if(map[y, x] == GameManager.RECOVERY)  //回復床
                {
                    gameObject[y, x] = (GameObject)Resources.Load("Recovery");
                    Instantiate(gameObject[y, x], new Vector3(x, -y, 0.5f), Quaternion.identity);
                }
            }
        }
    }

    private void TestDisplay()     //確認用
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
        Debug.Log(dist[startPos[0], startPos[1]]);  //スタート地点のコスト
        Debug.Log(dist[goalPos[0], goalPos[1]]);    //コール地点のコスト
    }
}
