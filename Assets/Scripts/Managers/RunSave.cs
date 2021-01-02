using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class RunSave {
    public int battle;
    public List<HeroRunSave> heroes;

    public void InitRun() { //Called at the beginning of each run
        battle = 1;
        
        heroes = new List<HeroRunSave>();
        for (int i = 0; i < Run.m.activeHeroPrefabs.Count; i++) {
            Hero hero = Run.m.activeHeroPrefabs[i];
            int index = Game.m.heroPrefabs.IndexOf(hero);
            heroes.Add(new HeroRunSave(index, i,hero, Game.m.save.heroes[index].data));
        }
    }

    public void SaveHeroes() { //Called at the end of each battle
        heroes.ForEach(h => h.Save());
    }

    public void LoadHeroes() { //Called at the beginning of each battle
        heroes.ForEach(h => h.Load());
    }
}