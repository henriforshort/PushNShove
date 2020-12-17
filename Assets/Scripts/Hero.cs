using System;
using UnityEngine;

public class Hero : MonoBehaviour {
    [Header("Balancing")]
    public float ultCooldown;
    public float ultDuration;
    
    [Header("State")]
    public float ultCooldownLeft;
    public UltStatus ultStatus;
    
    [Header("References")]
    public Unit unit;
    public Sprite image;
    public HeroIcon icon;
    
    public enum UltStatus { AVAILABLE, RELOADING, ACTIVATED }

    public void Start() {
        ultStatus = UltStatus.RELOADING;
        ultCooldownLeft = ultCooldown;
    }

    public void Update() {
        UpdateUlt();
    }

    public void UpdateUlt() {
        if (B.m.gameState != B.State.PLAYING) return;
        if (ultStatus != UltStatus.RELOADING) return;
        
        ultCooldownLeft -= Time.deltaTime;
        if (ultCooldownLeft < 0) EndUltReload();
    }

    public void EndUltReload() {
        ultCooldownLeft = 0;
        ultStatus = UltStatus.AVAILABLE;
        icon.EndUltReload();
    }

    public void Ult() {
        ultStatus = UltStatus.ACTIVATED;
        unit.Ult();
        this.Wait(ultDuration, EndUlt);
        unit.alwaysOnTop = true;
    }

    public void EndUlt() {
        ultStatus = UltStatus.RELOADING;
        ultCooldownLeft = ultCooldown;
        unit.EndUlt();
        icon.StartUltReload();
        unit.alwaysOnTop = false;
    }
}
