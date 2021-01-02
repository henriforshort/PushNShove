using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class UnitData {
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
    public List<Item> itemPrefabs; //instances or prefabs?

    public void CopyTo(UnitData copy) {
        copy.maxSpeed = maxSpeed;
        copy.maxHealth = maxHealth;
        copy.prot = prot;
        copy.weight = weight;
        copy.damage = damage;
        copy.strength = strength;
        copy.critChance = critChance;

        copy.currentHealth = currentHealth;
        copy.ultCooldownLeft = ultCooldownLeft;
        copy.itemPrefabs = itemPrefabs.Select(i => i.prefab).ToList();
    }
}
