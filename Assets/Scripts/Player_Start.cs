using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//StartSceneのStartPlayerにアタッチ(タイトル画面のプレイヤー)
public class Player_Start : MonoBehaviour {

    private int[] dir = new int[] { -1, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 0, 0 }; //0:右 1:上 2:左 3:下
    private int dnum;

    //回転
    Vector3 rotatePoint = Vector3.zero; //回転中心
    Vector3 rotateAxis = Vector3.zero; //回転軸

    private float cubeAngle = 0.0f;     //回転角度
    private float cubeSizeHalf = 0.0f;  //キューブの半分
    private bool isRotate = false;      //回転中か

    private int[] startPos = new int[] {5, 1};

    // Use this for initialization
    void Start () {
        dnum = -1;
        cubeSizeHalf = transform.localScale.x / 2.0f;
        transform.position = new Vector3(startPos[1], 0.0f, startPos[0]);
    }
	
	// Update is called once per frame
	void Update () {
        if (isRotate) return; //回転中は何もしない

        dnum++;
        if (dnum > 16)
        {
            dnum = 0;
            transform.position = new Vector3(startPos[1], 0.0f, startPos[0]); //スタート位置に移動
        }

        MoveDirection();

        StartCoroutine(MoveCube()); //ここで回転を始める
    }

    void MoveDirection()
    {
        if (dir[dnum] == 0)  //右
        {
            rotatePoint = transform.position + new Vector3(cubeSizeHalf, -cubeSizeHalf, 0f);
            rotateAxis = new Vector3(0, 0, -1);
        }

        if (dir[dnum] == 1)  //上
        {
            rotatePoint = transform.position + new Vector3(0f, -cubeSizeHalf, cubeSizeHalf);
            rotateAxis = new Vector3(1, 0, 0);
        }

        if (dir[dnum] == 2)  //左
        {
            rotatePoint = transform.position + new Vector3(-cubeSizeHalf, -cubeSizeHalf, 0f);
            rotateAxis = new Vector3(0, 0, 1);
        }

        if (dir[dnum] == 3)  //下
        {
            rotatePoint = transform.position + new Vector3(0f, -cubeSizeHalf, -cubeSizeHalf);
            rotateAxis = new Vector3(-1, 0, 0);
        }
    }

    IEnumerator MoveCube() //回転処理
    {
        isRotate = true; //回転中

        float sumAngle = 0.0f;
        while (sumAngle < 90.0f) //90度になるまで回す
        {
            cubeAngle = 6.0f;  //回転速度
            sumAngle += cubeAngle;

            if (sumAngle > 90.0f) //回しすぎないように調整
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
