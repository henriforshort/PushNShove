using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroGameSave {
    //Basic info
    public Hero prefab;
    public int index; //Index in the Game.m.heroesPrefabs list
    
    //Data
    public UnitData data;
    
    public HeroRunSave runSave => Run.m.save.heroes.First(h => h.prefabIndex == index);

    public HeroGameSave (int index, Hero prefab) {
        this.prefab = prefab;
        this.index = index;
        data = new UnitData();
    }
    
    public void InitGame() {
        prefab.unit.data.CopyTo(data);
        data.maxSpeed.Init(prefab.unit.baseMaxSpeed);
        data.maxHealth.Init(prefab.unit.baseMaxHealth);
        data.prot.Init(prefab.unit.baseProt);
        data.weight.Init(prefab.unit.baseWeight);
        data.damage.Init(prefab.unit.baseDamage);
        data.strength.Init(prefab.unit.baseStrength);
        data.critChance.Init(prefab.unit.baseCritChance);
        data.currentHealth = prefab.unit.data.maxHealth;
        data.ultCooldownLeft = 0;
        data.itemPrefabs.Clear();
        data.CopyTo(prefab.unit.data);
    }

    public void Save() {
        runSave.data.CopyTo(data);
    }

    public void Load() {
        data.CopyTo(runSave.data);
    }
}