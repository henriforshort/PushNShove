using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Tween : MonoBehaviour {
    [Header("Balancing")]
    public Property property;
    public Style style;
    public WhenDone whenDone;
    public float duration;
    public ValueType valueType;
    public Vector3 value;
    public float restartDelay;
    public Action onEnd;

    [Header("Alternative Parameters")]
    // public float targetValue; //overrides amplitude if different from 0
    public Graphic visuals; //for alpha changes
    public SpriteRenderer sprite; //for alpha changes

    // [Header("State")]
    private bool playing;
    private bool reversed;
    private Vector3 startValue;
    private Vector3 targetValue;
    private Vector3 currentValue;
    [Range(0,1)] private float linearValue;

    public enum Property { VERTICAL, HORIZONTAL, POSITION, SCALE, ALPHA }
    public enum Style { LINEAR, EASE_IN, EASE_OUT, EASE_IN_OUT, SINE, BOUNCE }
    public enum WhenDone { RESTART, PINGPONG, STOP, DESTROY }
    public enum ValueType { TARGET, AMPLITUDE }

    public Tween InitByValue(Vector3 target, Property property, Style style, float duration, 
        WhenDone whenDone, Action onEnd, float restartDelay, Graphic visuals) {
        return Init(target, true, property, style, duration, whenDone, onEnd, restartDelay, visuals);
    }

    public Tween InitByAmplitude(Vector3 amplitude, Property property, Style style, float duration, 
        WhenDone whenDone, Action onEnd, float restartDelay, Graphic visuals) {
        return Init(amplitude, false, property, style, duration, whenDone, onEnd, restartDelay, visuals);
    }

    private Tween Init(Vector3 value, bool isTarget, Property property, Style style, float duration, 
        WhenDone whenDone, Action onEnd, float restartDelay, Graphic visuals) {
        valueType = isTarget ? ValueType.TARGET : ValueType.AMPLITUDE;
        this.value = value;
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
        if (property == Property.ALPHA) startValue.x = GetAlpha();
        if (property == Property.SCALE) startValue = transform.localScale;
        if (property == Property.POSITION) startValue = transform.position;

        if (valueType == ValueType.TARGET) targetValue = value;
        else targetValue = startValue + value;
    }

    public void Update() {
        if (!playing) return;
        
        linearValue += (Time.deltaTime / duration).ReverseIf(reversed);
        currentValue = Vector3.LerpUnclamped(startValue, targetValue, GetValue(linearValue));
        
        if (property == Property.HORIZONTAL) this.SetX(currentValue.x);
        if (property == Property.VERTICAL) this.SetY(currentValue.x);
        if (property == Property.ALPHA) SetAlpha(currentValue.x);
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

    public void SetAlpha(float v) {
        if (visuals != null) visuals.SetAlpha(v);
        else if (sprite != null) sprite.SetAlpha(v);
        else Debug.LogError("visuals not assigned");
    }

    public float GetAlpha() {
        if (visuals != null) return visuals.color.a;
        else if (sprite != null) return sprite.color.a;
        else Debug.LogError("visuals not assigned");
        return default;
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
