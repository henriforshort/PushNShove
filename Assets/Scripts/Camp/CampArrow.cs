using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampArrow : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public bool locked;
    
    public void Update() {
        if (locked) this.SetX(Camp.m.selectedHero.GetX());
        if (!locked) this.LerpXTo(Camp.m.selectedHero.GetX(), 10);

        if (!locked && this.GetX().isAbout(Camp.m.selectedHero.GetX(), 0.05f)) locked = true;
    }
}