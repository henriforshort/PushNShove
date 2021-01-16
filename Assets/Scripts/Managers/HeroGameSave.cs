using System;
using UnityEngine;

[Serializable]
public class HeroGameSave {
    //Basic info
    public Hero battlePrefab;
    public CampHero campPrefab;
    public int prefabIndex; //Index of the prefab in Game.m.heroPrefabs
    
    //Data
    public UnitData data;
    
    public HeroGameSave (int prefabIndex, Hero battlePrefab) {
        this.battlePrefab = battlePrefab;
        this.prefabIndex = prefabIndex;
        this.campPrefab = battlePrefab.campHero;
        data = new UnitData();
    }
    
    public void InitGame() {
        data.maxSpeed.Init(battlePrefab.unit.baseMaxSpeed);
        data.maxHealth.Init(battlePrefab.unit.baseMaxHealth);
        data.prot.Init(battlePrefab.unit.baseProt);
        data.weight.Init(battlePrefab.unit.baseWeight);
        data.damage.Init(battlePrefab.unit.baseDamage);
        data.strength.Init(battlePrefab.unit.baseStrength);
        data.critChance.Init(battlePrefab.unit.baseCritChance);
        data.currentHealth = battlePrefab.unit.data.maxHealth;
        data.ultCooldownLeft = 0;
        data.itemPrefabs.Clear();
        data.activity = UnitData.Activity.IDLE;
    }
}