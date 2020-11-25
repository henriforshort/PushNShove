using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class B : MonoBehaviour { //Battle manager, handles a single battle.
                                 //Should contain only Balancing and References relative to this battle.
                                 //Should contain only State info to be deleted at the end of the battle.
    [Header("Balancing")]
    public List<EnemyCluster> enemyClusters;
    
    [Header("State")]
    public float gameOverDate;
    public float timeSinceGameOver;
    public State gameState;
    public Unit hero;
    public bool isFirstFrame;
    public float currentShake;
    
    [Header("References")]
    public GameObject restartButton;
    public TMP_Text gameOverText;
    public AudioSource audioSource;
    public GameObject cameraFocus;
    public GameObject cameraGO;
    public GameObject shakeGO;
    public GameObject hills;
    public GameObject sun;
    public List<GameObject> deathZones;
    public Image gameOverPanelBackGround;
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
        isFirstFrame = true;
        this.Wait(() => isFirstFrame = false);
        gameState = State.PLAYING;
        xpSlider.value = G.m.experience;
        level.text = G.m.level.ToString();
        battle.text = G.m.battle.ToString();
        lvUpNotif.SetActive(G.m.skillPoints != 0);
        Instantiate(enemyClusters.Random(), unitsHolder);
        // Instantiate(enemyClusters.Random(), unitsHolder);
        hero = Instantiate(
                G.m.heroPrefab,
                new Vector3(-4, -2, 0),
                Quaternion.identity,
                unitsHolder)
            .GetComponent<Unit>();
        
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
        G.m.experience += amount;
        if (G.m.experience >= 1) LevelUp();
        this.Wait(1, () => xpAnimator.SetInteger("shine", 1));
        this.Wait(2, () => xpAnimator.SetInteger("shine", 0));
    }

    public void UpdateXp() {
        if (xpSlider.value.isApprox(G.m.experience)) xpSlider.value = G.m.experience;
        else xpSlider.value = xpSlider.value.LerpTo(G.m.experience, 2);
        
        xpText.text = (100 * xpSlider.value).Round() + "/100";
    }

    public void LevelUp() {
        G.m.experience--;
        xpSlider.value = 0;
        
        G.m.level++;
        level.text = G.m.level.ToString();
        levelUp.gameObject.SetActive(true);
        this.Wait(1, () => levelUp.gameObject.SetActive(false));

        G.m.skillPoints++;
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
        GainExperience(0.47f);
        G.m.heroHp = Unit.player1Units[0].currentHealth;
        G.m.battle++;
        
        GameOver();
    }

    public void GameOver() {
        restartButton.SetActive(true);
        gameState = State.GAME_OVER;
        timeSinceGameOver = 0;
    }

    public void AwaitRestart() {
        timeSinceGameOver += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space)) Restart();
        if (timeSinceGameOver > G.m.timeToAutoRestart) Restart();

        gameOverPanelBackGround.fillAmount = timeSinceGameOver
            .Prel(0, G.m.timeToAutoRestart)
            .Clamp01();
    }

    public void Restart() {
        gameState = State.PLAYING;
        Unit.player1Units.Clear();
        Unit.player2Units.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
