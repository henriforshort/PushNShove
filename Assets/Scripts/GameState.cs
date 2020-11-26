using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameState {
    public float heroHp;
    public float heroDamage;
    public float heroWeight;
    public float heroMaxHealth;
    public float experience;
    public int level;
    public int battle;
    public int skillPoints;

    public void InitRun() {
        experience = 0;
        battle = 1;
        level = 1;
        skillPoints = 0;
    }

    public void SaveHero() { //Called at the end of each battle
        heroHp = B.m.hero.currentHealth;
        heroMaxHealth = B.m.hero.maxHealth;
        heroWeight = B.m.hero.weight;
        heroDamage = B.m.hero.damage;
    }

    public void LoadHero() { //Called at the beginning of each battle
        B.m.hero.maxHealth = heroMaxHealth;
        B.m.hero.SetHealth(heroHp);
        B.m.hero.weight = heroWeight;
        B.m.hero.damage = heroDamage;
    }
}