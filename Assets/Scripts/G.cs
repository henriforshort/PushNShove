using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class G : MonoBehaviour { //Game manager, handles the whold game flow.
                                 //Should contain the global balancing and the prefabs.
                                 //Should contain all State info that is persisted across levels
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
	
	[Header("State")]
	public float experience;
	public float heroHp;
	public int level;
	
	[Header("Prefabs")]
	public GameObject hpLossUIPrefab;
	public GameObject bumpDustFxPrefab;
	public GameObject deathCloudFxPrefab;
	public GameObject sparkFxPrefab;
	public List<AudioClip> damageSounds;
	public List<AudioClip> deathSounds;

	public static G m;


	public void Start() {
		if (m == null) m = this;
		if (m != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(this);
	}
}
