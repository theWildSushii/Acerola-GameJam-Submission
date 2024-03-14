using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class Weapon : MonoBehaviour {

    [Range(0f, 12f)]
    [SerializeField] private float fireRate = Mathf.PI;
    [Min(1)]
    [SerializeField] private int bulletsToSpawn = 1;
    [SerializeField] private int ammoCapacity = 10;
    [SerializeField] private int ammoReloaded = 10;
    [SerializeField] private bool emptyBulletsOnReload = true;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private float projectileSpeed = 400f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float spread = 0f;
    [SerializeField] private float maxOptimalRange = 10f;
    [SerializeField] private LayerMask bulletLayerMask;
    [Min(1)]
    [SerializeField] private int physicsSubsteps = 1;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform bulletVfxPrefab;
    [SerializeField] private ParticleSystem defaultDamageFX;
    [SerializeField] private FloatingAudio defaultDamageSFX;

    [SerializeField] private UnityEvent OnWeaponUse;
    [SerializeField] private UnityEvent OnWeaponReload;
    [SerializeField] private UnityEvent<string> OnWeaponAmmoCounterUpdated;

    [SerializeField] private UnityEvent<bool> OnGraphicsEnabledChanged;
    [SerializeField] private UnityEvent OnGraphicsEnable;
    [SerializeField] private UnityEvent OnGraphicsDisable;

    private float timeToNextUse = 0f;

    private ObjectPool<Transform> bulletVfxPool;
    private List<Bullet> activeBullets = new List<Bullet>();

    private int currentAmmo = 10;
    private int fireHash = 0;

    public bool CanBeUsed {
        get {
            return Time.time >= timeToNextUse && currentAmmo > 0;
        }
    }

    public bool IsReloading { get; protected set; } = false;

    public float MaxOptimalRange {
        get {
            return maxOptimalRange;
        }
    }

    public float Spread {
        get {
            return spread;
        }
        set {
            spread = Mathf.Clamp01(value);
        }
    }

    public float FireRate {
        get {
            return fireRate;
        }
        set {
            fireRate = Mathf.Clamp(value, 0f, 12f);
        }
    }

    public LayerMask BulletLayerMask {
        get {
            return bulletLayerMask;
        }
    }

    public bool GraphicsEnabled {
        set {
            OnGraphicsEnabledChanged?.Invoke(value);
            if(value) {
                OnGraphicsEnable?.Invoke();
            } else {
                OnGraphicsDisable?.Invoke();
            }
        }
    }

    private void Awake() {
        currentAmmo = ammoCapacity;
        if(bulletVfxPrefab) {
            bulletVfxPool = new ObjectPool<Transform>(OnVfxCreate, OnVfxGet, OnVfxRelease, OnVfxDestroy, false);
        }
        fireHash = Animator.StringToHash("Fire");
    }

    private void OnEnable() {
        UpdateAmmoCounter();
    }

    private void OnDisable() {
        StopAllCoroutines();
        IsReloading = false;
    }

    RaycastHit2D hit;
    private void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime / physicsSubsteps;
        float stepSize = projectileSpeed * deltaTime;
        for(int step = 0; step < physicsSubsteps; step++) {
            foreach(Bullet bullet in activeBullets) {

                if(!bullet.active) {
                    continue;
                }

                hit = Physics2D.Raycast(bullet.position, bullet.direction, stepSize, bulletLayerMask);

                if(hit.collider) {
                    bullet.position.x = hit.point.x;
                    bullet.position.y = hit.point.y;
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
                    bullet.active = false;
                    continue;
                } else {
                    bullet.position.x += bullet.direction.x * stepSize;
                    bullet.position.y += bullet.direction.y * stepSize;
                }

                bullet.lifetime -= deltaTime;
                if(bullet.lifetime <= 0f) {
                    bullet.active = false;
                }
            }
        }
    }

    private void LateUpdate() {
        for(int i = 0; i < activeBullets.Count; i++) {
            if(activeBullets[i].vfxTransform) {
                activeBullets[i].vfxTransform.position = activeBullets[i].position;
            }
            if(!activeBullets[i].active) {
                if(bulletVfxPool != null) {
                    bulletVfxPool.Release(activeBullets[i].vfxTransform);
                }
                activeBullets.RemoveAt(i--);
            }
        }
    }

    public void Use() {
        if(CanBeUsed) {
            timeToNextUse = Time.time + ( 1f / fireRate );
            StopAllCoroutines();
            IsReloading = false;
            currentAmmo--;
            UpdateAmmoCounter();
            for(int i = 0; i < bulletsToSpawn; i++) {
                activeBullets.Add(new Bullet(this));
            }
            if(currentAmmo <= 0) {
                Reload();
            }
            OnWeaponUse?.Invoke();
            if(anim) {
                anim.SetTrigger(fireHash);
            }
        } else {
            if(currentAmmo <= 0 && !IsReloading) {
                Reload();
            }
        }
    }

    public void UpdateAmmoCounter() {
        OnWeaponAmmoCounterUpdated?.Invoke(currentAmmo + "/" + ammoCapacity);
    }

    public void Reload() {
        if(IsReloading) {
            return;
        }
        if(currentAmmo < ammoCapacity) {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine() {
        IsReloading = true;
        OnWeaponReload?.Invoke();
        if(emptyBulletsOnReload) {
            currentAmmo = 0;
            UpdateAmmoCounter();
        }
        while(currentAmmo < ammoCapacity) {
            yield return new WaitForSeconds(reloadTime);
            currentAmmo += ammoReloaded;
            UpdateAmmoCounter();
        }
        currentAmmo = ammoCapacity;
        UpdateAmmoCounter();
        IsReloading = false;
    }

    private Transform OnVfxCreate() {
        return Instantiate(bulletVfxPrefab);
    }

    private void OnVfxGet(Transform vfxTransform) {
        vfxTransform.position = transform.position;
        vfxTransform.gameObject.SetActive(true);
    }

    private void OnVfxRelease(Transform vfxTransform) {
        StartCoroutine(ReleaseDelay(vfxTransform));
    }

    private IEnumerator ReleaseDelay(Transform vfxTransform) {
        yield return null;
        vfxTransform.gameObject.SetActive(false);
    }

    private void OnVfxDestroy(Transform vfxTransform) {
        Destroy(vfxTransform.gameObject);
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if(!anim) {
            anim = GetComponent<Animator>();
        }
    }
#endif

    public class Bullet {
        public Vector3 position = Vector3.zero;
        public Vector2 direction = Vector2.up;
        public float lifetime = 1f;
        public Transform vfxTransform;
        public bool active = true;

        public Bullet(Weapon weapon) {
            position = weapon.transform.position;
            direction = Vector2.Lerp(weapon.transform.up, Random.insideUnitCircle.normalized, Random.value * weapon.spread * 0.25f);
            lifetime = weapon.lifetime;
            if(weapon.bulletVfxPool != null) {
                vfxTransform = weapon.bulletVfxPool.Get();
                vfxTransform.position = position;
            }
            active = true;
        }
    }

}
