using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour {

    private void OnEnable() {
        StartCoroutine(Deactivate());
    }

    private IEnumerator Deactivate() {
        yield return null;
        gameObject.SetActive(false);
    }

}
