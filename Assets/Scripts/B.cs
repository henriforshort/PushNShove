using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class B : MonoBehaviour { //Battle manager, handles a single battle.
                                 //Should contain only Balancing and References relative to this battle.
                                 //Should contain only State info to be deleted at the end of the battle.
    [Header("Balancing")]
    public List<Transform> enemyClusters;
    public int numberOfEnemies;
    
    [Header("State")]
    public float timeSinceGameOver;
    public State gameState;
    public List<Unit> heroes;
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
        
        if (G.m.needRunInit) G.m.InitRun();

        G.m.s.heroes.ForEach(h =>
            heroes.Add(Instantiate(
                    h.prefab,
                    new Vector3(
                        Random.Range(-G.m.enemySpawnPosXRange.x, -G.m.enemySpawnPosXRange.y), 
                        -2, 
                        0),
                    Quaternion.identity,
                    unitsHolder)
                .GetComponent<Unit>()));
        
        G.m.s.LoadHeroes();
        
        //Create enemies and give them a random X
        this.Repeat(() => {
            Transform clusterInstance = Instantiate(enemyClusters.Random());
            while (clusterInstance.childCount > 0) {
                clusterInstance.GetChild(0).SetX(Random.Range(G.m.enemySpawnPosXRange.x, G.m.enemySpawnPosXRange.y));
                clusterInstance.GetChild(0).SetParent(unitsHolder);
            }
            Destroy(clusterInstance.gameObject);
        }, numberOfEnemies);
        
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
        if (Unit.heroUnits.Count == 0 || Unit.monsterUnits.Count == 0) return;
		
        cameraFocus.transform.position = (Vector3.forward * -10) + Vector3.right *
            (Unit.heroUnits.Select(unit => unit.GetX()).Max()
             + Unit.monsterUnits.Select(unit => unit.GetX()).Min()) / 2;
			
        if (Mathf.Abs(cameraFocus.GetX() - cameraGO.GetX()) > G.m.camMaxDistFromUnits) 
            cameraGO
                .LerpTo(cameraFocus, G.m.camSpeed)
                .ClampX(-G.m.camMaxDistFromMapCenter, G.m.camMaxDistFromMapCenter);

        hills.SetX(cameraGO.GetX() * G.m.hillsParallax);
        sun.SetX(cameraGO.GetX());
    }

    public GameObject SpawnFX(GameObject fx, Vector3 position, bool mirrored = false, 
        Transform holder = null, float duration = 0.5f, Vector3 rotation = default) {
        if (holder == null) holder = transform;
        if (rotation == default) rotation = Vector3.zero;
        GameObject spawnedFx = Instantiate(fx, position, Quaternion.Euler(rotation), holder);
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
        G.m.s.SaveHeroes();
        gameState = State.PLAYING;
        Unit.heroUnits.Clear();
        Unit.monsterUnits.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
