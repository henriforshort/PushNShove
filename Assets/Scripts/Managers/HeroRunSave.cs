using System;
using UnityEngine;

[Serializable]
public class HeroRunSave {
    //Basic info
    public Hero prefab;
    public int prefabIndex; //Index of the prefab in the Game.m.heroPrefabs list
    public int instanceIndex; //Index of the instance in the Unit.allHeroUnits list
    
    //Data
    public UnitData data;

    public Hero instance => Unit.allHeroUnits.Get(instanceIndex)?.hero;

    public HeroRunSave (int prefabIndex, int instanceIndex, Hero prefab, UnitData unitData) {//prefab vs instance index
        this.prefabIndex = prefabIndex;
        this.instanceIndex = instanceIndex;
        this.prefab = prefab;
        
        data = new UnitData();
        unitData.CopyTo(data);
    }
    
    public void InitRun() {
        data.itemPrefabs.Clear();
        data.ultCooldownLeft = 0;
    }

    public void Save() {
        Debug.Log(instance.unit.data.ultCooldownLeft);
        instance.unit.data.CopyTo(data);
    }

    public void Load() {
        Debug.Log(Unit.allHeroUnits.Count);
        data.CopyTo(instance.unit.data);
    }
}