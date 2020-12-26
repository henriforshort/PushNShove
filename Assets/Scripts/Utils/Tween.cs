using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tween : MonoBehaviour {
    [Header("Balancing")]
    public Property property;
    public Style style;
    public WhenDone whenDone;
    public float duration;
    public float restartDelay;
    public Vector3 targetValue;
    public Action onEnd;

    [Header("Alternative Parameters")]
    // public float targetValue; //overrides amplitude if different from 0
    public Graphic visuals; //for alpha changes

    [Header("State")]
    public bool playing;
    public bool reversed;
    public Vector3 startValue;
    public Vector3 currentValue;
    [Range(0,1)] public float linearValue;

    public enum Property { VERTICAL, HORIZONTAL, POSITION, SCALE, ALPHA }
    public enum Style { LINEAR, EASE_IN, EASE_OUT, EASE_IN_OUT, SINE, BOUNCE }
    public enum WhenDone { RESTART, PINGPONG, STOP, DESTROY }

    public Tween Init(Vector3 targetValue, Property property, Style style, float duration, 
        WhenDone whenDone, Action onEnd, float restartDelay, Graphic visuals) {
        this.targetValue = targetValue;
        this.property = property;
        this.style = style;
        this.duration = duration;
        this.whenDone = whenDone;
        this.onEnd = onEnd;
        this.restartDelay = restartDelay;
        this.visuals = visuals;
        Start();
        return this;
    }

    public void Start() {
        linearValue = 0;
        reversed = false;
        playing = true;

        if (property == Property.HORIZONTAL) startValue.x = transform.position.x;
        if (property == Property.VERTICAL) startValue.x = transform.position.y;
        if (property == Property.ALPHA) startValue.x = visuals.color.a;
        if (property == Property.SCALE) startValue = transform.localScale;
        if (property == Property.POSITION) startValue = transform.position;
    }

    public void Update() {
        if (!playing) return;
        
        linearValue += (Time.deltaTime / duration).ReverseIf(reversed);
        currentValue = Vector3.LerpUnclamped(startValue, targetValue, GetValue(linearValue));
        
        if (property == Property.HORIZONTAL) this.SetX(currentValue.x);
        if (property == Property.VERTICAL) this.SetY(currentValue.x);
        if (property == Property.ALPHA) visuals.SetAlpha(currentValue.x);
        if (property == Property.SCALE) transform.localScale = currentValue;
        if (property == Property.POSITION) transform.position = currentValue;

        if (linearValue > 1 || linearValue < 0) {
            if (onEnd != null) onEnd();
            if (whenDone == WhenDone.RESTART) { linearValue -= 1; Restart(); }
            if (whenDone == WhenDone.PINGPONG) { reversed = !reversed; Restart(); }
            if (whenDone == WhenDone.STOP) playing = false;
            if (whenDone == WhenDone.DESTROY) Destroy(gameObject);
        }
    }

    public void MaxOut() => linearValue = reversed ? 0 : 1;

    public float GetValue(float x) {
        
        if (style == Style.SINE) return Sine(x);
        if (style == Style.LINEAR) return Linear(x);
        if (style == Style.EASE_IN) return EaseIn(x);
        if (style == Style.EASE_OUT) return EaseOut(x);
        if (style == Style.EASE_IN_OUT) return EaseInOut(x);
        if (style == Style.BOUNCE) return Bounce(x);
        return 0;
    }

    public float Sine(float x) => Mathf.Sin(x * 2 * Mathf.PI);
    public float Linear(float x) => x;
    public float EaseIn(float x) => x.isAbout(0) ? 0 : 2.Pow(10 * (x - 1));
    public float EaseOut(float x) => x.isAbout(1) ? 1 : 1 - 2.Pow(-10 * x);
    public float EaseInOut(float x) => x < 0.5f ? 0.5f * EaseIn(2 * x) : 0.5f * (1 + EaseOut(2 * x - 1));
    public float Bounce(float x) => -4 * (x - 0.5f).Pow(2) + 1;

    public void Restart() {
        playing = false;
        this.Wait(restartDelay, () => playing = true);
    }
}



namespace SampleApp {
    public delegate string MyDel(string str);
	
    class EventProgram {
        event MyDel MyEvent;
		
        public EventProgram() {
            MyEvent += WelcomeUser;
        }
        public string WelcomeUser(string username) {
            return "Welcome " + username;
        }
        
        static void Main(string[] args) {
            Console.WriteLine(new EventProgram().MyEvent("Tutorials Point"));
        }
    }
}
