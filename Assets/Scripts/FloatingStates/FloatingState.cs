using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Floating State", menuName = "Floating States/Floating State")]
public class FloatingState : ScriptableObject {

    protected static Dictionary<int, FloatingState> currentStates = new Dictionary<int, FloatingState>();

    [Tooltip("Allows multiple concurrent floating states to be active, calling EnterState() only replaces the state with the same channel")]
    [SerializeField] private int channel = 0;

    public event UnityAction OnStateEnter;

    public event UnityAction OnStateExit;

    public bool IsActive {
        get {
            if(currentStates.ContainsKey(channel)) {
                return currentStates[channel] == this;
            }
            return false;
        }
    }

    public void EnterState() {
        if(currentStates.ContainsKey(channel)) {
            currentStates[channel].OnStateExit?.Invoke();
            currentStates[channel] = this;
        } else {
            currentStates.Add(channel, this);
        }
        OnStateEnter?.Invoke();
    }

}
