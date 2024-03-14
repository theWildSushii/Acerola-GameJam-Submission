using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour {

    [SerializeField] private Color ColorA = Color.white;
    [SerializeField] private Color ColorB = Color.black;
    [SerializeField] private float smoothTime = 2f / 24f;

    private Light target;
    private float currentFactor = 0f;
    private float velocity = 0f;

    private void Awake() {
        target = GetComponent<Light>();
    }

    private void Update() {
        currentFactor = Mathf.SmoothDamp(currentFactor, Random.value, ref velocity, smoothTime, 343f, Time.deltaTime);
        target.color = Color.Lerp(ColorA.linear, ColorB.linear, currentFactor).gamma;
    }

}
