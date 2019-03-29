using UnityEngine;
using UnityEngine.UI;

//通常プレイの結果表示
public class NormalEndDirector : MonoBehaviour
{
    [SerializeField]
    private Text nowCountText;          //今回のクリア回数
    [SerializeField]
    private Text bestCountText;         //最高クリア回数
    [SerializeField]
    private Text newRecodeText;         //新記録かどうか

    private void Awake()
    {
        SoundManager.Instance.EndBGM();   //BGM開始

        int nowCount = GameManager.Instance.GetGoalCount();     //今回クリアした回数

        string bestCountStr = "";   //保存する名前

        switch (GameManager.Instance.GetGameType())
        {
            case GameManager.GameType.EASY:
                bestCountStr = "BestCountEasy";
                break;

            case GameManager.GameType.NORMAL:
                bestCountStr = "BestCountNormal";
                break;

            case GameManager.GameType.DIFFICULT:
                bestCountStr = "BestCountDifficult";
                break;
        }

        int bestCount = PlayerPrefs.GetInt(bestCountStr, 0);    //最高回数

        if(nowCount > bestCount)    //記録の更新
        {
            PlayerPrefs.SetInt(bestCountStr, nowCount);
            if (!newRecodeText.enabled) newRecodeText.enabled = true;
        }
        else                        //そうでないとき
        {
            if (newRecodeText.enabled)   newRecodeText.enabled = false;
        }

        nowCountText.text  = "今回の記録："   + nowCount.ToString()  + "回";
        bestCountText.text = "過去の最高記録：" + bestCount.ToString() + "回";
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
