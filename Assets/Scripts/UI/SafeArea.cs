using UnityEngine;

[ExecuteInEditMode]
public class SafeArea : MonoBehaviour {

    [SerializeField] private RectTransform rectTransform;

    private Rect safeArea;
    private Vector2 minAnchor;
    private Vector2 maxAnchor;

    bool ignoreCallback = false;

    private void OnRectTransformDimensionsChange() {
        if(ignoreCallback) {
            ignoreCallback = false;
            return;
        }
        ResizeToFit();
    }

    void OnEnable() {
        ResizeToFit();
    }

    public void ResizeToFit() {
        if(!rectTransform) {
            return;
        }
        safeArea = Screen.safeArea;
        minAnchor = safeArea.position;
        maxAnchor = minAnchor + safeArea.size;

        minAnchor.x /= Screen.width;
        minAnchor.y /= Screen.height;
        maxAnchor.x /= Screen.width;
        maxAnchor.y /= Screen.height;

#if UNITY_EDITOR
        if(float.IsNaN(minAnchor.x) ||
           float.IsNaN(minAnchor.y) ||
           float.IsNaN(maxAnchor.x) ||
           float.IsNaN(maxAnchor.y) ||
           float.IsInfinity(minAnchor.x) ||
           float.IsInfinity(minAnchor.y) ||
           float.IsInfinity(maxAnchor.x) ||
           float.IsInfinity(maxAnchor.y)) {

            ignoreCallback = true;
            rectTransform.anchorMin = Vector2.zero;
            ignoreCallback = true;
            rectTransform.anchorMax = Vector2.one;
            return;
        }
#endif
        ignoreCallback = true;
        rectTransform.anchorMin = minAnchor;
        ignoreCallback = true;
        rectTransform.anchorMax = maxAnchor;
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!rectTransform) {
            rectTransform = GetComponent<RectTransform>();
        }
    }
#endif
}