using System;
using UnityEngine;

[Serializable]
public class HeroGameSave {
    //Basic info
    public Hero prefab;
    public int index; //Index in the Game.m.heroesPrefabs list
    
    //Data
    public UnitData data;

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
        data.CopyTo(prefab.unit.data);
    }

    public void Save() {
        Run.m.save.heroes[index].data.CopyTo(data);
    }

    public void Load() {
        data.CopyTo(Run.m.save.heroes[index].data);
    }
}