﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class Weapon : MonoBehaviour {

    [Range(0f, 12f)]
    [SerializeField] private float fireRate = Mathf.PI;
    [Min(1)]
    [SerializeField] private int bulletsToSpawn = 1;
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

    public LayerMask BulletLayerMask {
        get {
            return bulletLayerMask;
        }
    }

    public void Use() {
        if(CanBeUsed) {
            timeToNextUse = Time.time + ( 1f / fireRate );
            OnWeaponUse?.Invoke();
            for(int i = 0; i < bulletsToSpawn; i++) {
                activeBullets.Add(new Bullet(this));
            }
        }
    }

    private void Awake() {
        if(bulletVfxPrefab) {
            bulletVfxPool = new ObjectPool<Transform>(OnVfxCreate, OnVfxGet, OnVfxRelease, OnVfxDestroy, false);
        }
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
