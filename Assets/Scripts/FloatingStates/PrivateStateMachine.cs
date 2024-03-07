using UnityEngine.Events;

public class PrivateStateMachine {

    private State currentState;

    public State CurrentState {
        get {
            return currentState;
        }
        set {
            if(currentState != null) {
                currentState.OnExit?.Invoke();
            }
            currentState = value;
            if(currentState != null) {
                currentState.OnEnter?.Invoke();
            }
        }
    }

    public PrivateStateMachine(State initialState = null) {
        CurrentState = initialState;
    }

    public void Update() {
        CurrentState?.OnUpdate?.Invoke();
    }

    public class State {
        public UnityAction OnEnter;
        public UnityAction OnUpdate;
        public UnityAction OnExit;

        public State(UnityAction onEnter = null, UnityAction onExit = null, UnityAction onUpdate = null) {
            OnEnter = onEnter;
            OnUpdate = onUpdate;
            OnExit = onExit;
        }
    }

}