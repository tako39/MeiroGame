using UnityEngine;

//SelectSceneの初期化ボタンにアタッチ
public class Initialization : MonoBehaviour {
    
    [SerializeField]
    private GameObject initial;

    private int pushCount;  //押された回数

	// Use this for initialization
	private void Start () {
        pushCount = 0;
	}
	
	// Update is called once per frame
	private void Update () {
        
    }

    public void PushCount()
    {
        pushCount++;

        if (pushCount > 5)
        {
            pushCount = 0;
            initial.SetActive(true);
        }
    }

    public void PushYesButton() //はいを押したとき
    {
        PlayerPrefs.DeleteAll();
        initial.SetActive(false);
    }

    public void PushBackButton() //ｘを押したとき
    {
        pushCount = 0;
        initial.SetActive(false);
    }
}
