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
        Game.m.PlaySound(MedievalCombat.WHOOSH_8);
        this.Repeat(9, () => Game.m.PlaySound(MedievalCombat.WHOOSH_6), 0.5f);

        oldAttackAnimDuration = attackAnimDuration;
        oldAttackSpeed = attackSpeed;
        attackSpeed = 0f;
        attackAnimDuration = 0;

        ultStatModifs.Add(data.prot.AddModifier(0.9f, StatModifier.Type.SET));
        ultStatModifs.Add(data.weight.AddModifier(50, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(data.damage.AddModifier(0.3f, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(data.critChance.AddModifier(0.1f, StatModifier.Type.MULTIPLY));

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