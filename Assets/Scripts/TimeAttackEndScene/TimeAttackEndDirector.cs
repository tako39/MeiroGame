using UnityEngine;
using UnityEngine.UI;

//タイムアタックの結果表示
public class TimeAttackEndDirector : MonoBehaviour {

    [SerializeField]
    private Text clearTimeText;  //クリア時間の表示

    [SerializeField]
    private Text bestTimeText;   //ベストタイムの表示

	// Use this for initialization
	private void Start () {
        int clearTime = GameManager.Instance.GetTotalTime();    //クリア時間を取得

        int minutes = clearTime / 60;   //分
        int seconds = clearTime % 60;   //秒
        clearTimeText.text = "クリアジカン：" + minutes.ToString("00") + ":" + seconds.ToString("00");

        //今までのベストタイムを求める
        int bestTime = PlayerPrefs.GetInt("BestTime", (int)1e9);
        if(bestTime > clearTime)
        {
            PlayerPrefs.SetInt("BestTime", clearTime);

            bestTimeText.text = "ジコベスト：" + minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else
        {
            int min = bestTime / 60;
            int sec = bestTime % 60;
            bestTimeText.text = "ジコベスト：" + min.ToString("00") + ":" + sec.ToString("00");
        }
	}
	
	// Update is called once per frame
	private void Update () {
        if (Input.GetMouseButtonDown(0))    //押されたらSelectシーンへ
        {
            StartCoroutine(GameManager.Instance.LoadSceneAsync("SelectScene"));
        }
    }
}
