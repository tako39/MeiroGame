using UnityEngine;

//回復床にアタッチ
public class Recovery : MonoBehaviour
{
    private Renderer renderer;         //マテリアル変更用

    private bool firstRecover = true;  //回復床に乗ったか

    [SerializeField]
    private Material recoveryMaterial; //回復床
    [SerializeField]
    private Material roadMaterial;     //通路

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        renderer.material = recoveryMaterial;   //回復床
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (firstRecover) //プレイヤーが乗った時マテリアルを通路にする
        {
            if (other.gameObject.CompareTag("Player"))
            {
                renderer.material = roadMaterial;   //通路に変更
                firstRecover = false;
            }
        }
    }
}
