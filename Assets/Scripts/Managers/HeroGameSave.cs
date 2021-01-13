using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroGameSave {
    //Basic info
    public Hero battlePrefab;
    public int index; //Index in the Game.m.heroesPrefabs list
    
    //Data
    public UnitData data;
    
    public HeroRunSave runSave => Run.m.save.heroes.FirstOrDefault(h => h.prefabIndex == index);
    public CampHero campInstance => Camp.m.heroes[index];

    public HeroGameSave (int index, Hero battlePrefab) {
        this.battlePrefab = battlePrefab;
        this.index = index;
        data = new UnitData();
    }
    
    public void InitGame() {
        battlePrefab.unit.data.CopyTo(data);
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
        data.CopyTo(battlePrefab.unit.data);
        data.activity = UnitData.Activity.IDLE;
    }

    public void Save() => runSave.data.CopyTo(data); //Called at the end of each run
    public void Load() => data.CopyTo(runSave.data); //Called at the beginning of each run
    public void SaveCamp() => campInstance.data.CopyTo(data); //Called when leaving the camp
    public void LoadCamp() => data.CopyTo(campInstance.data); //Called when entering the camp
}