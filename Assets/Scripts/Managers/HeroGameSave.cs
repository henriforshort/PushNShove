using System;
using UnityEngine;

[Serializable]
public class HeroGameSave {
    //Basic info
    public string battlePrefabPath;
    public string campPrefabPath;
    public UnitHero battlePrefab => battlePrefabPath.ToPrefab<UnitHero>();
    public CampHero campPrefab => campPrefabPath.ToPrefab<CampHero>();
    public int prefabIndex; //Index of the prefab in Game.m.heroPrefabs
    
    //Data
    public UnitData data;
    
    public HeroGameSave (int prefabIndex, UnitHero battlePrefab) {
        this.prefabIndex = prefabIndex;
        battlePrefabPath = battlePrefab.ToPath("Heroes/");
        campPrefabPath = battlePrefab.campHero.ToPath("Camp/");
        data = new UnitData();
    }
    
    public void InitGame() {
        data.maxHealth.Init(battlePrefab.unit.baseMaxHealth);
        data.prot.Init(battlePrefab.unit.baseProt);
        data.weight.Init(battlePrefab.unit.baseWeight);
        data.damage.Init(battlePrefab.unit.baseDamage);
        data.strength.Init(battlePrefab.unit.baseStrength);
        data.critChance.Init(battlePrefab.unit.baseCritChance);
        data.currentHealth = battlePrefab.unit.data.maxHealth;
        data.ultCooldownLeft = 0;
        data.itemPrefabPaths.Clear();
        data.activity = CampActivity.Type.IDLE;
    }
}