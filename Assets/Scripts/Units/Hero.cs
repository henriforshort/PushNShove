using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Hero : MonoBehaviour {
    [Header("Balancing")]
    public float ultCooldown;
    public float ultDuration;
    
    [Header("State")]
    public UltStatus ultStatus;
    
    [Header("Self References")]
    public Unit unit;
    [FormerlySerializedAs("campUnit")]
    public CampHero campHero;
    public Sprite image;
    public HeroIcon _icon;
    
    public enum UltStatus { AVAILABLE, RELOADING, ACTIVATED }

    public List<Item> itemPrefabs => unit.data.itemPrefabs;
    public HeroIcon icon => _icon ?? (_icon = Battle.m.heroIcons[unit.index]);
    public float ultCooldownLeft {
        get { return unit.data.ultCooldownLeft; }
        set { unit.data.ultCooldownLeft = value; }
    }


    // ====================
    // BASICS
    // ====================

    public void Awake() { //Called before loading
        _icon = null;
        icon.ClearItems();
    }

    public void InitBattle(HeroIcon i) { //Called after loading
        ultStatus = UltStatus.RELOADING;
        icon.InitBattle(this);
        itemPrefabs.ForEach(item => GetItemAtStartup(item.prefab));
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

    public void GetItemFromFight(Item itemPrefab, Unit monster) {
        icon.GainItemFromFight(itemPrefab, monster.transform.position.SetY(-2.75f));
    }

    public void GetItemAtStartup(Item itemPrefab) {
        icon.GetItemAtStartup(itemPrefab);
    }
}
