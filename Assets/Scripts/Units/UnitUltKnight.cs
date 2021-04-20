using System.Collections.Generic;
using UnityEngine;

public class UnitUltKnight : UnitUlt {
    [Header("Balancing")]
    public float healDelay;
    
    [Header("State")]
    public List<StatModifier> ultStatModifs = new List<StatModifier>();
    
    [Header("Prefab References")]
    public GameObject ultFx;

    public override void Ult() {
        Game.m.PlaySound(MedievalCombat.MAGIC_HOLY, .5f, 1);
        Unit.heroUnits.ForEach(u => ultStatModifs.Add(u.data.prot.AddModifier(0.3f)));
        this.While(() => unit.hero.ultStatus == UnitHero.UltStatus.ACTIVATED, 
            () => {
                if (Battle.m.gameState == Battle.State.PAUSE) return;
                
                float heal = (unit.data.damage * this.Random(.5f, 1.5f)).CoinFlipRound();
                Unit target = Unit.allHeroUnits.Random();
                if (target.status != Unit.Status.DEAD) target.AddHealth(heal, heal.ToString(), Game.m.yellow);
                Game.m.SpawnFX(ultFx, transform.position + new Vector3(this.Random(-1f, 1f), 0, -3));
            }, 
            healDelay);
    }

    public override void EndUlt() {
        ultStatModifs.ForEach(m => m.Terminate());
        ultStatModifs.Clear();
    }
}
