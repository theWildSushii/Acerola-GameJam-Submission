using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Gameplay Model", menuName = "View Models/Gameplay")]
public class GameplayModel : ScriptableObject {

    public LiveData<float> currentTime = new LiveData<float>(0f);
    public LiveData<float> score = new LiveData<float>(0f);

    public LiveData<float> bestTime = new LiveData<float>(0f);
    public LiveData<float> bestScore = new LiveData<float>(0f);

    public void ResetScores() {
        currentTime.Value = 0f;
        score.Value = 0f;
    }

    public void GetBestScores() {
        bestTime.Value = PlayerPrefs.GetFloat("BestTime", 0f);
        bestScore.Value = PlayerPrefs.GetFloat("BestScore", 0f);
    }

    public void UpdateScores() {
        if(currentTime.Value > bestTime.Value) {
            bestTime.Value = currentTime.Value + 0f;
            PlayerPrefs.SetFloat("BestTime", bestTime.Value);
        }
        if(score.Value > bestScore.Value) {
            bestScore.Value = score.Value + 0f;
            PlayerPrefs.SetFloat("BestScore", bestScore.Value);
        }
    }

    public void RestartThisScene() {
        LoadingScreen.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu() {
        LoadingScreen.LoadScene(1);
    }

}
