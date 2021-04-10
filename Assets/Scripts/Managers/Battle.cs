using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Battle : Level<Battle> { //Battle manager, handles a single battle.
                                 //Should contain only Balancing and Scene References relative to this battle.
                                 //Should contain only State info to be deleted at the end of the battle.
    [Header("Balancing")]
    public List<Transform> enemyClusters;
    public int numberOfEnemyClusters;
    public int xpRewards;
    public int itemRewards;
    
    [Header("State")]
    public float timeSinceGameOver;
    public State gameState;
    private Game.SceneName nextScene;

    [Header("Scene References")]
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
    public List<HeroIcon> heroIcons;
	
    public enum State { PLAYING, PAUSE, GAME_OVER, RESTARTING }


    public void Start() {
        InitBattle();
    }
    
    
    // ====================
    // BATTLE INIT
    // ====================

    public void InitBattle() {
        gameState = State.PAUSE;
        
        //Create heroes
        Game.m.save.heroes
            .Where(hgs => Run.m.activeHeroPrefabs.Contains(hgs.battlePrefab))
            .ToList()
            .ForEach(hgs => {
                UnitHero newHero = Instantiate(hgs.battlePrefab, unitsHolder);
                newHero.unit.prefabIndex = hgs.prefabIndex;
                newHero.InitBattle();
            });

        //Create enemies
        this.Repeat(times:numberOfEnemyClusters, () => {
            Transform clusterInstance = Instantiate(enemyClusters.Random());
            while (clusterInstance.childCount > 0) {
                Transform monster = clusterInstance.GetChild(0);
                monster.SetParent(unitsHolder);
            }
            Destroy(clusterInstance.gameObject);
        });

        //Initiate enemy loot
        this.Repeat(times:itemRewards, () => {
            if (!Run.m.itemsDepleted)
                Unit.monsterUnits.Random().monster.droppedItems.Add(Run.m.GetRandomItem());
        });
        this.Repeat(times:xpRewards, () => 
            Unit.monsterUnits.Random().monster.droppedXp ++);
        
        //Init scene
        fightPrompt.SetActive(true);
        transition.FadeOut();
        Game.m.PlayMusic(AdventureRPG.ADVENTURE_TIME);
    }
    
    
    // ====================
    // CHEATS
    // ====================
    
    public void CheatVictory() { while (Unit.monsterUnits.Count > 0) Unit.monsterUnits[0].DeathByHp(); }
    public void CheatDefeat() { while (Unit.heroUnits.Count > 0) Unit.heroUnits[0].DeathByHp(); }
    public void CheatReloadUlts() => Unit.heroUnits.ForEach(u => u.hero.ultCooldownLeft = 0);
    
    
    // ====================
    // BATTLE END
    // ====================

    public void Update() {
        if (gameState == State.GAME_OVER) AwaitRestart();
    }

    public void Defeat() {
        if (gameState == State.GAME_OVER) return;
        if (gameState == State.RESTARTING) return;
        
        Game.m.PlaySound(Casual.NEGATIVE, 0.5f, 6);
        gameOverText.text = "Defeat";
        nextScene = Game.SceneName.Camp;
        GameOver();
    }

    public void Victory() {
        Game.m.PlaySound(Casual.POSITIVE, 0.5f, 6);
        if (gameState == State.GAME_OVER) return;
        if (gameState == State.RESTARTING) return;

        gameOverText.text = "Victory";
        if (Game.m.save.battle < Game.m.battlesPerRun) nextScene = Game.SceneName.Battle;
        else {
            nextScene = Game.SceneName.Camp;
        }
        Game.m.save.battle++;
        GameOver();
    }

    public void GameOver() {
        Game.m.MuteMusic();
        gameOverPanel.SetActive(true);
        gameState = State.GAME_OVER;
        timeSinceGameOver = 0;
        this.Wait(0.5f, () => {
            Unit.heroUnits.ForEach(u => u.hero.EndUlt());
            Game.m.PurgeStatModifiers(StatModifier.Scope.BATTLE);
        });
    }

    public void AwaitRestart() {
        timeSinceGameOver += Time.deltaTime;
        
        if (Input.GetKeyDown(KeyCode.Space)) PressRestartButton();
        if (timeSinceGameOver > Game.m.timeToAutoRestart) Restart();

        if (gameOverPanelWhiteButton.fillAmount < 1) gameOverPanelWhiteButton.fillAmount = timeSinceGameOver
            .Prel(0, Game.m.timeToAutoRestart)
            .Clamp01();
    }

    public void PressRestartButton() {
        if (gameState != State.GAME_OVER) return;
        
        gameOverPanelWhiteButton.fillAmount = 1;
        this.Wait(0.1f, then:Restart);
    }

    public void Restart() {
        gameState = State.RESTARTING;
        Unit.heroUnits.ForEach(u => u.hero.EndUlt());
        Unit.heroUnits.Clear();
        Unit.allHeroUnits.Clear();
        Unit.monsterUnits.Clear();
        Game.m.PlaySound(MedievalCombat.WHOOSH_6);
        transition.FadeIn();
        if (nextScene == Game.SceneName.Camp) Run.m.EndRun();
        this.Wait(0.4f, () => Game.m.LoadScene(nextScene));
    }
}
