﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Battle : Level<Battle> { //Battle manager, handles a single battle.
                                 //Should contain only Balancing and Scene References relative to this battle.
                                 //Should contain only State info to be deleted at the end of the battle.
    [Header("Balancing")]
    public List<Transform> enemyClusters;
    public int numberOfEnemyClusters;
    
    [Header("State")]
    public float timeSinceGameOver;
    public State gameState;
    private Game.SceneName nextScene;
    public float pauseDurationLeft;

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
    public TMP_Text battleNumber;
    public TMP_Text gemsText;
    public Image gemsIcon;
    public List<HeroIcon> heroIcons;
	
    public enum State { PLAYING, PAUSE, GAME_OVER, RESTARTING }
    
    
    // ====================
    // BASICS
    // ====================


    public void Start() {
        InitBattle();
    }

    public void Update() {
        CheckForGameOver();
        UpdatePause();
    }
    
    
    // ====================
    // BATTLE INIT
    // ====================

    public void InitBattle() {
        Pause(2);
        
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
        this.For(times:numberOfEnemyClusters, () => {
            Transform clusterInstance = Instantiate(enemyClusters.Random());
            while (clusterInstance.childCount > 0) {
                Transform monster = clusterInstance.GetChild(0);
                monster.SetParent(unitsHolder);
            }
            Destroy(clusterInstance.gameObject);
        });

        //Init enemy loot
        this.For(times:Game.m.itemsPerBattle, () => { if (!Run.m.itemsDepleted) 
            Unit.monsterUnits.Random().monster.droppedItems.Add(Run.m.GetRandomItem()); });
        this.For(times:Game.m.xpBubblesPerBattle, () =>
            Unit.monsterUnits.Random().monster.droppedXp ++);
        this.For(times:Game.m.gemsPerBattle.CoinFlipRound(), () => 
            Unit.monsterUnits.Random().monster.droppedGems++);
        
        //Init scene
        Game.m.PurgeStatModifiers(StatModifier.Scope.BATTLE);
        fightPrompt.SetActive(true);
        transition.FadeOut();
        Game.m.PlayMusic(AdventureRPG.ADVENTURE_TIME);
        battleNumber.text = "Battle " + Game.m.save.battle + "/" + Game.m.battlesPerRun;
        InitGems();
    }
    
    
    // ====================
    // PAUSE
    // ====================

    public void UpdatePause() {
        pauseDurationLeft -= Time.deltaTime;
        if (pauseDurationLeft <= 0 && gameState == State.PAUSE) gameState = State.PLAYING;
    }

    public void Pause(float duration = 1) {
        pauseDurationLeft = pauseDurationLeft.AtLeast(duration);
        gameState = State.PAUSE;
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

    [HideInInspector] public UnityEvent OnBattleEnd;

    public void CheckForGameOver() {
        if (gameState == State.GAME_OVER) AwaitRestart();
    }

    public void Defeat() {
        if (gameState == State.GAME_OVER) return;
        if (gameState == State.RESTARTING) return;
        
        this.Wait(1, () => Game.m.PlaySound(Casual.NEGATIVE, 0.5f, 6));
        gameOverText.text = "Defeat";
        nextScene = Game.SceneName.Camp;
        GameOver();
    }

    public void Victory() {
        this.Wait(1, () => Game.m.PlaySound(Casual.POSITIVE, 0.5f, 6));
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
        this.Wait(0.5f, () => Unit.heroUnits.ForEach(u => u.hero.EndUlt()));
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
        OnBattleEnd.Invoke();
        gameState = State.RESTARTING;
        Unit.heroUnits.ForEach(u => u.hero.EndUlt());
        Unit.heroUnits.Clear();
        Unit.allHeroUnits.Clear();
        Unit.monsterUnits.Clear();
        Game.m.PlaySound(MedievalCombat.WHOOSH_6);
        transition.FadeIn();
        this.Wait(0.4f, () => Game.m.LoadScene(nextScene));
    }


    // ====================
    // GEMS
    // ====================

    public void InitGems() {
        SetGems();
        Game.m.OnAddGems.AddListener(SetGems);
    }

    public void SetGems() {
        gemsText.text = Game.m.save.gems.ToString();
    }

    public void LootOneGem(Vector3 origin) {
        Game.m.PlaySound(MedievalCombat.COINS, .5f, -1, SoundManager.Pitch.RANDOM);
        
        //Create item
        GameObject gemInstance = Instantiate(Game.m.gemPrefab, 
            cameraManager.cam.WorldToScreenPoint(origin),
            Quaternion.identity,
            itemsCanvas);
        
        //Bounce item then move it to top left corner
        gemInstance.TweenPosition(Vector3.up * 50, 
            Tween.Style.BOUNCE, .5f, () => 
                this.Wait(0.25f, () => 
                    gemInstance.TweenPosition(gemsIcon.transform.position - gemInstance.transform.position, 
                        Tween.Style.EASE_OUT, 1f, () => {
                            Destroy(gemInstance);
                            Game.m.PlaySound(MedievalCombat.COIN_AND_PURSE, .5f, -1, SoundManager.Pitch.RANDOM);
                            Game.m.AddOneGem();
                            OnBattleEnd.RemoveListener(Game.m.AddOneGem);
                            gemsText.Bounce();
                            gemsIcon.Bounce();
                        })));
    }
}
