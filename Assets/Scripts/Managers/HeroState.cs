using System;
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
    
    public Hero instance => Unit.heroUnits.Get(index)?.hero;

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
    }

    public void Save() { //Called at the end of each battle
        isAlive = instance != null && instance.unit.status != Unit.Status.DEAD;
        if (!isAlive) return;
        
        isAlive = true;
        currentHealth = instance.unit.currentHealth;
        maxHealth = instance.unit.maxHealth;
        ultCooldownLeft = instance.ultCooldownLeft;
    }

    public void Load() { //Called at the beginning of each battle
        if (!isAlive) {
            instance.unit.Die();
            return;
        }
        instance.unit.maxHealth = maxHealth;
        instance.unit.SetHealth(currentHealth);
        instance.ultCooldownLeft = ultCooldownLeft;
    }

    public override string ToString() {
        return "index: " + index + ", currentHealth: " + currentHealth + ", maxHealth: " + maxHealth 
               + ", ultCoolDownLeft: " + ultCooldownLeft;
    }
}
