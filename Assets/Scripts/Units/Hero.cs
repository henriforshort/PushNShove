using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    [Header("Balancing")]
    public float ultCooldown;
    public float ultDuration;
    
    [Header("State")]
    public float ultCooldownLeft;
    public UltStatus ultStatus;

    [Header("Scene References (assigned at runtime)")]
    public List<Item> items;
    
    [Header("Self References")]
    public Unit unit;
    public Sprite image;
    public HeroIcon icon;
    
    public enum UltStatus { AVAILABLE, RELOADING, ACTIVATED }
    
    
    // ====================
    // BASICS
    // ====================

    public void InitBattle(HeroIcon i) { //Called after loading
        items.ForEach(item => GetItemAtStartup(item.prefab));
        ultStatus = UltStatus.RELOADING;
        icon = i;
        icon.InitBattle(this);
        ClearItems();
        unit.InitBattle();
    }

    public void Update() {
        UpdateUlt();
    }
    
    
    // ====================
    // ULT
    // ====================

    public void UpdateUlt() {
        if (Battle.m.gameState != Battle.State.PLAYING) return;
        if (unit.status != Unit.Status.ALIVE) return;
        if (ultStatus != UltStatus.RELOADING) return;
        
        ultCooldownLeft -= Time.deltaTime;
        if (ultCooldownLeft < 0) ReadyUlt();
    }

    public void ReadyUlt() {
        ultCooldownLeft = 0;
        ultStatus = UltStatus.AVAILABLE;
        icon.ReadyUlt();
    }

    public bool CanUlt() {
        if (Battle.m.gameState != Battle.State.PLAYING) return false;
        if (ultStatus != UltStatus.AVAILABLE) return false;

        return true;
    }

    public void Ult() {
        ultStatus = UltStatus.ACTIVATED;
        unit.Ult();
        this.Wait(ultDuration, EndUlt);
        unit.lockZOrder = true;
    }

    public void EndUlt() {
        if (ultStatus != UltStatus.ACTIVATED) return;
        ultStatus = UltStatus.RELOADING;
        ultCooldownLeft = ultCooldown;
        unit.EndUlt();
        icon.StartUltReload();
        unit.lockZOrder = false;
    }
    
    
    // ====================
    // ITEMS
    // ====================

    public void ClearItems() {
        items = new List<Item>();
        icon.ClearItems();
    }

    public void GetItemFromFight(Item itemPrefab, Unit monster) {
        icon.GainItemFromFight(itemPrefab, monster.transform.position.SetY(-2.75f));
    }

    public void GetItemAtStartup(Item itemPrefab) {
        icon.GetItemAtStartup(itemPrefab);
    }
}
