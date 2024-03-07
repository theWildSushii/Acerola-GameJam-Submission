using UnityEngine;
using UnityEngine.Events;

public class FloatingStateListener : MonoBehaviour {

    [SerializeField] private FloatingState state;
    [SerializeField] private bool enterOnEnable = false;
    [SerializeField] private UnityEvent<bool> OnStateChanged;
    [SerializeField] private UnityEvent OnStateEnter;
    [SerializeField] private UnityEvent OnStateExit;

    private void OnEnable() {
        if(state) {
            if(OnStateChanged != null) {
                state.OnStateChanged += OnStateChanged.Invoke;
            }
            if(OnStateExit != null) {
                state.OnStateExit += OnStateExit.Invoke;
            }
            if(OnStateEnter != null) {
                state.OnStateEnter += OnStateEnter.Invoke;
                if(state.IsActive) {
                    OnStateEnter.Invoke();
                }
            }
            if(enterOnEnable) {
                state.EnterState();
            }
        }
    }

    private void OnDisable() {
        if(state) {
            if(OnStateChanged != null) {
                state.OnStateChanged -= OnStateChanged.Invoke;
            }
            if(OnStateEnter != null) {
                state.OnStateEnter -= OnStateEnter.Invoke;
            }
            if(OnStateExit != null) {
                state.OnStateExit -= OnStateExit.Invoke;
            }
        }
    }

    private void Start() {
        if(!enterOnEnable) {
            if(state) {
                if(state.IsActive) {
                    OnStateEnter?.Invoke();
                }
            }
        }
    }

}
