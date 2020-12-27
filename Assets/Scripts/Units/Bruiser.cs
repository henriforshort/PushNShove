using System.Collections.Generic;
using UnityEngine;

public class Bruiser : Unit {
    [Header("--------------------")]
    [Header("BRUISER VARIABLES", order = 2)]
    
    [Header("State", order = 3)]
    public float oldAttackAnimDuration;
    public float oldAttackSpeed;
    public List<StatModifier> ultStatModifs = new List<StatModifier>();

    public override void Ult() {
        SetAnim(Anim.ULT_BRUISER);
        lockAnim = true;

        oldAttackAnimDuration = attackAnimDuration;
        oldAttackSpeed = attackSpeed;
        attackSpeed = 0f;
        attackAnimDuration = 0;

        ultStatModifs.Add(prot.AddModifier(0.9f, StatModifier.Type.SET));
        ultStatModifs.Add(weight.AddModifier(50, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(damage.AddModifier(0.3f, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(critChance.AddModifier(0.1f, StatModifier.Type.MULTIPLY));

        isInvincible = true;
    }

    public override void EndUlt() {
        lockAnim = false;
        SetAnim(Anim.DEFEND);
        attackAnimDuration = oldAttackAnimDuration;
        attackSpeed = oldAttackSpeed;
        ultStatModifs.ForEach(m => m.Terminate());
        ultStatModifs.Clear();
        
        isInvincible = false;
    }
}