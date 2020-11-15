using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {
    // --------------------
    // LISTS
    // --------------------
    
    public static T Random<T> (this List<T> target) {
        if (target == null || target.Count == 0) return default;
        return target[UnityEngine.Random.Range(0, target.Count)];
    }

    
    // --------------------
    // MATH
    // --------------------

    public static float Clamp(this float target, float min, float max) {
        return Mathf.Clamp(target, min, max);
    }

    public static float Clamp01(this float target) {
        return Mathf.Clamp01(target);
    }

    public static float Lerp(this float x, float a, float b) {
        return Mathf.Lerp(a, b, x);
    }

    public static float Prel(this float x, float a, float b) {
        return (x - a) / (b - a);
    }

    public static float Remap(this float target, float oldA, float oldB, float newA, float newB) {
        return Mathf.Lerp(newA, newB, target.Prel(oldA, oldB));
    }

    
    // --------------------
    // VECTOR2
    // --------------------

    public static Quaternion Slerp(this Quaternion obj, Quaternion target, float smoothness = 2) {
        return Quaternion.Slerp(obj, target, smoothness / 100);
    }

    
    // --------------------
    // VECTOR2
    // --------------------

    public static Vector2 Clamp(this Vector2 target, float min, float max) {
        return new Vector2(
            target.x.Clamp(min, max),
            target.y.Clamp(min, max));
    }

    
    // --------------------
    // VECTOR3
    // --------------------

    public static Vector3 Clamp(this Vector3 target, float min, float max) {
        return new Vector3(
            target.x.Clamp(min, max),
            target.y.Clamp(min, max),
            target.z.Clamp(min, max));
    }

    
    // --------------------
    // QUATERNION
    // --------------------

    public static Quaternion RandomWithin(this Quaternion obj, float range) {
        return obj * Quaternion.Euler(
            UnityEngine.Random.Range(-range, range), 
            UnityEngine.Random.Range(-range, range), 
            0);
    }

    
    // --------------------
    // RANDOM
    // --------------------

    public static T Random<T>(T t1, T t2) {
        return UnityEngine.Random.Range(0, 2) == 0 ? t1 : t2;
    }

    public static T Random<T>(T t1, T t2, T t3) {
        int i = UnityEngine.Random.Range(0, 3);
        if (i == 0) return t1;
        else if (i == 1) return t2;
        else return t3;
    }

    
    // --------------------
    // TRANSFORM
    // --------------------

    public static void LerpTo(this Transform obj, Vector3 target, float smoothness = 2) {
        obj.position = Vector3.Lerp(obj.position, target, smoothness / 100);
    }

    public static void LerpTo(this GameObject obj, Vector3 target, float smoothness = 2) {
        obj.transform.LerpTo(target, smoothness);
    }

    public static void LerpTo(this Transform obj, Transform target, float smoothness = 2) {
        obj.position = Vector3.Lerp(obj.position, target.position, smoothness / 100);
    }

    public static void LerpTo(this GameObject obj, Transform target, float smoothness = 2) {
        obj.transform.LerpTo(target, smoothness);
    }

    public static void LerpTo(this Transform obj, GameObject target, float smoothness = 2) {
        obj.position = Vector3.Lerp(obj.position, target.transform.position, smoothness / 100);
    }

    public static void LerpTo(this GameObject obj, GameObject target, float smoothness = 2) {
        obj.transform.LerpTo(target, smoothness);
    }

    public static void SlerpTo(this Transform obj, Quaternion target, float smoothness = 2) {
        obj.rotation = Quaternion.Slerp(obj.rotation, target, smoothness / 100);
    }

    public static void SlerpTo(this GameObject obj, Quaternion target, float smoothness = 2) {
        obj.transform.SlerpTo(target, smoothness);
    }

    public static void LocalLerpTo(this Transform obj, Vector3 target, float smoothness = 2) {
        obj.localPosition = Vector3.Lerp(obj.localPosition, target, smoothness / 100);
    }

    public static void LocalLerpTo(this GameObject obj, Vector3 target, float smoothness = 2) {
        obj.transform.LocalLerpTo(target, smoothness);
    }

    public static void LocalSlerpTo(this Transform obj, Quaternion target, float smoothness = 2) {
        obj.localRotation = Quaternion.Slerp(obj.localRotation, target, smoothness / 100);
    }

    public static void LocalSlerpTo(this GameObject obj, Quaternion target, float smoothness = 2) {
        obj.transform.LocalSlerpTo(target, smoothness);
    }

    public static void RotateTowards(this Transform obj, Quaternion target, float distance) {
        obj.rotation = Quaternion.RotateTowards(obj.rotation, target, distance);
    }

    public static void RotateTowards(this GameObject obj, Quaternion target, float distance) {
        obj.transform.RotateTowards(target, distance);
    }

    
    // --------------------
    // DELAY
    // --------------------

    public static Coroutine Wait(this MonoBehaviour obj, float duration, Action then) {
        return obj.StartCoroutine(_Wait(duration, then));
    }

    private static IEnumerator _Wait(float duration, Action then) {
        yield return new WaitForSeconds(duration);
        then();
    }

    public static Coroutine Wait(this MonoBehaviour obj, Action then) {
        return obj.StartCoroutine(_Wait(then));
    }

    private static IEnumerator _Wait(Action then) {
        yield return new WaitForEndOfFrame();
        then();
    }

    
    // --------------------
    // REPEAT
    // --------------------

    public static void Repeat(Action action, int amount) {
        if (amount < 1) return;
        for (int i = 0; i < amount; i++) action();
    }
}