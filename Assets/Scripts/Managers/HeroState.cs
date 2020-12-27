using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HeroState {
    //Basic info
    public Hero prefab;
    public bool isAlive;
    public int index;
    
    //Stats
    public Stat maxSpeed;
    public Stat maxHealth; //Resistance
    public Stat prot; //How much collision force is reduced (whether I got hit or not)
    public Stat weight; //Chance to hit & not get hit
    public Stat damage; //How much damage I deal when i hit
    public Stat strength; //How far I push people I collide with (wether I hit them or not)
    public Stat critChance;
    
    //Other info
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
        
        //Update this from prefab
        maxSpeed = prefab.unit.maxSpeed;
        maxHealth = prefab.unit.maxHealth;
        prot = prefab.unit.prot;
        weight = prefab.unit.weight;
        damage = prefab.unit.damage;
        strength = prefab.unit.strength;
        critChance = prefab.unit.critChance;
        
        currentHealth = prefab.unit.maxHealth;
        ultCooldownLeft = prefab.ultCooldown;
        itemPrefabs = new List<Item>();
    }

    public void Save() { //Called at the end of each battle
        isAlive = instance.unit.status != Unit.Status.DEAD;
        
        //Update this from instance
        maxSpeed = instance.unit.maxSpeed;
        maxHealth = instance.unit.maxHealth;
        prot = instance.unit.prot;
        weight = instance.unit.weight;
        damage = instance.unit.damage;
        strength = instance.unit.strength;
        critChance = instance.unit.critChance;
        
        ultCooldownLeft = instance.ultCooldownLeft;
        itemPrefabs = instance.items.Select(i => i.prefab).ToList();
        
        if (isAlive) currentHealth = instance.unit.currentHealth;
    }

    public void Load() { //Called at the beginning of each battle
        instance.ClearItems();
        itemPrefabs.ForEach(i => instance.GetItemAtStartup(i));
        
        //Update instance from this
        instance.unit.maxSpeed = maxSpeed;
        instance.unit.maxHealth = maxHealth;
        instance.unit.prot = prot;
        instance.unit.weight = weight;
        instance.unit.damage = damage;
        instance.unit.strength = strength;
        instance.unit.critChance = critChance;
        
        if (!isAlive) {
            instance.unit.Die();
            return;
        }
        
        instance.ultCooldownLeft = ultCooldownLeft;
        instance.unit.SetHealth(currentHealth);
    }
}
