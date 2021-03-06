﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightPrompt : MonoBehaviour {
    [Header("Balancing")]
    public float battleDuration;
    public float battleFadeDuration;
    public float fightDuration;
    public float fightFadeDuration;
    
    [Header("State")]
    public List<Tween> currentTweens;
    public int currentPhase;
    
    [Header("References")]
    public TMP_Text fightText;
    public TMP_Text battleText;
    public List<Image> battleImages;
    public UITransition bg;
    
    public void Start() {
        Phase1();
    }

    public void Phase1() {//Enable battle text
        Game.m.PlaySound(MedievalCombat.SPECIAL_CLICK, .5f, 2);
        currentPhase = 1;
        StopAll();
        battleText.text = "Battle " + Game.m.save.battle;
        battleText.gameObject.SetActive(true);
        fightText.gameObject.SetActive(false);
        this.Wait(battleDuration, Phase2);
    }

    public void Phase2() {//Fade out battle text, enable battle text
        currentPhase = 2;
        StopAll();
        this.Wait(battleFadeDuration, () => Game.m.PlaySound(MedievalCombat.SWORD_SWING_10, .5f, 2));
        
        currentTweens.Add(battleText.TweenAlpha(0, Tween.Style.LINEAR, battleFadeDuration, 
            () => fightText.gameObject.SetActive(true), Tween.WhenDone.DESTROY));
        battleImages.ForEach(i => 
            currentTweens.Add(i.TweenAlpha(0, Tween.Style.LINEAR, battleFadeDuration, () => { }, 
                Tween.WhenDone.DESTROY)));
        this.Wait(battleFadeDuration + fightDuration, Phase3);
    }

    public void Phase3() {//Fade out fight text 
        Game.m.PlaySound(MedievalCombat.SWORD_SWING_2, .5f, 2);
        currentPhase = 3;
        StopAll();
        bg.FadeOut();
        currentTweens.Add(fightText.TweenAlpha(0, Tween.Style.LINEAR, fightFadeDuration, StartGame));
    }

    public void StopAll() {
        currentTweens.ForEach(t => t.MaxOut());
        StopAllCoroutines();
        currentTweens.Clear();
    }

    public void NextPhase() {
        if (currentPhase == 0) Phase1();
        else if (currentPhase == 1) Phase2();
        else if (currentPhase == 2) Phase3();
        else if (currentPhase == 3) StartGame();
    }

    public void StartGame() {
        Battle.m.gameState = Battle.State.PLAYING;
        Destroy(gameObject);
    }
}
