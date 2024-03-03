using System.Collections;
using UnityEngine;

public class PauseManager : MonoBehaviour {

    public static PauseManager Instance { get; protected set; }

    [SerializeField] private BasicChannel OnPause;
    [SerializeField] private BasicChannel OnResume;

    public bool IsPaused { get; protected set; } = false;

    private void Awake() {
        Instance = this;
    }

    public void TogglePause() {
        if(IsPaused) {
            Resume();
        } else {
            Pause();
        }
    }

    public void Pause() {
        IsPaused = true;
        Time.timeScale = 0f;
        StopAllCoroutines();
        OnPause?.FireEvent();
    }

    public void Resume() {
        IsPaused = false;
        StartCoroutine(RecoverTimeScale());
        OnResume?.FireEvent();
    }

    //We slowly set timeScale to 1 instead of setting it
    //directly to give player time to react to any
    //obstacles they forgot when they paused
    private IEnumerator RecoverTimeScale() {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float velocity = 0f;
        do {
            Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1f, ref velocity, 2f/24f, 1f, Time.unscaledDeltaTime);
            yield return wait;
        } while(!Mathf.Approximately(Time.timeScale, 1f));
        Time.timeScale = 1f;
    }

}
