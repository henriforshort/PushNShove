using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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
            if (!e.Equals(target.LastOrDefault())) result += ", ";
        });
        result += "]";
        Debug.Log(result);
    }

    public static T WithLowest<T, TKey>(this IEnumerable<T> target, Func<T, TKey> keySelector) =>
        target.OrderBy(keySelector).FirstOrDefault();
    
    public static T WithHighest<T, TKey>(this IEnumerable<T> target, Func<T, TKey> keySelector) =>
        target.OrderBy(keySelector).LastOrDefault();
    
    public static List<T> AsList<T>(this T item) => new List<T> { item };

    public static T Get<T>(this List<T> target, int index) {
        try { return target[index]; } 
        catch (Exception) { return default; }
    }

    public static List<T> Clone<T>(this List<T> target) {
        List<T> result = new List<T>();
        target.ForEach(t => result.Add(t));
        return result;
    }

    
    // --------------------
    // MATH
    // --------------------

    public static float Clamp(this float target, float min, float max) => Mathf.Clamp(target, min, max);
    public static float Clamp01(this float target) => Mathf.Clamp01(target);

    public static float Lerp(this float x, float a, float b) => Mathf.Lerp(a, b, x);
    public static float Prel(this float x, float a, float b) => (x - a) / (b - a);
    public static float Remap(this float target, float oldA, float oldB, float newA, float newB) => 
        target.Prel(oldA, oldB).Lerp(newA, newB);
    
    public static float LerpTo(this float a, float b, float speed = 2) => //Call in Update only
        (1 - speed/100).Pow(60 * Time.deltaTime).Lerp(b, a);
    public static float FixedLerpTo(this float a, float b, float speed = 2) => //Call in FixedUpdate only
        (1 - speed/100).Pow(60 * Time.fixedDeltaTime).Lerp(b, a);

    public static float Round(this float target) => Mathf.Round(target);
    public static float Round(this float target, float precision) => 
        (target * 10.Pow(precision)).Round() / 10.Pow(precision);

    public static bool isAbout(this float target, float other) => Mathf.Approximately(target, other);
    public static bool isClearlyNot(this float target, float other) => !Mathf.Approximately(target, other);
    
    public static bool isAboutOrHigherThan(this float target, float other) => target.isAbout(other) || target > other;
    public static bool isClearlyHigherThan(this float target, float other) => 
        target.isClearlyNot(other) && target > other;
    public static bool isClearlyPositive(this float target) => target.isClearlyNot(0) && target > 0;
    
    public static bool isAboutOrLowerThan(this float target, float other) => target.isAbout(other) || target < other;
    public static bool isClearlyLowerThan(this float target, float other) => 
        target.isClearlyNot(other) && target < other;
    public static bool isClearlyNegative(this float target) => target.isClearlyNot(0) && target < 0;

    public static float AtLeast(this float target, float other) => Mathf.Max(target, other);
    public static float AtMost(this float target, float other) => Mathf.Min(target, other);

    public static int AtLeast(this int target, int other) => Mathf.Max(target, other);
    public static int AtMost(this int target, int other) => Mathf.Min(target, other);

    public static float Abs(this float target) => Mathf.Abs(target);
    public static float Abs(this int target) => Mathf.Abs(target);

    public static float Pow(this float target, float other) => Mathf.Pow(target, other);
    public static float Pow(this float target, int other) => Mathf.Pow(target, other);
    public static float Pow(this int target, float other) => Mathf.Pow(target, other);
    public static float Pow(this int target, int other) => Mathf.Pow(target, other);
    
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
    
    public static Vector3 RandomWithin(this Component obj, float range) {
        return new Vector3(
            UnityEngine.Random.Range(-range, range), 
            UnityEngine.Random.Range(-range, range), 
            0);
    }

    
    // --------------------
    // VECTOR3 - GET
    // --------------------
    
    //Get from Component
    public static float GetX (this Component target) => target == null ? default : target.transform.position.x;
    public static float GetY (this Component target) => target == null ? default : target.transform.position.y;
    public static float GetZ (this Component target) => target == null ? default : target.transform.position.z;
    
    //Get from GameObject
    public static float GetX (this GameObject target) => target == null ? default : target.transform.position.x;
    public static float GetY (this GameObject target) => target == null ? default : target.transform.position.y;
    public static float GetZ (this GameObject target) => target == null ? default : target.transform.position.z;
    
    // --------------------
    // VECTOR3 - SET
    // --------------------

    //Set Vector3 with float
    public static Vector3 SetX(this Vector3 target, float x) => new Vector3(x, target.y, target.z);
    public static Vector3 SetY(this Vector3 target, float y) => new Vector3(target.x, y, target.z);
    public static Vector3 SetZ(this Vector3 target, float z) => new Vector3(target.x, target.y, z);

    //Set Component with float
    public static Component SetX(this Component target, float x) {
        target.transform.position = target.transform.position.SetX(x);
        return target;
    }
    public static Component SetY(this Component target, float y) {
        target.transform.position = target.transform.position.SetY(y);
        return target;
    }
    public static Component SetZ(this Component target, float z) {
        target.transform.position = target.transform.position.SetZ(z);
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


    // --------------------
    // VECTOR3 - CLAMP
    // --------------------

    //Clamp Vector3 with floats
    public static Vector3 Clamp(this Vector3 target, float min, float max) => 
        new Vector3(target.x.Clamp(min, max), target.y.Clamp(min, max), target.z.Clamp(min, max));
    public static Vector3 ClampX(this Vector3 target, float min, float max) => target.SetX(target.x.Clamp(min, max));
    public static Vector3 ClampY(this Vector3 target, float min, float max) => target.SetY(target.y.Clamp(min, max));
    public static Vector3 ClampZ(this Vector3 target, float min, float max) => target.SetZ(target.z.Clamp(min, max));

    //Clamp Component with floats
    public static void Clamp(this Component target, float min, float max) => 
        target.transform.position = target.transform.position.Clamp(min, max);
    public static void ClampX(this Component target, float min, float max) => target.SetX(target.GetX().Clamp(min,max));
    public static void ClampY(this Component target, float min, float max) => target.SetY(target.GetY().Clamp(min,max));
    public static void ClampZ(this Component target, float min, float max) => target.SetZ(target.GetZ().Clamp(min,max));

    //Clamp GameObject with floats
    public static void Clamp(this GameObject target, float min, float max) => target.transform.Clamp(min, max);
    public static void ClampX(this GameObject target, float min, float max) => target.transform.ClampX(min, max);
    public static void ClampY(this GameObject target, float min, float max) => target.transform.ClampY(min, max);
    public static void ClampZ(this GameObject target, float min, float max) => target.transform.ClampZ(min, max);

    
    // --------------------
    // VECTOR3 - LERPTO
    // --------------------

    public static Vector3 LerpTo(this Vector3 obj, Vector3 target, float speed = 2) => //Update only
        new Vector3(obj.x.LerpTo(target.x, speed), obj.y.LerpTo(target.y, speed), obj.z.LerpTo(target.z, speed));
    
    //Lerp Component
    public static Component LerpTo(this Component obj, Vector3 target, float speed = 2) {
        obj.transform.position = obj.transform.position.LerpTo(target, speed);
        return obj;
    }
    public static Component LerpTo(this Component obj, Component target, float speed = 2) {
        obj.transform.position = obj.transform.position.LerpTo(target.transform.position, speed);
        return obj;
    }
    public static Component LerpTo(this Component obj, GameObject target, float speed = 2) {
        obj.transform.position = obj.transform.position.LerpTo(target.transform.position, speed);
        return obj;
    }
    
    //Lerp GameObject
    public static GameObject LerpTo(this GameObject obj, Vector3 target, float speed = 2) => 
        obj.transform.LerpTo(target, speed).gameObject;
    public static GameObject LerpTo(this GameObject obj, Component target, float speed = 2) =>
        obj.transform.LerpTo(target, speed).gameObject;
    public static GameObject LerpTo(this GameObject obj, GameObject target, float speed = 2) => 
        obj.transform.LerpTo(target, speed).gameObject;
    
    //Lerp Component to float by axis
    public static void LerpXTo(this Component obj, float target, float speed = 2) =>
        obj.SetX(obj.GetX().LerpTo(target, speed));
    public static void LerpYTo(this Component obj, float target, float speed = 2) =>
        obj.SetY(obj.GetY().LerpTo(target, speed));
    public static void LerpZTo(this Component obj, float target, float speed = 2) =>
        obj.SetZ(obj.GetZ().LerpTo(target, speed));
    
    //Lerp Component to Vector3 by axis
    public static void LerpXTo(this Component obj, Vector3 target, float speed = 2) =>
        obj.SetX(obj.GetX().LerpTo(target.x, speed));
    public static void LerpYTo(this Component obj, Vector3 target, float speed = 2) =>
        obj.SetY(obj.GetY().LerpTo(target.y, speed));
    public static void LerpZTo(this Component obj, Vector3 target, float speed = 2) =>
        obj.SetZ(obj.GetZ().LerpTo(target.z, speed));

    //Lerp Component to Component by axis
    public static void LerpXTo(this Component obj, Component target, float speed = 2) =>
        obj.SetX(obj.GetX().LerpTo(target.GetX(), speed));
    public static void LerpYTo(this Component obj, Component target, float speed = 2) =>
        obj.SetY(obj.GetY().LerpTo(target.GetY(), speed));
    public static void LerpZTo(this Component obj, Component target, float speed = 2) =>
        obj.SetZ(obj.GetZ().LerpTo(target.GetZ(), speed));
    
    //Lerp Component to GameObject by axis
    public static void LerpXTo(this Component obj, GameObject target, float speed = 2) =>
        obj.SetX(obj.GetX().LerpTo(target.GetX(), speed));
    public static void LerpYTo(this Component obj, GameObject target, float speed = 2) =>
        obj.SetY(obj.GetY().LerpTo(target.GetY(), speed));
    public static void LerpZTo(this Component obj, GameObject target, float speed = 2) =>
        obj.SetZ(obj.GetZ().LerpTo(target.GetZ(), speed));
    
    //Lerp GameObject to float by axis
    public static void LerpXTo(this GameObject obj, float target, float speed = 2) =>
        obj.SetX(obj.GetX().LerpTo(target, speed));
    public static void LerpYTo(this GameObject obj, float target, float speed = 2) =>
        obj.SetY(obj.GetY().LerpTo(target, speed));
    public static void LerpZTo(this GameObject obj, float target, float speed = 2) =>
        obj.SetZ(obj.GetZ().LerpTo(target, speed));
    
    //Lerp GameObject to Vector3 by axis
    public static void LerpXTo(this GameObject obj, Vector3 target, float speed = 2) => 
        obj.SetX(obj.GetX().LerpTo(target.x, speed));
    public static void LerpYTo(this GameObject obj, Vector3 target, float speed = 2) =>
        obj.SetY(obj.GetY().LerpTo(target.y, speed));
    public static void LerpZTo(this GameObject obj, Vector3 target, float speed = 2) => 
        obj.SetZ(obj.GetZ().LerpTo(target.z, speed));

    //Lerp GameObject to Component by axis
    public static void LerpXTo(this GameObject obj, Component target, float speed = 2) =>
        obj.SetX(obj.GetX().LerpTo(target.GetX(), speed));
    public static void LerpYTo(this GameObject obj, Component target, float speed = 2) =>
        obj.SetY(obj.GetY().LerpTo(target.GetY(), speed));
    public static void LerpZTo(this GameObject obj, Component target, float speed = 2) =>
        obj.SetZ(obj.GetZ().LerpTo(target.GetZ(), speed));
    
    //Lerp GameObject to GameObject by axis
    public static void LerpXTo(this GameObject obj, GameObject target, float speed = 2) =>
        obj.SetX(obj.GetX().LerpTo(target.GetX(), speed));
    public static void LerpYTo(this GameObject obj, GameObject target, float speed = 2) =>
        obj.SetY(obj.GetY().LerpTo(target.GetY(), speed));
    public static void LerpZTo(this GameObject obj, GameObject target, float speed = 2) =>
        obj.SetZ(obj.GetZ().LerpTo(target.GetZ(), speed));

    
    // --------------------
    // VECTOR3 - LOCAL GET
    // --------------------
    
    //Get from GameObject
    public static float GetLocalX (this GameObject target) => target.transform.localPosition.x;
    public static float GetLocalY (this GameObject target) => target.transform.localPosition.y;
    public static float GetLocalZ (this GameObject target) => target.transform.localPosition.z;
    
    //Get from Component
    public static float GetLocalX (this Component target) => target.transform.localPosition.x;
    public static float GetLocalY (this Component target) => target.transform.localPosition.y;
    public static float GetLocalZ (this Component target) => target.transform.localPosition.z;
    
    
    // --------------------
    // VECTOR3 - LOCAL SET
    // --------------------

    //Set Component with float
    public static Component SetLocalX(this Component target, float x) {
        target.transform.position = target.transform.localPosition.SetX(x);
        return target;
    }
    public static Component SetLocalY(this Component target, float y) {
        target.transform.position = target.transform.localPosition.SetY(y);
        return target;
    }
    public static Component SetLocalZ(this Component target, float z) {
        target.transform.position = target.transform.localPosition.SetZ(z);
        return target;
    }

    //Set GameObject with float
    public static GameObject SetLocalX(this GameObject target, float x) {
        target.transform.SetLocalX(x);
        return target.gameObject;
    }
    public static GameObject SetLocalY(this GameObject target, float y) {
        target.transform.SetLocalY(y);
        return target.gameObject;
    }
    public static GameObject SetLocalZ(this GameObject target, float z) {
        target.transform.SetLocalZ(z);
        return target.gameObject;
    }

    
    // --------------------
    // VECTOR3 - LOCALLERPTO
    // --------------------

    public static void LocalLerpTo(this Component obj, Vector3 target, float speed = 2) =>
        obj.transform.localPosition = obj.transform.localPosition.LerpTo(target, speed);

    public static void LocalLerpTo(this GameObject obj, Vector3 target, float speed = 2) =>
        obj.transform.LocalLerpTo(target, speed);
    public static void LocalLerpTo(this Component obj, Component target, float speed = 2) =>
        obj.transform.LocalLerpTo(target.transform.position, speed);
    public static void LocalLerpTo(this GameObject obj, Component target, float speed = 2) =>
        obj.transform.LocalLerpTo(target.transform.position, speed);
    public static void LocalLerpTo(this Component obj, GameObject target, float speed = 2) =>
        obj.transform.LocalLerpTo(target.transform.position, speed);
    public static void LocalLerpTo(this GameObject obj, GameObject target, float speed = 2) =>
        obj.transform.LocalLerpTo(target.transform.position, speed);

    
    // --------------------
    // VECTOR3 - CLOSER
    // --------------------

    public static bool isCloserTo(this Component obj, Vector3 target, Component than) =>
        Vector3.SqrMagnitude(obj.transform.position - target) < 
        Vector3.SqrMagnitude(than.transform.position - target);

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

    public static void LocalSlerpTo(this Component obj, Quaternion target, float speed = 2) {
        obj.transform.localRotation = Quaternion.Slerp(obj.transform.localRotation, target, speed / 100);
    }

    public static void LocalSlerpTo(this GameObject obj, Quaternion target, float speed = 2) {
        obj.transform.LocalSlerpTo(target, speed);
    }

    public static void RotateTowards(this Component obj, Quaternion target, float distance) {
        obj.transform.rotation = Quaternion.RotateTowards(obj.transform.rotation, target, distance);
    }

    public static void RotateTowards(this GameObject obj, Quaternion target, float distance) {
        obj.transform.RotateTowards(target, distance);
    }

    public static Quaternion SlerpTo(this Quaternion obj, Quaternion target, float speed = 2) {
        return Quaternion.Slerp(obj, target, speed / 100);
    }

    public static void SlerpTo(this Component obj, Quaternion target, float speed = 2) {
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, target, speed / 100);
    }

    public static void SlerpTo(this GameObject obj, Quaternion target, float speed = 2) {
        obj.transform.SlerpTo(target, speed);
    }

    
    // --------------------
    // RANDOM
    // --------------------

    public static float Random(this MonoBehaviour target) => UnityEngine.Random.value;
    public static float Random(this MonoBehaviour target, float max) => UnityEngine.Random.Range(0, max);
    public static int Random(this MonoBehaviour target, int max) => UnityEngine.Random.Range(0, max);
    public static float Random(this MonoBehaviour target, float min, float max) => UnityEngine.Random.Range(min, max);
    public static int Random(this MonoBehaviour target, int min, int max) => UnityEngine.Random.Range(min, max);

    public static bool PercentChance(this float target) => UnityEngine.Random.value < target / 100;
    public static bool PercentChance(this int target) => UnityEngine.Random.value < (float)target / 100;
    public static bool Chance(this float target) => UnityEngine.Random.value < target;
    public static bool Chance(this int target) => UnityEngine.Random.value < target;

    public static float MoreOrLessPercent(this float target, float amount) =>
        UnityEngine.Random.Range((1-amount) * target, (1+amount) * target);

    public static bool CoinFlip(this MonoBehaviour target) => target.Random(2) == 0;

    public static T Random<T>(this MonoBehaviour target, T t1, T t2) => target.CoinFlip() ? t1 : t2;

    public static T Random<T>(this MonoBehaviour target, T t1, T t2, T t3) {
        int i = target.Random(3);
        if (i == 0) return t1;
        else if (i == 1) return t2;
        else return t3;
    }

    public static T Random<T>(this MonoBehaviour target, T t1, T t2, T t3, T t4) {
        int i = target.Random(3);
        if (i == 0) return t1;
        else if (i == 1) return t2;
        else if (i == 2) return t3;
        else return t4;
    }

    public static T WeightedRandom<T>(this MonoBehaviour target, T t1, float weight1, T t2, float weight2) {
        return target.Random() < weight1 / (weight1 + weight2) ? t1 : t2;
    }

    public static T WeightedRandom<T>(this MonoBehaviour target, T t1, float weight1, T t2, float weight2, 
        T t3, float weight3) {
        float f = target.Random(weight1 + weight2 + weight3);
        if (f < weight1) return t1;
        else if (f < weight1 + weight2) return t2;
        else return t3;
    }

    public static T WeightedRandom<T>(this MonoBehaviour target, T t1, float weight1, T t2, float weight2, 
        T t3, float weight3, T t4, float weight4) {
        float f = target.Random(weight1 + weight2 + weight3 + weight4);
        if (f < weight1) return t1;
        else if (f < weight1 + weight2) return t2;
        else if (f < weight1 + weight2 + weight3) return t3;
        else return t4;
    }

    
    // --------------------
    // DELAY
    // --------------------

    public static Coroutine Wait(this MonoBehaviour obj, float duration, Action then) => 
        obj.StartCoroutine(_Wait(duration, then));
    private static IEnumerator _Wait(float duration, Action then) {
        yield return new WaitForSeconds(duration);
        then();
    }

    public static Coroutine WaitOneFrame(this MonoBehaviour obj, Action then) => 
        obj.StartCoroutine(_WaitOneFrame(then));
    private static IEnumerator _WaitOneFrame(Action then) {
        yield return new WaitForEndOfFrame();
        then();
        yield return null;
    }

    public static Coroutine When(this MonoBehaviour obj, Func<bool> condition, Action then) =>
        obj.StartCoroutine(_When(condition, then));
    private static IEnumerator _When(Func<bool> condition, Action then) {
        while (!condition()) yield return new WaitForEndOfFrame();
        then();
        yield return null;
    }
        
    public static Coroutine While(this MonoBehaviour obj, Func<bool> condition, Action then, float delay = -1, 
        Action eventually = null) => obj.StartCoroutine(_While(condition, then, delay, eventually));
    private static IEnumerator _While(Func<bool> condition, Action then, float delay, Action eventually) {
        while (condition()) {
            if (delay < 0) yield return new WaitForEndOfFrame();
            else yield return new WaitForSeconds(delay);
            then();
        }
        eventually?.Invoke();
        yield return null;
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

    public static void MoveChildrenTo(this Component obj, Component newParent) {
        while (obj.transform.childCount > 0) obj.transform.GetChild(0).SetParent(newParent.transform);
    }
    
    public static void MoveChildrenTo(this Component obj, GameObject newParent) =>
        obj.MoveChildrenTo(newParent.transform);
    public static void MoveChildrenTo(this GameObject obj, Component newParent) =>
        obj.transform.MoveChildrenTo(newParent);
    public static void MoveChildrenTo(this GameObject obj, GameObject newParent) =>
        obj.MoveChildrenTo(newParent.transform);

    
    // --------------------
    // COLORS
    // --------------------

    public static T SetAlpha<T>(this T g, float alpha) where T : Graphic {         
        Color color = g.color;
        color.a = alpha;
        g.color = color;
        return g;
    }
    
    
    // --------------------
    // MISC
    // --------------------

    public static T If<T>(this T target, Func<T, bool> condition) => condition(target) ? target : default;
    
    public static float ReverseIf(this float target, bool condition) => target * (condition ? -1 : 1);
    public static float ReverseIf(this int target, bool condition) => target * (condition ? -1 : 1);
    
    public static float ToInt(this bool target) => target ? 1 : 0;
    
    
    // --------------------
    // TWEEN
    // --------------------

    public static Tween TweenPosition(this MonoBehaviour target, Vector3 targetValue, Tween.Style style, float duration, 
        Action onEnd = default, Tween.WhenDone whenDone = Tween.WhenDone.STOP, float restartDelay = 0) {
        return target.gameObject.AddComponent<Tween>().Init(
            targetValue, Tween.Property.POSITION, style, duration,
            whenDone, onEnd, restartDelay, null);
    }

    public static Tween TweenAlpha(this Graphic visuals, float targetValue, Tween.Style style, float duration, 
        Action onEnd = default, Tween.WhenDone whenDone = Tween.WhenDone.STOP, float restartDelay = 0) {
        return visuals.gameObject.AddComponent<Tween>().Init(
            targetValue * Vector3.right, Tween.Property.ALPHA, 
            style, duration, whenDone, onEnd, restartDelay, visuals);
    }
}