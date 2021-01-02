using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Strongman : Unit {
    [Header("--------------------")]
    [Header("STRONGMAN VARIABLES", order = 2)]
    [Header("Balancing", order = 3)]
    public float range;
    public List<StatModifier> ultStatModifs = new List<StatModifier>();

    public GameObject strongmanUltFx;
    
    
    public override void Ult() {
        SetAnim(Anim.ULT_STRONGMAN);
        lockAnim = true;
        lockPosition = true;
        this.Wait(hero.ultDuration, then:() => PatateDeForain(TargetInRange()));
    }

    public void PatateDeForain(Unit target) {
        lockAnim = false;
        SetAnim(Anim.HIT);
        ultStatModifs.Add(data.strength.AddModifier(1.25f, StatModifier.Type.MULTIPLY));
        ultStatModifs.Add(data.critChance.AddModifier(1, StatModifier.Type.SET));
        
        Game.m.SpawnFX(strongmanUltFx, 
            transform.position + new Vector3(3, 1, -1), false, 0.5f);
        if (target != null) {
            target.GetBumpedBy(this);
            DefendFrom(target);
        }

        lockPosition = false;
        ultStatModifs.ForEach(m => m.Terminate());
        ultStatModifs.Clear();
    }

    public Unit TargetInRange() {
        return enemies
            .Where(e => e.status == Status.ALIVE)
            .WithLowest(DistanceToMe)
            .If(e => DistanceToMe(e) < range);
    }

    public override void EndUlt() { }
}
