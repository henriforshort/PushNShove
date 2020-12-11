using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public static class Util {
    // --------------------
    // STRINGS
    // --------------------


    public static string ToSentenceCase(this string target) {
        if (string.IsNullOrEmpty(target)) return target;
        return target.First().ToString().ToUpper() + target.Substring(1).ToLower();
    }
    
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

    public static void Log<T>(this List<T> target) {
        if (target == null) {
            Debug.Log("null");
            return;
        }
        
        string result = "[";
        target.ForEach(e => {
            result += e;
            if (!target.LastOrDefault().Equals(e)) result += ", ";
        });
        result += "]";
        Debug.Log(result);
    }

    
    // --------------------
    // MATH
    // --------------------

    public static float Clamp(this float target, float min, float max) => Mathf.Clamp(target, min, max);
    public static float Clamp01(this float target) => Mathf.Clamp01(target);

    public static float Lerp(this float x, float a, float b) => Mathf.Lerp(a, b, x);
    public static float LerpTo(this float a, float b, float speed) => Mathf.Lerp(a, b, speed/100);
    public static float Prel(this float x, float a, float b) => (x - a) / (b - a);
    public static float Remap(this float target, float oldA, float oldB, float newA, float newB) => 
        Mathf.Lerp(newA, newB, target.Prel(oldA, oldB));

    public static float Round(this float target, float precision = 0) => 
        Mathf.Round(target * Mathf.Pow(10, precision)) / Mathf.Pow(10, precision);

    public static bool isApprox(this float target, float other) => Mathf.Approximately(target, other);
    public static bool isNotApprox(this float target, float other) => !Mathf.Approximately(target, other);

    public static float AtLeast(this float target, float other) => Mathf.Max(target, other);
    public static float AtMost(this float target, float other) => Mathf.Min(target, other);

    public static int AtLeast(this int target, int other) => Mathf.Max(target, other);
    public static int AtMost(this int target, int other) => Mathf.Min(target, other);

    public static float Abs(this float target) => Mathf.Abs(target);
    public static float Abs(this int target) => Mathf.Abs(target);
    
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
    // VECTOR3 - GET
    // --------------------
    
    //Get from Transform
    public static float GetX (this Transform target) => target.position.x;
    public static float GetY (this Transform target) => target.position.y;
    public static float GetZ (this Transform target) => target.position.z;
    
    //Get from GameObject
    public static float GetX (this GameObject target) => target.transform.position.x;
    public static float GetY (this GameObject target) => target.transform.position.y;
    public static float GetZ (this GameObject target) => target.transform.position.z;
    
    //Get from MonoBehaviour
    public static float GetX (this MonoBehaviour target) => target.transform.position.x;
    public static float GetY (this MonoBehaviour target) => target.transform.position.y;
    public static float GetZ (this MonoBehaviour target) => target.transform.position.z;
    
    // --------------------
    // VECTOR3 - SET
    // --------------------

    //Set Vector3 with float
    public static Vector3 SetX(this Vector3 target, float x) => new Vector3(x, target.y, target.z);
    public static Vector3 SetY(this Vector3 target, float y) => new Vector3(target.x, y, target.z);
    public static Vector3 SetZ(this Vector3 target, float z) => new Vector3(target.x, target.y, z);

    //Set Transform with float
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

    //Set GameObject with float
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

    // Set MonoBehaviour with float
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

    //Clamp Vector3 with floats
    public static Vector3 Clamp(this Vector3 target, float min, float max) => 
        new Vector3(target.x.Clamp(min, max), target.y.Clamp(min, max), target.z.Clamp(min, max));
    public static Vector3 ClampX(this Vector3 target, float min, float max) => 
        new Vector3(target.x.Clamp(min, max), target.y, target.z);
    public static Vector3 ClampY(this Vector3 target, float min, float max) => 
        new Vector3(target.x, target.y.Clamp(min, max), target.z);
    public static Vector3 ClampZ(this Vector3 target, float min, float max) => 
        new Vector3(target.x, target.y, target.z.Clamp(min, max));

    //Clamp Transform with floats
    public static void Clamp(this Transform target, float min, float max) => 
        target.position = target.position.Clamp(min, max);
    public static void ClampX(this Transform target, float min, float max) => 
        target.position = target.position.ClampX(min, max);
    public static void ClampY(this Transform target, float min, float max) => 
        target.position = target.position.ClampY(min, max);
    public static void ClampZ(this Transform target, float min, float max) => 
        target.position = target.position.ClampZ(min, max);

    //Clamp GameObject with floats
    public static void Clamp(this GameObject target, float min, float max) => target.transform.Clamp(min, max);
    public static void ClampX(this GameObject target, float min, float max) => target.transform.ClampX(min, max);
    public static void ClampY(this GameObject target, float min, float max) => target.transform.ClampY(min, max);
    public static void ClampZ(this GameObject target, float min, float max) => target.transform.ClampZ(min, max);

    
    // --------------------
    // VECTOR3 - LERPTO
    // --------------------
    
    //Basic LerpTo methods
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
    public static GameObject LerpTo(this GameObject obj, Vector3 target, float speed = 2) => 
        obj.transform.LerpTo(target, speed).gameObject;
    public static GameObject LerpTo(this GameObject obj, Transform target, float speed = 2) =>
        obj.transform.LerpTo(target, speed).gameObject;
    public static GameObject LerpTo(this GameObject obj, GameObject target, float speed = 2) => 
        obj.transform.LerpTo(target, speed).gameObject;
    
    //Lerp Transform to float
    public static void LerpXTo(this Transform obj, float target, float speed = 2) =>
        obj.LerpTo(new Vector3(target, obj.position.y, obj.position.z), speed);
    public static void LerpYTo(this Transform obj, float target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.position.x, target, obj.position.z), speed);
    public static void LerpZTo(this Transform obj, float target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.position.x, obj.position.y, target), speed);
    
    //Lerp Transform to Vector3
    public static void LerpXTo(this Transform obj, Vector3 target, float speed = 2) =>
        obj.LerpTo(new Vector3(target.x, obj.position.y, obj.position.z), speed);
    public static void LerpYTo(this Transform obj, Vector3 target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.position.x, target.y, obj.position.z), speed);
    public static void LerpZTo(this Transform obj, Vector3 target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.position.x, obj.position.y, target.z), speed);

    //Lerp Transform to Transform
    public static void LerpXTo(this Transform obj, Transform target, float speed = 2) =>
        obj.LerpTo(new Vector3(target.position.x, obj.position.y, obj.position.z), speed);
    public static void LerpYTo(this Transform obj, Transform target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.position.x, target.position.y, obj.position.z), speed);
    public static void LerpZTo(this Transform obj, Transform target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.position.x, obj.position.y, target.position.z), speed);
    
    //Lerp Transform to GameObejct
    public static void LerpXTo(this Transform obj, GameObject target, float speed = 2) =>
        obj.LerpTo(new Vector3(target.transform.position.x, obj.position.y, obj.position.z), speed);
    public static void LerpYTo(this Transform obj, GameObject target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.position.x, target.transform.position.y, obj.position.z), speed);
    public static void LerpZTo(this Transform obj, GameObject target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.position.x, obj.position.y, target.transform.position.z), speed);
    
    //Lerp GameObject to float
    public static void LerpXTo(this GameObject obj, float target, float speed = 2) =>
        obj.LerpTo(new Vector3(target, obj.transform.position.y, obj.transform.position.z), speed);
    public static void LerpYTo(this GameObject obj, float target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.transform.position.x, target, obj.transform.position.z), speed);
    public static void LerpZTo(this GameObject obj, float target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.transform.position.x, obj.transform.position.y, target), speed);
    
    //Lerp GameObject to Vector3
    public static void LerpXTo(this GameObject obj, Vector3 target, float speed = 2) => 
        obj.LerpTo(new Vector3(target.x, obj.transform.position.y, obj.transform.position.z), speed);
    public static void LerpYTo(this GameObject obj, Vector3 target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.transform.position.x, target.y, obj.transform.position.z), speed);
    public static void LerpZTo(this GameObject obj, Vector3 target, float speed = 2) => 
        obj.LerpTo(new Vector3(obj.transform.position.x, obj.transform.position.y, target.z), speed);

    //Lerp GameObject to Transform
    public static void LerpXTo(this GameObject obj, Transform target, float speed = 2) =>
        obj.LerpTo(new Vector3(target.position.x, obj.transform.position.y, obj.transform.position.z), speed);
    public static void LerpYTo(this GameObject obj, Transform target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.transform.position.x, target.position.y, obj.transform.position.z), speed);
    public static void LerpZTo(this GameObject obj, Transform target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.transform.position.x, obj.transform.position.y, target.position.z), speed);
    
    //Lerp GameObject to GameObject
    public static void LerpXTo(this GameObject obj, GameObject target, float speed = 2) =>
        obj.LerpTo(new Vector3(target.transform.position.x, obj.transform.position.y, obj.transform.position.z), 
            speed);
    public static void LerpYTo(this GameObject obj, GameObject target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.transform.position.x, target.transform.position.y, obj.transform.position.z), 
            speed);
    public static void LerpZTo(this GameObject obj, GameObject target, float speed = 2) =>
        obj.LerpTo(new Vector3(obj.transform.position.x, obj.transform.position.y, target.transform.position.z), 
            speed);

    
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
    // VECTOR3 - CLOSER
    // --------------------

    public static bool isCloserTo(this Component obj, Vector3 target, Component than) =>
        Vector3.Distance(obj.transform.position, target) < 
        Vector3.Distance(than.transform.position, target);

    public static bool isFurtherFrom(this Component obj, Vector3 target, Component than) => 
        !obj.isCloserTo(target, than);
    
    

    
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

    public static float Random(this MonoBehaviour target) => UnityEngine.Random.value;
    public static float Random(this MonoBehaviour target, float max) => UnityEngine.Random.Range(0, max);
    public static float Random(this MonoBehaviour target, int max) => UnityEngine.Random.Range(0, max);
    public static float Random(this MonoBehaviour target, float min, float max) => UnityEngine.Random.Range(min, max);
    public static float Random(this MonoBehaviour target, int min, int max) => UnityEngine.Random.Range(min, max);

    public static bool PercentChance(this float target) => UnityEngine.Random.value < target / 100;
    public static bool PercentChance(this int target) => UnityEngine.Random.value < (float)target / 100;
    public static bool Chance(this float target) => UnityEngine.Random.value < target;
    public static bool Chance(this int target) => UnityEngine.Random.value < target;

    public static float MoreOrLessPercent(this float target, float amount) =>
        UnityEngine.Random.Range((1-amount) * target, (1+amount) * target);

    public static bool CoinFlip(this MonoBehaviour target) => UnityEngine.Random.Range(0, 2) == 0;

    public static T Random<T>(this MonoBehaviour target, T t1, T t2) => target.CoinFlip() ? t1 : t2;

    public static T Random<T>(this MonoBehaviour target, T t1, T t2, T t3) {
        int i = UnityEngine.Random.Range(0, 3);
        if (i == 0) return t1;
        else if (i == 1) return t2;
        else return t3;
    }

    public static T Random<T>(this MonoBehaviour target, T t1, T t2, T t3, T t4) {
        int i = UnityEngine.Random.Range(0, 4);
        if (i == 0) return t1;
        else if (i == 1) return t2;
        else if (i == 2) return t3;
        else return t4;
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

    public static void Repeat(this MonoBehaviour target, Action action, int times) {
        if (times < 1) return;
        for (int i = 0; i < times; i++) action();
    }

    
    // --------------------
    // SLIDERS
    // --------------------

    public static void LerpTo(this Slider obj, float target, float speed = 2) {
        obj.value = obj.value.LerpTo(target, speed);
    }

    
    // --------------------
    // TRANSFORMS
    // --------------------

    //Move children of a transform
    public static void MoveChildrenTo(this Transform obj, Transform newParent) {
        while (obj.childCount > 0) obj.GetChild(0).SetParent(newParent);
    }
    public static void MoveChildrenTo(this Transform obj, GameObject newParent) =>
        obj.MoveChildrenTo(newParent.transform);
    public static void MoveChildrenTo(this Transform obj, MonoBehaviour newParent) =>
        obj.MoveChildrenTo(newParent.transform);

    //Move children of a gameobject
    public static void MoveChildrenTo(this GameObject obj, Transform newParent) =>
        obj.transform.MoveChildrenTo(newParent);
    public static void MoveChildrenTo(this GameObject obj, GameObject newParent) =>
        obj.MoveChildrenTo(newParent.transform);
    public static void MoveChildrenTo(this GameObject obj, MonoBehaviour newParent) =>
        obj.MoveChildrenTo(newParent.transform);

    //Move children of a monobehavior
    public static void MoveChildrenTo(this MonoBehaviour obj, Transform newParent) =>
        obj.transform.MoveChildrenTo(newParent);
    public static void MoveChildrenTo(this MonoBehaviour obj, GameObject newParent) =>
        obj.MoveChildrenTo(newParent.transform);
    public static void MoveChildrenTo(this MonoBehaviour obj, MonoBehaviour newParent) =>
        obj.MoveChildrenTo(newParent.transform);
}