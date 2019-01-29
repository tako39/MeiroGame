using UnityEngine;


//GameSceneのカメラにアタッチ
public class CameraController : MonoBehaviour {

    [SerializeField]
    private GameObject player;

    private float[] diffX  = new float[] { 0.0f, 3.0f,  3.0f, -3.0f,  -3.0f }; //視点変更用(位置)
    private float[] diffY  = new float[] { 0.0f, 3.0f, -3.0f,  3.0f,  -3.0f };

    private int rand; //視点のランダム

	// Use this for initialization
	private void Start () {
        rand = Random.Range(0, 5);
    }
	
	// Update is called once per frame
	private void Update () {
        Vector3 playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x + diffX[rand], playerPos.y + diffY[rand], -15);    //Playerについていく
        transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y, 0.0f));  //playerの方を向く
    }
}
