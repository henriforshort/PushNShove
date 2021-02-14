using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMonster : UnitSide {
    [Header("Balancing")]
    [Range(0,1)] public float dropRate;
    
    [Header("State")]
    public UnitData data;
    
    public void Awake() {
        if (!unit.gameObject.activeInHierarchy) return;
            
        Unit.monsterUnits.Add(unit);
        unit.data.maxHealth.Init(unit.baseMaxHealth);
        unit.data.prot.Init(unit.baseProt);
        unit.data.weight.Init(unit.baseWeight);
        unit.data.damage.Init(unit.baseDamage);
        unit.data.strength.Init(unit.baseStrength);
        unit.data.critChance.Init(unit.baseCritChance);
        unit.SetHealth(unit.data.maxHealth);
    }

    public override void Die() {
        Destroy(unit.gameObject);

        if (!dropRate.Chance()) return;
        if (Run.m.itemsDepleted) return;
        
        Unit.heroUnits
            .RandomWhere(u => u.hero.itemPrefabPaths.Count < Game.m.maxItemsPerHero)
            ?.hero
            ?.GetItemFromFight(Run.m.GetRandomItem(), unit);
    }

    public override void GetDefeated() {
        Battle.m.Victory();
    }
}