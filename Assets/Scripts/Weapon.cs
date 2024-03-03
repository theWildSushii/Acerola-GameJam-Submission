using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class Weapon : MonoBehaviour {

    [Range(0f, 12f)]
    [SerializeField] private float fireRate = Mathf.PI;
    [SerializeField] private float projectileSpeed = 400f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float spread = 0f;
    [SerializeField] private float maxOptimalRange = 10f;
    [SerializeField] private LayerMask bulletLayerMask;
    [Min(1)]
    [SerializeField] private int physicsSubsteps = 1;
    [SerializeField] private Transform bulletVfxPrefab;
    [SerializeField] private ParticleSystem defaultDamageFX;
    [SerializeField] private FloatingAudio defaultDamageSFX;

    [SerializeField] private UnityEvent OnWeaponUse;

    private float timeToNextUse = 0f;

    private ObjectPool<Transform> bulletVfxPool;
    private List<Bullet> activeBullets = new List<Bullet>();

    public bool CanBeUsed {
        get {
            return Time.time >= timeToNextUse;
        }
    }

    public float MaxOptimalRange {
        get {
            return maxOptimalRange;
        }
    }

    public void Use() {
        if(CanBeUsed) {
            timeToNextUse = Time.time + ( 1f / fireRate );
            OnWeaponUse?.Invoke();
            activeBullets.Add(new Bullet(this));
        }
    }

    private void Awake() {
        if(bulletVfxPrefab) {
            bulletVfxPool = new ObjectPool<Transform>(OnVfxCreate, OnVfxGet, OnVfxRelease, OnVfxDestroy, false);
        }
    }

    Bullet activeBullet;
    RaycastHit2D hit;
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime / physicsSubsteps;
        for(int step = 0; step < physicsSubsteps; step++) {
            for(int i = 0; i < activeBullets.Count; i++) {
                activeBullet = activeBullets[i];

                hit = Physics2D.Raycast(activeBullet.position, activeBullet.direction, projectileSpeed * deltaTime, bulletLayerMask);

                if(hit.collider) {
                    if(hit.collider.TryGetComponent(out IDamageable damageable)) {
                        damageable.ApplyDamage(damage);
                        damageable.PlayDamageFX(hit.point, hit.normal);
                    } else {
                        if(defaultDamageFX) {
                            defaultDamageFX.transform.position = hit.point;
                            defaultDamageFX.transform.up = hit.normal;
                            defaultDamageFX.Play();
                        }
                        if(defaultDamageSFX) {
                            defaultDamageSFX.Play(hit.point);
                        }
                    }

                    if(activeBullet.vfxTransform) {
                        bulletVfxPool.Release(activeBullet.vfxTransform);
                    }
                    activeBullets.RemoveAt(i--);
                    continue;
                }

                activeBullet.lifetime -= deltaTime;
                if(activeBullet.lifetime <= 0f) {
                    if(activeBullet.vfxTransform) {
                        bulletVfxPool.Release(activeBullet.vfxTransform);
                    }
                    activeBullets.RemoveAt(i--);
                }
            }
        }
    }

    private void LateUpdate() {
        if(bulletVfxPool != null) {
            foreach(Bullet bullet in activeBullets) {
                bullet.vfxTransform.position = bullet.position;
            }
        }
    }

    private Transform OnVfxCreate() {
        return Instantiate(bulletVfxPrefab);
    }

    private void OnVfxGet(Transform vfxTransform) {
        vfxTransform.position = transform.position;
        vfxTransform.gameObject.SetActive(true);
    }

    private void OnVfxRelease(Transform vfxTransform) {
        vfxTransform.gameObject.SetActive(false);
    }

    private void OnVfxDestroy(Transform vfxTransform) {
        Destroy(vfxTransform.gameObject);
    }

    public class Bullet {
        public Vector3 position;
        public Vector2 direction;
        public float lifetime;
        public Transform vfxTransform;

        public Bullet(Weapon weapon) {
            position = weapon.transform.position;
            direction = Vector2.Lerp(weapon.transform.up, Random.insideUnitCircle.normalized, Random.value * weapon.spread * 0.25f);
            lifetime = weapon.lifetime;
            if(weapon.bulletVfxPool != null) {
                vfxTransform = weapon.bulletVfxPool.Get();
                vfxTransform.position = position;
            }
        }
    }

}
