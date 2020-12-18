using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
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

    [Header("Scene References (Assigned at runtime)")]
    public List<Hero> heroes;

    [Header("Scene References")]
    public List<GameObject> deathZones;
    public List<HeroIcon> heroIcons;
    public CameraManager cameraManager;
    public XPManager xpManager;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public AudioSource audioSource;
    public Image gameOverPanelWhiteButton;
    public Transform unitsHolder;
    public TMP_Text battle;
    public GameObject fightPrompt;
    public UITransition transition;
	
    public enum State { PLAYING, GAME_OVER, PAUSE }

    public static B m;


    public void Start() {
        if (m == null) m = this;
        if (m != this) Destroy(this);
        
        InitBattle();
    }

    public void InitBattle() {
        gameState = State.PAUSE;
        if (R.m.needRunInit) R.m.InitRun();
        
        //Create heroes
        for (int i = 0; i < R.m.save.heroes.Count; i++) {
            Hero hero = Instantiate(
                R.m.save.heroes[i].prefab,
                new Vector3(this.Random(-R.m.spawnPosXRange.x, -R.m.spawnPosXRange.y), -2, 0),
                Quaternion.identity,
                unitsHolder);
            heroes.Add(hero);
            hero.InitIcon(heroIcons[i]);
        }
        R.m.save.LoadHeroes();
        
        //Create enemies
        this.Repeat(() => {
            Transform clusterInstance = Instantiate(enemyClusters.Random());
            while (clusterInstance.childCount > 0) {
                clusterInstance.GetChild(0).SetX(Random.Range(R.m.spawnPosXRange.x, R.m.spawnPosXRange.y));
                clusterInstance.GetChild(0).SetParent(unitsHolder);
            }
            Destroy(clusterInstance.gameObject);
        }, numberOfEnemies);
        
        //Init scene
        fightPrompt.SetActive(true);
        transition.FadeOut();
        battle.text = R.m.save.battle.ToString();
    }

    public void Update() {
        if (gameState == State.GAME_OVER) {
            AwaitRestart();
        }
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
        if (gameState == State.GAME_OVER) return;
        
        gameOverText.text = "Defeat";
        R.m.needRunInit = true;
        GameOver();
    }

    public void Victory() {
        if (gameState == State.GAME_OVER) return;

        gameOverText.text = "Victory";
        // xpManager.GainExperience(R.m.xpGainPerLevel / 100);
        R.m.save.battle++;
        GameOver();
    }

    public void GameOver() {
        heroes.ForEach(h => h.unit.EndUlt());
        gameOverPanel.SetActive(true);
        gameState = State.GAME_OVER;
        timeSinceGameOver = 0;
    }

    public void AwaitRestart() {
        timeSinceGameOver += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space)) Restart();
        if (timeSinceGameOver > R.m.timeToAutoRestart) Restart();

        gameOverPanelWhiteButton.fillAmount = timeSinceGameOver
            .Prel(0, R.m.timeToAutoRestart)
            .Clamp01();
    }

    public void Restart() {
        transition.FadeIn();
        R.m.save.SaveHeroes();
        Unit.heroUnits.Clear();
        Unit.monsterUnits.Clear();

        this.Wait(0.2f, () => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }
}
