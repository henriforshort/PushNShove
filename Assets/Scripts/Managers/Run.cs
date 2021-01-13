using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Run : MonoBehaviour { //Run manager, handles a single run.
                                 //Should contain all balancing and prefabs relative to battles
                                 //Should contain only State info that is persisted across a single run
	[Header("Balancing")]
	public bool enableCheats;
	[Space(20)]
	public float timeToAutoRestart;
	public Vector2 spawnPosXRange;
	public int amountOfHeroes;
	[Space(20)]
	public float camSpeed;
	public float camMaxDistFromUnits;
	public float camMaxDistFromMapCenter;
	[Space(20)]
	public float attackDistance;
	public float collideDistance;
	public float freezeFrameDuration;
	public float bumpRecoverySpeed;
	public float bumpSpeed;
	public float defendSpeed;
	[Space(20)]
	public int maxItemsPerHero;
	public float commonDropChance;
	public float rareDropChance;
	public float leggyDropChance;
	
	[Header("State")]
	public RunSave save;
	public List<Hero> activeHeroPrefabs;
	public List<Item> commonItems;
	public List<Item> rareItems;
	public List<Item> leggyItems;
	public List<Action> onRunEnd = new List<Action>();
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
			.Where(hgs => hgs.data.activity == UnitData.Activity.COMBAT)
			.Select(hgs => hgs.battlePrefab).ToList();
		//If there are none (ie we didn't get here from camp), select random heroes
		while (activeHeroPrefabs.Count < amountOfHeroes) activeHeroPrefabs.Add(Game.m.save.heroes
				.Select(hgs => hgs.battlePrefab)
				.RandomWhere(h => !activeHeroPrefabs.Contains(h)));
		save.InitRun();
		
		commonItems = items.Where(i => i.rarity == Item.Rarity.COMMON).ToList();
		rareItems = items.Where(i => i.rarity == Item.Rarity.RARE).ToList();
		leggyItems = items.Where(i => i.rarity == Item.Rarity.LEGGY).ToList();
	}

	public void EndRun() {
		onRunEnd.ForEach(a => a());
		Destroy(gameObject);
	}
	
    
	// ====================
	// ITEMS
	// ====================
	
	public Item GetRandomItem() {
		Item.Rarity rarity = this.WeightedRandom(
			Item.Rarity.COMMON, (commonItems.Count == 0) ? 0 : commonDropChance, 
			Item.Rarity.RARE, (rareItems.Count == 0) ? 0 : rareDropChance,
			Item.Rarity.LEGGY, (leggyItems.Count == 0) ? 0 : leggyDropChance);

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
