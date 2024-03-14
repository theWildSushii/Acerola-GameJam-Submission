using UnityEngine;

public class Puppet : MonoBehaviour {

    [SerializeField] private Actor target;
    [Range(0f, 1f)]
    public float speedMultiplier = 1f;

    private void FixedUpdate() {
        if(target) {
            target.MoveTowards(transform.position, speedMultiplier);
            target.AimTowards(transform.position + transform.up, speedMultiplier);
        }
    }

}
