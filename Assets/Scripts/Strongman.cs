﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Strongman : Unit {
    [Header("--------------------")]
    [Header("STRONGMAN VARIABLES", order = 2)]
    [Header("Balancing", order = 3)]
    public float strengthIncrease;

    public GameObject strongmanUltFx;
    
    
    public override void Ult() {
        SetAnim(Anim.ULT_STRONGMAN);
        lockAnim = true;
        lockPosition = true;
        this.Wait(hero.ultDuration, then:() => PatateDeForain(TargetInRange()));
    }

    public void PatateDeForain(Unit target) {
        lockAnim = false;
        SetAnim(Anim.HIT);
        strength *= (1 + strengthIncrease);
        float oldCrit = critChance;
        critChance = 1;
        
        target.GetBumpedBy(this);
        DefendFrom(target);
        B.m.SpawnFX(strongmanUltFx, transform.position + new Vector3(3, 0, -1), 
            false, B.m.transform, 0.5f);

        lockPosition = false;
        critChance = oldCrit;
        strength /= (1 + strengthIncrease);
    }

    public Unit TargetInRange() {
        return enemies
            .Where(e => e.status == Status.ALIVE)
            .WithLowest(DistanceToMe);
    }

    public override void EndUlt() { }
}
