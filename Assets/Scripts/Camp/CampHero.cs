using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Slider healthBar;
    
    [Header("Self References (Assigned at runtime)")]
    public int prefabIndex;
    
    public UnitData data => Game.m.save.heroes[prefabIndex].data;

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
        currentSlot = newSlot;
        currentActivity = newActivity;
        currentSlot.hero = this;
        SetStatus(CampActivity.Type.WALKING);
        body.SetMirrored(currentSlot.x < this.GetX());
        this.SetZ(-2);
    }

    public void ReachGoal() {
        currentSlot.emptyMarkers.ForEach(m => m.SetActive(false));
        currentActivity.fullMarkers.ForEach(m => m.SetActive(currentActivity.emptySlot == default));
        SetStatus(currentActivity.type);
        this.SetX(currentSlot.x);
        if (data.activity == CampActivity.Type.IDLE) {
            this.SetZ(-0.1f*currentSlot.x.Abs());
            body.SetMirrored(currentSlot.x > 0);
        }
        if (data.activity == CampActivity.Type.SLEEPING) {
            if (Time.frameCount != 1) {
                data.lastSeenSleeping = DateTime.Now;
                data.lastSeenSleepingString = data.lastSeenSleeping.ToLongTimeString();
            }
            this.SetZ(-2 - 0.1f*currentSlot.x);
            body.SetMirrored(false);
        }
        if (data.activity == CampActivity.Type.READY) {
            this.SetZ(0);
            body.SetMirrored(false);
        }
    }

    public void Sleep() {
        if (data.activity != CampActivity.Type.SLEEPING) return;
        if (Time.frameCount == 1) return;
        
        if (data.currentHealth.isAbout(data.maxHealth))
            this.Wait(0.5f, () => Camp.m.GetActivity(CampActivity.Type.IDLE)?.Add(this));
        else {
            float sleepDuration = (float)(DateTime.Now - data.lastSeenSleeping).TotalMilliseconds;
            AddHealth(0.1f * sleepDuration / Game.m.secondsToAHundredHp);
            data.lastSeenSleeping = DateTime.Now;
            data.lastSeenSleepingString = data.lastSeenSleeping.ToLongTimeString();
        }
    }

    public bool CanBeClicked() {
        if (data.activity == CampActivity.Type.SLEEPING && data.currentHealth < data.maxHealth * 0.1f) return false;
        return true;
    }

    public void AddHealth(float amount) => SetHealth(data.currentHealth + amount);
    public void SetHealth(float amount) {
        data.currentHealth = amount;
        if (data.currentHealth > data.maxHealth) data.currentHealth = data.maxHealth;
        healthBar.value = data.currentHealth / data.maxHealth;
    }

    public void SetStatus(CampActivity.Type newStatus) {
        data.activity = newStatus;
        List<GameObject> visuals = new List<GameObject>{ idleVisuals, sleepingVisuals, readyVisuals, walkingVisuals };
        visuals.ForEach(s => s.SetActive(false));
        visuals[(int)data.activity].SetActive(true);
        visuals[(int)data.activity].GetComponent<Hanimator>()?.Play(0);
    }
}
