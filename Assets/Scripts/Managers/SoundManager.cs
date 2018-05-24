using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;
    public static SoundManager Instance { get { if (instance == null) { instance = FindObjectOfType<SoundManager>(); } return instance; } }

    public float masterVolume = 1;
    public float fadeSpeed = 0.5f;

    public Sound[] effects = new Sound[0];
    public Sound[] music = new Sound[0];

    private Sound currentTrack;
    private Coroutine fadeRoutine;
    
    public bool IsMusicMuted { get; private set; }
    public bool IsEffectsMuted { get; private set; }

    private Dictionary<string, Sound> effectsLibrary = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> musicLibrary = new Dictionary<string, Sound>();

    private void Start()
    {
        effectsLibrary = BuildDictionary(effects);
        musicLibrary = BuildDictionary(music);
        PlayTrack("background");
    }

    private Dictionary<string, Sound> BuildDictionary(Sound[] soundArr)
    {
        var dict = new Dictionary<string, Sound>();
        for (int i = 0; i < soundArr.Length; i++)
        {
            soundArr[i].source = new GameObject("AudioSource_" + soundArr[i].name, typeof(AudioSource)).GetComponent<AudioSource>();
            soundArr[i].source.transform.SetParent(transform);
            soundArr[i].source.clip = soundArr[i].clip;
            soundArr[i].source.loop = soundArr[i].isLooped;
            soundArr[i].source.volume = soundArr[i].volume;

            dict.Add(soundArr[i].name.ToLower(), soundArr[i]);
        }

        return dict;
    }

    public void PlayEffect(string name)
    {
        name = name.ToLower();
        if (!effectsLibrary.ContainsKey(name))
        {
            Debug.LogWarning("Effects library does not contain: " + name);
            return;
        }

        effectsLibrary[name].Play();
    }

    public void PlayTrack(string name) {
        if (!musicLibrary.ContainsKey(name))
        {
            Debug.LogWarning("Music library does not contain: " + name);
            return;
        }

        //TODO: Properly stop current fade coroutine
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeToNext(musicLibrary[name], fadeSpeed));
    }

    public void StopTrack(string name) {
        if (!musicLibrary.ContainsKey(name))
        {
            Debug.LogWarning("Music library does not contain: " + name);
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        musicLibrary[name].source.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;

        if (!IsEffectsMuted)
            SetBulkVolume(effectsLibrary, masterVolume);

        if (!IsMusicMuted)
            SetBulkVolume(musicLibrary, masterVolume);
    }

    public void SetEffectsMuteState(bool isMuted)
    {
        IsEffectsMuted = isMuted;
        SetBulkVolume(effectsLibrary, IsEffectsMuted ? 0 : masterVolume);
    }

    public void SetMusicMuteState(bool isMuted)
    {
        IsMusicMuted = isMuted;
        SetBulkVolume(musicLibrary, IsMusicMuted ? 0 : masterVolume);
    }

    private void SetBulkVolume(Dictionary<string, Sound> soundLibrary, float volume)
    {
        foreach (var sound in soundLibrary)
            sound.Value.source.volume = sound.Value.volume * volume;
    }

    private IEnumerator FadeToNext(Sound track, float fadeSpeed)
    {
        if (currentTrack != null && currentTrack.source.isPlaying && fadeSpeed > 0)
        {
            var startVol = currentTrack.source.volume;
            var steps = 20f;

            // Fade current track out
            for (int i = 0; i < steps; i++)
            {
                // If current track is at end, stop it now
                if (!currentTrack.source.isPlaying)
                    break;

                currentTrack.source.volume = Mathf.Lerp(startVol, 0, i / steps);
                yield return new WaitForSeconds(fadeSpeed / steps);
            }
            currentTrack.source.Stop();
            currentTrack.source.volume = currentTrack.volume * masterVolume;
        }

        yield return 0;

        currentTrack = track;
        currentTrack.source.Play();

        fadeRoutine = null;
    }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public float volume = 1f;
        public bool isLooped = false;
        public bool interruptible = true;

        [HideInInspector]
        public AudioSource source;

        public void Play()
        {
            if (!interruptible && source.isPlaying)
                return;

            source.Stop();
            source.Play();
        }
    }
}
