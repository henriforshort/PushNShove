using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Battle : Level<Battle> { //Battle manager, handles a single battle.
                                 //Should contain only Balancing and Scene References relative to this battle.
                                 //Should contain only State info to be deleted at the end of the battle.
    [Header("Balancing")]
    public List<Transform> enemyClusters;
    [FormerlySerializedAs("numberOfEnemies")]
    public int numberOfEnemyClusters;
    
    [Header("State")]
    public float timeSinceGameOver;
    public State gameState;
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
	
    public enum State { PLAYING, PAUSE, GAME_OVER, RESTARTING }


    public void Start() {
        InitBattle();
    }
    
    
    // ====================
    // BATTLE INIT
    // ====================

    public void InitBattle() {
        gameState = State.PAUSE;
        if (Run.m.needRunInit) Run.m.InitRun();
        
        //Create heroes
        for (int i = 0; i < Run.m.save.heroes.Count; i++) {
            Hero hero = Instantiate(
                Run.m.save.heroes[i].prefab,
                new Vector3(this.Random(-Run.m.spawnPosXRange.x, -Run.m.spawnPosXRange.y), -3, 0),
                Quaternion.identity,
                unitsHolder);
            hero.InitBattle(heroIcons[i]);
        }
        Run.m.save.LoadHeroes();
        
        //Create enemies
        this.Repeat(times:numberOfEnemyClusters, () => {
            Transform clusterInstance = Instantiate(enemyClusters.Random());
            while (clusterInstance.childCount > 0) {
                Transform monster = clusterInstance.GetChild(0);
                monster.SetX(Random.Range(Run.m.spawnPosXRange.x, Run.m.spawnPosXRange.y));
                monster.SetParent(unitsHolder);
                monster.GetComponent<Unit>().InitBattle();
            }
            Destroy(clusterInstance.gameObject);
        });
        
        //Init scene
        fightPrompt.SetActive(true);
        transition.FadeOut();
    }
    
    
    // ====================
    // BATTLE END
    // ====================

    public void Update() {
        if (gameState == State.GAME_OVER) AwaitRestart();
    }

    public void CheatVictory() { while (Unit.monsterUnits.Count > 0) Unit.monsterUnits[0].DeathByHp(); }
    public void CheatDefeat() { while (Unit.heroUnits.Count > 0) Unit.heroUnits[0].DeathByHp(); }

    public void Defeat() {
        if (gameState != State.PLAYING) return;
        
        gameOverText.text = "Defeat";
        Run.m.needRunInit = true;
        GameOver();
        Run.m.EndRun();
    }

    public void Victory() {
        if (gameState != State.PLAYING) return;

        gameOverText.text = "Victory";
        Run.m.save.battle++;
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
        if (timeSinceGameOver > Run.m.timeToAutoRestart) Restart();

        if (gameOverPanelWhiteButton.fillAmount < 1) gameOverPanelWhiteButton.fillAmount = timeSinceGameOver
            .Prel(0, Run.m.timeToAutoRestart)
            .Clamp01();
    }

    public void PressRestartButton() {
        gameOverPanelWhiteButton.fillAmount = 1;
        this.Wait(0.1f, then:Restart);
    }

    public void Restart() {
        gameState = State.RESTARTING;
        transition.FadeIn();
        Run.m.save.SaveHeroes();
        Unit.heroUnits.Clear();
        Unit.allHeroUnits.Clear();
        Unit.monsterUnits.Clear();
        this.Wait(0.4f, () => Game.m.LoadScene(Game.SceneName.Battle));
    }
}
