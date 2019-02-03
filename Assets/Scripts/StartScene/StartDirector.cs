using UnityEngine;


//StartSceneのStartDirectorにアタッチ(最初の画面の迷路の表示)
public class StartDirector : MonoBehaviour {

    private const int HEIGHT = 7;         //迷路の縦幅
    private const int WIDTH  = 7;         //迷路の横幅

    private int[,] map = new int[7, 7] { { 1, 1, 1, 1 ,1, 1, 1 },
                                         { 1, 0, 0, 0, 0, 0 ,1 },
                                         { 1, 0, 1, 1, 1, 0, 1 },
                                         { 1, 0, 0, 3, 1, 0, 1 },
                                         { 1, 1, 1, 1, 1, 0, 1 },
                                         { 1, 2, 0, 0, 0, 0, 1 },
                                         { 1, 1, 1, 1, 1, 1, 1 } }; //スタート画面の迷路

    // Use this for initialization
    private void Start () {
        mazeDisplay();     //迷路を表示
	}
	
	// Update is called once per frame
	private void Update () {
        if (Input.GetMouseButtonDown(0))    //押されたらSelectシーンへ
        {
            SoundManager.Instance.StartSound();      //スタート音を鳴らす
            StartCoroutine(GameManager.Instance.LoadSceneAsync("SelectScene"));
        }
    }

    private void mazeDisplay() //迷路を表示する
    {
        for (int z = HEIGHT - 1; z >= 0; z--)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                if (map[z, x] == (int)GameManager.MapType.ROAD)          //通路の床
                {
                    GameObject road = (GameObject)Resources.Load("Road_s");
                    Instantiate(road, new Vector3(x, -0.5f, -z + 10), Quaternion.identity);
                }
                else if (map[z, x] == (int)GameManager.MapType.WALL)     //壁
                {
                    GameObject wall = (GameObject)Resources.Load("Wall_s");
                    Instantiate(wall, new Vector3(x, 0.0f, -z + 10), Quaternion.identity);
                }
                else if (map[z, x] == (int)GameManager.MapType.START)    //スタートの床
                {
                    GameObject start = (GameObject)Resources.Load("Start_s");
                    Instantiate(start, new Vector3(x, -0.5f, -z + 10), Quaternion.identity);
                }
                else if (map[z, x] == (int)GameManager.MapType.GOAL)     //ゴールの床
                {
                    GameObject goal = (GameObject)Resources.Load("Goal_s");
                    Instantiate(goal, new Vector3(x, -0.5f, -z + 10), Quaternion.identity);
                }
            }
        }
    }
}
