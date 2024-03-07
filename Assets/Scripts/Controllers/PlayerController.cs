using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { get; protected set; }

    [SerializeField] private Actor actor;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Canvas crosshairCanvas;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private float jumpDuration = 0.382f;
    [SerializeField] private float jumpRate = Mathf.PI;
    [Range(0f, 12f)]
    [SerializeField] private float normalFireRate = Mathf.PI;
    [Range(0f, 12f)]
    [SerializeField] private float autoFireFireRate = 8f;
    [Range(0f, 1f)]
    [SerializeField] private float normalWeaponSpread = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float autoFireWeaponSpread = 0.5f;

    public UnityEvent<float> OnNormalizedHP;

    private Vector2 movement = Vector3.zero;
    private float aim = 0f;
    private Vector3 currentAim = Vector3.zero;
    private Vector2 targetAim = Vector2.zero;
    private Vector2 aimVelocity = Vector2.zero;
    private float aimMultiplier = 1f;
    private float jumpFactor = 0f;
    private Vector2 jumpDirection = Vector2.up;
    private float timeToNextJump = 0f;
    private bool isAutoFiring = false;

    public Actor Actor {
        get {
            return actor;
        }
    }

    public Vector2 Position {
        get {
            return actor.Position;
        }
    }

    private bool IsCurrentDeviceMouse {
        get {
            return playerInput.currentControlScheme == "Keyboard&Mouse";
        }
    }

    private float DeltaTimeMultiplier {
        get {
            return IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
        }
    }

    private void Awake() {
        Instance = this;
        isAutoFiring = false;
        Actor.Weapon.Spread = normalWeaponSpread;
        Actor.Weapon.FireRate = normalFireRate;
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
        if(isAutoFiring && jumpFactor <= 0f) {
            actor.Attack();
        }
    }

    private void FixedUpdate() {
        if(jumpFactor > 0f) {
            actor.Move(jumpDirection * Mathf.PI, Space.Self);
            jumpFactor -= Time.deltaTime;
        } else {
            actor.Move(movement, Space.Self);
            actor.Rotate(aim * DeltaTimeMultiplier * aimMultiplier);
        }
        if(actor.DamageableOnSight(out targetAim)) {
            aimMultiplier = 0.382f;
        } else {
            aimMultiplier = 1f;
        }
        currentAim = Vector2.SmoothDamp(currentAim, targetAim, ref aimVelocity, 0.041666f, 343f, Time.fixedUnscaledDeltaTime);
        //currentAim = targetAim;
        currentAim.z = actor.Weapon.transform.position.z;
        crosshair.anchoredPosition = crosshairCanvas.WorldToCanvasPosition(currentAim, playerInput.camera);
    }

    public void OnMove(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context) {
        aim = context.ReadValue<Vector2>().x;
    }

    public void OnFire(InputAction.CallbackContext context) {
        if(context.performed && jumpFactor <= 0f) {
            actor.Attack();
        }
    }

    public void OnAutoFire(InputAction.CallbackContext context) {
        isAutoFiring = context.ReadValueAsButton();
        if(isAutoFiring) {
            Actor.Weapon.Spread =  autoFireWeaponSpread;
            Actor.Weapon.FireRate = autoFireFireRate;
        } else {
            Actor.Weapon.Spread = normalWeaponSpread;
            Actor.Weapon.FireRate = normalFireRate;
        }
    }

    public void OnReload(InputAction.CallbackContext context) {
        if(context.performed) {
            actor.Reload();
        }
    }

    public void OnPause(InputAction.CallbackContext context) {
        if(context.performed) {
            //TODO toggle pause
        }
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
