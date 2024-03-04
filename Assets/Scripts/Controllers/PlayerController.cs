using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { get; protected set; }

    [SerializeField] private Actor actor;
    [SerializeField] private Transform crosshair;
    [SerializeField] private float jumpDuration = 0.382f;
    [SerializeField] private float jumpRate = Mathf.PI;

    public UnityEvent<float> OnNormalizedHP;
    public UnityEvent<bool> OnAimChanged;

    private Vector2 movement = Vector3.zero;
    private float aim = 0f;
    private Vector2 currentAim = Vector2.zero;
    private Vector2 aimVelocity = Vector2.zero;
    private Vector2 targetAim = Vector2.zero;
    private float speedMultiplier = 1f;
    private float aimMultiplier = 1f;
    private float jumpFactor = 0f;
    private Vector2 jumpDirection = Vector2.up;
    private float timeToNextJump = 0f;

    public Vector2 Position {
        get {
            return actor.Position;
        }
    }

    private void Awake() {
        Instance = this;
    }

    void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update() {
        if(actor.DamageableOnSight(out targetAim)) {
            actor.Attack();
            aimMultiplier = 0.5f;
        } else {
            aimMultiplier = 1f;
        }
    }

    private void FixedUpdate() {
        if(jumpFactor > 0f) {
            actor.Move(jumpDirection * Mathf.PI, Space.Self);
            jumpFactor -= Time.deltaTime;
        } else {
            actor.Move(movement * speedMultiplier, Space.Self);
            actor.Rotate(aim * Time.fixedDeltaTime * speedMultiplier * aimMultiplier);
        }
        currentAim = Vector2.SmoothDamp(currentAim, targetAim, ref aimVelocity, 0.08333f, 128f, Time.fixedDeltaTime);
        //crosshair.position = currentAim;
    }

    public void OnMove(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context) {
        if(context.control.device == Pointer.current) {
            aim = ( context.ReadValue<Vector2>().x / Screen.width ) * 32f;
        } else {
            aim = context.ReadValue<Vector2>().x;
        }
    }

    public void OnFire(InputAction.CallbackContext context) {
        if(context.performed && jumpFactor <= 0f) {
            actor.Weapon.Use();
        }
    }

    public void OnPause(InputAction.CallbackContext context) {
        if(context.performed) {
            //TODO toggle pause
        }
    }

    bool previousAimValue = false;
    bool isAiming = false;
    public void OnAim(InputAction.CallbackContext context) {
        isAiming = context.ReadValueAsButton();
        if(previousAimValue != isAiming) {
            OnAimChanged?.Invoke(isAiming);
            speedMultiplier = isAiming ? 0.618f : 1f;
        }
        previousAimValue = isAiming;
    }

    public void OnJump(InputAction.CallbackContext context) {
        if(context.performed && Time.time >= timeToNextJump) {
            timeToNextJump = Time.time + ( 1f / jumpRate );
            jumpFactor = jumpDuration;
            if(movement.IsZero()) {
                jumpDirection = Vector2.up;
            } else {
                jumpDirection = movement.normalized;
            }
        }
    }

    public void OnDamaged(Actor actor) {
        OnNormalizedHP?.Invoke(actor.Stats.NormalizedHP);
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!actor) {
            actor = GetComponent<Actor>();
        }
    }
#endif

}
