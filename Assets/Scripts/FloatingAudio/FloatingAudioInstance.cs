using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FloatingAudioInstance : MonoBehaviour {

    public AudioSource source { get; set; }

    private void Awake() {
        if(!source) {
            source = GetComponent<AudioSource>();
        }
    }

    private void LateUpdate() {
        if(!source.isPlaying) {
            FloatingAudioManager.Release(this);
        }
    }

}
