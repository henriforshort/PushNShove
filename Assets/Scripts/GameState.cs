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

    public void InitRun() { //Called at the beginning of each run
        experience = 0;
        battle = 1;
        level = 1;
        skillPoints = 0;
        
        heroes = new List<HeroState>();
        for (int i = 0; i < R.m.heroPrefabs.Count; i++) heroes.Add(new HeroState(i, R.m.heroPrefabs[i]));
    }

    public void SaveHeroes() { //Called at the end of each battle
        heroes.RemoveAll(h => h.instance==null); //Remove dead heroes from save
        for (int i = 0; i < heroes.Count; i++) heroes[i].index = i; //Update heroes index
        
        heroes.ForEach(h => h.Save());
    }

    public void LoadHeroes() { //Called at the beginning of each battle
        heroes.ForEach(h => h.Load());
    }
}