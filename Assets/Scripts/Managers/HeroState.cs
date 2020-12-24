using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroState {
    public Hero prefab;
    public bool isAlive;
    public int index;
    public float maxHealth;
    public float currentHealth;
    public float ultCooldownLeft;
    public List<Item> itemPrefabs;
    
    public Hero instance => Unit.allHeroUnits.Get(index)?.hero;

    public HeroState (int index, Hero prefab) {
        this.prefab = prefab;
        this.index = index;
        
        InitRun();
    }

    public void InitRun() { //Called at the beginning of each run
        isAlive = true;
        currentHealth = prefab.unit.maxHealth;
        maxHealth = prefab.unit.maxHealth;
        ultCooldownLeft = prefab.ultCooldown;
        itemPrefabs = new List<Item>();
    }

    public void Save() { //Called at the end of each battle
        itemPrefabs = instance.items.Select(i => i.prefab).ToList();
        
        isAlive = instance.unit.status != Unit.Status.DEAD;
        if (!isAlive) return;
        
        currentHealth = instance.unit.currentHealth;
        maxHealth = instance.unit.maxHealth;
        ultCooldownLeft = instance.ultCooldownLeft;
    }

    public void Load() { //Called at the beginning of each battle
        instance.ClearItems();
        itemPrefabs.ForEach(i => instance.GetItemAtStartup(i));
        
        if (!isAlive) {
            instance.unit.Die();
            return;
        }
        instance.unit.maxHealth = maxHealth;
        instance.unit.SetHealth(currentHealth);
        instance.ultCooldownLeft = ultCooldownLeft;
    }
}
