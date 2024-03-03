using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GradientFillAmount : MonoBehaviour {

    public Gradient gradient;
    public Image img;

    private void OnGUI() {
        if(img) {
            img.color = gradient.Evaluate(img.fillAmount);
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!img) {
            img = GetComponent<Image>();
        }
    }
#endif

}
