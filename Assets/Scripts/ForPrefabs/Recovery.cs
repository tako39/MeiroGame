using UnityEngine;

public class Recovery : MonoBehaviour
{
    private MeshRenderer mRenderer;         //マテリアル変更用

    private bool firstRecover = true;       //回復床に乗ったか

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
        mRenderer.material = mRenderer.materials[0];
    }

    // Start is called before the first frame update
    private void Start()
    {
        firstRecover = true;
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
                mRenderer.material = mRenderer.materials[1];
                firstRecover = false;
            }
        }
    }
}
