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
    public float amplitude;
    public bool playBackwards;
    public float restartDelay;

    [Header("Alternative Parameters")]
    public float targetValue; //overrides amplitude if different from 0
    public Graphic graphicComponent; //for alpha changes

    [Header("State")]
    public bool playing;
    public bool reversed;
    public float startValue;
    public float currentValue;
    [Range(0,1)] public float linearValue;

    public enum Property { VERTICAL, HORIZONTAL, SCALE, ALPHA }
    public enum Style { LINEAR, EASE_IN, EASE_OUT, EASE_IN_OUT, SINE, BOUNCE }
    public enum WhenDone { RESTART, PINGPONG, STOP, DESTROY }

    public void Start() {
        linearValue = 0;
        reversed = false;
        playing = true;

        if (property == Property.HORIZONTAL) startValue = transform.position.x;
        if (property == Property.VERTICAL) startValue = transform.position.y;
        if (property == Property.ALPHA) startValue = graphicComponent.color.a;
        if (property == Property.SCALE) startValue = transform.localScale.x;

        if (targetValue != 0) amplitude = targetValue - startValue;
    }

    private void Update() {
        if (!playing) return;
        
        linearValue += (Time.deltaTime / duration).ReverseIf(reversed);
        currentValue = startValue + (amplitude * GetValue(linearValue));
        
        if (property == Property.HORIZONTAL) this.SetX(currentValue);
        if (property == Property.VERTICAL) this.SetY(currentValue);
        if (property == Property.ALPHA) graphicComponent.SetAlpha(currentValue);
        if (property == Property.SCALE) transform.localScale = currentValue / startValue * Vector3.one;

        if (linearValue > 1 || linearValue < 0) {
            if (whenDone == WhenDone.RESTART) { linearValue -= 1; Restart(); }
            if (whenDone == WhenDone.PINGPONG) { reversed = !reversed; Restart(); }
            if (whenDone == WhenDone.STOP) playing = false;
            if (whenDone == WhenDone.DESTROY) Destroy(gameObject);
        }
    }

    public float GetValue(float x) {
        if (playBackwards) x = 1 - x;
        
        if (style == Style.SINE) return Sine(x);
        if (style == Style.LINEAR) return Linear(x);
        if (style == Style.EASE_IN) return EaseIn(x);
        if (style == Style.EASE_OUT) return EaseOut(x);
        if (style == Style.EASE_IN_OUT) return EaseInOut(x);
        if (style == Style.BOUNCE) return Bounce(x);
        return 0;
    }
    
    public float Sine(float x) => Mathf.Sin(x*2*Mathf.PI) ;
    public float Linear(float x) => x;
    public float EaseIn(float x) => x.isAbout(0) ? 0 : 2.Pow(10*(x - 1));
    public float EaseOut(float x) => x.isAbout(1) ? 1 : 1 - 2.Pow(-10*x);
    public float EaseInOut(float x) => x < 0.5f ? 0.5f*EaseIn(2*x) : 0.5f*(1 + EaseOut(2*x - 1));
    public float Bounce(float x) => x < 0.5f ? 2*x : -2*x;

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
