using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class B : MonoBehaviour { //Battle manager, handles a single battle.
                                 //Should contain only Balancing and References relative to this battle.
                                 //Should contain only State info to be deleted at the end of the battle.
    [Header("Balancing")]
    public List<Transform> enemyClusters;
    public int numberOfEnemies;
    
    [Header("State")]
    public float timeSinceGameOver;
    public bool restarting;
    public State gameState;
    public Item movingItem;
    public List<Action> onBattleEnd = new List<Action>();

    [Header("Scene References")]
    public List<GameObject> deathZones;
    public List<HeroIcon> heroIcons;
    public CameraManager cameraManager;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public Image gameOverPanelWhiteButton;
    public Transform unitsHolder;
    public GameObject fightPrompt;
    public UITransition transition;
    public Transform itemsCanvas;
    public GameObject itemDescription;
    public TMP_Text itemDescriptionText;
	
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
                new Vector3(this.Random(-R.m.spawnPosXRange.x, -R.m.spawnPosXRange.y), -3, 0),
                Quaternion.identity,
                unitsHolder);
            hero.InitBattle(heroIcons[i]);
        }
        R.m.save.LoadHeroes();
        
        //Create enemies
        this.Repeat(() => {
            Transform clusterInstance = Instantiate(enemyClusters.Random());
            while (clusterInstance.childCount > 0) {
                Transform monster = clusterInstance.GetChild(0);
                monster.SetX(Random.Range(R.m.spawnPosXRange.x, R.m.spawnPosXRange.y));
                monster.SetParent(unitsHolder);
                monster.GetComponent<Unit>().InitBattle();
            }
            Destroy(clusterInstance.gameObject);
        }, numberOfEnemies);
        
        //Init scene
        fightPrompt.SetActive(true);
        transition.FadeOut();
        
        if (G.m.needInitGame) G.m.InitGame();
    }

    public void Update() {
        if (gameState == State.GAME_OVER && !restarting) AwaitRestart();
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

    public void CheatVictory() { while (Unit.monsterUnits.Count > 0) Unit.monsterUnits[0].DeathByHp(); }
    public void CheatDefeat() { while (Unit.heroUnits.Count > 0) Unit.heroUnits[0].DeathByHp(); }

    public void Defeat() {
        if (gameState == State.GAME_OVER) return;
        
        gameOverText.text = "Defeat";
        R.m.needRunInit = true;
        GameOver();
        R.m.EndRun();
    }

    public void Victory() {
        if (gameState == State.GAME_OVER) return;

        gameOverText.text = "Victory";
        R.m.save.battle++;
        GameOver();
    }

    public void GameOver() {
        Unit.allHeroUnits.ForEach(h => h.EndUlt());
        gameOverPanel.SetActive(true);
        gameState = State.GAME_OVER;
        timeSinceGameOver = 0;
        onBattleEnd.ForEach(a => a());
        this.Wait(0.5f, () => Unit.heroUnits.ForEach(u => u.hero.EndUlt()));
    }

    public void AwaitRestart() {
        timeSinceGameOver += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space)) PressRestartButton();
        if (timeSinceGameOver > R.m.timeToAutoRestart) Restart();

        if (gameOverPanelWhiteButton.fillAmount < 1) gameOverPanelWhiteButton.fillAmount = timeSinceGameOver
            .Prel(0, R.m.timeToAutoRestart)
            .Clamp01();
    }

    public void PressRestartButton() {
        gameOverPanelWhiteButton.fillAmount = 1;
        this.Wait(0.1f, then:Restart);
    }

    public void Restart() {
        restarting = true;
        transition.FadeIn();
        R.m.save.SaveHeroes();
        Unit.heroUnits.Clear();
        Unit.allHeroUnits.Clear();
        Unit.monsterUnits.Clear();
        G.m.LoadScene(G.SceneName.Battle);
    }
}
