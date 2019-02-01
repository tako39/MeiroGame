using UnityEngine;

//SelectSceneのボタンにアタッチ
public class SelectButton : MonoBehaviour {

    [SerializeField]
    private GameObject normalPlay;           //通常プレイ
    [SerializeField]
    private GameObject timeAttack;           //タイムアタック
    [SerializeField]
    private GameObject description;          //ルール
    [SerializeField]
    private GameObject operation;            //操作

    [SerializeField]
    private GameObject desGuideDisplay;      //ルール説明
    [SerializeField]
    private GameObject opeGuideDisplay;      //操作説明
    [SerializeField]
    private GameObject difficultyDisplay;    //難易度選択

	// Use this for initialization
	private void Start () {
        GameManager.Instance.SetPlayerHp(GameManager.maxPlayerHp);
        GameManager.Instance.SetGoalCount(0);   //ゴール数の初期化
    }
	
	// Update is called once per frame
	private void Update () {
		
	}

    public void PushNormalPlay()  //通常プレイを押したときの動作
    {
        SoundManager.Instance.ClickSound();

        difficultyDisplay.SetActive(true);
        normalPlay.SetActive(false);
        timeAttack.SetActive(false);
        description.SetActive(false);
        operation.SetActive(false);
    }

    public void PushEasy()        //かんたんを選択
    {
        SoundManager.Instance.ClickSound();

        GameManager.Instance.SetGameType(GameManager.EASY);
        GameManager.Instance.SetMazeSize();

        StartCoroutine(GameManager.Instance.LoadSceneAsync("CountScene"));
    }

    public void PushNormal()      //ふつうを選択
    {
        SoundManager.Instance.ClickSound();

        GameManager.Instance.SetGameType(GameManager.NORMAL);
        GameManager.Instance.SetMazeSize();

        StartCoroutine(GameManager.Instance.LoadSceneAsync("CountScene"));
    }
    
    public void PushDifficult()   //難しいを選択
    {
        SoundManager.Instance.ClickSound();

        GameManager.Instance.SetGameType(GameManager.DIFFICULT);
        GameManager.Instance.SetMazeSize();

        StartCoroutine(GameManager.Instance.LoadSceneAsync("CountScene"));
    }

    public void PushTimeAttack()  //タイムアタックを押したときの動作
    {
        SoundManager.Instance.ClickSound();

        GameManager.Instance.SetGameType(GameManager.TIME_ATTACK);
        GameManager.Instance.SetMazeSize();

        StartCoroutine(GameManager.Instance.LoadSceneAsync("CountScene"));
    }

    public void PushDescription() //ルール説明を押したときの動作
    {
        SoundManager.Instance.ClickSound();
        desGuideDisplay.SetActive(true);

        normalPlay.SetActive(false);
        timeAttack.SetActive(false);
        description.SetActive(false);
        operation.SetActive(false);
    }

    public void PushOperation()   //操作説明を押したときの動作
    {
        SoundManager.Instance.ClickSound();
        opeGuideDisplay.SetActive(true);

        normalPlay.SetActive(false);
        timeAttack.SetActive(false);
        description.SetActive(false);
        operation.SetActive(false);
    }

    public void PushDesClose()    //ルール説明の×ボタンを押したときの動作
    {
        desGuideDisplay.SetActive(false);

        normalPlay.SetActive(true);
        timeAttack.SetActive(true);
        description.SetActive(true);
        operation.SetActive(true);
    }

    public void PushOpeClose()    //操作説明の×ボタンを押したときの動作
    {
        opeGuideDisplay.SetActive(false);

        normalPlay.SetActive(true);
        timeAttack.SetActive(true);
        description.SetActive(true);
        operation.SetActive(true);
    }
}
