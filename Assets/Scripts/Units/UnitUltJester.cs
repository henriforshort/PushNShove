using UnityEngine;

public class UnitUltJester : UnitUlt {
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_JESTER);
        unit.lockAnim = true;
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, .5f, 2);
        unit.isInvincible = true;
    }

    public override void EndUlt() {
        unit.lockAnim = false;
        unit.SetAnim(Unit.Anim.DEFEND);
        unit.isInvincible = false;
    }
}