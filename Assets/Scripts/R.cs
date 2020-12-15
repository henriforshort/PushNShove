using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class R : MonoBehaviour { //Run manager, handles a single run.
                                 //Should contain the global balancing and the prefabs.
                                 //Should contain all State info that is persisted across battles
	[Header("Balancing")]
	public bool enableCheats;
	[Space(20)]
	public float camSpeed;
	public float camMaxDistFromUnits;
	public float camMaxDistFromMapCenter;
	[Space(20)]
	public float freezeFrameDuration;
	public float bumpRecoverySpeed;
	public float bumpSpeed;
	public float defendSpeed;
	[Space(20)]
	public float timeToAutoRestart;
	public float xpGainPerLevel;
	public Vector2 spawnPosXRange;
	
	[Header("State")]
	public bool needRunInit;
	public GameSave save;

	[Header("Prefabs")]
	public List<Hero> heroPrefabs;
	public GameObject hpLossUIPrefab;
	public GameObject bumpDustFxPrefab;
	public GameObject deathCloudFxPrefab;
	public GameObject sparkFxPrefab;
	public List<AudioClip> damageSounds;
	public List<AudioClip> deathSounds;

	public static R m;


	public void Start() {
		if (m == null) m = this;
		if (m != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(this);
		
		needRunInit = true;
	}

	public void InitRun() { //Called at the start of each run, after init the first battle
		needRunInit = false;
		
		save.InitRun();
	}
}
