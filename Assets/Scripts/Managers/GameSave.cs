using System;
using System.Collections.Generic;

[Serializable]
public class GameSave {
    public int battle;
    public Game.SceneName currentScene;
    public DateTime lastAssignedDoubleXp;
    public int gems;
    public List<HeroGameSave> heroes;

    public void InitGame() { //Called at the beginning of the game, if no save file exists
        heroes = new List<HeroGameSave>();
        Game.m.For(Game.m.heroPrefabs.Count, i => heroes.Add(new HeroGameSave(i, Game.m.heroPrefabs[i])));
        heroes.ForEach(h => h.data.InitFrom(h.battlePrefab.unit));
    }
}
