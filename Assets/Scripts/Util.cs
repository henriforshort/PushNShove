using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Util {
    // --------------------
    // LISTS
    // --------------------
    
    public static T Random<T> (this List<T> target) {
        if (target == null || target.Count == 0) return default;
        return target[UnityEngine.Random.Range(0, target.Count)];
    }
    
    public static T RandomExcept<T> (this List<T> target, T except) {
        if (target == null || target.Count == 0) return default;
        return target.Where(t => !t.Equals(except)).ToList().Random();
    }
    
    public static List<T> Except<T> (this List<T> target, T except) {
        if (target == null || target.Count == 0) return default;
        return target.Where(t => !t.Equals(except)).ToList();
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

    public static float LerpTo(this float a, float b, float speed) {
        return Mathf.Lerp(a, b, speed/100);
    }

    public static float Prel(this float x, float a, float b) {
        return (x - a) / (b - a);
    }

    public static float Remap(this float target, float oldA, float oldB, float newA, float newB) {
        return Mathf.Lerp(newA, newB, target.Prel(oldA, oldB));
    }

    public static float Round(this float target, float precision) {
        float pow = Mathf.Pow(10, precision);
        return Mathf.Round(target * pow) / pow;
    }

    public static float Round(this float target) {
        return Mathf.Round(target);
    }

    public static bool isApprox(this float target, float other) {
        return Mathf.Approximately(target, other);
    }

    public static float AtLeast(this float target, float other) {
        return Mathf.Max(target, other);
    }

    public static float AtMost(this float target, float other) {
        return Mathf.Min(target, other);
    }
    
    // --------------------
    // VECTOR2
    // --------------------

    public static Vector2 Clamp(this Vector2 target, float min, float max) {
        return new Vector2(
            target.x.Clamp(min, max),
            target.y.Clamp(min, max));
    }

    public static Vector2 ClampX(this Vector2 target, float min, float max) {
        return new Vector2(target.x.Clamp(min, max), target.y);
    }

    public static Vector2 ClampY(this Vector2 target, float min, float max) {
        return new Vector2(target.x, target.y.Clamp(min, max));
    }

    
    // --------------------
    // VECTOR3 - GET
    // --------------------
    
    public static float GetX (this Transform target) => target.position.x;
    public static float GetY (this Transform target) => target.position.y;
    public static float GetZ (this Transform target) => target.position.z;
    public static float GetX (this GameObject target) => target.transform.position.x;
    public static float GetY (this GameObject target) => target.transform.position.y;
    public static float GetZ (this GameObject target) => target.transform.position.z;
    public static float GetX (this MonoBehaviour target) => target.transform.position.x;
    public static float GetY (this MonoBehaviour target) => target.transform.position.y;
    public static float GetZ (this MonoBehaviour target) => target.transform.position.z;
    
    
    // --------------------
    // VECTOR3
    // --------------------

    
    public static Vector3 RandomWithin(this Vector3 obj, float range) {
        return obj + new Vector3(
            UnityEngine.Random.Range(-range, range), 
            UnityEngine.Random.Range(-range, range), 
            0);
    }
    
    public static Vector3 RandomWithin(this MonoBehaviour obj, float range) {
        return new Vector3(
            UnityEngine.Random.Range(-range, range), 
            UnityEngine.Random.Range(-range, range), 
            0);
    }
    
    // --------------------
    // VECTOR3 - SET
    // --------------------


    public static Vector3 SetX(this Vector3 target, float x) => new Vector3(x, target.y, target.z);
    public static Vector3 SetY(this Vector3 target, float y) => new Vector3(target.x, y, target.z);
    public static Vector3 SetZ(this Vector3 target, float z) => new Vector3(target.x, target.y, z);

    public static Transform SetX(this Transform target, float x) {
        target.position = SetX(target.position, x);
        return target;
    }

    public static Transform SetY(this Transform target, float y) {
        target.position = SetY(target.position, y);
        return target;
    }

    public static Transform SetZ(this Transform target, float z) {
        target.position = SetZ(target.position, z);
        return target;
    }

    public static GameObject SetX(this GameObject target, float x) {
        target.transform.SetX(x);
        return target.gameObject;
    }

    public static GameObject SetY(this GameObject target, float y) {
        target.transform.SetY(y);
        return target.gameObject;
    }

    public static GameObject SetZ(this GameObject target, float z) {
        target.transform.SetZ(z);
        return target.gameObject;
    }

    public static MonoBehaviour SetX(this MonoBehaviour target, float x) {
        target.transform.SetX(x);
        return target;
    }

    public static MonoBehaviour SetY(this MonoBehaviour target, float y) {
        target.transform.SetY(y);
        return target;
    }

    public static MonoBehaviour SetZ(this MonoBehaviour target, float z) {
        target.transform.SetZ(z);
        return target;
    }


    // --------------------
    // VECTOR3 - CLAMP
    // --------------------

    public static Vector3 Clamp(this Vector3 target, float min, float max) {
        return new Vector3(target.x.Clamp(min, max), target.y.Clamp(min, max), target.z.Clamp(min, max));
    }

    public static Vector3 ClampX(this Vector3 target, float min, float max) {
        return new Vector3(target.x.Clamp(min, max), target.y, target.z);
    }

    public static Vector3 ClampY(this Vector3 target, float min, float max) {
        return new Vector3(target.x, target.y.Clamp(min, max), target.z);
    }

    public static Vector3 ClampZ(this Vector3 target, float min, float max) {
        return new Vector3(target.x, target.y, target.z.Clamp(min, max));
    }

    public static void Clamp(this Transform target, float min, float max) {
        target.position = target.position.Clamp(min, max);
    }

    public static void ClampX(this Transform target, float min, float max) {
        target.position = target.position.ClampX(min, max);
    }

    public static void ClampY(this Transform target, float min, float max) {
        target.position = target.position.ClampY(min, max);
    }

    public static void ClampZ(this Transform target, float min, float max) {
        target.position = target.position.ClampZ(min, max);
    }

    public static void Clamp(this GameObject target, float min, float max) {
        target.transform.Clamp(min, max);
    }

    public static void ClampX(this GameObject target, float min, float max) {
        target.transform.ClampX(min, max);
    }

    public static void ClampY(this GameObject target, float min, float max) {
        target.transform.ClampY(min, max);
    }

    public static void ClampZ(this GameObject target, float min, float max) {
        target.transform.ClampZ(min, max);
    }

    
    // --------------------
    // VECTOR3 - LERPTO
    // --------------------
    
    public static Transform LerpTo(this Transform obj, Vector3 target, float speed = 2) {
        obj.position = Vector3.Lerp(obj.position, target, speed / 100);
        return obj;
    }

    public static Transform LerpTo(this Transform obj, Transform target, float speed = 2) {
        obj.position = Vector3.Lerp(obj.position, target.position, speed / 100);
        return obj;
    }

    public static Transform LerpTo(this Transform obj, GameObject target, float speed = 2) {
        obj.position = Vector3.Lerp(obj.position, target.transform.position, speed / 100);
        return obj;
    }

    public static GameObject LerpTo(this GameObject obj, Vector3 target, float speed = 2) {
        obj.transform.LerpTo(target, speed);
        return obj;
    }

    public static GameObject LerpTo(this GameObject obj, Transform target, float speed = 2) {
        obj.transform.LerpTo(target, speed);
        return obj;
    }

    public static GameObject LerpTo(this GameObject obj, GameObject target, float speed = 2) {
        obj.transform.LerpTo(target, speed);
        return obj;
    }
    
    public static void LerpXTo(this GameObject obj, Vector3 target, float speed = 2) {
        obj.LerpTo(new Vector3(target.x, obj.transform.position.y, obj.transform.position.z), speed);
    }
    
    public static void LerpYTo(this GameObject obj, Vector3 target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.transform.position.x, target.y, obj.transform.position.z), speed);
    }
    
    public static void LerpZTo(this GameObject obj, Vector3 target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.transform.position.x, obj.transform.position.y, target.z), speed);
    }
    
    public static void LerpXTo(this Transform obj, Vector3 target, float speed = 2) {
        obj.LerpTo(new Vector3(target.x, obj.position.y, obj.position.z), speed);
    }
    
    public static void LerpYTo(this Transform obj, Vector3 target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.position.x, target.y, obj.position.z), speed);
    }
    
    public static void LerpZTo(this Transform obj, Vector3 target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.position.x, obj.position.y, target.z), speed);
    }

    public static void LerpXTo(this Transform obj, Transform target, float speed = 2) {
        obj.LerpTo(new Vector3(target.position.x, obj.position.y, obj.position.z), speed);
    }
    
    public static void LerpYTo(this Transform obj, Transform target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.position.x, target.position.y, obj.position.z), speed);
    }
    
    public static void LerpZTo(this Transform obj, Transform target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.position.x, obj.position.y, target.position.z), speed);
    }
    
    public static void LerpXTo(this Transform obj, GameObject target, float speed = 2) {
        obj.LerpTo(new Vector3(target.transform.position.x, obj.position.y, obj.position.z), speed);
    }
    
    public static void LerpYTo(this Transform obj, GameObject target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.position.x, target.transform.position.y, obj.position.z), speed);
    }
    
    public static void LerpZTo(this Transform obj, GameObject target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.position.x, obj.position.y, target.transform.position.z), speed);
    }

    public static void LerpXTo(this GameObject obj, Transform target, float speed = 2) {
        obj.LerpTo(new Vector3(target.position.x, obj.transform.position.y, obj.transform.position.z), speed);
    }
    
    public static void LerpYTo(this GameObject obj, Transform target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.transform.position.x, target.position.y, obj.transform.position.z), speed);
    }
    
    public static void LerpZTo(this GameObject obj, Transform target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.transform.position.x, obj.transform.position.y, target.position.z), speed);
    }
    
    public static void LerpXTo(this GameObject obj, GameObject target, float speed = 2) {
        obj.LerpTo(new Vector3(target.transform.position.x, obj.transform.position.y, obj.transform.position.z), speed);
    }
    
    public static void LerpYTo(this GameObject obj, GameObject target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.transform.position.x, target.transform.position.y, obj.transform.position.z), speed);
    }
    
    public static void LerpZTo(this GameObject obj, GameObject target, float speed = 2) {
        obj.LerpTo(new Vector3(obj.transform.position.x, obj.transform.position.y, target.transform.position.z), speed);
    }

    
    // --------------------
    // VECTOR3 - LOCALLERPTO
    // --------------------

    public static void LocalLerpTo(this Transform obj, Vector3 target, float speed = 2) {
        obj.localPosition = Vector3.Lerp(obj.localPosition, target, speed / 100);
    }

    public static void LocalLerpTo(this GameObject obj, Vector3 target, float speed = 2) {
        obj.transform.LocalLerpTo(target, speed);
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

    public static void LocalSlerpTo(this Transform obj, Quaternion target, float speed = 2) {
        obj.localRotation = Quaternion.Slerp(obj.localRotation, target, speed / 100);
    }

    public static void LocalSlerpTo(this GameObject obj, Quaternion target, float speed = 2) {
        obj.transform.LocalSlerpTo(target, speed);
    }

    public static void RotateTowards(this Transform obj, Quaternion target, float distance) {
        obj.rotation = Quaternion.RotateTowards(obj.rotation, target, distance);
    }

    public static void RotateTowards(this GameObject obj, Quaternion target, float distance) {
        obj.transform.RotateTowards(target, distance);
    }

    public static Quaternion SlerpTo(this Quaternion obj, Quaternion target, float speed = 2) {
        return Quaternion.Slerp(obj, target, speed / 100);
    }

    public static void SlerpTo(this Transform obj, Quaternion target, float speed = 2) {
        obj.rotation = Quaternion.Slerp(obj.rotation, target, speed / 100);
    }

    public static void SlerpTo(this GameObject obj, Quaternion target, float speed = 2) {
        obj.transform.SlerpTo(target, speed);
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

    public static T Random<T>(T t1, T t2, T t3, T t4) {
        int i = UnityEngine.Random.Range(0, 4);
        if (i == 0) return t1;
        else if (i == 1) return t2;
        else if (i == 2) return t3;
        else return t4;
    }

    public static bool Random<T>() {
        return UnityEngine.Random.Range(0, 2) == 0;
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

    // Waits only one frame
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

    public static void Repeat(this Action action, int amount) {
        if (amount < 1) return;
        for (int i = 0; i < amount; i++) action();
    }
}