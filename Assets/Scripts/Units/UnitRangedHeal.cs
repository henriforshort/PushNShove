using System.Collections.Generic;
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

        Unit healTarget = unit.allies.WithLowest(u => u.tempHealth);
        Unit hurtTarget = unit.enemies.Random();
        
        if (this.CoinFlip()) HealAlly(healTarget);
        else HurtEnemy(hurtTarget);
    }

    public void HurtEnemy(Unit enemy) {
        if (enemy == null) return;
        
        enemy.GetBumpedBy(unit);
    }

    public void HealAlly(Unit ally) {
        if (ally == null) return;
        
        float randomHealAmount = (healAmount * this.Random(.5f, 1.5f)).Round();
        string uiText = "+" + randomHealAmount.AtMost(ally.data.maxHealth - ally.data.currentHealth);
        ally.AddHealth(randomHealAmount, uiText, Game.m.grey, true);
    }
}