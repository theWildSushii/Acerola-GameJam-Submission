using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { get; protected set; }

    [SerializeField] private Actor actor;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform gfxRoot;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Canvas crosshairCanvas;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private Volume damageVolume;
    [SerializeField] private float regenerationRate = 0.5f;
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

    private Vector2 movement = Vector3.zero;
    private float aim = 0f;
    private Vector3 currentAim = Vector3.zero;
    private Vector2 targetAim = Vector2.zero;
    private Vector2 aimVelocity = Vector2.zero;
    private float aimMultiplier = 1f;
    private Vector2 movementMultiplier = Vector2.one;
    private float jumpFactor = 0f;
    private Vector2 jumpDirection = Vector2.up;
    private Vector2 jumpVelocity = Vector2.zero;
    private Vector2 currentGfxDirection = Vector2.up;
    private Vector2 targetGfxDirection = Vector2.up;
    private float timeToNextJump = 0f;
    private bool isAutoFiring = false;
    private float mouseSensitivity = 1f;
    private float gamepadSensitivity = 1f;
    private float aimAssistMultiplier = 1f;
    private float movementAimAssistMultiplier = 1f;

    private int RollHash = 0;

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
        RollHash = Animator.StringToHash("Roll");
    }

    void OnEnable() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateControlSettings();
    }

    void OnDisable() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update() {
        if(isAutoFiring && jumpFactor <= 0f) {
            actor.Attack();
        }
        currentGfxDirection = Vector2.SmoothDamp(currentGfxDirection, targetGfxDirection, ref jumpVelocity, 0.08333f);
        gfxRoot.localRotation = Quaternion.LookRotation(currentGfxDirection, Vector3.back);
        actor.Stats.HP += regenerationRate * Time.deltaTime;
        damageVolume.weight = 1f - actor.Stats.NormalizedHP;
    }

    private void FixedUpdate() {
        if(actor.DamageableOnSight(out targetAim)) {
            aimMultiplier = aimAssistMultiplier;
            movementMultiplier.x = movementAimAssistMultiplier;
        } else if(actor.DamageableOnSight(1.618f)) {
            aimMultiplier = aimAssistMultiplier;
            movementMultiplier.x = movementAimAssistMultiplier;
        } else {
            aimMultiplier = 1f;
            movementMultiplier.x = 1f;
        }
        if(jumpFactor > 0f) {
            actor.Move(jumpDirection * Mathf.PI, Space.Self);
            jumpFactor -= Time.deltaTime;
            if(jumpFactor <= 0f) {
                targetGfxDirection = Vector2.up;
                actor.Weapon.GraphicsEnabled = true;
            }
        } else {
            actor.Move(movement * movementMultiplier, Space.Self);
        }
        actor.Rotate(aim * DeltaTimeMultiplier * aimMultiplier);
        currentAim = Vector2.SmoothDamp(currentAim, targetAim, ref aimVelocity, 0.041666f, 343f, Time.fixedUnscaledDeltaTime);
        //currentAim = targetAim;
        currentAim.z = actor.Weapon.transform.position.z;
        crosshair.anchoredPosition = crosshairCanvas.WorldToCanvasPosition(currentAim, playerInput.camera);
    }

    public void OnMove(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context) {
        if(context.control.device == Mouse.current) {
            aim = context.ReadValue<Vector2>().x * mouseSensitivity;
        } else {
            aim = context.ReadValue<Vector2>().x * gamepadSensitivity;
        }
    }

    public void OnFire(InputAction.CallbackContext context) {
        if(!enabled) {
            return;
        }
        if(context.performed && jumpFactor <= 0f) {
            actor.Attack();
        }
    }

    public void OnAutoFire(InputAction.CallbackContext context) {
        isAutoFiring = context.ReadValueAsButton();
        if(isAutoFiring) {
            Actor.Weapon.Spread = autoFireWeaponSpread;
            Actor.Weapon.FireRate = autoFireFireRate;
        } else {
            Actor.Weapon.Spread = normalWeaponSpread;
            Actor.Weapon.FireRate = normalFireRate;
        }
    }

    public void OnReload(InputAction.CallbackContext context) {
        if(!enabled) {
            return;
        }
        if(context.performed) {
            actor.Reload();
        }
    }

    public void OnPause(InputAction.CallbackContext context) {
        if(!enabled) {
            return;
        }
        if(context.performed) {
            PauseManager.Instance.TogglePause();
        }
    }

    public void OnPauseTrigger() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnResumeTrigger() {
        if(enabled) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context) {
        if(!enabled) {
            return;
        }
        if(context.performed && Time.time >= timeToNextJump) {
            timeToNextJump = Time.time + ( 1f / jumpRate );
            jumpFactor = jumpDuration;
            actor.Weapon.GraphicsEnabled = false;
            anim.SetTrigger(RollHash);
            if(movement.IsZero()) {
                jumpDirection = Vector2.up;
            } else {
                jumpDirection = movement.normalized;
            }
            targetGfxDirection = jumpDirection;
        }
    }

    public void UpdateControlSettings() {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1f);
        gamepadSensitivity = PlayerPrefs.GetFloat("GamepadSensitivity", 1f);
        float aimAssistIntensity = PlayerPrefs.GetFloat("AimAssistIntensity", 0.5f);
        aimAssistMultiplier = Mathf.Lerp(1f, 0.15f, aimAssistIntensity);
        movementAimAssistMultiplier = Mathf.Lerp(1f, 0.618f, aimAssistIntensity);
        movementMultiplier.y = 1f;
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!actor) {
            actor = GetComponent<Actor>();
        }
    }
#endif

}
