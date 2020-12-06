using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameState {
    public float experience;
    public int level;
    public int battle;
    public int skillPoints;
    public List<HeroState> heroes;

    public void InitRun() {
        experience = 0;
        battle = 1;
        level = 1;
        skillPoints = 0;
        
        heroes = new List<HeroState>();
        for (int i = 0; i < G.m.heroPrefabs.Count; i++) heroes.Add(new HeroState(i, G.m.heroPrefabs[i]));
        heroes.ForEach(h => h.InitRun());
    }

    public void Save() { //Called at the end of each battle
        heroes = heroes.Where(h => h.instance != null).ToList();
        for (int i = 0; i < heroes.Count; i++) heroes[i].index = i;
        
        heroes.ForEach(h => h.Save());
    }

    public void Load() { //Called at the beginning of each battle
        heroes.ForEach(h => h.Load());
    }
}