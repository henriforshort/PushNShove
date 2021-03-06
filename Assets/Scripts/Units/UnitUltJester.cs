using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitUltJester : UnitUlt {
    [Header("Balancing")]
    public float range;
    public float buffDuration;
    public float attackAnimDuration;
    public UnitStat buffStat;
    public StatModifier.Type buffType;
    public float buffAmount;
    
    // [Header("Status")]

    [Header("References")]
    public GameObject buffFx;
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_JESTER);
        unit.lockAnim = true;
        unit.isInvincible = true;
        Game.m.PlaySound(MedievalCombat.WHOOSH_3);
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, .5f, 2);
        List<StatModifier> currentModifiers = null;
        this.Wait(attackAnimDuration, () => currentModifiers = ApplyBuff());
        this.Wait(buffDuration, () => currentModifiers?.ForEach(m => m.Terminate()));
    }

    public override void EndUlt() {
        unit.lockAnim = false;
        unit.isInvincible = false;
        unit.SetAnim(Unit.Anim.DEFEND);
    }

    public List<StatModifier> ApplyBuff() {
        if (unit.status != Unit.Status.ALIVE) return null;

        //Hit all enemies
        unit.enemies
            .Where(e => unit.DistanceToMe(e) < range)
            .ToList()
            .ForEach(e => e.GetBumpedBy(unit));
        
        //Hit one enemy
        // unit.enemies
        //     .WithLowest(e => unit.DistanceToMe(e))
        //     .If(e => unit.DistanceToMe(e) < Game.m.attackDistance * 2)
        //     ?.GetBumpedBy(unit);

        return unit.allies
            .Except(unit)
            .Select(target => {
                Game.m.SpawnFX(buffFx, new Vector3(target.GetX(), target.GetY() + 1f, 8f),
                    false, buffDuration, target.transform);
                StatModifier buff =  target.data.stats[(int) buffStat].AddModifier(buffAmount, buffType);
                unit.onDeath.AddListener(() => buff?.Terminate());
                return buff;
            })
            .ToList();
    }
}