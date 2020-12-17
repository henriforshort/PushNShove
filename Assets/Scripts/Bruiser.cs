using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bruiser : Unit {
    [Header("--------------------")]
    [Header("BRUISER VARIABLES", order = 2)]
    
    [Header("State", order = 3)]
    public float oldProt;
    public float oldDamage;
    public float oldWeight;
    public float oldAttackSpeed;
    public float oldAttackAnimDuration;
    public float oldCrit;

    public override void SetAnim(Anim a) {
        if (hero.ultStatus != Hero.UltStatus.ACTIVATED) base.SetAnim(a);
    }

    public override void Ult() {;
        anim = Anim.ULT_BRUISER;
        PlayAnim();

        oldProt = prot;
        oldDamage = damage;
        oldWeight = weight;
        oldAttackSpeed = attackSpeed;
        oldCrit = critChance;
        oldAttackAnimDuration = attackAnimDuration;

        prot = 0.9f;
        weight *= 50;
        damage /= 3;
        attackSpeed = 0f;
        attackAnimDuration = 0;
        critChance /= 10;

        isInvincible = true;
    }

    public override void EndUlt() {
        SetAnim(Anim.DEFEND);

        prot = oldProt;
        damage = oldDamage;
        weight = oldWeight;
        attackSpeed = oldAttackSpeed;
        critChance = oldCrit;
        attackAnimDuration = oldAttackAnimDuration;
        
        isInvincible = false;
    }
}