using UnityEngine;

[CreateAssetMenu(fileName = "Main Menu Model", menuName = "View Models/Main Menu")]
public class MainMenuModel : ScriptableObject {

    public void LoadMainMenu() {
        LoadingScreen.LoadScene(1);
    }

    public void LoadCampaign() {
        
    }

    public void LoadEndless() {
        LoadingScreen.LoadScene(2);
    }

    public void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
