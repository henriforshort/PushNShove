using UnityEngine;

public class UnitUltMage : UnitUlt {
    [Header("Balancing")]
    public float tmpHealthAmount;
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.HIT);
        unit.lockAnim = true;
        unit.lockPosition = true;
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK);
        Unit.heroUnits.ForEach(u => {
            float randomHealAmount = (tmpHealthAmount * this.Random(.5f, 1.5f)).Round();
            string uiText = "+" + randomHealAmount.AtMost(u.data.maxHealth - u.data.currentHealth);
            u.AddHealth(randomHealAmount, uiText, Game.m.grey, true);
        });
    }

    public override void EndUlt() {
        unit.lockPosition = false;
        unit.lockAnim = false;
    }
}