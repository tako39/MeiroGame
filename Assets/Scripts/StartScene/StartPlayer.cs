using System.Collections;
using UnityEngine;

//StartSceneのStartPlayerにアタッチ(タイトル画面のプレイヤー)
public class StartPlayer : MonoBehaviour {

    private readonly int[] direct 
        = new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 0, 0 };    //進む方向

    private int directNum;          //directの番号

    private const int startDir = 0; //最初の向き
    private const int endDir = 15;  //最後の向き

    private enum Direct
    {
        RIGHT = 0,  //右
        UP = 1,     //上
        LEFT = 2,   //左
        DOWN = 3,   //下

        NONE = -1,  //なし
    }

    private Vector3 rotatePoint = Vector3.zero; //回転中心
    private Vector3 rotateAxis = Vector3.zero;  //回転軸

    private float cubeAngle = 0.0f;     //回転角度
    private float cubeSizeHalf = 0.0f;  //キューブの半分
    private bool isRotate = false;      //回転中か

    private readonly int[] startPos = new int[] {5, 1};  //最初の位置

    // Use this for initialization
    private void Start () {
        directNum = startDir - 1;
        cubeSizeHalf = transform.localScale.x / 2.0f;
        transform.position = new Vector3(startPos[1], 0.0f, startPos[0]);
    }
	
	// Update is called once per frame
	private void Update () {
        if (isRotate) return;   //回転中は何もしない

        NextDirect();           //方向の切り替え

        MoveDirect();           //回転中心と回転軸を決める

        StartCoroutine(MoveCube()); //ここで回転を始める
    }

    private void NextDirect()   //方向を切り替える
    {
        directNum++;

        if (directNum > endDir)
        {
            directNum = startDir;
            transform.position = new Vector3(startPos[1], 0.0f, startPos[0]);   //スタート位置に移動
        }
    }

    private void MoveDirect()    //回転中心と回転軸の決定
    {
        switch(direct[directNum])   //現在の方向
        {
            case (int)Direct.RIGHT:
                rotatePoint = transform.position + new Vector3(cubeSizeHalf, -cubeSizeHalf, 0.0f);
                rotateAxis = new Vector3(0.0f, 0.0f, -1.0f);
                break;

            case (int)Direct.UP:
                rotatePoint = transform.position + new Vector3(0.0f, -cubeSizeHalf, cubeSizeHalf);
                rotateAxis = new Vector3(1.0f, 0.0f, 0.0f);
                break;

            case (int)Direct.LEFT:
                rotatePoint = transform.position + new Vector3(-cubeSizeHalf, -cubeSizeHalf, 0.0f);
                rotateAxis = new Vector3(0.0f, 0.0f, 1.0f);
                break;

            case (int)Direct.DOWN:
                rotatePoint = transform.position + new Vector3(0.0f, -cubeSizeHalf, -cubeSizeHalf);
                rotateAxis = new Vector3(-1.0f, 0.0f, 0.0f);
                break;
        }
    }

    private IEnumerator MoveCube()  //回転処理
    {
        isRotate = true;    //回転中

        float sumAngle = 0.0f;
        while (sumAngle < 90.0f)    //90度になるまで回す
        {
            cubeAngle = 6.0f;       //回転速度
            sumAngle += cubeAngle;

            if (sumAngle > 90.0f)   //回しすぎないように調整
            {
                cubeAngle -= sumAngle - 90.0f;
            }
            transform.RotateAround(rotatePoint, rotateAxis, cubeAngle);

            yield return null;
        }

        isRotate = false;   //回転終了
        rotatePoint = Vector3.zero;
        rotateAxis = Vector3.zero;

        yield break;
    }
}
