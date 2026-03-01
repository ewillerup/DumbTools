using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DumbAudio : MonoBehaviour
{
    private IObjectPool<AudioSource> _pool;
    private readonly HashSet<AudioSource> _activeSources = new HashSet<AudioSource>();

#region lifecycle and Singleton
    public static DumbAudio Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        _pool = new ObjectPool<AudioSource>(
            CreatePooledSource,
            OnTakeFromPool,
            OnReturnToPool,
            OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: 16,
            maxSize: 64
        );
    }
#endregion


#region Public Functions that u can call

// basic play sound
    public AudioSource PlaySound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null) return null;
        AudioSource source = _pool.Get();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        StartCoroutine(ReturnToPoolWhenFinished(source, clip.length));
        return source;
    }

    public void PlaySoundOneShot(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null) return;
        AudioSource source = _pool.Get();
        source.volume = volume;
        source.pitch = pitch;
        source.PlayOneShot(clip);
        StartCoroutine(ReturnToPoolWhenFinished(source, clip.length));
    }

    public void StopSound(AudioSource source)
    {
        if (source != null) source.Stop();
        ReleaseToPool(source);
    }

    public void StopSound(AudioClip clip)
    {
        if (clip == null) return;
        var toStop = new List<AudioSource>();
        foreach (AudioSource s in _activeSources)
            if (s != null && s.clip == clip) toStop.Add(s);
        foreach (AudioSource s in toStop) { s.Stop(); ReleaseToPool(s); }
    }

// random sound from a list
    public AudioSource PlayRandomSound(AudioClip[] clips, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clips == null || clips.Length == 0) return null;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        return PlaySound(clip, volume, pitch);
    }

    public AudioSource PlayRandomSound(List<AudioClip> clips, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clips == null || clips.Count == 0) return null;
        AudioClip clip = clips[Random.Range(0, clips.Count)];
        return PlaySound(clip, volume, pitch);
    }

    public void PlayRandomSoundOneShot(AudioClip[] clips, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlaySoundOneShot(clip, volume, pitch);
    }

    public void PlayRandomSoundOneShot(List<AudioClip> clips, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clips == null || clips.Count == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Count)];
        PlaySoundOneShot(clip, volume, pitch);
    }

// looping sounds
    public AudioSource PlayLoopingSound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null) return null;
        AudioSource source = _pool.Get();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = true;
        source.Play();
        return source;
    }

    public void StopLoopingSound(AudioSource source)
    {
        if (source != null) source.Stop();
        ReleaseToPool(source);
    }

#endregion


// many audio sources are created at start
// sources are taken from the pool when needed
// sources are released back into the pool when they are finished
// this helps with performance since we're not instantiating and destroying them every time

    public void ReleaseToPool(AudioSource source)
    {
        if (source != null) _pool.Release(source);
    }

    private AudioSource CreatePooledSource()
    {
        GameObject go = new GameObject("Pooled AudioSource");
        go.transform.SetParent(transform);
        AudioSource s = go.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.loop = false;
        s.spatialBlend = 0f;
        go.SetActive(false);
        return s;
    }

    private void OnTakeFromPool(AudioSource source)
    {
        _activeSources.Add(source);
        source.gameObject.SetActive(true);
        source.volume = 1f;
        source.pitch = 1f;
        source.loop = false;
        source.clip = null;
    }

    private void OnReturnToPool(AudioSource source)
    {
        _activeSources.Remove(source);
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(AudioSource source)
    {
        Destroy(source.gameObject);
    }

    private IEnumerator ReturnToPoolWhenFinished(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        _pool.Release(source);
    }
}