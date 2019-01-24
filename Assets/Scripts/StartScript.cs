using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//StartSceneのStartDirectorにアタッチ(最初の画面の迷路の表示)
public class StartScript : MonoBehaviour {

    public AudioSource audioSource; //クリック音

    private int HEIGHT = 7;         //迷路の縦幅
    private int WIDTH  = 7;         //迷路の横幅

    private const int ROAD  = 0;    //通路
    private const int WALL  = 1;    //壁
    private const int START = 2;    //スタート地点
    private const int GOAL  = 3;    //ゴール地点

    private int[,] map = new int[7, 7] { { 1, 1, 1, 1 ,1, 1, 1 },
                                         { 1, 0, 0, 0, 0, 0 ,1 },
                                         { 1, 0, 1, 1, 1, 0, 1 },
                                         { 1, 0, 0, 3, 1, 0, 1 },
                                         { 1, 1, 1, 1, 1, 0, 1 },
                                         { 1, 2, 0, 0, 0, 0, 1 },
                                         { 1, 1, 1, 1, 1, 1, 1 } }; //スタート画面の迷路

    // Use this for initialization
    void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();

        maze_display();     //迷路を表示
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))    //押されたらSelectシーンへ
        {
            audioSource.Play();     //クリック音
            StartCoroutine(LoadScene(1.0f));
        }
    }

    private IEnumerator LoadScene(float waitTime)   //シーンの遷移
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("SelectScene");
    }

    private void maze_display() //迷路を表示する
    {
        for (int z = HEIGHT - 1; z >= 0; z--)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (map[z, x] == START)         //スタートの床
                {
                    GameObject start = (GameObject)Resources.Load("Start_s");
                    Instantiate(start, new Vector3(x, -0.5f, -z + 10), Quaternion.identity);
                }
                else if (map[z, x] == GOAL)     //ゴールの床
                {
                    GameObject goal = (GameObject)Resources.Load("Goal_s");
                    Instantiate(goal, new Vector3(x, -0.5f, -z + 10), Quaternion.identity);
                }
                else                            //それ以外(壁がある所にも床を作る)
                {
                    GameObject road = (GameObject)Resources.Load("Road_s");
                    Instantiate(road, new Vector3(x, -0.5f, -z + 10), Quaternion.identity);
                }

                if (map[z, x] == WALL)          //壁
                {
                    GameObject wall = (GameObject)Resources.Load("Wall");
                    Instantiate(wall, new Vector3(x, 0.0f, -z + 10), Quaternion.identity);
                }
            }
        }
    }
}
