﻿using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class HeroState {
    public Unit prefab;
    public int index;
    public float maxHealth;
    public float currentHealth;
    public float damage;
    public float weight;

    public Unit instance => B.m.heroes[index];

    public HeroState (int index, Unit prefab) {
        this.prefab = prefab;
        this.index = index;
        
        InitRun();
    }

    public void InitRun() { //Called at the beginning of each run
        currentHealth = prefab.maxHealth;
        maxHealth = prefab.maxHealth;
        damage = prefab.damage;
        weight = prefab.weight;
    }

    public void Save() { //Called at the end of each battle
        currentHealth = instance.currentHealth;
        maxHealth = instance.maxHealth;
        weight = instance.weight;
        damage = instance.damage;
    }

    public void Load() { //Called at the beginning of each battle
        instance.maxHealth = maxHealth;
        instance.SetHealth(currentHealth);
        instance.weight = weight;
        instance.damage = damage;
    }

    public override string ToString() {
        return "index: " + index + ", currentHealth: " + currentHealth + ", damage: " + damage 
               + ", weight: " + weight + ", maxHealth: " + maxHealth;
    }
}