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
                float heal = this.Random(1, 4);
                Unit randomUnit = Unit.allHeroUnits.Random();
                if (randomUnit.status != Unit.Status.DEAD) 
                    randomUnit.AddHealth(heal, heal.ToString(), Game.m.yellow);
                Game.m.SpawnFX(ultFx, 
                    transform.position + new Vector3(this.Random(-1f, 1f), 0, -3));
            }, 
            healDelay);
    }

    public override void EndUlt() {
        ultStatModifs.ForEach(m => m.Terminate());
        ultStatModifs.Clear();
    }
}
