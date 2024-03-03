﻿using UnityEngine;
using UnityEngine.Pool;

public class FloatingAudioManager : MonoBehaviour {

    private static FloatingAudioManager _instance;

    public static FloatingAudioManager Instance {
        get {
            if(!_instance) {
                GameObject managerObject = new GameObject("Floating Audio Manager");
                _instance = managerObject.AddComponent<FloatingAudioManager>();
            }
            return _instance;
        }
        protected set {
            _instance = value;
        }
    }

    public static FloatingAudioInstance NextInstance {
        get {
            return Instance.pool.Get();
        }
    }

    public static void Release(FloatingAudioInstance instance) {
        Instance.pool.Release(instance);
    }

    private ObjectPool<FloatingAudioInstance> pool;

    private void Awake() {
        if(Instance) {
            if(Instance != this) {
                Destroy(gameObject);
                return;
            }
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        pool = new ObjectPool<FloatingAudioInstance>(OnCreate, OnGet, OnRelease, OnDestroy, false);
    }

    private FloatingAudioInstance OnCreate() {
        GameObject created = new GameObject("Floating Audio");
        created.transform.parent = transform;
        created.transform.localPosition = Vector3.zero;
        created.transform.localRotation = Quaternion.identity;
        created.SetActive(false);
        AudioSource source = created.AddComponent<AudioSource>();
        FloatingAudioInstance instance = created.AddComponent<FloatingAudioInstance>();
        instance.source = source;
        DontDestroyOnLoad(created);
        return instance;
    }

    private void OnGet(FloatingAudioInstance instance) {
        instance.gameObject.SetActive(true);
    }

    private void OnRelease(FloatingAudioInstance instance) {
        instance.source.Stop();
        instance.gameObject.SetActive(false);
        instance.transform.parent = transform;
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
    }

    private static void OnDestroy(FloatingAudioInstance instance) {
        Destroy(instance.gameObject);
    }

}
