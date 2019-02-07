using UnityEngine;
using UnityEngine.UI;

//通常プレイの結果表示
public class NormalEndDirector : MonoBehaviour
{
    [SerializeField]
    private Text nowCountText;

    [SerializeField]
    private Text bestCountText;

    private void Awake()
    {
        SoundManager.Instance.EndBGM();   //BGM開始
    }

    // Start is called before the first frame update
    private void Start()
    {
        int nowCount = GameManager.Instance.GetGoalCount();     //今回クリアした回数

        int bestCount = 0;          //最高回数

        string bestCountStr = "";   //保存する名前

        if(GameManager.Instance.GetGameType() == GameManager.GameType.EASY)
        {
            bestCountStr = "BestCountEasy";
        }
        else if(GameManager.Instance.GetGameType() == GameManager.GameType.NORMAL)
        {
            bestCountStr = "BestCountNormal";
        }
        else if(GameManager.Instance.GetGameType() == GameManager.GameType.DIFFICULT)
        {
            bestCountStr = "BestCountDifficult";
        }

        bestCount = PlayerPrefs.GetInt(bestCountStr, 0);

        if(nowCount > bestCount)
        {
            PlayerPrefs.SetInt(bestCountStr, nowCount);
        }

        nowCountText.text  = "今回の記録："   + nowCount.ToString()  + "回";
        bestCountText.text = "最高記録：" + bestCount.ToString() + "回";
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))    //押されたらSelectシーンへ
        {
            StartCoroutine(GameManager.Instance.LoadSceneAsync("SelectScene"));
        }
    }
}
