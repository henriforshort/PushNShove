using System.Linq;
using UnityEngine;

public class UnitRangedHeal : MonoBehaviour {
    [Header("Balancing")]
    public float attackAnimDuration;
    
    [Header("References")]
    public UnitRanged unitRanged;

    public Unit unit => unitRanged.unit;

    public void Awake() {
        unitRanged.OnAttack.AddListener(Heal);
    }

    public void Heal() {
        this.Wait(attackAnimDuration, ApplyBuff);
    }

    public void ApplyBuff() {
        if (unit.status != Unit.Status.ALIVE) return;
        
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, .25f, 2);
        Unit healTarget = unit.allies
            .Where(u => u.tempHealth <= unit.data.damage * 2)
            .WithLowest(u => (u.data.currentHealth + u.tempHealth)/u.data.maxHealth);
        Unit hurtTarget = unit.enemies.Random();
        
        if (healTarget != null && this.CoinFlip()) HealAlly(healTarget);
        else HurtEnemy(hurtTarget);
    }

    public void HealAlly(Unit ally) {
        ally.AddTmpHealth((unit.data.damage * this.Random(.5f, 1.5f)).Round());
    }

    public void HurtEnemy(Unit enemy) => enemy?.GetBumpedBy(unit);
}