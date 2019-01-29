using UnityEngine;

public class GameOverDirector : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
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
