using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//GameSceneのカメラにアタッチ
public class CameraController : MonoBehaviour {

    GameObject player;
    private float[] diffX  = new float[] { 0.0f, 3.0f,  3.0f, -3.0f,  -3.0f }; //視点変更用(位置)
    private float[] diffY  = new float[] { 0.0f, 3.0f, -3.0f,  3.0f,  -3.0f };
    private int rand; //視点のランダム

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player"); //Playerを見つける
        rand = Random.Range(0, 5);
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

            ChangeDirection.change = false;
        }
        Vector3 playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x + diffX[rand], playerPos.y + diffY[rand], -15); //Playerについていく
        transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y, 0.0f)); //playerの方を向く
    }
}
