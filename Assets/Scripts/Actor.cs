using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Actor : MonoBehaviour, IDamageable {

    protected static List<Actor> actors = new List<Actor>();

    [SerializeField] private ActorStats stats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Weapon weapon;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem damageFX;
    [SerializeField] private FloatingAudio damageSFX;
    [SerializeField] private LayerMask sightMask;

    public UnityEvent<Actor> OnActorAlive;
    public UnityEvent<Actor> OnActorDamaged;
    public UnityEvent<Actor> OnActorDeath;

    private int moveZHash = 0;
    private int moveXHash = 0;
    private int deadHash = 0;

    public Vector2 Position {
        get {
            if(rb) {
                return rb.position;
            }
            return transform.position;
        }
        set {
            if(rb) {
                rb.position = value;
            }
            transform.position = value;
        }
    }

    public ActorStats Stats {
        get {
            return stats;
        }
    }

    public Weapon Weapon {
        get {
            return weapon;
        }
    }

    public LayerMask SightLayerMask {
        get {
            return sightMask;
        }
    }

    private void Awake() {
        moveZHash = Animator.StringToHash("move.z");
        moveXHash = Animator.StringToHash("move.x");
        deadHash = Animator.StringToHash("dead");
    }

    private void Start() {
        if(anim) {
            anim.SetBool(deadHash, false);
        }
    }

    private void OnEnable() {
        if(damageFX) {
            damageFX.transform.parent = transform;
        }
        stats.currentHP = stats.baseHP;
        OnActorAlive?.Invoke(this);
        actors.Add(this);
        if(anim) {
            anim.SetFloat(moveZHash, 0.0f);
            anim.SetFloat(moveXHash, 0.0f);
            anim.SetBool(deadHash, false);
        }
    }

    private void OnDisable() {
        actors.Remove(this);
    }

    Vector2 currentAnimationMovement = Vector2.zero;
    Vector2 animationMovementVelocity = Vector2.zero;
    private void Update() {
        if(anim) {
            currentAnimationMovement = Vector2.SmoothDamp(currentAnimationMovement, movement, ref animationMovementVelocity, 0.04166f);
            anim.SetFloat(moveXHash, currentAnimationMovement.x);
            anim.SetFloat(moveZHash, currentAnimationMovement.y);
        }
    }

    Ray2D sightRay;
    float aimDistance = 7.5f;
    public bool DamageableOnSight(out Vector2 hitPoint) {
        if(!enabled) {
            hitPoint = Position;
            return false;
        }
        sightRay = new Ray2D(rb.position, rb.transform.up);
        aimDistance = weapon ? weapon.MaxOptimalRange : 7.5f;
        RaycastHit2D hit = Physics2D.Raycast(sightRay.origin, sightRay.direction, aimDistance, sightMask);
        if(hit.collider) {
            hitPoint = hit.point;
            if(hit.collider.TryGetComponent<IDamageable>(out _)) {
                return true;
            }
        } else {
            hitPoint = sightRay.origin + ( sightRay.direction * aimDistance );
        }
        return false;
    }

    public bool DamageableOnSight() {
        return DamageableOnSight(out _);
    }

    Vector2 movement = Vector2.zero;
    Vector2 targetAcceleration = Vector2.zero;
    public void Move(Vector2 direction, Space space = Space.Self) {
        if(!enabled) {
            return;
        }
        switch(space) {
            case Space.World:
                movement = transform.InverseTransformVector(direction);
                targetAcceleration = ( Stats.moveSpeed * direction ) - rb.velocity;
                targetAcceleration /= Time.fixedDeltaTime;
                targetAcceleration = Vector2.ClampMagnitude(targetAcceleration, Stats.moveAcceleration);
                rb.AddForce(targetAcceleration, ForceMode.Acceleration);
                break;
            default:
            case Space.Self:
                movement = direction;
                targetAcceleration = ( Stats.moveSpeed * direction ) - (Vector2)rb.transform.InverseTransformVector(rb.velocity);
                targetAcceleration /= Time.fixedDeltaTime;
                targetAcceleration = Vector2.ClampMagnitude(targetAcceleration, Stats.moveAcceleration);
                rb.AddRelativeForce(targetAcceleration, ForceMode.Acceleration);
                break;
        }
    }

    Vector2 targetDirection = Vector3.zero;
    public void MoveTowards(Vector2 destination) {
        targetDirection = destination - Position;
        targetDirection = Vector2.ClampMagnitude(targetDirection, 1f);
        Move(targetDirection, Space.World);
    }

    public void Rotate(float ammount) {
        if(!enabled) {
            return;
        }
        rb.MoveRotation(rb.rotation - ( ammount * stats.aimSpeed ));
    }

    Vector2 direction = Vector2.up;
    float rotateAmmount = 0f;
    public void AimTowards(Vector2 targetPoint, float deltaTime = 1f) {
        if(!enabled) {
            return;
        }
        direction = ( targetPoint - Position ).normalized;
        rotateAmmount = Vector3.Cross(direction, transform.up).z;
        Rotate(rotateAmmount * 300f * deltaTime);
    }

    public bool CanAttack {
        get {
            if(weapon) {
                return weapon.CanBeUsed;
            }
            return false;
        }
    }

    public void Attack() {
        if(!enabled) {
            return;
        }
        if(weapon) {
            weapon.Use();
        }
    }

    public void Reload() {
        if(!enabled) {
            return;
        }
        if(weapon) {
            weapon.Reload();
        }
    }

    public void ApplyDamage(float ammount) {
        stats.currentHP -= ammount;
        OnActorDamaged?.Invoke(this);
        if(stats.currentHP <= 0f) {
            stats.currentHP = 0f;
            if(damageFX) {
                damageFX.transform.parent = transform.parent;
            }
            if(anim) {
                anim.SetBool(deadHash, true);
            }
            if(rb) {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
            OnActorDeath?.Invoke(this);
            enabled = false;
        }
    }

    public void PlayDamageFX(Vector3 point, Vector2 normal) {
        if(damageFX) {
            damageFX.transform.position = point;
            damageFX.transform.up = normal;
            damageFX.Play();
        }
        if(damageSFX) {
            damageSFX.Play(point);
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!rb) {
            rb = GetComponent<Rigidbody2D>();
        }
    }
#endif

}