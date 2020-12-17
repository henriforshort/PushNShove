using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tween : MonoBehaviour {
    [Header("Balancing")]
    public Style style;
    public Vector3 targetPosition;
    public float duration;
    public bool destroyWhenDone;
    public float easingPower;

    [Header("State")]
    public bool isOver;
    [Range(0,1)] public float currentValue;
    public Vector3 startPosition;
    public float startDate;

    [Header("References")]
    public AnimationCurve animationCurve;
    
    
    public enum Style { LINEAR, EASE_IN, EASE_OUT, BY_CURVE }

    public void Start() {
        startDate = Time.time;
        startPosition = transform.position;
        currentValue = 0;
    }

    private void Update() {
        if (isOver) return;

        float linearValue = Time.time.Prel(startDate, startDate + duration);
        if (style == Style.LINEAR) currentValue = linearValue;
        if (style == Style.EASE_IN) currentValue = linearValue.Pow(easingPower);
        if (style == Style.EASE_OUT) currentValue = 1 - (1 - linearValue).Pow(easingPower);
        if (style == Style.BY_CURVE) currentValue = animationCurve.Evaluate(linearValue);

        transform.position = Vector3.Lerp(startPosition, targetPosition, currentValue);
        currentValue = currentValue.Clamp01();
        if (currentValue.isAbout(1)) {
            if (destroyWhenDone) Destroy(gameObject);
            else isOver = true;
        }
    }
}
