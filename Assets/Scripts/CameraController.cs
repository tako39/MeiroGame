using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//GameSceneのカメラにアタッチ
public class CameraController : MonoBehaviour {

    GameObject player;
    private float[] xplus  = new float[] { 0.0f, 3.0f,  3.0f, -3.0f,  -3.0f }; //視点変更用(位置)
    private float[] yplus  = new float[] { 0.0f, 3.0f, -3.0f,  3.0f,  -3.0f };
    private float dx, dy; //x,y方向の増減
    private int rand; //視点のランダム

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player"); //Playerを見つける
        rand = Random.Range(0, 5);
        dx = xplus[rand];
        dy = yplus[rand];
    }
	
	// Update is called once per frame
	void Update () {
        if(ChangeDirection.change) //視点変更ボタンが押された時に視点を変更
        {
            rand++;
            if(rand > 4)
            {
                rand = 0;
            }
            dx = xplus[rand];
            dy = yplus[rand];

            ChangeDirection.change = false;
        }
        Vector3 playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x + dx, playerPos.y + dy, -15); //Playerについていく
        transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y, 0.0f)); //playerの方を向く
    }
}
