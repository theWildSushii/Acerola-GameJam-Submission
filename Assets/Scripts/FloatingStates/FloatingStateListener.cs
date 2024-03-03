using UnityEngine;
using UnityEngine.Events;

public class FloatingStateListener : MonoBehaviour {

    [SerializeField] private FloatingState state;
    [SerializeField] private bool enterOnEnable = false;
    [SerializeField] private UnityEvent OnStateEnter;
    [SerializeField] private UnityEvent OnStateExit;

    private void OnEnable() {
        if(state) {
            if(OnStateEnter != null) {
                state.OnStateEnter += OnStateEnter.Invoke;
                if(state.IsActive) {
                    OnStateEnter.Invoke();
                }
            }
            if(OnStateExit != null) {
                state.OnStateExit += OnStateExit.Invoke;
            }
            if(enterOnEnable) {
                state.EnterState();
            }
        }
    }

    private void OnDisable() {
        if(state) {
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
