using UnityEngine;
using UnityEngine.Audio;

public class PauseManager : MonoBehaviour {

    public static PauseManager Instance { get; protected set; }

    [SerializeField] private BasicChannel OnPause;
    [SerializeField] private BasicChannel OnResume;
    [SerializeField] private AudioMixer mixer;
    [Range(10f, 22000f)]
    [SerializeField] private float LowpassHz = 700f;

    public bool IsPaused { get; protected set; } = false;

    private void Awake() {
        Instance = this;
    }

    private void OnDisable() {
        Resume();
    }

    public void TogglePause() {
        if(IsPaused) {
            Resume();
        } else {
            Pause();
        }
    }

    public void Pause() {
        if(IsPaused) {
            return;
        }
        IsPaused = true;
        mixer.SetFloat("LowpassHz", LowpassHz);
        Time.timeScale = 0f;
        StopAllCoroutines();
        OnPause?.FireEvent();
    }

    public void Resume() {
        if(!IsPaused) {
            return;
        }
        IsPaused = false;
        Time.timeScale = 1f;
        mixer.SetFloat("LowpassHz", 22000f);
        OnResume?.FireEvent();
    }

}
