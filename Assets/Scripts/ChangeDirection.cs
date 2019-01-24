using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//GameSceneの視点変更ボタンにアタッチ
public class ChangeDirection : MonoBehaviour {

    public static bool change = false;  //向きの変更

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeDir_Click()       //CameraControllerの向きを変える
    {
        change = true;
    }
}
