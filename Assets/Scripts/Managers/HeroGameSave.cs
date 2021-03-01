using System;

[Serializable]
public class HeroGameSave {
    //Basic info
    public string battlePrefabPath;
    public string campPrefabPath;
    public UnitHero battlePrefab => battlePrefabPath.ToPrefab<UnitHero>();
    public CampHero campPrefab => campPrefabPath.ToPrefab<CampHero>();
    public int prefabIndex; //Index of the prefab in Game.m.heroPrefabs
    
    //Data
    public UnitData data;
    
    public HeroGameSave (int prefabIndex, UnitHero battlePrefab) {
        this.prefabIndex = prefabIndex;
        battlePrefabPath = battlePrefab.ToPath("Heroes/");
        campPrefabPath = battlePrefab.campHero.ToPath("Camp/");
        data = new UnitData();
    }
}