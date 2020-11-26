using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvUpMenu : MonoBehaviour {
    public Animator animator;
    public Anim anim;
    public bool available;
    public B.State previousState;
    public Upgrade upgrade1;
    public Upgrade upgrade2;
    public Upgrade upgrade3;
    public float timeToFade;
    public float timeToShow1;
    public float timeToShow2;
    public float timeToShow3;
    public enum Anim { IDLE = 0, SELECT1 = 1, SELECT2 = 2, SELECT3 = 3, SHINE = -1 }

    private void OnEnable() {
        available = true;
        Shine();
        previousState = B.m.gameState;
        B.m.gameState = B.State.PAUSE;
        upgrade1.gameObject.SetActive(false);
        upgrade2.gameObject.SetActive(false);
        upgrade3.gameObject.SetActive(false);
        this.Wait(timeToShow1, () => upgrade1.gameObject.SetActive(true));
        this.Wait(timeToShow2, () => upgrade2.gameObject.SetActive(true));
        this.Wait(timeToShow3, () => upgrade3.gameObject.SetActive(true));
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
        upgrade1.Apply();
        
        Close();
        upgrade2.gameObject.SetActive(false);
        upgrade3.gameObject.SetActive(false);
    }

    public void Select2() {
        if (!available) return;
        SetAnimation(Anim.SELECT2);
        upgrade2.Apply();

        Close();
        upgrade1.gameObject.SetActive(false);
        upgrade3.gameObject.SetActive(false);
    }

    public void Select3() {
        if (!available) return;
        SetAnimation(Anim.SELECT3);
        upgrade3.Apply();

        Close();
        upgrade1.gameObject.SetActive(false);
        upgrade2.gameObject.SetActive(false);
    }

    public void Close() {
        available = false;
        G.m.s.skillPoints--;
        B.m.lvUpNotif.SetActive(G.m.s.skillPoints > 0);
        this.Wait(2, () => {
            B.m.gameState = previousState;
            gameObject.SetActive(false);
        });
        this.Wait(timeToFade, () => {
            upgrade1.gameObject.SetActive(false);
            upgrade2.gameObject.SetActive(false);
            upgrade3.gameObject.SetActive(false);
        });
    }
}
