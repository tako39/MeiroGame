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

    private void StartVibrate()  //バイブレーションを鳴らす
    {
        if (SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
        }
    }

    public void StartSound()
    {
        audioSource.PlayOneShot(startSound);
    }

    public void ClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

    public void RotateSound()
    {
        audioSource.PlayOneShot(rotateSound);
    }

    public void CountSound()
    {
        audioSource.PlayOneShot(countSound);
    }

    public void IncorrectSound()
    {
        StartVibrate();
        audioSource.PlayOneShot(incorrectSound);
    }

    public void DamageSound()
    {
        audioSource.PlayOneShot(damageSound);
    }

    public void RecoverySound()
    {
        audioSource.PlayOneShot(recoverySound);
    }
}
