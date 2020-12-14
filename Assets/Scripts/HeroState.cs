using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class HeroState {
    public Hero prefab;
    public int index;
    public float maxHealth;
    public float currentHealth;
    public float damage;
    public float weight;

    public Hero instance => B.m.heroes[index];

    public HeroState (int index, Hero prefab) {
        this.prefab = prefab;
        this.index = index;
        
        InitRun();
    }

    public void InitRun() { //Called at the beginning of each run
        currentHealth = prefab.unit.maxHealth;
        maxHealth = prefab.unit.maxHealth;
        damage = prefab.unit.damage;
        weight = prefab.unit.weight;
    }

    public void Save() { //Called at the end of each battle
        currentHealth = instance.unit.currentHealth;
        maxHealth = instance.unit.maxHealth;
        weight = instance.unit.weight;
        damage = instance.unit.damage;
    }

    public void Load() { //Called at the beginning of each battle
        instance.unit.maxHealth = maxHealth;
        instance.unit.SetHealth(currentHealth);
        instance.unit.weight = weight;
        instance.unit.damage = damage;
    }

    public override string ToString() {
        return "index: " + index + ", currentHealth: " + currentHealth + ", damage: " + damage 
               + ", weight: " + weight + ", maxHealth: " + maxHealth;
    }
}
