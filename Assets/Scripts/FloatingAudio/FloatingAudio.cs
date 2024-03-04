using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Floating Audio", menuName = "tWS/Floating Audio")]
public class FloatingAudio : ScriptableObject {

    public AudioResource[] clips;
    public Vector2 pitchRange = Vector2.one;
    public AudioMixerGroup output;
    [Range(256, 0)]
    public int priority = 128;
    public Vector2 volumeRange = Vector2.one;
    [Range(-1f, 1f)]
    public float stereoPan = 0f;
    [Range(0f, 1f)]
    public float spatialBlend = 1f;
    [Range(0f, 5f)]
    public float dopplerLevel = 1f;
    public Vector2 minDistanceRange = Vector2.one;
    [Min(0f)]
    public float maxDistance = 500f;
    [Range(0f, 360f)]
    public float spread = 0f;
    public bool simulateSpeedOfSound = false;

    public float pitch {
        get {
            return pitchRange.Range(0.5f);
        }
        set {
            pitchRange.Set(value, value);
        }
    }

    public float volume {
        get {
            return volumeRange.Range(0.5f);
        }
        set {
            volumeRange.Set(value, value);
        }
    }

    public float minDistance {
        get {
            return minDistanceRange.Range(0.5f);
        }
        set {
            minDistanceRange.Set(value, value);
        }
    }

    private AudioSource GetAudioSource() {
        AudioSource source = FloatingAudioManager.NextInstance.source;
        source.resource = clips.Random();
#if UNITY_EDITOR
        //We don't need to set the name, but may be useful for
        //debugging in the editor
        source.name = source.resource.name;
#endif
        source.pitch = pitchRange.Range(Random.value);
        source.outputAudioMixerGroup = output;
        source.priority = priority;
        source.volume = volumeRange.Range(Random.value);
        source.panStereo = stereoPan;
        source.dopplerLevel = dopplerLevel;
        source.minDistance = minDistanceRange.Range(Random.value);
        source.maxDistance = maxDistance;
        source.spread = spread;
        return source;
    }

    public void Play() {
        AudioSource source = GetAudioSource();
        source.spatialBlend = 0f;
        source.Play();
    }

    public void Play(Vector3 position) {
        if(simulateSpeedOfSound) {
            FloatingAudioManager.Instance.StartCoroutine(SpeedOfSoundDelay(position));
            return;
        }
        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.spatialBlend = spatialBlend;
        source.Play();
    }

    public void Play(Transform parent) {
        if(simulateSpeedOfSound) {
            FloatingAudioManager.Instance.StartCoroutine(SpeedOfSoundDelay(parent.position));
            return;
        }
        AudioSource source = GetAudioSource();
        source.transform.position = parent.position;
        source.spatialBlend = spatialBlend;
        source.Play();
    }

    public void Play(Transform parent, Vector3 localPosition) {
        if(simulateSpeedOfSound) {
            FloatingAudioManager.Instance.StartCoroutine(SpeedOfSoundDelay(parent.TransformPoint(localPosition)));
            return;
        }
        AudioSource source = GetAudioSource();
        source.transform.position = parent.TransformPoint(localPosition);
        source.spatialBlend = spatialBlend;
        source.Play();
    }

    private IEnumerator SpeedOfSoundDelay(Vector3 position) {
        yield return new WaitForSeconds(Vector3.Distance(FloatingAudioManager.Instance.Listener.transform.position, position) / 343f);
        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.spatialBlend = spatialBlend;
        source.Play();
    }

}
