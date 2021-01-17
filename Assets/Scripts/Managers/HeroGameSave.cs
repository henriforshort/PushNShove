using System;

[Serializable]
public class HeroGameSave {
    //Basic info
    public string battlePrefabPath;
    public string campPrefabPath;
    public Hero battlePrefab => battlePrefabPath.ToPrefab<Hero>();
    public CampHero campPrefab => campPrefabPath.ToPrefab<CampHero>();
    public int prefabIndex; //Index of the prefab in Game.m.heroPrefabs
    
    //Data
    public UnitData data;
    
    public HeroGameSave (int prefabIndex, Hero battlePrefab) {
        this.prefabIndex = prefabIndex;
        battlePrefabPath = battlePrefab.ToPath("Heroes/");
        campPrefabPath = battlePrefab.campHero.ToPath("Camp/");
        data = new UnitData();
    }
    
    public void InitGame() {
        data.maxSpeed.Init(battlePrefab.unit.baseMaxSpeed);
        data.maxHealth.Init(battlePrefab.unit.baseMaxHealth);
        data.prot.Init(battlePrefab.unit.baseProt);
        data.weight.Init(battlePrefab.unit.baseWeight);
        data.damage.Init(battlePrefab.unit.baseDamage);
        data.strength.Init(battlePrefab.unit.baseStrength);
        data.critChance.Init(battlePrefab.unit.baseCritChance);
        data.currentHealth = battlePrefab.unit.data.maxHealth;
        data.ultCooldownLeft = 0;
        data.itemPrefabPaths.Clear();
        data.activity = Camp.Activity.Type.IDLE;
    }
}