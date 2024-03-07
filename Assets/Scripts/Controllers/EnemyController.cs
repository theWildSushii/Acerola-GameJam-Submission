using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Actor))]
public class EnemyController : MonoBehaviour {

    public static List<EnemyController> enemies = new List<EnemyController>();

    public static float CurrentStress { get; protected set; } = 0f;

    public static void CommandAttackAt(Vector2 target) {
        foreach(EnemyController enemy in enemies) {
            if(enemy.ai.CurrentState != enemy.combat) {
                //Vector2 targetDirection = target - enemy.actor.Position;
                //if(targetDirection.sqrMagnitude <= enemy.sightRadiusSqr) {
                    enemy.SearchPoint(target);
                //}
            }
        }
    }

    [SerializeField] private Actor actor;
    [SerializeField] private float idleTime = 1f;
    [SerializeField] private float searchMoveTime = 1f;
    [SerializeField] private float searchingTime = 10f;
    [SerializeField] private float evadeTime = 1.5f;
    [SerializeField] private float sightRadius = 10f;
    [SerializeField] private float stressContribution = 1f;

    private PrivateStateMachine ai;
    private PrivateStateMachine.State idle;
    private PrivateStateMachine.State combat;
    private PrivateStateMachine.State searching;

    private float timeToNextMove = 0f;
    private Vector2 targetPoint = Vector2.zero;
    private Vector2 playerOffset = Vector2.zero;
    private Vector2 lastKnownPlayerPosition = Vector2.zero;
    private Vector2 targetAim = Vector2.zero;
    private Vector2 playerDirection = Vector2.zero;
    private float playerSqrDistance = 0f;
    private float sightRadiusSqr = 100f;
    private float weaponRadiusSqr = 100f;
    private float searchTimer = 0f;

    public ObjectPool<EnemyController> ParentPool { protected get; set; }

    public float StressContribution {
        get {
            return stressContribution;
        }
    }

    public Actor Actor {
        get {
            return actor;
        }
    }

    private void Awake() {
        idle = new PrivateStateMachine.State(onUpdate: IdleUpdate);
        combat = new PrivateStateMachine.State(onUpdate: CombatUpdate);
        searching = new PrivateStateMachine.State(onEnter: SearchingEnter, onUpdate: SearchingUpdate);
        ai = new PrivateStateMachine(idle);
        sightRadiusSqr = sightRadius * sightRadius;
        if(actor.Weapon) {
            weaponRadiusSqr = actor.Weapon.MaxOptimalRange * actor.Weapon.MaxOptimalRange;
        } else {
            weaponRadiusSqr = sightRadius * sightRadius;
        }
    }

    private void OnEnable() {
        targetPoint = actor.Position;
        targetAim = targetPoint;
        enemies.Add(this);
        CurrentStress += stressContribution;
    }

    private void OnDisable() {
        enemies.Remove(this);
        CurrentStress -= stressContribution;
    }

    private void Update() {
        ai.Update();
    }

    private void FixedUpdate() {
        actor.MoveTowards(targetPoint);
        actor.AimTowards(targetAim, Time.fixedDeltaTime);
    }

    private void IdleUpdate() {
        if(Time.time >= timeToNextMove) {
            timeToNextMove = Time.time + idleTime;
            targetPoint = actor.Position + ( Random.insideUnitCircle * actor.Weapon.MaxOptimalRange );
        }
        targetAim = targetPoint;
        if(CheckIfPlayerIsInSight()) {
            lastKnownPlayerPosition = PlayerController.Instance.Position;
            ai.CurrentState = combat;
            CommandAttackAt(PlayerController.Instance.Position);
        }
    }

    private void CombatUpdate() {
        if(Time.time >= timeToNextMove) {
            timeToNextMove = Time.time + evadeTime;
            playerOffset = Random.insideUnitCircle * actor.Weapon.MaxOptimalRange;
        }
        if(CheckIfPlayerIsInSight()) {
            lastKnownPlayerPosition = PlayerController.Instance.Position;
            targetAim = PlayerController.Instance.Position;
        } else {
            targetAim = targetPoint;
            CommandAttackAt(lastKnownPlayerPosition);
            ai.CurrentState = searching;
        }
        if(actor.DamageableOnSight()) {
            actor.Attack();
            targetPoint = actor.Position;
            return;
        }
        targetPoint = lastKnownPlayerPosition + playerOffset;
    }

    public void SearchPoint(Vector2 point) {
        targetPoint = point;
        lastKnownPlayerPosition = point;
        ai.CurrentState = searching;
    }

    private void SearchingEnter() {
        searchTimer = searchingTime;
    }

    private void SearchingUpdate() {
        if(Time.time >= timeToNextMove) {
            timeToNextMove = Time.time + searchMoveTime;
            targetPoint = lastKnownPlayerPosition + ( Random.insideUnitCircle * sightRadius );
        }
        targetAim = targetPoint;
        if(CheckIfPlayerIsInSight()) {
            lastKnownPlayerPosition = PlayerController.Instance.Position;
            ai.CurrentState = combat;
            CommandAttackAt(PlayerController.Instance.Position);
        }
        searchTimer -= Time.deltaTime;
        if(searchTimer <= 0f) {
            ai.CurrentState = idle;
        }
    }

    RaycastHit2D hit;
    float relativeSightRadius = 10f;
    float sightDot = 1f;
    private bool CheckIfPlayerIsInSight() {
        playerDirection = PlayerController.Instance.Position - actor.Position;
        playerSqrDistance = playerDirection.sqrMagnitude;
        playerDirection.Normalize();
        if(playerSqrDistance > weaponRadiusSqr) {
            sightDot = Vector2.Dot(playerDirection, actor.transform.up);
            if(sightDot <= 0f) {
                return false;
            }
            relativeSightRadius = Mathf.Lerp(0f, sightRadius, sightDot);
        } else {
            relativeSightRadius = sightRadius;
        }
        if(playerSqrDistance <= sightRadiusSqr) {
            hit = Physics2D.Raycast(actor.Position, playerDirection, relativeSightRadius, actor.SightLayerMask);
            if(hit.collider) {
                if(hit.collider.TryGetComponent(out PlayerController player)) {
                    return true;
                }
            }
        }
        return false;
    }

    public void OnDeath() {
        ParentPool.Release(this);
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!actor) {
            actor = GetComponent<Actor>();
        }
    }
#endif


}
