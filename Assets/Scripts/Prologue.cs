using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Prologue : MonoBehaviour {

    [TextArea]
    [SerializeField] private List<string> dialogs;
    [SerializeField] private float smoothTime = Mathf.PI;
    [SerializeField] private CanvasGroup group;

    [SerializeField] private UnityEvent<string> OnTextUpdate;
    [SerializeField] private UnityEvent OnCompleted;

    private int currentDialogIndex = 0;
    private float currentLength = 0f;
    private float targetLength = 0f;
    private float velocity = 0f;
    private bool isCurrentTextFinish = false;
    private string currentString = "";
    private string targetString = "";

    private void Start() {
        if(PlayerPrefs.GetInt("ShowPrologue", 1) > 0) {
            currentDialogIndex = 0;
            currentString = string.Empty;
            targetString = dialogs[0];
            velocity = 0f;
            currentLength = 0f;
            targetLength = (float)targetString.Length;
        } else {
            OnCompleted?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void Update() {
        if(!isCurrentTextFinish) {
            currentLength = Mathf.SmoothDamp(currentLength, targetLength, ref velocity, smoothTime, 343f);
            if(Mathf.Ceil(currentLength) < targetLength) {
                currentString = "";
                for(int i = 0; i < currentLength; i++) {
                    currentString += targetString[i];
                }
            } else {
                isCurrentTextFinish = true;
                currentString = targetString;
            }
            OnTextUpdate?.Invoke(currentString);
        }
    }

    public void OnSubmit(InputAction.CallbackContext context) {
        if(context.performed) {
            if(isCurrentTextFinish) {
                if(++currentDialogIndex >= dialogs.Count) {
                    Completed();
                } else {
                    currentString = string.Empty;
                    targetString = dialogs[currentDialogIndex];
                    isCurrentTextFinish = false;
                    currentLength = 0f;
                    velocity = 0f;
                    targetLength = (float)targetString.Length;
                }
            } else {
                isCurrentTextFinish = true;
                currentLength = targetLength;
                currentString = targetString;
                OnTextUpdate?.Invoke(currentString);
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context) {
        if(context.performed) {
            Completed();
        }
    }

    private void Completed() {
        PlayerPrefs.SetInt("ShowPrologue", 0);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut() {
        yield return null;
        OnCompleted?.Invoke();
        gameObject.SetActive(false);
    }

}
