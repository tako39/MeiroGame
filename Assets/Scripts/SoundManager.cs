using UnityEngine;

//音楽を管理するクラス
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip startSound;       //スタート音
    [SerializeField]
    private AudioClip clickSound;       //クリック音
    [SerializeField]
    private AudioClip rotateSound;      //回転音
    [SerializeField]
    private AudioClip countSound;       //カウントダウン音
    [SerializeField]
    private AudioClip incorrectSound;   //不正解音
    [SerializeField]
    private AudioClip damageSound;      //ダメージ音
    [SerializeField]
    private AudioClip recoverySound;    //回復音

    [SerializeField]
    private AudioClip startBGM;         //スタートのBGM
    [SerializeField]
    private AudioClip selectBGM;        //選択中のBGM
    [SerializeField]
    private AudioClip gameBGM;          //ゲーム中のBGM
    [SerializeField]
    private AudioClip endBGM;           //結果発表のBGM

    private float stopTime = 0.0f;             //停止時間

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);

        audioSource  = GetComponent<AudioSource>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartBGM();   //BGM開始
        audioSource.Play();
    }

    private void StartVibrate()  //バイブレーションを鳴らす
    {
        if (SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
        }
    }

    public void StopAudio()     //音を止める
    {
        if (audioSource.isPlaying)
        {
            stopTime = Instance.audioSource.time;
            audioSource.Stop();
        }
    }

    public void PlayAudio()     //音を再生する
    {
        if (!audioSource.isPlaying)
        {
            audioSource.time = stopTime;
            audioSource.Play();
        }
    }

    public void StartSound()    //スタート音を鳴らす
    {
        audioSource.PlayOneShot(startSound);
    }

    public void ClickSound()    //クリック音を鳴らす
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void RotateSound()   //回転音を鳴らす
    {
        audioSource.PlayOneShot(rotateSound);
    }

    public void CountSound()    //カウントダウン音を鳴らす
    {
        if(audioSource.isPlaying) audioSource.Stop();
        audioSource.PlayOneShot(countSound);
    }

    public void IncorrectSound()    //不正解音を鳴らす
    {
        StartVibrate();     //バイブレーション
        audioSource.PlayOneShot(incorrectSound);
    }

    public void DamageSound()   //ダメージ音を鳴らす
    {
        audioSource.PlayOneShot(damageSound);
    }

    public void RecoverySound() //回復音を鳴らす
    {
        audioSource.PlayOneShot(recoverySound);
    }

    public void StartBGM()      //スタートのBGMを鳴らす
    {
        if (audioSource.clip != startBGM)
        {
            audioSource.clip = startBGM;
            audioSource.time = 0.0f;
            audioSource.Play();
        }
    }

    public void SelectBGM()     //選択中のBGMを鳴らす
    {
        if (audioSource.clip != selectBGM)
        {
            audioSource.clip = selectBGM;
            audioSource.time = 0.0f;
            audioSource.Play();
        }
    }

    public void GameBGM()       //ゲーム中のBGM鳴らす
    {
        if (audioSource.clip != gameBGM)
        {
            audioSource.clip = gameBGM;
            audioSource.time = 0.0f;
            audioSource.Play();
        }
    }

    public void EndBGM()        //結果発表のBGMを鳴らす
    {
        if (audioSource.clip != endBGM)
        {
            audioSource.clip = endBGM;
            audioSource.time = 0.0f;
            audioSource.Play();
        }
    }
}
