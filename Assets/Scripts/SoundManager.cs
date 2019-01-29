using UnityEngine;

//音楽を管理するクラス
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip startSound;

    [SerializeField]
    private AudioClip clickSound;

    [SerializeField]
    private AudioClip rotateSound;

    [SerializeField]
    private AudioClip countSound;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);

        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void StartSound()
    {
        audioSource.clip = startSound;
        audioSource.Play();
    }

    public void ClickSound()
    {
        audioSource.clip = clickSound;
        audioSource.Play();
    }

    public void RotateSound()
    {
        audioSource.clip = rotateSound;
        audioSource.Play();
    }

    public void CountSound()
    {
        audioSource.clip = countSound;
        audioSource.Play();
    }
}
