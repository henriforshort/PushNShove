using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDefenseUpgrade : Upgrade {
    public float hpGain;
    public bool alsoHeal;
    
    public override void Apply() {
        Unit hero = B.m.heroes.Random();
        
        hero.maxHealth += hpGain;
        
        if (alsoHeal) {
            hero.AddHealth(hpGain);
            G.m.s.SaveHeroes();
        }
    }
}
