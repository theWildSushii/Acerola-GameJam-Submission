using UnityEngine;

public class GirlController : MonoBehaviour {

    [SerializeField] private Actor actor;
    [SerializeField] private Actor playerActor;
    [SerializeField] private Animator anim;

    private int horrorHash = 0;
    private bool isHorrified = false;

    private void Awake() {
        horrorHash = Animator.StringToHash("Horror");
    }

    private void OnEnable() {
        playerActor.OnActorAlive.AddListener(OnPlayerAlive);
        playerActor.OnActorDeath.AddListener(OnPlayerDeath);
    }

    private void OnDisable() {
        playerActor.OnActorAlive.RemoveListener(OnPlayerAlive);
        playerActor.OnActorDeath.RemoveListener(OnPlayerDeath);
    }

    private void FixedUpdate() {
        if(!isHorrified) {
            actor.MoveTowards(playerActor.Position - (Vector2)playerActor.transform.up, 1f, false);
            actor.AimTowards(playerActor.Position, Time.fixedDeltaTime);
        } else {
            actor.Move(Vector2.zero);
        }
    }

    private void OnPlayerAlive(Actor target) {
        isHorrified = false;
        anim.SetBool(horrorHash, false);
    }

    private void OnPlayerDeath(Actor target) {
        isHorrified = true;
        anim.SetBool(horrorHash, true);
    }

}
