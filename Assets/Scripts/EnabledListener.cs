using UnityEngine;
using UnityEngine.Events;

public class EnabledListener : MonoBehaviour {

    [SerializeField] private UnityEvent<bool> OnEnabledChanged;
    [SerializeField] private UnityEvent OnEnableTrigger;
    [SerializeField] private UnityEvent OnDisableTrigger;

    private void OnEnable() {
        OnEnabledChanged?.Invoke(true);
        OnEnableTrigger?.Invoke();
    }

    private void OnDisable() {
        OnEnabledChanged?.Invoke(false);
        OnDisableTrigger?.Invoke();
    }

}
