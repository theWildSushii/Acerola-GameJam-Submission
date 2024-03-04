using System.Collections.Generic;
using UnityEngine;

public static class Extensions {

    public static T Random<T>(this List<T> list) {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    //public static T PopRandom<T>(this List<T> list) {
    //    T item = list.Random();
    //    list.Remove(item);
    //    return item;
    //}

    // Fisher-Yates shuffle algorithm
    public static void Shuffle<T>(this List<T> list) {
        for(int i = list.Count - 1; i > 0; i--) {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    // Fisher-Yates shuffle algorithm
    public static void Shuffle<T>(this T[] array) {
        for(int i = array.Length - 1; i > 0; i--) {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public static T Random<T>(this T[] array) {
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static float Remap(this float value, float rangemin, float rangemax, float targetmin, float targetmax) {
        return targetmin + ( value - rangemin ) * ( targetmax - targetmin ) / ( rangemax - rangemin );
    }

    public static int ToInt(this bool value) {
        return value ? 1 : 0;
    }

    public static bool ToBool(this int value) {
        return value > 0;
    }

    public static void AddForce(this Rigidbody2D rb, Vector2 force, ForceMode forceMode = ForceMode.Force) {
        switch(forceMode) {
            case ForceMode.Force:
                rb.AddForce(force, ForceMode2D.Force);
                break;
            case ForceMode.Acceleration:
                rb.AddForce(force * rb.mass, ForceMode2D.Force);
                break;
            case ForceMode.Impulse:
                rb.AddForce(force, ForceMode2D.Impulse);
                break;
            case ForceMode.VelocityChange:
                rb.AddForce(force * rb.mass / Time.fixedDeltaTime, ForceMode2D.Force);
                break;
        }
    }

    public static void AddRelativeForce(this Rigidbody2D rb, Vector2 force, ForceMode forceMode = ForceMode.Force) {
        switch(forceMode) {
            case ForceMode.Force:
                rb.AddRelativeForce(force, ForceMode2D.Force);
                break;
            case ForceMode.Acceleration:
                rb.AddRelativeForce(force * rb.mass, ForceMode2D.Force);
                break;
            case ForceMode.Impulse:
                rb.AddRelativeForce(force, ForceMode2D.Impulse);
                break;
            case ForceMode.VelocityChange:
                rb.AddRelativeForce(force * rb.mass / Time.fixedDeltaTime, ForceMode2D.Force);
                break;
        }
    }

    public static float Range(this Vector2 range, float t) {
        return Mathf.Lerp(range.x, range.y, t);
    }

    public static Color GetComputedColor(this Light light) {
        if(light.useColorTemperature) {
            return Mathf.CorrelatedColorTemperatureToRGB(light.colorTemperature).linear * light.color.linear * light.intensity;
        }
        return light.color.linear * light.intensity;
    }

    public static bool IsZero(this Vector2 value) {
        return Mathf.Approximately(value.x, 0f) && Mathf.Approximately(value.y, 0f);
    }

    public static bool IsZero(this Vector3 value) {
        return Mathf.Approximately(value.x, 0f) && Mathf.Approximately(value.y, 0f) && Mathf.Approximately(value.z, 0f);
    }
}