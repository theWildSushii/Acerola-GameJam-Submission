using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraSettings : MonoBehaviour {

    private static List<CameraSettings> cameras = new List<CameraSettings>();

    public static void UpdateAllCameras() {
        foreach(CameraSettings camera in cameras) {
            camera.UpdateSettings();
        }
    }

    [SerializeField] private Camera target;
    [SerializeField] private UniversalAdditionalCameraData additionalData;

    private void LateUpdate() {
        target.backgroundColor = RenderSettings.ambientSkyColor;
    }

    private void  OnEnable() {
        cameras.Add(this);
        UpdateSettings();
    }

    private void OnDisable() {
        cameras.Remove(this);
    }

    private void UpdateSettings() {
        switch(SettingsManager.AntiAliasingQuality) {
            default:
            case 0: // Off
                additionalData.antialiasing = AntialiasingMode.None;
                additionalData.antialiasingQuality = AntialiasingQuality.Low;
                break;
            case 1: // Low FXAA 
                additionalData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                additionalData.antialiasingQuality = AntialiasingQuality.Low;
                break;
            case 2: // Medium
                additionalData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                additionalData.antialiasingQuality = AntialiasingQuality.Low;
                break;
            case 3: // High
                additionalData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                additionalData.antialiasingQuality = AntialiasingQuality.Medium;
                break;
            case 4: // Very High
                additionalData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                additionalData.antialiasingQuality = AntialiasingQuality.High;
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!target) {
            target = GetComponent<Camera>();
        }
        if(!additionalData) {
            additionalData = GetComponent<UniversalAdditionalCameraData>();
        }
    }
#endif

}
