using System;
using System.Collections.Generic;

[Serializable]
public class GameSave {
    public int battle;
    public List<HeroGameSave> heroes;

    public void InitGame() { //Called at the beginning of the game
        heroes = new List<HeroGameSave>();
        for (int i = 0; i < Game.m.heroPrefabs.Count; i++) heroes.Add(new HeroGameSave(i, Game.m.heroPrefabs[i]));
        heroes.ForEach(h => h.InitGame());
    }
}
