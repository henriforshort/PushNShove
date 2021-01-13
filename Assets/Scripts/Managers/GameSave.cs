using System;
using System.Collections.Generic;

[Serializable]
public class GameSave {
    public List<HeroGameSave> heroes;

    public void InitGame() { //Called at the beginning of the game
        heroes = new List<HeroGameSave>();
        for (int i = 0; i < Game.m.heroPrefabs.Count; i++) heroes.Add(new HeroGameSave(i, Game.m.heroPrefabs[i]));
        heroes.ForEach(h => h.InitGame());
    }

    public void SaveHeroes() => heroes.ForEach(h => h.Save()); //Called at the end of each run
    public void LoadHeroes() => heroes.ForEach(h => h.Load()); //Called at the beginning of each run
    public void SaveCampHeroes() => heroes.ForEach(h => h.SaveCamp()); //Called when leaving the camp
    public void LoadCampHeroes() => heroes.ForEach(h => h.LoadCamp()); //Called awhen entering the camp
}
