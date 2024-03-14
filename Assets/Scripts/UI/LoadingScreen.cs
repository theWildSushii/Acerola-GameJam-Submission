using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    public static LoadingScreen Instance { get; protected set; }

    [SerializeField] private UnityEvent OnLoadStarted;
    [SerializeField] private UnityEvent OnLoadFinish;
    [SerializeField] private UnityEvent<float> OnProgressUpdated;

    private AsyncOperation async;

    public bool IsLoading { get; protected set; } = false;

    private void Awake() {
        if(Instance) {
            if(Instance != this) {
                Destroy(Instance.gameObject);
            }
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if(IsLoading) {
            OnProgressUpdated?.Invoke(async.progress / 0.9f);
        }
    }

    public static void LoadScene(int index) {
        if(Instance) {
            Instance.async = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
            Instance.async.completed += Instance.OnSceneLoaded;
            Instance.OnLoadStarted?.Invoke();
            Instance.IsLoading = true;
            Instance.gameObject.SetActive(true);
        } else {
            SceneManager.LoadScene(index);
        }
    }

    private void OnSceneLoaded(AsyncOperation obj) {
        OnProgressUpdated?.Invoke(1f);
        IsLoading = false;
        OnLoadFinish?.Invoke();
        Instance.gameObject.SetActive(false);
    }
}
