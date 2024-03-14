using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventsHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, ISelectHandler, ICancelHandler, ISubmitHandler, IMoveHandler {

    [SerializeField] private FloatingAudio navigationAudio;
    [SerializeField] private FloatingAudio submitAudio;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private UnityEvent OnCancelTrigger;

    private RectTransform rectTransform;
    private bool scroll = true;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        if(!scrollRect) {
            scrollRect = GetComponentInParent<ScrollRect>();
        }
    }

    public void OnCancel(BaseEventData eventData) {
        OnCancelTrigger?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        navigationAudio.Play();
        scroll = false;
    }

    public void OnSelect(BaseEventData eventData) {
        navigationAudio.Play();
        if(scroll) {
            if(scrollRect) {
                ScrollToCenter(scrollRect, rectTransform);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        submitAudio.Play();
    }

    public void OnPointerExit(PointerEventData eventData) {
        scroll = true;
    }

    public void OnSubmit(BaseEventData eventData) {
        submitAudio.Play();
    }

    public void OnMove(AxisEventData eventData) {
        navigationAudio.Play();
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!scrollRect) {
            scrollRect = GetComponentInParent<ScrollRect>();
        }
    }
#endif

    static readonly Vector3[] Corners = new Vector3[4];
    private static Bounds TransformBoundsTo(RectTransform source, Transform target) {
        Bounds bounds = new Bounds();
        if(source != null) {
            source.GetWorldCorners(Corners);
            Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Matrix4x4 matrix = target.worldToLocalMatrix;
            for(int j = 0; j < 4; j++) {
                Vector3 v = matrix.MultiplyPoint3x4(Corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }
            bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
        }
        return bounds;
    }

    private static float NormalizeScrollDistance(ScrollRect scrollRect, int axis, float distance) {
        RectTransform viewport = scrollRect.viewport;
        RectTransform viewRect = viewport != null ? viewport : scrollRect.GetComponent<RectTransform>();
        Rect rect = viewRect.rect;
        Bounds viewBounds = new Bounds(rect.center, rect.size);
        RectTransform content = scrollRect.content;
        Bounds contentBounds = content != null ? TransformBoundsTo(content, viewRect) : new Bounds();
        float hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
        return distance / hiddenLength;
    }

    private static void ScrollToCenter(ScrollRect scrollRect, RectTransform target) {
        RectTransform view = scrollRect.viewport != null ? scrollRect.viewport : scrollRect.GetComponent<RectTransform>();
        Rect viewRect = view.rect;
        Bounds elementBounds = TransformBoundsTo(target, view);
        float offset = viewRect.center.y - elementBounds.center.y;
        float scrollPos = scrollRect.verticalNormalizedPosition - NormalizeScrollDistance(scrollRect, 1, offset);
        scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollPos, 0f, 1f);
    }
}