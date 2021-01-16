using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public Activity activity;
    public float currentHealth;
    public float ultCooldownLeft;
    [SerializeField] private List<Item> _itemPrefabs;
    
    public List<Item> itemPrefabs => _itemPrefabs ?? (_itemPrefabs = new List<Item>());
    
    public enum Activity { IDLE, SLEEP, COMBAT }

    public UnitData() {
        maxSpeed = new Stat();
        maxHealth = new Stat();
        prot  = new Stat();
        weight = new Stat();
        damage = new Stat();
        strength = new Stat();
        critChance = new Stat();
    }
}
