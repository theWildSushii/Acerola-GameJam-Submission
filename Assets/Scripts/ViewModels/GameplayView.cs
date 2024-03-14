using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class GameplayView : MonoBehaviour {

    [SerializeField] private GameplayModel gameplayModel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Volume gameOverVolume;
    [SerializeField] private UnityEvent<string> OnClockUpdated;
    [SerializeField] private UnityEvent<string> OnScoreStringUpdated;
    [SerializeField] private UnityEvent<string> OnBestClockUpdated;
    [SerializeField] private UnityEvent<string> OnBestScoreStringUpdated;
    [SerializeField] private UnityEvent OnGameOverComplete;

    private bool updateTime = false;

    private void Start() {
        gameplayModel.GetBestScores();
    }

    private void OnEnable() {
        gameplayModel.currentTime.OnValueChanged += OnTimeUpdated;
        gameplayModel.score.OnValueChanged += OnScoreUpdated;
        gameplayModel.bestTime.OnValueChanged += OnBestTimeUpdated;
        gameplayModel.bestScore.OnValueChanged += OnBestScoreUpdated;
    }

    private void OnDisable() {
        gameplayModel.currentTime.OnValueChanged -= OnTimeUpdated;
        gameplayModel.score.OnValueChanged -= OnScoreUpdated;
        gameplayModel.bestTime.OnValueChanged -= OnBestTimeUpdated;
        gameplayModel.bestScore.OnValueChanged -= OnBestScoreUpdated;
    }

    private void Update() {
        if(updateTime) {
            gameplayModel.currentTime.Value += Time.deltaTime;
        }
    }

    private void OnScoreUpdated(float value) {
        OnScoreStringUpdated?.Invoke(Mathf.FloorToInt(value).ToString());
    }

    private void OnTimeUpdated(float value) {
        TimeSpan t = TimeSpan.FromSeconds(value);
        string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);
        OnClockUpdated?.Invoke(answer);
    }

    private void OnBestScoreUpdated(float value) {
        OnBestScoreStringUpdated?.Invoke(Mathf.FloorToInt(value).ToString());
    }

    private void OnBestTimeUpdated(float value) {
        TimeSpan t = TimeSpan.FromSeconds(value);
        string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);
        OnBestClockUpdated?.Invoke(answer);
    }

    public void OnGameplay() {
        gameplayModel.ResetScores();
        updateTime = true;
    }

    public void OnGameOver() {
        updateTime = false;
        gameplayModel.UpdateScores();
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine() {
        float velocity = 0f;
        while(!Mathf.Approximately(canvasGroup.alpha, 1f) || canvasGroup.alpha >= 1f) {
            canvasGroup.alpha = Mathf.SmoothDamp(canvasGroup.alpha, 1f, ref velocity, Mathf.PI / 24f, 343f, Time.deltaTime);
            gameOverVolume.weight = canvasGroup.alpha;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        gameOverVolume.weight = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        OnGameOverComplete?.Invoke();
    }

}
