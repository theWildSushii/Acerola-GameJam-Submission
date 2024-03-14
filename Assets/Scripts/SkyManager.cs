using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class SkyManager : MonoBehaviour {

    [SerializeField] private float sunIntensity = 76.10925f;
    [SerializeField] private Vector2 sunTemperatureRange = new Vector2(4100f, 6500f);
    [SerializeField] private Light sun;
    [SerializeField] private Light moon;
    [SerializeField] private Color skyColor = Color.white;
    [SerializeField] private Volume sunVolume;

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

        float sunDot = Vector3.Dot(sun.transform.forward, Vector3.forward);
        sun.intensity = sunIntensity * Mathf.Clamp01(sunDot);
        if(sunDot > 0f) {
            sun.enabled = true;
            sun.colorTemperature = sunTemperatureRange.Range(sunDot);
            sunVolume.weight = sunDot;
        } else {
            sun.enabled = false;
            sun.colorTemperature = sunTemperatureRange.Range(0f);
            sunVolume.weight = 0f;
        }

        Light activeLight = ActiveLight;
        Color ambientColor = (activeLight.GetComputedColor() * skyColor.linear * 0.2f).gamma;
        RenderSettings.ambientLight = ambientColor;
        RenderSettings.ambientGroundColor = ambientColor;
        RenderSettings.ambientEquatorColor = ambientColor;
        RenderSettings.ambientSkyColor = ambientColor;
    }

}
