using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraSettings : MonoBehaviour {

    [SerializeField] private Camera target;
    [SerializeField] private UniversalAdditionalCameraData additionalData;

    private void LateUpdate() {
        target.backgroundColor = RenderSettings.ambientSkyColor;
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
