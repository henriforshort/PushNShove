using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GameSave {
    public int battle;
    public List<HeroState> heroes;

    public void InitRun() { //Called at the beginning of each run
        battle = 1;
        
        heroes = new List<HeroState>();
        for (int i = 0; i < Run.m.usedHeroes.Count; i++) heroes.Add(new HeroState(i, Run.m.usedHeroes[i]));
    }

    public void SaveHeroes() { //Called at the end of each battle
        heroes.ForEach(h => h.Save());
    }

    public void LoadHeroes() { //Called at the beginning of each battle
        heroes.ForEach(h => h.Load());
    }
}