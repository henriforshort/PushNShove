using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitData {
    //Stats
    public Stat maxHealth; //Resistance
    public Stat prot; //Chance to absorb a hit completely
    public Stat weight; //Chance to hit & not get hit (melee only)
    public Stat damage; //How much damage I deal when I hit
    public Stat strength; //How far I push enemies I hit
    public Stat critChance;
    
    //Other info
    public CampActivity.Type activity;
    public float currentHealth;
    public float ultCooldownLeft;
    public DateTime lastSeenSleeping;
    [SerializeField] private List<string> _itemPrefabPaths;
    
    public List<string> itemPrefabPaths => _itemPrefabPaths ?? (_itemPrefabPaths = new List<string>());
    public List<Stat> stats => new List<Stat> { maxHealth, prot, weight, damage, strength, critChance };

    public UnitData() {
        maxHealth = new Stat();
        prot  = new Stat();
        weight = new Stat();
        damage = new Stat();
        strength = new Stat();
        critChance = new Stat();
    }

    public void InitFrom(Unit unit) {
        maxHealth.Init(unit.baseMaxHealth);
        prot.Init(unit.baseProt);
        weight.Init(unit.baseWeight);
        damage.Init(unit.baseDamage);
        strength.Init(unit.baseStrength);
        critChance.Init(unit.baseCritChance);
        
        currentHealth = maxHealth;
        ultCooldownLeft = 0;
        itemPrefabPaths.Clear();
        activity = CampActivity.Type.IDLE;
    }
}
    
public enum UnitStat { MAX_HEALTH, PROT, WEIGHT, DAMAGE, STRENGTH, CRIT_CHANCE }
