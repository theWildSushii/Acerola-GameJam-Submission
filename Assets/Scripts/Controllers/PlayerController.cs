using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public static PlayerController Instance { get; protected set; }

    [SerializeField] private Actor actor;
    [SerializeField] private Transform crosshair;

    public UnityEvent<float> OnNormalizedHP;

    private Vector2 movement = Vector3.zero;
    private float aim = 0f;
    private Vector2 currentAim = Vector2.zero;
    private Vector2 aimVelocity = Vector2.zero;
    private Vector2 targetAim = Vector2.zero;

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
        }
    }

    private void FixedUpdate() {
        actor.Move(movement, Space.Self);
        actor.Rotate(aim * Time.fixedDeltaTime);
        currentAim = Vector2.SmoothDamp(currentAim, targetAim, ref aimVelocity, 0.08333f, 128f, Time.fixedDeltaTime);
        //crosshair.position = currentAim;
    }

    public void OnMove(InputAction.CallbackContext context) {
        movement = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context) {
        if(context.control.device == Pointer.current) {
            aim = ( context.ReadValue<Vector2>().x / Screen.width ) * 32f;
        } else {
            aim = context.ReadValue<Vector2>().x;
        }
    }

    public void OnFire(InputAction.CallbackContext context) {
        if(context.ReadValueAsButton()) {
            actor.Weapon.Use();
        }
    }

    public void OnPause(InputAction.CallbackContext context) {
        if(context.ReadValueAsButton()) {
            //TODO toggle pause
        }
    }

    public void OnDamaged(Actor actor) {
        OnNormalizedHP?.Invoke(actor.Stats.NormalizedHP);
    }

}
