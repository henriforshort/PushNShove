using System.Collections.Generic;
using UnityEngine;

public class Knight : Unit {
    [Header("--------------------")]
    [Header("KNIGHT VARIABLES", order = 2)]
    [Header("Balancing", order = 3)]
    public float healDelay;
    public List<StatModifier> ultStatModifs = new List<StatModifier>();
    
    [Header("Prefabs")]
    public GameObject ultFx;

    public override void Ult() {
        heroUnits.ForEach(u => ultStatModifs.Add(u.data.prot.AddModifier(0.3f)));
        this.While(() => hero.ultStatus == Hero.UltStatus.ACTIVATED, 
            () => {
                float heal = this.Random(1, 4);
                Unit randomUnit = allHeroUnits.Random();
                if (randomUnit.status != Status.DEAD) randomUnit.AddHealth(heal, heal.ToString(), Game.m.yellow);
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
