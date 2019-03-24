using UnityEngine;
using UnityEngine.UI;

//CountSceneのCountDirectorにアタッチ
public class CountDirector : MonoBehaviour {

    private float sumTime;      //時間の計測
    private int seconds;        //カウントダウン用

    [SerializeField]
    private Text countText;     //カウントの表示

    bool isSceneChange;         //シーン遷移するか

    // Use this for initialization
    private void Start () {
        SoundManager.Instance.CountSound();  //カウントダウン音
        sumTime = 3.0f;
        isSceneChange = true;
	}
	
	// Update is called once per frame
	private void Update () {
        sumTime -= Time.deltaTime;  //カウントダウンする

        TextChange();   //表示を変更
    }

    private void TextChange()   //2 -> 1 -> 0
    {
        seconds = (int)sumTime;

        if(seconds > 1)
        {
            countText.text = "2";
        }
        else if(seconds > 0)
        {
            countText.text = "1";
        }
        else
        {
            countText.text = "0";

            if (isSceneChange)
            {
                isSceneChange = false;
                StartCoroutine(GameManager.Instance.LoadSceneAsync("GameScene"));   //ゲームシーンへ
            }
        }
    }
}
