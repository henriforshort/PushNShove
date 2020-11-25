using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvUpMenu : MonoBehaviour {
    public Animator animator;
    public Anim anim;
    public bool available;
    public B.State previousState;
    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public float timeToFade;
    
    public enum Anim { IDLE = 0, SELECT1 = 1, SELECT2 = 2, SELECT3 = 3, SHINE = -1 }

    private void OnEnable() {
        available = true;
        Shine();
        previousState = B.m.gameState;
        B.m.gameState = B.State.PAUSE;
        button1.SetActive(true);
        button2.SetActive(true);
        button3.SetActive(true);
    }

    public void SetAnimation(Anim a) {
        anim = a;
        animator.SetInteger("Anim", (int)a);
    }

    public void Shine() {
        SetAnimation(Anim.SHINE);
    }

    public void Select1() {
        if (!available) return;
        SetAnimation(Anim.SELECT1);
        Close();
        button2.SetActive(false);
        button3.SetActive(false);
    }

    public void Select2() {
        if (!available) return;
        SetAnimation(Anim.SELECT2);
        Close();
        button1.SetActive(false);
        button3.SetActive(false);
    }

    public void Select3() {
        if (!available) return;
        SetAnimation(Anim.SELECT3);
        Close();
        button1.SetActive(false);
        button2.SetActive(false);
    }

    public void Close() {
        available = false;
        G.m.skillPoints--;
        B.m.lvUpNotif.SetActive(G.m.skillPoints != 0);
        this.Wait(2, () => {
            B.m.gameState = previousState;
            gameObject.SetActive(false);
        });
        // this.Wait(timeToFade, () => {
        //     button1.SetActive(false);
        //     button2.SetActive(false);
        //     button3.SetActive(false);
        // });
    }
}
