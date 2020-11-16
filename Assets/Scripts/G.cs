using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class G : MonoBehaviour {
	public S gameState;
	public float bumpRecoverySpeed;
	public GameObject restartButton;
	public TMP_Text gameOverText;
	public List<AudioClip> damageSounds;
	public List<AudioClip> deathSounds;
	public AudioSource audioSource;
	public float collisionForceIncrease;
	public float minMomentum;
	public float speedToBump; //negative speed after which a unit is considered bumped
	public GameObject bumpDustParticles;
	public GameObject deathCloud;
	public GameObject cameraFocus;
	public GameObject cameraGO;
	public float camDist;
	[FormerlySerializedAs("camSmooth")]
	public float camSpeed;
	public GameObject hills;
	public float hillsParallax;
	public GameObject hpLossUIPrefab;
	public List<GameObject> hpLossUIs;
	public float hpLossUITargetY;
	public float hpLossUISpeed;
	public float hpLossUIDuration;
	
	public static G m;
	
	public enum S { PLAYING, GAME_OVER }


	public void Start() {
		gameState = S.PLAYING;
		if (m == null) m = this;
		if (m != this) Destroy(this);
	}

	public void Update() {
		if (gameState == S.PLAYING) CameraAndParallax();
		UpdateHpLossUIs();
	}

	public void UpdateHpLossUIs() {
		foreach (GameObject go in hpLossUIs) {
			go.LerpTo(new Vector3(go.GetX(), hpLossUITargetY, go.GetZ()), 
				hpLossUISpeed);
		}
	}

	public void CameraAndParallax() {
		cameraFocus.transform.position = (Vector3.forward * -10) + Vector3.right *
			(Unit.player1Units.Select(unit => unit.GetX()).Max()
			 + Unit.player2Units.Select(unit => unit.GetX()).Min()) / 2;
			
		if (Mathf.Abs(cameraFocus.GetX() - cameraGO.GetX()) > camDist) 
			cameraGO.LerpTo(cameraFocus, camSpeed).ClampX(-0.5f, 0.5f);

		hills.SetX(cameraGO.GetX() * hillsParallax);
	}

	public void CreateHpLossUI(Vector3 position) {
		GameObject go = Instantiate(hpLossUIPrefab, position, Quaternion.identity, transform);
		hpLossUIs.Add(go);
		this.Wait(hpLossUIDuration, () => {
			hpLossUIs.Remove(go);
			Destroy(go);
		});
	}

	public void Defeat() {
		gameOverText.text = "Defeat";
		GameOver();
	}

	public void Victory() {
		gameOverText.text = "Victory";
		GameOver();
	}

	public void GameOver() {
		restartButton.SetActive(true);
		gameState = S.GAME_OVER;
	}

	public void Restart() {
		Unit.player1Units.Clear();
		Unit.player2Units.Clear();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
