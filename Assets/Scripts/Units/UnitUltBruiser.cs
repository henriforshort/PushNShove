using System.Collections.Generic;
using UnityEngine;

public class UnitUltBruiser : UnitUlt {
    [Header("State")]
    public float oldAttackAnimDuration;
    public float oldAttackSpeed;
    public List<StatModifier> ultStatModifs = new List<StatModifier>();

    public override void Ult() {
        unit.SetAnim(Unit.Anim.PREPARE_ULT);
        this.Wait(1.1f, () => unit.SetAnim(Unit.Anim.ULT, true));
        unit.lockAnim = true;
        Game.m.PlaySound(MedievalCombat.WHOOSH_8);
        this.For(9, () => Game.m.PlaySound(MedievalCombat.WHOOSH_6), 0.5f);

        oldAttackAnimDuration = unit.melee.attackAnimDuration;
        oldAttackSpeed = unit.melee.attackSpeed;
        unit.melee.attackSpeed = 0f;
        unit.melee.attackAnimDuration = 0;
        unit.isInvincible = true;

        ultStatModifs.Add(unit.data.damage.AddModifier(0.5f, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(unit.data.critChance.AddModifier(0, StatModifier.Type.SET));
    }

    public override void EndUlt() {
        unit.lockAnim = false;
        unit.SetAnim(Unit.Anim.DEFEND);
        unit.melee.attackAnimDuration = oldAttackAnimDuration;
        unit.melee.attackSpeed = oldAttackSpeed;
        unit.isInvincible = false;
        ultStatModifs.ForEach(m => m.Terminate());
        ultStatModifs.Clear();
    }
}
