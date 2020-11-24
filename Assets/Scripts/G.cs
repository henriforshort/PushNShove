using System.Collections.Generic;
using UnityEngine;

public class G : MonoBehaviour { //Game manager, handles the whole game flow.
                                 //Should contain the global balancing and the prefabs.
                                 //Should contain all State info that is persisted across battles
	[Header("Balancing")]
	public float camSpeed;
	public float camMaxDistFromUnits;
	public float camMaxDistanceFromCenter;
	public float hillsParallax;
	public float bumpRecoverySpeed;
	public float collisionForceIncrease;
	public float minMomentum;
	public float speedToBump; //negative speed after which a unit is considered bumped
	public float timeToAutoRestart;
	public Vector2 enemySpawnPosXRange;
	public bool enableCheats;
	
	[Header("State")]
	public float experience;
	public int level;
	public float heroHp;
	public int battle;
	public bool needRunInit;
	public int skillPoints;

	[Header("Prefabs")]
	public GameObject heroPrefab;
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

	public static G m;


	public void Start() {
		if (m == null) m = this;
		if (m != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(this);
		
		needRunInit = true;
	}

	public void InitRun() {
		if (!needRunInit) return;
		experience = 0;
		battle = 1;
		level = 1;
		skillPoints = 0;
	}

	private void LateUpdate() {
		if (needRunInit && B.m.isFirstFrame) LateInitRun();
	}

	public void LateInitRun() { //Called at the start of each run, after init the first battle
		heroHp = B.m.hero.maxHealth;
		B.m.hero.InitRun();
		needRunInit = false;
	}
}
