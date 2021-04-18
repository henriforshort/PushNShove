public class UnitUltMage : UnitUlt {
    public override void Ult() {
        unit.SetAnim(Unit.Anim.HIT);
        unit.lockAnim = true;
        unit.lockPosition = true;
        unit.isInvincible = true;
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK);
        Unit.heroUnits.ForEach(u => {
            float randomHealAmount = (unit.data.damage * 2 * this.Random(.5f, 1.5f)).Round();
            string uiText = "+" + randomHealAmount.AtMost(u.data.maxHealth - u.data.currentHealth);
            u.AddHealth(randomHealAmount, uiText, Game.m.grey, true);
        });
    }

    public override void EndUlt() {
        unit.lockPosition = false;
        unit.lockAnim = false;
        unit.isInvincible = false;
    }
}