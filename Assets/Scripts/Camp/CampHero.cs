using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampHero : MonoBehaviour {
    [Header("State")]
    public CampSlot currentSlot;
    [HideInInspector] public CampActivity currentActivity;
    
    [Header("References")]
    public GameObject body;
    public GameObject idleVisuals;
    public GameObject sleepingVisuals;
    public GameObject walkingVisuals;
    public GameObject readyVisuals;
    public GameObject timer;
    public TMP_Text timerText;
    public Slider healthBar;
    
    [Header("Self References (Assigned at runtime)")]
    public int prefabIndex;
    
    public UnitData data => Game.m.save.heroes[prefabIndex].data;

    private List<GameObject> _visuals;
    public List<GameObject> visuals => _visuals ?? (_visuals = new List<GameObject> {idleVisuals, 
        sleepingVisuals, readyVisuals, walkingVisuals});
    public GameObject currentVisuals => visuals[(int) data.activity];

    
    // ====================
    // BASICS
    // ====================
    
    public void Start() {
        Camp.m.GetActivity(data.activity)?.Add(this);
        this.SetX(currentSlot.x);
        this.SetY(-3);
        SetHealth(data.currentHealth);
    }

    public void Update() {
        MoveToGoal();
        Sleep();
    }
    
    
    // ====================
    // SET GOAL
    // ====================

    public void MoveToGoal() {
        if (data.activity != CampActivity.Type.WALKING) return;

        float dif = currentSlot.x - this.GetX();
        this.SetX(this.GetX() + dif.Sign() * (Time.deltaTime * 2f).AtMost(dif.Abs()));
        if (this.GetX().isAbout(currentSlot.x)) ReachGoal();
    }

    public void SetGoal(CampActivity newActivity, CampSlot newSlot) {
        if (currentSlot != null) {
            currentSlot.hero = null;
            currentSlot.emptyMarkers.ForEach(m => m.SetActive(true));
            currentActivity.fullMarkers.ForEach(m => m.SetActive(false));
        }
        if (data.activity == CampActivity.Type.SLEEPING) timer.SetActive(false);
        currentSlot = newSlot;
        currentActivity = newActivity;
        currentSlot.hero = this;
        SetStatus(CampActivity.Type.WALKING);
        body.SetMirrored(currentSlot.x < this.GetX());
        this.SetZ(-2);
    }
    
    
    // ====================
    // REACH GOAL
    // ====================

    public void ReachGoal() {
        Game.m.PlaySound(MedievalCombat.UI_TIGHT, .5f, 5);
        currentSlot.emptyMarkers.ForEach(m => m.SetActive(false));
        currentActivity.fullMarkers.ForEach(m => m.SetActive(currentActivity.emptySlot == default));
        SetStatus(currentActivity.type);
        this.SetX(currentSlot.x);
        if (data.activity == CampActivity.Type.IDLE) StartIdling();
        if (data.activity == CampActivity.Type.SLEEPING) StartSleeping();
        if (data.activity == CampActivity.Type.READY) StartReadying();
    }

    public void SetStatus(CampActivity.Type newStatus) {
        data.activity = newStatus;
        visuals.ForEach(s => s.SetActive(false));
        currentVisuals.SetActive(true);
        currentVisuals.GetComponent<Hanimator>()?.Play(0);
    }

    public void StartIdling() {
        this.SetZ(-0.1f*currentSlot.x.Abs());
        body.SetMirrored(currentSlot.x > 0);
    }

    public void StartSleeping() {
        if (Time.frameCount != 1) {
            data.lastSeenSleeping = DateTime.Now;
            Game.m.PlaySound(MedievalCombat.BAG);
            Game.m.PlaySound(MedievalCombat.POTION_AND_ALCHEMY, 0.5f, 10);
        }
        timer.SetActive(true);
        this.SetZ(-2 - 0.1f*currentSlot.x);
        body.SetMirrored(false);
    }

    public void StartReadying() {
        this.SetZ(0);
        body.SetMirrored(false);
        Game.m.PlaySound(MedievalCombat.MAGIC_BUFF_ATTACK, 0.5f, 1);
        if (currentActivity.emptySlot == default) Game.m.PlaySound(MedievalCombat.SPECIAL_CLICK, .5f, 5);
    }
    
    
    // ====================
    // SLEEP
    // ====================

    public void Sleep() {
        if (data.activity != CampActivity.Type.SLEEPING) return;
        if (Time.frameCount == 1) return;

        //sec = %hp * secper%hp
        float secondsToFullLife = (1 - data.currentHealth/data.maxHealth) * Game.m.secondsToMaxHp;
        timerText.text = ToShortString(secondsToFullLife + .5f);
        
        float sleepDuration = (float)(DateTime.Now - data.lastSeenSleeping).TotalMilliseconds;
        // %hp = sec / secper%hp
        AddHealth(data.maxHealth * (sleepDuration/1000) / Game.m.secondsToMaxHp);
        data.lastSeenSleeping = DateTime.Now;

        if (data.currentHealth.isAbout(data.maxHealth)) {
            this.Wait(0.5f, () => {
                Camp.m.GetActivity(CampActivity.Type.IDLE)?.Add(this);
                Game.m.PlaySound(Casual.POSITIVE, .5f, 5); //TODO fix sound bug
            });
        }
    }

    public void AddHealth(float amount) => SetHealth(data.currentHealth + amount);
    public void SetHealth(float amount) {
        data.currentHealth = amount.Clamp(0, data.maxHealth);
        healthBar.value = data.currentHealth / data.maxHealth;
    }

    public string ToShortString(float duration) {
        if (duration > 3600) {
            int hours = duration.RoundToInt() / 3600;
            int minutes = duration.RoundToInt() % (hours * 3600) / 60;
            return hours+"h"+(minutes < 10 ? "0" : "")+minutes;
        } else if (duration > 60) {
            int minutes = duration.RoundToInt() / 60;
            int seconds = duration.RoundToInt() % (minutes * 60);
            return minutes+"m"+(seconds < 10 ? "0" : "")+seconds;
        } else {
            int seconds = duration.RoundToInt();
            return seconds+"s";
        }
    }
}
