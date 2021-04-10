using System.Collections.Generic;
using UnityEngine;

public class UnitMonster : UnitSide {
    [Header("State")]
    public UnitData data;
    public List<Item> droppedItems;
    public int droppedXp;
    
    protected override void Init() { //Called before loading
        if (!unit.gameObject.activeInHierarchy) return;
            
        Unit.monsterUnits.Add(unit);
        unit.data.InitFrom(unit);
        unit.SetHealth(unit.data.currentHealth);

        while (data.level < Run.m.runLevel) unit.LevelUp();
    }

    protected override void OnDeath() {
        Destroy(unit.gameObject);

        droppedItems.ForEach(item => Unit.heroUnits
            .RandomWhere(u => u.hero.itemPrefabPaths.Count < Game.m.maxItemsPerHero)
            ?.hero
            ?.GetItemFromFight(item, transform.position));

        Vector3 pos = transform.position;
        Battle.m.Wait(.1f, () => 
            Battle.m.Repeat(times:droppedXp, 
                () => Unit.heroUnits.Random()?.hero?.GetXp(1, pos), 
                .1f));
    }

    protected override void OnDefeat() {
        Battle.m.Victory();
    }
}
