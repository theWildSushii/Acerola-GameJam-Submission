using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenuBG : MonoBehaviour {

    [SerializeField] private UnityEvent<float> OnBGAlpha;

    private float currentAlpha = 1f;
    private float targetAlpha = 1f;
    private float alphaVelocity = 0f;

    private void Start() {
        AsyncOperation async = SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
        async.completed += SceneLoaded;
    }

    private void Update() {
        currentAlpha = Mathf.SmoothDamp(currentAlpha, targetAlpha, ref alphaVelocity, 1f, 343f);
        OnBGAlpha?.Invoke(currentAlpha);
    }

    private void SceneLoaded(AsyncOperation obj) {
        targetAlpha = 0f;
    }
}
