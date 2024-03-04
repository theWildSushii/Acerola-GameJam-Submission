using UnityEngine;

[ExecuteInEditMode]
public class SkyManager : MonoBehaviour {

    [SerializeField] private Light sun;
    [SerializeField] private Light moon;
    [SerializeField] private Color skyColor = Color.white;

    private Light ActiveLight {
        get {
            if(sun.isActiveAndEnabled) {
                if(sun.intensity > moon.intensity) {
                    return sun;
                }
                return moon;
            }
            return moon;
        }
    }

    private void Update() {
        Light activeLight = ActiveLight;
        Color ambientColor = (activeLight.GetComputedColor() * skyColor.linear * 0.2f).gamma;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientGroundColor = ambientColor;
        RenderSettings.ambientEquatorColor = ambientColor;
        RenderSettings.ambientSkyColor = ambientColor;
    }



}
