using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransition : MonoBehaviour {
    public Anim startAnim;
    public Anim currentAnim;
    public Animator animator;
    
    public enum Anim { FADE_IN, BLACK, FADE_OUT }

    public void Start() {
        SetAnim(startAnim);
    }

    public void FadeIn() => SetAnim(Anim.FADE_IN);
    public void FadeOut() => SetAnim(Anim.FADE_OUT);

    public void SetAnim(Anim a) {
        currentAnim = a;
        animator.Play(a.ToString());
    }
}
