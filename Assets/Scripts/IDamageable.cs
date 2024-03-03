using UnityEngine;

public interface IDamageable {

    public void ApplyDamage(float ammount);

    public void PlayDamageFX(Vector3 point, Vector2 normal);

}