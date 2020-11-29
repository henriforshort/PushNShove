using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class B : MonoBehaviour { //Battle manager, handles a single battle.
                                 //Should contain only Balancing and References relative to this battle.
                                 //Should contain only State info to be deleted at the end of the battle.
    [Header("Balancing")]
    public List<EnemyCluster> enemyClusters;
    
    [Header("State")]
    public float timeSinceGameOver;
    public State gameState;
    public Unit hero;
    public float currentShake;
    
    [Header("References")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public AudioSource audioSource;
    public GameObject cameraFocus;
    public GameObject cameraGO;
    public GameObject shakeGO;
    public GameObject hills;
    public GameObject sun;
    public List<GameObject> deathZones;
    public Image gameOverPanelWhiteButton;
    public Slider xpSlider;
    public Animator xpAnimator;
    public TMP_Text xpText;
    public Transform unitsHolder;
    public TMP_Text battle;
    public TMP_Text level;
    public TMP_Text levelUp;
    public GameObject lvUpNotif;
    public GameObject lvUpMenu;
	
    public enum State { PLAYING, GAME_OVER, PAUSE }

    public static B m;


    public void Start() {
        if (m == null) m = this;
        if (m != this) Destroy(this);
        
        InitBattle();
    }

    public void InitBattle() {
        gameState = State.PLAYING;
        
        hero = Instantiate(
                G.m.heroPrefab,
                new Vector3(-4, -2, 0),
                Quaternion.identity,
                unitsHolder)
            .GetComponent<Unit>();
        
        if (G.m.needRunInit) G.m.InitRun();
        G.m.s.LoadHero();
        
        Instantiate(enemyClusters.Random(), unitsHolder);
        // Instantiate(enemyClusters.Random(), unitsHolder);
        
        xpSlider.value = G.m.s.experience; //Set xp bar with no lerp
        level.text = G.m.s.level.ToString();
        battle.text = G.m.s.battle.ToString();
        lvUpNotif.SetActive(G.m.s.skillPoints > 0);
    }

    public void Update() {
        if (gameState == State.PLAYING) {
            UpdateCameraAndParallax();
        }

        if (gameState == State.GAME_OVER) {
            AwaitRestart();
        }
        
        
        UpdateXp();
        UpdateShake();
    }

    public void Pause() {
        lvUpMenu.SetActive(true);
    }

    public void UpdateShake() {
        currentShake = currentShake.LerpTo(0, 5);
        shakeGO.transform.localPosition = new Vector3(
            Random.Range(-currentShake, currentShake), 
            Random.Range(-currentShake, currentShake), 
            0);
    }

    public void Shake(float amount) {
        currentShake = amount;
    }

    public void GainExperience(float amount) {
        G.m.s.experience += amount;
        if (G.m.s.experience >= 1) LevelUp();
        this.Wait(1, () => xpAnimator.SetInteger("shine", 1));
        this.Wait(2, () => xpAnimator.SetInteger("shine", 0));
    }

    public void UpdateXp() {
        if (xpSlider.value.isApprox(G.m.s.experience)) xpSlider.value = G.m.s.experience;
        else xpSlider.value = xpSlider.value.LerpTo(G.m.s.experience, 2);
        
        xpText.text = (100 * xpSlider.value).Round() + "/100";
    }

    public void LevelUp() {
        G.m.s.experience--;
        xpSlider.value = 0;
        
        G.m.s.level++;
        level.text = G.m.s.level.ToString();
        levelUp.gameObject.SetActive(true);
        this.Wait(1, () => levelUp.gameObject.SetActive(false));

        G.m.s.skillPoints++;
        lvUpNotif.SetActive(true);
    }

    public void UpdateCameraAndParallax() {
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

    public GameObject SpawnFX(GameObject fx, Vector3 position, bool mirrored = false, Transform holder = null, 
        float duration = 0.5f) {
        if (holder == null) holder = transform;
        GameObject spawnedFx = Instantiate(fx, position, Quaternion.identity, holder);
        if (mirrored) spawnedFx.transform.localScale = new Vector3(-1, 1, 1);
        Destroy(spawnedFx, duration);
        return spawnedFx;
    }

    public void Defeat() {
        gameOverText.text = "Defeat";
        G.m.needRunInit = true;
        
        GameOver();
    }

    public void Victory() {
        gameOverText.text = "Victory";
        GainExperience(G.m.xpGainPerLevel / 100);
        G.m.s.battle++;
        
        GameOver();
    }

    public void GameOver() {
        gameOverPanel.SetActive(true);
        gameState = State.GAME_OVER;
        timeSinceGameOver = 0;
    }

    public void AwaitRestart() {
        timeSinceGameOver += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space)) Restart();
        if (timeSinceGameOver > G.m.timeToAutoRestart) Restart();

        gameOverPanelWhiteButton.fillAmount = timeSinceGameOver
            .Prel(0, G.m.timeToAutoRestart)
            .Clamp01();
    }

    public void Restart() {
        G.m.s.SaveHero();
        gameState = State.PLAYING;
        Unit.player1Units.Clear();
        Unit.player2Units.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
