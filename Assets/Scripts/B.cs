using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class B : MonoBehaviour { //Battle manager, handles a single battle.
                                 //Should contain only Balancing and References relative to this battle.
                                 //Should contain only State info to be deleted at the end of the level.
    [Header("Balancing")]
    public List<EnemyCluster> enemyClusters;
    
    [Header("State")]
    public float gameOverDate;
    public State gameState;
    
    [Header("References")]
    public GameObject restartButton;
    public TMP_Text gameOverText;
    public AudioSource audioSource;
    public GameObject cameraFocus;
    public GameObject cameraGO;
    public GameObject hills;
    public GameObject sun;
    public List<GameObject> deathZones;
    public Image gameOverPanelBackGround;
    public Slider xpSlider;
    public TMP_Text xpText;
    public Transform unitsHolder;
    public TMP_Text levelText;
	
    public enum State { PLAYING, GAME_OVER }

    public static B m;


    public void Start() {
        if (m == null) m = this;
        if (m != this) Destroy(this);
        
        gameState = State.PLAYING;
        GainExperience(0);
        G.m.level++;
        levelText.text = "Level " + G.m.level;
        Instantiate(enemyClusters.Random(), unitsHolder);
        Instantiate(enemyClusters.Random(), unitsHolder);
    }

    public void Update() {
        if (gameState == State.PLAYING) CameraAndParallax();
        if (gameState == State.GAME_OVER) AwaitRestart();
    }

    public void GainExperience(float amount) {
        SetExperience(G.m.experience + amount);
        if (G.m.experience >= 1) LevelUp();
    }

    public void SetExperience(float amount) {
        G.m.experience = amount;
        xpSlider.value = amount;
        xpText.text = (100 * G.m.experience).Round() + "/100";
    }

    public void LevelUp() {
        SetExperience(G.m.experience - 1);
        Debug.Log("level up!");
    }

    public void CameraAndParallax() {
        if (Unit.player1Units.Count == 0 || Unit.player2Units.Count == 0) return;
		
        cameraFocus.transform.position = (Vector3.forward * -10) + Vector3.right *
            (Unit.player1Units.Select(unit => unit.GetX()).Max()
             + Unit.player2Units.Select(unit => unit.GetX()).Min()) / 2;
			
        if (Mathf.Abs(cameraFocus.GetX() - cameraGO.GetX()) > G.m.camMaxDistFromUnits) 
            cameraGO
                .LerpTo(cameraFocus, G.m.camSpeed)
                .ClampX(-G.m.camMaxDistanceFromCenter, G.m.camMaxDistanceFromCenter);

        hills.SetX(cameraGO.GetX() * G.m.hillsParallax);
        sun.SetX(cameraGO.GetX());
    }

    public void SpawnFX(GameObject fx, Vector3 position, bool mirrored = false, Transform holder = null, 
        float duration = 0.5f) {
        if (holder == null) holder = transform;
        GameObject spawnedFx = Instantiate(fx, position, Quaternion.identity, holder);
        if (mirrored) spawnedFx.transform.localScale = new Vector3(-1, 1, 1);
        Destroy(spawnedFx, duration);
    }

    public void Defeat() {
        gameOverText.text = "Defeat";
        GameOver();
    }

    public void Victory() {
        gameOverText.text = "Victory";
        GameOver();
        GainExperience(0.2f);
    }

    public void GameOver() {
        restartButton.SetActive(true);
        gameState = State.GAME_OVER;
        gameOverDate = Time.time;
    }

    public void AwaitRestart() {
        if (Input.GetKeyDown(KeyCode.Space)) Restart();
        if (Time.time - gameOverDate > G.m.timeToAutoRestart) Restart();

        gameOverPanelBackGround.fillAmount = Time.time
            .Prel(gameOverDate, gameOverDate + G.m.timeToAutoRestart)
            .Clamp01();
    }

    public void Restart() {
        gameState = State.PLAYING;
        G.m.heroHp = Unit.player1Units[0].currentHealth;
        Unit.player1Units.Clear();
        Unit.player2Units.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
