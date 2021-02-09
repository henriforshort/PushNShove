using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUltBruiser : UnitUlt {
    [Header("State")]
    public float oldAttackAnimDuration;
    public float oldAttackSpeed;
    public List<StatModifier> ultStatModifs = new List<StatModifier>();

    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_BRUISER);
        unit.lockAnim = true;
        Game.m.PlaySound(MedievalCombat.WHOOSH_8);
        this.Repeat(9, () => Game.m.PlaySound(MedievalCombat.WHOOSH_6), 0.5f);

        oldAttackAnimDuration = unit.attackAnimDuration;
        oldAttackSpeed = unit.attackSpeed;
        unit.attackSpeed = 0f;
        unit.attackAnimDuration = 0;

        ultStatModifs.Add(unit.data.prot.AddModifier(0.9f, StatModifier.Type.SET));
        ultStatModifs.Add(unit.data.weight.AddModifier(50, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(unit.data.damage.AddModifier(0.3f, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(unit.data.critChance.AddModifier(0.1f, StatModifier.Type.MULTIPLY));

        unit.isInvincible = true;
    }

    public override void EndUlt() {
        unit.lockAnim = false;
        unit.SetAnim(Unit.Anim.DEFEND);
        unit.attackAnimDuration = oldAttackAnimDuration;
        unit.attackSpeed = oldAttackSpeed;
        ultStatModifs.ForEach(m => m.Terminate());
        ultStatModifs.Clear();
        
        unit.isInvincible = false;
    }
}
