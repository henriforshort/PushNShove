using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitData {
    //Stats
    public Stat maxHealth; //Resistance
    public Stat prot; //Chance to absorb a hit completely
    public Stat skill; //Chance to hit & not get hit (melee only)
    public Stat damage; //How much damage I deal when I hit
    public Stat strength; //How far I push enemies I hit
    public Stat resistance; //How well I can resist being pushed
    public Stat critChance;
    
    //Other info
    public CampActivity.Type activity;
    public float currentHealth;
    public float ultCooldownLeft;
    public DateTime lastSeenSleeping;
    [SerializeField] private List<string> _itemPrefabPaths;
    public int level;
    public float xp;
    public float xpToNextLevel;
    
    public List<string> itemPrefabPaths => _itemPrefabPaths ?? (_itemPrefabPaths = new List<string>());
    public List<Stat> stats => new List<Stat> { maxHealth, prot, skill, damage, strength, resistance, critChance };

    public UnitData() {
        maxHealth = new Stat();
        prot  = new Stat();
        skill = new Stat();
        damage = new Stat();
        strength = new Stat();
        resistance = new Stat();
        critChance = new Stat();
    }

    //Called when creating a unit for the very first time
    public void InitFrom(Unit unit) {
        maxHealth.Init(unit.baseMaxHealth);
        prot.Init(unit.baseProt);
        skill.Init(unit.baseSkill);
        damage.Init(unit.baseDamage);
        strength.Init(unit.baseStrength);
        resistance.Init(unit.baseResistance);
        critChance.Init(unit.baseCritChance);
        
        activity = CampActivity.Type.IDLE;
        currentHealth = maxHealth;
        ultCooldownLeft = 0;
        itemPrefabPaths.Clear();
        level = 1;
        xp = 0;
        xpToNextLevel = Game.m.firstLevelXp;
    }
}
    
public enum UnitStat { MAX_HEALTH, PROT, SKILL, DAMAGE, STRENGTH, RESISTANCE, CRIT_CHANCE }
