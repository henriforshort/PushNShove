using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class G : MonoBehaviour {
	public S gameState;
	public float bumpRecoverySpeed;
	public GameObject restartButton;
	public TMP_Text gameOverText;
	public List<AudioClip> damageSounds;
	public List<AudioClip> deathSounds;
	public AudioSource audioSource;
	public GameObject deathParticles;
	public float collisionForceIncrease;
	public float minMomentum;
	
	public static G m;
	
	public enum S { PLAYING, GAME_OVER }


	public void Start() {
		gameState = S.PLAYING;
		if (m == null) m = this;
		if (m != this) Destroy(this);
	}

	public void Defeat() {
		gameOverText.text = "Defeat!";
		GameOver();
	}

	public void Victory() {
		gameOverText.text = "Victory!";
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
