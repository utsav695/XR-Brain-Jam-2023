using UnityEngine;
using MyBox;

public enum ReferenceType { Self, GameObject, Component }

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    #region Fields and Properties

    [SerializeField]
    private ReferenceType audioSourceReferenceType = ReferenceType.Self;

    [SerializeField]
    [ConditionalField(true, nameof(IsGameObject))]
    private GameObject audioSourceObject;

    [SerializeField]
    [ConditionalField(true, nameof(IsComponent))]
    private AudioSource audioSource;

    private static AudioClip _uiClickSound;
    private static AudioSource _source;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        switch (audioSourceReferenceType)
        {
            case ReferenceType.Self:
                InitializeAudioSource(gameObject);
                break;
            case ReferenceType.GameObject:
                if (audioSourceObject)
                {
                    InitializeAudioSource(audioSourceObject);
                }
                else
                {
                    Debug.LogWarning("Audio Source object not assigned!");
                }
                break;
            case ReferenceType.Component:
                if (audioSource)
                {
                    SetAudioSource(audioSource);
                }
                break;
        }
    }

    #endregion

    #region Private Methods

    private bool IsGameObject() => audioSourceReferenceType == ReferenceType.GameObject;

    private bool IsComponent() => audioSourceReferenceType == ReferenceType.Component;

    private void InitializeAudioSource(GameObject audioParent)
    {
        if (SetAudioSource(audioParent.GetComponent<AudioSource>()) ||
            SetAudioSource(audioParent.GetComponentInChildren<AudioSource>()))
        {
            return;
        }

        _ = SetAudioSource(audioParent.AddComponent<AudioSource>());
    }

    private bool SetAudioSource(AudioSource source)
    {
        if (!source || source == _source)
        {
            return false;
        }

        _source = source;
        _source.playOnAwake = false;
        _source.loop = false;

        return true;
    }

    #endregion

    #region Public Static Methods

    public static void PlayUiClickSound() => Play(_uiClickSound, true);

    public static void PlayUiClickSound(float volume)
    {
        Play(_uiClickSound, true);
    }

    public static void Play(AudioClip clip, bool oneShot = false)
    {
        if (!_source || !clip || clip.length == 0f)
        {
            return;
        }

        if (oneShot)
        {
            _source.PlayOneShot(clip);
        }
        else
        {
            Stop();
            _source.clip = clip;
            _source.Play();
        }
    }

    public static void PreviewVolumeSettings(AudioClip clip, float _volume)
    {
        Play(clip, false);
        if (!_source)
        {
            return;
        }
        _source.volume = _volume;
    }

    public static bool IsPlaying()
    {
        return _source && _source.isPlaying;
    }

    public static void Stop()
    {
        if (IsPlaying())
        {
            _source.Stop();
        }
    }

    #endregion
}
