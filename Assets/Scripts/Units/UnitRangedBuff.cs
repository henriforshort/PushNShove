using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitRangedBuff : MonoBehaviour {
    [Header("Balancing")]
    public float buffDuration;
    public float attackAnimDuration;
    public UnitStat buffStat;
    public StatModifier.Type buffType;
    public float buffAmount;
    public bool targetAllAllies;
    
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

        if (targetAllAllies) unit.allies.Except(unit)?.ForEach(ApplyBuff);
        else ApplyBuff(unit.allies.Except(unit).Random());
    }

    public void ApplyBuff(Unit ally) {
        BuffFx targetFx = currentBuffs.FirstOrDefault(fx => fx.target == ally);
        if (targetFx == null) {
            BuffFx buffFx = Instantiate(buffFxPrefab, 
                new Vector3(ally.GetX(), ally.GetY() + .6f, 0f),
                Quaternion.identity, ally.transform);
            buffFx.Init(unit, ally, buffDuration, ally.data.stats[(int) buffStat]
                .AddModifier(buffAmount > 0 ? buffAmount : unit.data.stats[(int)buffStat], buffType));
            currentBuffs.Add(buffFx);
        } else {
            targetFx.durationLeft = buffDuration;
        }
    }
}