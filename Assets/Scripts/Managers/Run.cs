using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Run : MonoBehaviour { //Run manager, handles a single run.
                                 //Should not contain any balancing or prefabs
                                 //Should contain only State info that is persisted across a single run
    [Header("State")]
	public List<UnitHero> activeHeroPrefabs;
	public List<Item> commonItems;
	public List<Item> rareItems;
	public List<Item> leggyItems;
	public Item movingItem;

	[Header("Prefabs")]
	public GameObject hpLossUIPrefab;
	public GameObject bumpDustFxPrefab;
	public GameObject deathCloudFxPrefab;
	public GameObject sparkFxPrefab;
	public List<Item> items;

	public static Run m;
	
	public void Start() {
		if (m == null) m = this;
		if (m != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(this);
		InitRun();
	}
    
    
	// ====================
	// START & END
	// ====================

	public void InitRun() { //Called at the start of each run, before init the first battle
		//active heroes are the ones selected from the camp
		activeHeroPrefabs = Game.m.save.heroes
			.Where(hgs => hgs.data.activity == CampActivity.Type.READY)
			.Select(hgs => hgs.battlePrefab).ToList();
		//If there are none (ie we didn't get here from camp), select random heroes
		while (activeHeroPrefabs.Count < Game.m.amountOfHeroes) 
			activeHeroPrefabs.Add(Game.m.save.heroes
				.Select(hgs => hgs.battlePrefab)
				.RandomWhere(h => !activeHeroPrefabs.Contains(h)));
		Game.m.save.battle = 1;
		Game.m.save.heroes.ForEach(h => {
			h.data.ultCooldownLeft = 0;
			h.data.itemPrefabPaths.Clear();
		});
		
		commonItems = items.Where(i => i.rarity == Item.Rarity.COMMON).ToList();
		rareItems = items.Where(i => i.rarity == Item.Rarity.RARE).ToList();
		leggyItems = items.Where(i => i.rarity == Item.Rarity.LEGGY).ToList();
	}

	public void EndRun() {
		Game.m.PurgeStatModifiers(StatModifier.Scope.RUN);
		Game.m.save.heroes.ForEach(h => h.data.itemPrefabPaths.Clear());
		Destroy(gameObject);
	}
	
    
	// ====================
	// ITEMS
	// ====================
	
	public Item GetRandomItem() {
		Item.Rarity rarity = this.WeightedRandom(
			Item.Rarity.COMMON, (commonItems.Count == 0) ? 0 : Game.m.commonDropChance, 
			Item.Rarity.RARE, (rareItems.Count == 0) ? 0 : Game.m.rareDropChance,
			Item.Rarity.LEGGY, (leggyItems.Count == 0) ? 0 : Game.m.leggyDropChance);

		Item pickedItem;
		if (rarity == Item.Rarity.COMMON) {
			pickedItem = commonItems.Random();
			commonItems.Remove(pickedItem);
		} else if (rarity == Item.Rarity.RARE) {
			pickedItem = rareItems.Random();
			rareItems.Remove(pickedItem);
		} else  {
			pickedItem = leggyItems.Random();
			leggyItems.Remove(pickedItem);
		}
		return pickedItem;
	}
	
	public bool itemsDepleted => (commonItems.Count == 0) && 
	                             (rareItems.Count == 0) && 
	                             (leggyItems.Count == 0);
}
