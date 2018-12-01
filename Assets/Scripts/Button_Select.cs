using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//SelectSceneのボタンにアタッチ
public class Button_Select : MonoBehaviour {

    public AudioSource audioSource; //クリック音

    public GameObject normalplay;   //通常プレイ
    public GameObject timeattack;  //タイムアタック
    public GameObject description; //ルール
    public GameObject operation;   //操作

    public GameObject des_GuideDisplay;  //ルール説明
    public GameObject ope_GuideDisplay;  //操作説明
    public GameObject difficultyDisplay; //難易度選択

    public static bool is_Normalplay; //通常プレイかタイムアタックか

	// Use this for initialization
	void Start () {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NormalPlay_Click() //通常プレイを押したときの動作
    {
        audioSource.Play();
        is_Normalplay = true;
        normalplay.SetActive(false);
        difficultyDisplay.SetActive(true);
        timeattack.SetActive(false);
        description.SetActive(false);
        operation.SetActive(false);
    }

    public void Easy_Click() //かんたんを選択
    {
        audioSource.Play();
        GameScript.msize = new int[] { 11, 13, 15 }; //迷路の大きさをセット
        StartCoroutine(LoadScene("GameScene", 1.0f));
    }

    public void Normal_Click() //ふつうを選択
    {
        audioSource.Play();
        GameScript.msize = new int[] { 15, 17, 19 };
        StartCoroutine(LoadScene("GameScene", 1.0f));
    }
    
    public void Difficult_Click() //難しいを選択
    {
        audioSource.Play();
        GameScript.msize = new int[] { 19, 21, 23 };
        StartCoroutine(LoadScene("GameScene", 1.0f));
    }

    public void TimeAttack_Click() //タイムアタックを押したときの動作
    {
        audioSource.Play();
        is_Normalplay = false;
        GameScript.msize = new int[] { 13, 17, 21 };
        StartCoroutine(LoadScene("CountScene", 1.0f));
    }

    public void Description_Click() //ルール説明を押したときの動作
    {
        audioSource.Play();
        des_GuideDisplay.SetActive(true);
    }

    public void Operation_Click() //操作説明を押したときの動作
    {
        audioSource.Play();
        ope_GuideDisplay.SetActive(true);
    }

    public void DesClose_Click() //ルール説明の×ボタンを押したときの動作
    {
        des_GuideDisplay.SetActive(false);
    }

    public void OpeClose_Click() //操作説明の×ボタンを押したときの動作
    {
        ope_GuideDisplay.SetActive(false);
    }

    IEnumerator LoadScene(string name, float waitTime) //待ってからシーン遷移
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(name);
    }
}
