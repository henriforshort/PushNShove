using System;

[Serializable]
public class HeroState {
    public Hero prefab;
    public bool isAlive;
    public int index;
    public float maxHealth;
    public float currentHealth;
    public float damage;
    public float weight;
    public float ultCooldownLeft;
    
    public Hero instance => B.m.heroes[index];

    public HeroState (int index, Hero prefab) {
        this.prefab = prefab;
        this.index = index;
        
        InitRun();
    }

    public void InitRun() { //Called at the beginning of each run
        isAlive = true;
        currentHealth = prefab.unit.maxHealth;
        maxHealth = prefab.unit.maxHealth;
        damage = prefab.unit.damage;
        weight = prefab.unit.weight;
        ultCooldownLeft = prefab.ultCooldown;
    }

    public void Save() { //Called at the end of each battle
        isAlive = (instance.unit.status != Unit.Status.DEAD);
        currentHealth = instance.unit.currentHealth;
        maxHealth = instance.unit.maxHealth;
        weight = instance.unit.weight;
        damage = instance.unit.damage;
        ultCooldownLeft = instance.ultCooldownLeft;
    }

    public void Load() { //Called at the beginning of each battle
        if (!isAlive) instance.unit.Die();
        instance.unit.maxHealth = maxHealth;
        instance.unit.SetHealth(currentHealth);
        instance.unit.weight = weight;
        instance.unit.damage = damage;
        instance.ultCooldownLeft = ultCooldownLeft;
    }

    public override string ToString() {
        return "index: " + index + ", currentHealth: " + currentHealth + ", damage: " + damage 
               + ", weight: " + weight + ", maxHealth: " + maxHealth;
    }
}
