using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Renderer renderer; //マテリアル変更用

    private float timeOut = 1.5f;       //1.5秒毎
    private float timeElapsed = 0.0f;   //経過時間

    public static bool isTrap { get; private set; } //罠の状態かどうか

    [SerializeField]
    private Material trapMaterial;     //罠
    [SerializeField]
    private Material roadMaterial;     //通路

    [SerializeField]
    private GameObject prick;           //トゲ

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        renderer.material = trapMaterial;    //罠
        isTrap = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            isTrap = !isTrap;

            if (isTrap)
            {
                renderer.material = trapMaterial;    //罠
                prick.SetActive(true);
            }
            else
            {
                renderer.material = roadMaterial;    //通路
                prick.SetActive(false);
            }

            timeElapsed = 0.0f;
        }
    }
}
