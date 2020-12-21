using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Knight : Unit {
    [Header("--------------------")]
    [Header("Knight VARIABLES", order = 2)]
    [Header("Balancing", order = 3)]
    public float healDelay;
    public float healFxDuration;
    public float protBuff;
    
    [Header("Prefabs")]
    public GameObject ultFx;

    public override void Ult() {
        heroUnits.ForEach(u => u.prot += protBuff);
        this.While(() => hero.ultStatus == Hero.UltStatus.ACTIVATED, 
            () => {
                float heal = this.Random(3, 7);
                Unit randomUnit = heroUnits.Random();
                if (randomUnit.status != Status.DEAD) randomUnit.AddHealth(heal, heal.ToString(), G.m.yellow);
                B.m.SpawnFX(ultFx,
                    transform.position + new Vector3(this.Random(-1f, 1f), -1, -3),
                    false,
                    B.m.transform,
                    healFxDuration);
            }, 
            healDelay);
    }

    public override void EndUlt() {
        heroUnits.ForEach(u => u.prot -= protBuff);
    }
}
