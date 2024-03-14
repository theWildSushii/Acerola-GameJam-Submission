using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class LiveData<T> {

    [SerializeField] private T _value;

    public event UnityAction<T> OnValueChanged;

    public LiveData() {
        Value = default(T);
    }

    public LiveData(T defaultValue) {
        Value = defaultValue;
    }

    public T Value {
        get {
            return _value;
        }
        set {
            _value = value;
            OnValueChanged?.Invoke(value);
        }
    }

    public void SendValue() {
        OnValueChanged?.Invoke(Value);
    }

}