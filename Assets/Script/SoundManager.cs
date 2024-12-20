using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip SpeedBuffSound;
    public AudioClip HideBuffSound;
    public AudioClip ShieldBuffSound;
    public AudioClip GameOverSound;
    public AudioClip LevelTransition;

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
}
