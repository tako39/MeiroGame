using UnityEngine;
using UnityEngine.SceneManagement;


//GameSceneのPauseButtonにアタッチ
public class PauseScript : MonoBehaviour {

    [SerializeField]
    private GameObject pauseButton;  //プレイ中に表示する画像

    [SerializeField]
    private GameObject startButton;  //ポーズ中に表示する画像

    [SerializeField]
    private GameObject pauseScreen;  //ポーズ画面

    [SerializeField]
    private GameObject endButton;    //やめるボタン
    
    private int clickCount;          //何回押したか

    // Use this for initialization
    private void Start () {
        clickCount = 0;
	}
	
	// Update is called once per frame
	private void Update () {
		
	}

    public void PushPauseButton() //ポーズボタンを押したとき
    {
        clickCount++;

        if (clickCount % 2 == 1)
        {
            GameManager.Instance.SetIsPause(true);

            pauseButton.SetActive(false);
            startButton.SetActive(true);
            pauseScreen.SetActive(true);
            endButton.SetActive(true);
        }
        else
        {
            GameManager.Instance.SetIsPause(false);

            pauseButton.SetActive(true);
            startButton.SetActive(false);
            pauseScreen.SetActive(false);
            endButton.SetActive(false);
        }
    }

    public void PushEndButton() //やめるボタンを押したとき
    {
        GameManager.Instance.SetIsPause(false);

        PlayerPrefs.SetInt("Minutes", 0);
        PlayerPrefs.SetFloat("Seconds", 0.0f);
        PlayerPrefs.SetInt("Goalnum", 0);

        if (GameManager.Instance.GetGameType() == GameManager.GameType.TIME_ATTACK) {
            SceneManager.LoadScene("SelectScene");      //Selectシーンへ
        }
        else {
            SceneManager.LoadScene("NormalEndScene");   //結果表示
        }
    }
}
