using System.Linq;
using UnityEngine;

public class UnitUltStrongman : UnitUlt {
    [Header("Balancing")]
    public float range;

    [Header("Prefab References")]
    public GameObject strongmanUltFx;
    
    
    public override void Ult() {
        unit.SetAnim(Unit.Anim.ULT_STRONGMAN);
        unit.lockAnim = true;
        unit.lockPosition = true;
        this.For(3, () => Game.m.PlaySound(MedievalCombat.WHOOSH_1), 0.1f);
        float now = Time.time;
        this.When(() => (Time.time > now+unit.hero.ultDuration) && (Battle.m.gameState == Battle.State.PLAYING), 
            then:() => PatateDeForain(TargetInRange()));
    }

    public void PatateDeForain(Unit target) {
        Game.m.PlaySound(MedievalCombat.REALISTIC_PUNCH_1);
        Game.m.PlaySound(MedievalCombat.BODY_FALL);
        unit.lockAnim = false;
        unit.lockPosition = false;
        unit.SetAnim(Unit.Anim.HIT);
        
        Game.m.SpawnFX(strongmanUltFx, 
            transform.position + new Vector3(3, 1, -1), false, 0.5f);
        if (target != null) {
            target.GetBumpedBy(1, unit.data.damage * 2, unit.data.strength);
            unit.currentSpeed = Game.m.defendSpeed;
        }
    }

    public Unit TargetInRange() {
        return unit.enemies
            .Where(e => e.status == Unit.Status.ALIVE)
            .WithLowest(unit.DistanceToMe)
            .If(e => unit.DistanceToMe(e) < range);
    }
}
