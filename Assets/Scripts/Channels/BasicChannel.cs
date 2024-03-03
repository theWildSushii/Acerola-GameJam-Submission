using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Basic Channel", menuName = "Channels/Basic")]
public class BasicChannel : ScriptableObject {

    public event UnityAction OnTrigger;

    public void FireEvent() {
        OnTrigger?.Invoke();
    }

}