using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitData {
    //Stats
    public Stat maxHealth; //Resistance
    public Stat prot; //How much collision force is reduced (whether I got hit or not)
    public Stat weight; //Chance to hit & not get hit
    public Stat damage; //How much damage I deal when i hit
    public Stat strength; //How far I push people I collide with (wether I hit them or not)
    public Stat critChance;
    
    //Other info
    public CampActivity.Type activity;
    public float currentHealth;
    public float ultCooldownLeft;
    public string lastSeenSleepingString;
    public DateTime lastSeenSleeping;
    [SerializeField] private List<string> _itemPrefabPaths;
    
    public List<string> itemPrefabPaths => _itemPrefabPaths ?? (_itemPrefabPaths = new List<string>());

    public UnitData() {
        maxHealth = new Stat();
        prot  = new Stat();
        weight = new Stat();
        damage = new Stat();
        strength = new Stat();
        critChance = new Stat();
    }
}
