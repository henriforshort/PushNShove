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
	public float hillsParallax;
	[Space(20)]
	public float freezeFrameDuration;
	public float bumpRecoverySpeed;
	public float bumpSpeed;
	public float defendSpeed;
	[Space(20)]
	public float timeToAutoRestart;
	public float xpGainPerLevel;
	public Vector2 enemySpawnPosXRange;
	
	[Header("State")]
	public bool needRunInit;
	public GameState s;

	[Header("Prefabs")]
	public List<Unit> heroPrefabs;
	public GameObject hpLossUIPrefab;
	public GameObject bumpDustFxPrefab;
	public GameObject deathCloudFxPrefab;
	public GameObject sparkFxPrefab;
	public List<AudioClip> damageSounds;
	public List<AudioClip> deathSounds;

	[Header("Colors")]
	public Color white = new Color(201, 204, 161);
	public Color yellow = new Color(202, 160, 90);
	public Color orange = new Color(174, 106, 71);
	public Color red = new Color(139, 64, 73);
	public Color black = new Color(84, 51, 68);
	public Color darkGrey = new Color(81, 82, 98);
	public Color grey = new Color(99, 120, 125);
	public Color lightGrey = new Color(142, 160, 145);

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
		
		s.InitRun();
	}
}
