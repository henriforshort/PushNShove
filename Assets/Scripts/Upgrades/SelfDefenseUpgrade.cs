using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDefenseUpgrade : Upgrade {
    public float hpGain;
    public bool alsoHeal;
    
    public override void Apply() {
        Hero hero = B.m.heroes.Random();
        
        hero.unit.maxHealth += hpGain;
        
        if (alsoHeal) {
            hero.unit.AddHealth(hpGain);
            R.m.save.SaveHeroes();
        }
    }
}
