using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    private static List<SpawnPoint> points = new List<SpawnPoint>();

    private void OnEnable() {
        points.Add(this);
    }

    private void OnDisable() {
        points.Remove(this);
    }

    public static Vector2 RandomPoint {
        get {
            points.Sort((a, b) => {
                float sqrDistanceA = ( PlayerController.Instance.Position - (Vector2)a.transform.position ).sqrMagnitude;
                float sqrDistanceB = ( PlayerController.Instance.Position - (Vector2)b.transform.position ).sqrMagnitude;
                return -sqrDistanceA.CompareTo(sqrDistanceB);
            });
            float random = Random.value;
            random = random * random * random;
            return (Vector2)points[Mathf.FloorToInt(random * ( points.Count - 1 ))].transform.position;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
#endif

}
