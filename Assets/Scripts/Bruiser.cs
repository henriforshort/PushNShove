using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bruiser : Unit {
    [Header("--------------------")]
    [Header("BRUISER VARIABLES", order = 2)]
    
    [Header("State", order = 3)]
    public bool ultActive;
    public float oldProt;
    public float oldStrength;
    public float oldDamage;
    public float oldWeight;
    public float oldAttackSpeed;
    public float oldAttackAnimDuration;
    public float oldCrit;

    public override void SetAnim(Anim a) {
        if (!ultActive) base.SetAnim(a);
    }

    public override void UpdateZ() {
        if (!ultActive) base.UpdateZ();
    }

    public override void Ult() {
        SetAnim(Anim.ULT_BRUISER);
        ultActive = true;

        oldProt = prot;
        oldStrength = strength;
        oldDamage = damage;
        oldWeight = weight;
        oldAttackSpeed = attackSpeed;
        oldCrit = critChance;
        oldAttackAnimDuration = attackAnimDuration;

        prot = 0.9f;
        // strength =
        weight *= 50;
        damage /= 3;
        attackSpeed = 0f;
        attackAnimDuration = 0;
        critChance /= 10;

        isInvincible = true;
        
        this.SetZ(-3);
        this.Wait(hero.ultDuration, EndUlt);
    }

    public override void EndUlt() {
        ultActive = false;
        SetAnim(Anim.DEFEND);

        prot = oldProt;
        strength = oldStrength;
        damage = oldDamage;
        weight = oldWeight;
        attackSpeed = oldAttackSpeed;
        critChance = oldCrit;
        attackAnimDuration = oldAttackAnimDuration;
        
        isInvincible = false;
    }
}