using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitUltStrongman : UnitUlt {
    [Header("Balancing")]
    public float range;
    
    [Header("State")]
    public List<StatModifier> ultStatModifs = new List<StatModifier>();

    [Header("Prefab References")]
    public GameObject strongmanUltFx;
    
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_STRONGMAN);
        unit.lockAnim = true;
        unit.lockPosition = true;
        this.Repeat(3, () => Game.m.PlaySound(MedievalCombat.WHOOSH_1), 0.1f);
        this.Wait(unit.hero.ultDuration, then:() => PatateDeForain(TargetInRange()));
    }

    public void PatateDeForain(Unit target) {
        Game.m.PlaySound(MedievalCombat.REALISTIC_PUNCH_1);
        Game.m.PlaySound(MedievalCombat.BODY_FALL);
        unit.lockAnim = false;
        unit.SetAnim(Unit.Anim.HIT);
        ultStatModifs.Add(unit.data.strength.AddModifier(1.25f, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(unit.data.critChance.AddModifier(1, StatModifier.Type.SET));
        
        Game.m.SpawnFX(strongmanUltFx, 
            transform.position + new Vector3(3, 1, -1), false, 0.5f);
        if (target != null) {
            target.GetBumpedBy(unit);
            unit.melee.DefendFrom(target);
        }

        unit.lockPosition = false;
        ultStatModifs.ForEach(m => m.Terminate());
        ultStatModifs.Clear();
    }

    public Unit TargetInRange() {
        return unit.enemies
            .Where(e => e.status == Unit.Status.ALIVE)
            .WithLowest(unit.melee.DistanceToMe)
            .If(e => unit.melee.DistanceToMe(e) < range);
    }
}
