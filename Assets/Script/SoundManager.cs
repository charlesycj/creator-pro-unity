using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip SpeedBuffSound;
    public AudioClip HideBuffSound;
    public AudioClip ShieldBuffSound;

    public AudioClip SpeedDeBuffSound;
    public AudioClip ReverseDeBuffSound;

    public AudioClip GameOverSound;
    public AudioClip LevelTransition;
    public AudioClip RemoveShield;

    public AudioClip stage1BGM;
    public AudioClip stage2BGM;
    public AudioClip stage3BGM;

    [SerializeField] private SoundManager soundManager;

    private void Awake()
    {
        if (audioSource == null) // audioSource가 Inspector에서 할당되지 않았을 경우 초기화
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource 컴포넌트를 찾을 수 없습니다! SoundManager에 AudioSource를 추가하세요.");
            }
        }
    }

    public bool IsStageBGMPlaying(int stage)
    {
        AudioClip stageBGM = stage == 1 ? stage1BGM :
                             stage == 2 ? stage2BGM :
                             stage == 3 ? stage3BGM : null;

        return audioSource.clip == stageBGM && audioSource.isPlaying;
    }

    // 사운드 재생
    public void PlaySpeedBuffSound()
    {
        PlaySound(SpeedBuffSound);
    }

    public void PlayHideBuffSound()
    {
        PlaySound(HideBuffSound);
    }

    public void PlayShieldBuffSound()
    {
        PlaySound(ShieldBuffSound);
    }
    public void PlayGameOverSound()
    {
        PlaySound(GameOverSound);
    }

    public void PlaySpeedDeBuffSound()
    {
        PlaySound(SpeedDeBuffSound);
    }
    public void PlayReverseDeBuffSound()
    {
        PlaySound(ReverseDeBuffSound);
    }
    public void playLevelTransition()
    {
        PlaySound(LevelTransition);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    public void PlayRemoveShield()
    {
        PlaySound(RemoveShield);
    }
    public void PlayStageBGM(int stage)
    {
        switch (stage)
        {
            case 1:
                audioSource.clip = stage1BGM;
                break;
            case 2:
                audioSource.clip = stage2BGM;
                break;
            case 3:
                audioSource.clip = stage3BGM;
                break;
            default:
                Debug.LogWarning("Invalid stage number for BGM.");
                return;
        }

        audioSource.loop = true; // BGM은 반복 재생
        audioSource.Play();
    }

}
