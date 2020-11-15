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
	public float camSmooth;
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
			go.LerpTo(new Vector3(go.transform.position.x, hpLossUITargetY, go.transform.position.z), 
				hpLossUISpeed);
		}
	}

	public void CameraAndParallax() {
		cameraFocus.transform.position = (Vector3.forward * -10) + Vector3.right *
			(Unit.player1Units.Select(unit => unit.transform.position.x).Max()
			 + Unit.player2Units.Select(unit => unit.transform.position.x).Min()) / 2;
			
		if (Mathf.Abs(cameraFocus.transform.position.x - cameraGO.transform.position.x) > camDist) 
			cameraGO.LerpTo(cameraFocus, camSmooth);

		if (cameraGO.transform.position.x < -0.5f) cameraGO.transform.position = Vector3.right * -0.5f 
			+ -10 * Vector3.forward;
		if (cameraGO.transform.position.x > 0.5f) cameraGO.transform.position = Vector3.right * 0.5f 
			+ -10 * Vector3.forward;
			
		hills.transform.position = cameraGO.transform.position.x * hillsParallax * Vector3.right 
		                           + 10 * Vector3.forward;
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
