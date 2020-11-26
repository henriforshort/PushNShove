using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDefenseUpgrade : Upgrade {
    public float hpGain;
    public bool alsoHeal;
    
    public override void Apply() {
        B.m.hero.maxHealth += hpGain;
        if (alsoHeal) G.m.s.heroHp += hpGain;
    }
}
