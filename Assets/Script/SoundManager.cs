using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip SpeedBuffSound;
    public AudioClip HideBuffSound;
    public AudioClip ShieldBuffSound;
    public AudioClip GameOverSound;
    public AudioClip LevelTransition;
    public AudioClip RemoveShield;

    public AudioClip stage1BGM;
    public AudioClip stage2BGM;
    public AudioClip stage3BGM;

    private void Awake()
    {
        if (audioSource == null) // audioSource�� Inspector���� �Ҵ���� �ʾ��� ��� �ʱ�ȭ
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource ������Ʈ�� ã�� �� �����ϴ�! SoundManager�� AudioSource�� �߰��ϼ���.");
            }
        }
    }
    // ���� ���
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

        audioSource.loop = true; // BGM�� �ݺ� ���
        audioSource.Play();
    }

}
