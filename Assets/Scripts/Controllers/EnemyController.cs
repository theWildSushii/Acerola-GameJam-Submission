using UnityEngine;

[RequireComponent(typeof(Actor))]
public class EnemyController : MonoBehaviour {

    [SerializeField] private Actor actor;
    [SerializeField] private float searchRate = 1f;
    [SerializeField] private float evadeRate = 1.5f;
    [SerializeField] private float sightRadius = 10f;

    private bool playerInSight = false;
    private bool playerWasInSight = false;
    private bool playerInAttackRange = false;
    private float timeToNextMovement = 0f;
    private Vector2 targetPoint = Vector2.zero;
    private Vector2 targetRelativePoint = Vector2.zero;

    private void Awake() {
        targetPoint = actor.Position;
    }

    Vector2 playerDirection = Vector2.zero;
    private void Update() {
        if(Time.time >= timeToNextMovement) {
            if(playerWasInSight) {
                timeToNextMovement = Time.time + ( 1f / evadeRate );
                targetPoint = PlayerController.Instance.Position
                    + ( Random.insideUnitCircle
                    * ( actor.Weapon ? actor.Weapon.MaxOptimalRange : sightRadius ) );
                targetRelativePoint = ( Random.insideUnitCircle
                    * ( actor.Weapon ? actor.Weapon.MaxOptimalRange : sightRadius ) );
            } else {
                timeToNextMovement = Time.time + ( 1f / searchRate );
                targetPoint = actor.Position + ( Random.insideUnitCircle * sightRadius );
            }
        }
        if(playerInSight && actor.DamageableOnSight()) {
            playerInAttackRange = true;
            actor.Weapon.Use();
        } else {
            playerInAttackRange = false;
        }
        playerDirection = PlayerController.Instance.Position - actor.Position;
        if(playerDirection.sqrMagnitude <= sightRadius * sightRadius) {
            playerDirection.Normalize();
            float dot = Vector2.Dot(playerDirection, actor.transform.up);
            if(dot > 0f) {
                RaycastHit2D hit = Physics2D.Raycast(actor.Position, playerDirection, sightRadius * dot, actor.Weapon.BulletLayerMask);
                if(hit.collider) {
                    if(hit.collider.TryGetComponent(out PlayerController player)) {
                        playerInSight = true;
                    }
                }
            } else {
                playerInSight = false;
            }
        } else {
            playerInSight = false;
            playerWasInSight = false;
        }
    }

    Vector2 direction = Vector2.zero;
    private void FixedUpdate() {
        if(playerInSight) {
            if(!playerInAttackRange) {
                direction = ( PlayerController.Instance.Position + targetRelativePoint ) - actor.Position;
                direction = Vector2.ClampMagnitude(direction, 1f);
                actor.Move(direction, Space.World);
            }
            actor.AimTowards(PlayerController.Instance.Position, Time.fixedDeltaTime);
        } else {
            direction = targetPoint - actor.Position;
            direction = Vector2.ClampMagnitude(direction, 1f);
            actor.Move(direction, Space.World);
            actor.AimTowards(targetPoint, Time.fixedDeltaTime);
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!actor) {
            actor = GetComponent<Actor>();
        }
    }
#endif


}
