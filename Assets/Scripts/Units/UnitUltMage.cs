public class UnitUltMage : UnitUlt {
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_MAGE);
        unit.lockAnim = true;
        unit.lockPosition = true;
        unit.isInvincible = true;
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK);
        Unit.monsterUnits.ForEach(m => {
            this.Wait(Game.m.ultAnimDuration, () => m.GetBumpedBy(unit));
        });
    }

    public override void EndUlt() {
        unit.lockPosition = false;
        unit.lockAnim = false;
        unit.isInvincible = false;
    }
}