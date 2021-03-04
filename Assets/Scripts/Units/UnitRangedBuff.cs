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
    
    // [Header("Status")]

    [Header("References")]
    public GameObject buffFx;
    public UnitRanged unitRanged;

    public Unit unit => unitRanged.unit;

    public void Awake() {
        unitRanged.OnAttack.AddListener(Attack);
    }

    public void Attack() {
        List<StatModifier> currentModifiers = null;
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, .5f, 2);
        this.Wait(attackAnimDuration, () => currentModifiers = ApplyBuff());
        this.Wait(buffDuration, () => currentModifiers?.ForEach(m => m.Terminate()));
    }

    public List<StatModifier> ApplyBuff() {
        if (unit.status != Unit.Status.ALIVE) return null;

        //Hit all enemies
        unit.enemies
            .Where(e => unit.DistanceToMe(e) < Game.m.attackDistance * 2)
            .ToList()
            .ForEach(e => e.GetBumpedBy(unit));
        
        //Hit one enemy
        // unit.enemies
        //     .WithLowest(e => unit.DistanceToMe(e))
        //     .If(e => unit.DistanceToMe(e) < Game.m.attackDistance * 2)
        //     ?.GetBumpedBy(unit);

        List<StatModifier> modifiers = new List<StatModifier>();
        unit.allies
            .Except(unit)
            .ForEach(target => {
                Game.m.SpawnFX(buffFx, new Vector3(target.GetX(), target.GetY() + .7f, 8f), 
                    false, buffDuration, target.transform);
                modifiers.Add(target.data.stats[(int)buffStat].AddModifier(buffAmount, buffType));
        });
        
        return modifiers;
    }
}