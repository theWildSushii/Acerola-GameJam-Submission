using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{

    [SerializeField] private Light sun;
    [SerializeField] private Light moon;
    [SerializeField] private Color skyColor = Color.white;

    private Light ActiveLight {
        get {
            if(sun.isActiveAndEnabled) {
                if(sun.intensity > moon.intensity) {
                    return sun;
                }
                return moon;
            }
            return moon;
        }
    }

    private void LateUpdate() {
        Light activeLight = ActiveLight;
    }



}
