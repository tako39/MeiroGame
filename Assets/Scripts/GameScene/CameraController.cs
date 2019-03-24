using UnityEngine;

//GameSceneのカメラにアタッチ
public class CameraController : MonoBehaviour {

    [SerializeField]
    private GameObject player;

    private float[] viewX  = new float[] { 0.0f, 3.0f,  3.0f, -3.0f,  -3.0f }; //視点変更用(位置)
    private float[] viewY  = new float[] { 0.0f, 3.0f, -3.0f,  3.0f,  -3.0f };

    private int randomView; //ランダムの始点

	// Use this for initialization
	private void Start () {
        randomView = Random.Range(0, 5);
    }
	
	// Update is called once per frame
	private void Update () {
        followPlayer();     //プレイヤ―に追従
    }

    private void followPlayer() {
        Vector3 playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x + viewX[randomView], playerPos.y + viewY[randomView], -15);    //Playerについていく
        transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y, 0.0f));  //playerの方を向く
    }
}
