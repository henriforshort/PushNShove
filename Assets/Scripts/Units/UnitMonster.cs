using UnityEngine;

public class UnitMonster : UnitSide {
    [Header("Balancing")]
    [Range(0,1)] public float dropRate;
    
    [Header("State")]
    public UnitData data;
    
    protected override void Init() { //Called before loading
        if (!unit.gameObject.activeInHierarchy) return;
            
        Unit.monsterUnits.Add(unit);
        unit.data.InitFrom(unit);
        unit.SetHealth(unit.data.currentHealth);
    }

    protected override void OnDeath() {
        Destroy(unit.gameObject);

        if (!dropRate.Chance()) return;
        if (Run.m.itemsDepleted) return;
        
        Unit.heroUnits
            .RandomWhere(u => u.hero.itemPrefabPaths.Count < Game.m.maxItemsPerHero)
            ?.hero
            ?.GetItemFromFight(Run.m.GetRandomItem(), unit);
    }

    protected override void OnDefeat() {
        Battle.m.Victory();
    }
}
