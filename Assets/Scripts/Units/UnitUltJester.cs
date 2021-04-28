using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitUltJester : UnitUlt {
    [Header("Balancing")]
    public float range;
    public float buffDuration;
    public float attackAnimDuration;
    public UnitStat buffStat;
    
    [Header("Status")]
    public List<BuffFx> currentBuffs;

    [Header("References")]
    public BuffFx buffFxPrefab;
    public Projectile projectilePrefab;
    public UnitRanged unitRanged;
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_JESTER);
        unit.lockAnim = true;
        unit.isInvincible = true;
        Game.m.PlaySound(MedievalCombat.WHOOSH_3);
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, .5f, 2);
        this.Wait(attackAnimDuration, ApplyBuff);
    }

    public override void EndUlt() {
        unit.lockAnim = false;
        unit.isInvincible = false;
        unit.SetAnim(Unit.Anim.DEFEND);
    }

    public void ApplyBuff() {
        if (unit.status != Unit.Status.ALIVE) return;

        //Hit all enemies
        // unit.enemies
        //     .Where(e => unit.DistanceToMe(e) < range)
        //     .ToList()
        //     .ForEach(e => e.GetBumpedBy(unit.data.critChance, 
        //         unit.data.damage * 2, unit.data.strength));
        
        //Hit one enemy
        // unit.enemies
        //     .WithLowest(e => unit.DistanceToMe(e))
        //     .If(e => unit.DistanceToMe(e) < Game.m.attackDistance * 2)
        //     ?.GetBumpedBy(unit.data.critChance, 
        // unit.data.damage * 2, unit.data.strength);

        //Buff allies
        // unit.allies
        //     .Except(unit)
        //     .ForEach(target => {
        //         BuffFx targetFx = currentBuffs.FirstOrDefault(fx => fx.target == target);
        //         if (targetFx == null) {
        //             BuffFx buffFx = Instantiate(buffFxPrefab, 
        //                 new Vector3(target.GetX(), target.GetY() + 1f, 0f),
        //                 Quaternion.identity, target.transform);
        //             buffFx.Init(unit, target, buffDuration, 
        //                 target.data.stats[(int) buffStat].AddModifier(unit.data.stats[(int)buffStat]));
        //             currentBuffs.Add(buffFx);
        //         } else {
        //             targetFx.durationLeft = buffDuration;
        //         }
        //     });
        
        //Send note
        Instantiate(projectilePrefab, 
            transform.position + new Vector3(13/36f, 21/36f), 
            Quaternion.identity, 
            Battle.m.transform)
            .Init(unit.data.critChance, unit.data.damage, unit.data.strength * 10);
    }
}