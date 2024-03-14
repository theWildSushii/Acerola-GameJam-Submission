using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SliderText : MonoBehaviour {

    [SerializeField] private Slider slider;

    [SerializeField] private UnityEvent<string> OnTextUpdate;

    //private void OnEnable() {
    //    slider.onValueChanged.AddListener(OnSliderUpdate);
    //}

    //private void OnDisable() {
    //    slider.onValueChanged.RemoveListener(OnSliderUpdate);
    //}

    private void OnGUI() {
        if(slider) {
            float percentage = Mathf.Round(slider.value * 100f);
            OnTextUpdate?.Invoke(percentage + "%");
        }
    }

    //private void OnSliderUpdate(float value) {
    //    float percentage = Mathf.Round(value * 100f);
    //    OnTextUpdate?.Invoke(percentage + "%");
    //}

#if UNITY_EDITOR
    private void OnValidate() {
        if(!slider) {
            slider = GetComponent<Slider>();
        }
    }
#endif

}
