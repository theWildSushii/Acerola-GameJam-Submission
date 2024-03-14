using UnityEngine;

public class RandomMaterials : MonoBehaviour {

    [SerializeField] private Renderer target;
    [SerializeField] private MaterialArray[] materials;

    private void OnEnable() {
        if(target) {
            target.sharedMaterials = materials.Random().materials;
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!target) {
            target = GetComponent<Renderer>();
        }
    }
#endif

    [System.Serializable]
    public struct MaterialArray {
        public Material[] materials;
    }

}
