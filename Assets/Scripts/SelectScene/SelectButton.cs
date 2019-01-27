using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//SelectSceneのボタンにアタッチ
public class SelectButton : MonoBehaviour {

    public AudioSource audioSource;         //クリック音

    public GameObject normalPlay;           //通常プレイ
    public GameObject timeAttack;           //タイムアタック
    public GameObject description;          //ルール
    public GameObject operation;            //操作

    public GameObject desGuideDisplay;      //ルール説明
    public GameObject opeGuideDisplay;      //操作説明
    public GameObject difficultyDisplay;    //難易度選択

	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
        GameManager.SetPlayerHp(GameManager.maxPlayerHp);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NormalPlay_Click()  //通常プレイを押したときの動作
    {
        audioSource.Play();
        GameManager.SetIsNormalPlay(true);

        normalPlay.SetActive(false);
        difficultyDisplay.SetActive(true);  //難易度選択だけを表示する
        timeAttack.SetActive(false);
        description.SetActive(false);
        operation.SetActive(false);
    }

    public void Easy_Click()        //かんたんを選択
    {
        audioSource.Play();

        GameManager.SetMazeSize(GameManager.EASY);

        StartCoroutine(GameManager.Instance.LoadSceneAsync("CountScene"));
    }

    public void Normal_Click()      //ふつうを選択
    {
        audioSource.Play();

        GameManager.SetMazeSize(GameManager.NORMAL);

        StartCoroutine(GameManager.Instance.LoadSceneAsync("CountScene"));
    }
    
    public void Difficult_Click()   //難しいを選択
    {
        audioSource.Play();

        GameManager.SetMazeSize(GameManager.DIFFICULT);

        StartCoroutine(GameManager.Instance.LoadSceneAsync("CountScene"));
    }

    public void TimeAttack_Click()  //タイムアタックを押したときの動作
    {
        audioSource.Play();

        GameManager.SetIsNormalPlay(false);

        GameManager.SetMazeSize(GameManager.TIME_ATTACK);

        StartCoroutine(GameManager.Instance.LoadSceneAsync("CountScene"));
    }

    private IEnumerator LoadScene(string name, float waitTime)  //シーン遷移
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(name);
    }

    public void Description_Click() //ルール説明を押したときの動作
    {
        audioSource.Play();
        desGuideDisplay.SetActive(true);
    }

    public void Operation_Click()   //操作説明を押したときの動作
    {
        audioSource.Play();
        opeGuideDisplay.SetActive(true);
    }

    public void DesClose_Click()    //ルール説明の×ボタンを押したときの動作
    {
        desGuideDisplay.SetActive(false);
    }

    public void OpeClose_Click()    //操作説明の×ボタンを押したときの動作
    {
        opeGuideDisplay.SetActive(false);
    }
}
