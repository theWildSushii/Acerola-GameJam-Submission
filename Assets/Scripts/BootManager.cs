using UnityEngine;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour {

    private void Start() {
        LoadingScreen.LoadScene(1);
    }

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init() {
        SceneManager.LoadScene(0);
    }
#endif

}
