using System.Collections.Generic;
using UnityEngine;

public class UnitHero : UnitSide {
    [Header("Balancing")]
    public float ultCooldown;
    public float ultDuration;
    
    [Header("State")]
    public UltStatus ultStatus;
    
    [Header("Scene References (assigned at runtime)")]
    public HeroIcon _icon;
    
    [Header("References")]
    public UnitUlt ult;
    public CampHero campHero;
    public Sprite image;
    
    public enum UltStatus { AVAILABLE, RELOADING, ACTIVATED }

    public List<string> itemPrefabPaths => unit.data.itemPrefabPaths;
    public HeroIcon icon => _icon ?? (_icon = Battle.m.heroIcons[unit.index]);
    public float ultCooldownLeft {
        get { return unit.data.ultCooldownLeft; }
        set { unit.data.ultCooldownLeft = value; }
    }


    // ====================
    // BASICS
    // ====================

    public void Awake() { //Called before loading
        unit.index = Unit.allHeroUnits.Count;
        Unit.allHeroUnits.Add(unit);
        Unit.heroUnits.Add(unit);
        _icon = null;
        icon.ClearItems();
        if (ult) ult.unit = unit;
    }

    public void InitBattle() { //Called after loading
        ultStatus = UltStatus.RELOADING;
        icon.InitBattle(this);
        itemPrefabPaths.ForEach(ipp => GetItemAtStartup(ipp.ToPrefab<Item>()));
        unit.InitBattle();
    }

    public void Update() {
        UpdateUlt();
    }

    public override void Die() {
        unit.data.activity = CampActivity.Type.IDLE;
        unit.allies.Remove(unit);
        unit.hanimator.gameObject.SetActive(false);
        unit.hero.icon.Die();
        unit.hero.EndUlt();
        unit.OnDestroy();
    }

    public override void GetDefeated() {
        Battle.m.Defeat();
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
        ult.Ult();
        this.Wait(ultDuration, EndUlt);
        unit.lockZOrder = true;
    }

    public void EndUlt() {
        if (ultStatus != UltStatus.ACTIVATED) return;
        ultStatus = UltStatus.RELOADING;
        ultCooldownLeft = ultCooldown;
        ult.EndUlt();
        icon.StartUltReload();
        unit.lockZOrder = false;
    }
    
    
    // ====================
    // ITEMS
    // ====================

    public void GetItemFromFight(Item itemPrefab, Unit monster) {
        Game.m.PlaySound(MedievalCombat.COINS);
        icon.GainItemFromFight(itemPrefab, monster.transform.position.SetY(-2.75f));
    }

    public void GetItemAtStartup(Item itemPrefab) {
        icon.GetItemAtStartup(itemPrefab);
    }
}
