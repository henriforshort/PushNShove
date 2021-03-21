using System.Linq;
using UnityEngine;

public class UnitRangedHeal : MonoBehaviour {
    [Header("Balancing")]
    public float healAmount;
    public float attackAnimDuration;
    
    [Header("References")]
    public UnitRanged unitRanged;

    public Unit unit => unitRanged.unit;

    public void Awake() {
        unitRanged.OnAttack.AddListener(Heal);
    }

    public void Heal() {
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, .5f, 2);
        this.Wait(attackAnimDuration, ApplyBuff);
    }

    public void ApplyBuff() {
        if (unit.status != Unit.Status.ALIVE) return;

        Unit healTarget = unit.allies
            // .Except(unit)
            // .Where(a => a.data.maxHealth.value.isClearlyGreaterThan(a.data.currentHealth))
            .Random();
        if (healTarget == null) return;
        
        float randomHealAmount = (healAmount * this.Random(.5f, 1.5f)).Round();
        string uiText = "+" + randomHealAmount.AtMost(healTarget.data.maxHealth - healTarget.data.currentHealth);
        healTarget.AddHealth(randomHealAmount, uiText, Game.m.white, true);
    }
}