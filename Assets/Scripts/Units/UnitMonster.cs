using System.Collections.Generic;
using UnityEngine;

public class UnitMonster : UnitSide {
    [Header("State")]
    public UnitData data;
    public List<Item> droppedItems;
    public int droppedXp;
    public int droppedGems;
    
    protected override void Init() { //Called before loading
        if (!unit.gameObject.activeInHierarchy) return;
            
        Unit.monsterUnits.Add(unit);
        unit.data.InitFrom(unit);
        unit.SetHealth(unit.data.currentHealth);

        while (data.level < Run.m.runLevel) unit.LevelUp();
    }

    protected override void OnDeath() {
        DropLoot();
        Destroy(unit.gameObject);
    }

    protected override void OnDefeat() {
        Battle.m.Victory();
    }

    public void DropLoot() {
        if (Unit.heroUnits.Count == 0) return;

        droppedItems.ForEach(item => Unit.heroUnits
            ?.RandomWhere(u => u.hero.itemPrefabPaths.Count < Game.m.maxItemsPerHero)
            ?.hero
            ?.GetItemFromFight(item, transform.position));

        Vector3 pos = transform.position; //Needs to be defined in advance, before I die
        this.For(droppedXp, i => {
            UnitHero hero = Unit.heroUnits.Random().hero;
            float xpValue = Game.m.levelUpXpGainedIncrease.Pow(Run.m.runLevel);
            hero.predictiveXp += xpValue;
            Battle.m.Wait(.1f * (i + 1), () => hero.GetXp(xpValue, pos));
        });

        this.For(droppedGems, i => {
            Battle.m.OnBattleEnd.AddListener(Game.m.AddOneGem);
            Battle.m.Wait(.1f * i, () => Battle.m.LootOneGem(pos.SetY(-2.75f)));
        });
    }
}
