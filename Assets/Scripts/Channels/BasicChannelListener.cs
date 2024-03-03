using UnityEngine;
using UnityEngine.Events;

public class BasicChannelListener : MonoBehaviour {

    [SerializeField] private BasicChannel channel;

    [SerializeField] private UnityEvent OnTrigger;

    private void OnEnable() {
        if(!channel) {
            return;
        }
        channel.OnTrigger += OnTrigger.Invoke;
    }

    private void OnDisable() {
        if(!channel) {
            return;
        }
        channel.OnTrigger -= OnTrigger.Invoke;
    }

}