using UnityEngine;

public class Rotator : MonoBehaviour {

    [SerializeField] private Vector3 angularSpeed = Vector3.zero;
    [SerializeField] private Space space = Space.Self;

    private void Awake() {
        transform.Rotate(angularSpeed * (Random.value * 360f), space);
    }

    void Update() {
        transform.Rotate(angularSpeed * Time.deltaTime, space);
    }
}
