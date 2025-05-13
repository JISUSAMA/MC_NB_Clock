using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [Header("����� �ҽ�")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;

    [Header("���� ����")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Clip List")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;
    public AudioClip[] narrationClips;

    public bool isPlaying = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� ������ ������Ʈ�� ������
        }
        else if (instance != this)
        {
            Destroy(gameObject); // ���� ������ ���� ���� �� �ı���
        }
        LoadVolumeSettings();
    }
    private void Update()
    {
        if (bgmSource != null)
            bgmSource.volume = bgmVolume;

        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        if (voiceSource != null)
            voiceSource.volume = sfxVolume;

    }
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("BGM_VOLUME", bgmVolume);
        PlayerPrefs.SetFloat("SFX_VOLUME", sfxVolume);
        PlayerPrefs.Save();
    }
    public void LoadVolumeSettings()
    {
        // �⺻�� ����: BGM 0.5, SFX 1.0
        bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1.0f);
    }

    // =============================
    // BGM ���
    // =============================
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[SoundManager] BGM Clip is null!");
            return;
        }

        if (bgmSource.clip == clip)
            return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayBGM(string clipName)
    {
        AudioClip clip = FindClipByName(bgmClips, clipName);

        if (clip != null)
        {
            PlayBGM(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] BGM Clip '{clipName}' not found!");
        }
    }

    // =============================
    // SFX ���
    // =============================
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[SoundManager] SFX Clip is null!");
            return;
        }
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySFX(string clipName)
    {
        AudioClip clip = FindClipByName(sfxClips, clipName);
      
        if (clip != null)
        {
            PlaySFX(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] SFX Clip '{clipName}' not found!");
        }
    }
    private IEnumerator WaitForSFXToEnd()
    {
        // ���尡 ��� ���� ���� ���
        yield return new WaitWhile(() => sfxSource.isPlaying);
        Debug.Log("���� ��� �Ϸ�!");
    }

    // =============================
    // VOICE ���
    // =============================
    public void PlayVOICE(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[SoundManager] SFX Clip is null!");
            return;
        }
        voiceSource.PlayOneShot(clip, sfxVolume);
    }
    public void PlayVOICE(string clipName)
    {
        voiceSource.Stop();
        AudioClip clip = FindClipByName(narrationClips, clipName);
        if (clip != null)
        {
            PlayVOICE(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] VOICE Clip '{clipName}' not found!");
        }
    }

    // =============================
    // ����
    // =============================
    public void StopAllSound()
    {
        bgmSource.Stop();
        sfxSource.Stop();
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }
    public void StopSFX()
    {
        sfxSource.Stop();
    }
    public void StopVOICE()
    {
        voiceSource.Stop();
    }

    // =============================
    // Ŭ�� �̸����� �˻�
    // =============================
    private AudioClip FindClipByName(AudioClip[] clips, string clipName)
    {
        foreach (var clip in clips)
        {
            if (clip != null && clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }
    public AudioClip GetNarrationClipByName(string clipName)
    {
        return FindClipByName(narrationClips, clipName);
    }
}
