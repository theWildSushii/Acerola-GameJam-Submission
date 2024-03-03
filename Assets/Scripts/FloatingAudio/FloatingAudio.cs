﻿using UnityEngine;
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
        AudioSource source = GetAudioSource();
        source.transform.position = position;
        source.spatialBlend = spatialBlend;
        source.Play();
    }

    public void Play(Transform parent) {
        AudioSource source = GetAudioSource();
        source.transform.parent = parent;
        source.transform.localPosition = Vector3.zero;
        source.spatialBlend = spatialBlend;
        source.Play();
    }

    public void Play(Transform parent, Vector3 localPosition) {
        AudioSource source = GetAudioSource();
        source.transform.parent = parent;
        source.transform.localPosition = localPosition;
        source.spatialBlend = spatialBlend;
        source.Play();
    }

}
