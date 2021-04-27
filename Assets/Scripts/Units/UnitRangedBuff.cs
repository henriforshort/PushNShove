using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitRangedBuff : MonoBehaviour {
    [Header("Balancing")]
    public float buffDuration;
    public float attackAnimDuration;
    public UnitStat buffStat;
    
    [Header("Status")]
    public List<BuffFx> currentBuffs;

    [Header("References")]
    public BuffFx buffFxPrefab;
    public UnitRanged unitRanged;

    public Unit unit => unitRanged.unit;

    public void Awake() {
        unitRanged.OnAttack.AddListener(Attack);
    }

    public void Attack() {
        this.Wait(attackAnimDuration, ApplyBuff);
    }

    public void ApplyBuff() {
        if (unit.status != Unit.Status.ALIVE) return;

        // //Hit all enemies
        // unit.enemies
        //     .Where(e => unit.DistanceToMe(e) < Game.m.attackDistance * 2)
        //     .ToList()
        //     .ForEach(e => e.GetBumpedBy(unit));
        
        // Hit one enemy
        unit.enemies
            .WithLowest(e => unit.DistanceToMe(e))
            .If(e => unit.DistanceToMe(e) < Game.m.attackDistance * 2)
            ?.GetBumpedBy(unit);

        unit.allies
            .Except(unit)
            ?.ForEach(target => {
                BuffFx targetFx = currentBuffs.FirstOrDefault(fx => fx.target == target);
                if (targetFx == null) {
                    BuffFx buffFx = Instantiate(buffFxPrefab, new Vector3(target.GetX(), target.GetY() + .7f, 0f),
                        Quaternion.identity, target.transform);
                    buffFx.Init(unit, target, buffDuration, 
                        target.data.stats[(int) buffStat].AddModifier(unit.data.stats[(int)buffStat]));
                    currentBuffs.Add(buffFx);
                } else {
                    targetFx.durationLeft = buffDuration;
                }
            });
    }
}