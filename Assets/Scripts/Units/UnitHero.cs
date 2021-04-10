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
    // INIT
    // ====================

    protected override void Init() { //Called before loading
        unit.index = Unit.allHeroUnits.Count;
        Unit.allHeroUnits.Add(unit);
        Unit.heroUnits.Add(unit);
        _icon = null;
        icon.ClearItems();
        if (ult) ult.unit = unit;
        unit.onDeactivate.AddListener(() => icon.Die());
    }

    public void InitBattle() { //Called after loading
        ultStatus = UltStatus.RELOADING;
        icon.InitBattle(this);
        itemPrefabPaths.ForEach(ipp => GetItemAtStartup(ipp.ToPrefab<Item>()));
    }

    public void Start() { //Called after every unit has been created and loaded
        if (unit.data.currentHealth.isAboutOrLowerThan(0)) unit.onDeath.Invoke();
        else unit.SetHealth(unit.data.currentHealth);
        unit.SetXp(unit.data.xp);
    }


    // ====================
    // EVENTS
    // ====================

    protected override void OnDeath() {
        unit.data.activity = CampActivity.Type.IDLE;
        unit.allies.Remove(unit);
        unit.hanimator.gameObject.SetActive(false);
        icon.Die();
        EndUlt();
        unit.OnDestroy();
    }

    protected override void OnDefeat() {
        Battle.m.Defeat();
    }
    
    
    // ====================
    // ULT
    // ====================

    public void Update() {
        UpdateUlt();
    }

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

    public void GetItemFromFight(Item itemPrefab, Vector3 monsterPosition) {
        Game.m.PlaySound(MedievalCombat.COINS);
        icon.GainItemFromFight(itemPrefab, monsterPosition.SetY(-2.75f));
    }

    public void GetItemAtStartup(Item itemPrefab) {
        icon.GetItemAtStartup(itemPrefab);
    }
    
    
    // ====================
    // XP
    // ====================

    public void GetXp(int amount, Vector3 monsterPosition) {
        Game.m.PlaySound(MedievalCombat.COINS);
        this.Repeat(
            times:amount, 
            () => 
                icon.GainXpFromFight(
                    this.WeightedRandom(
                        Game.m.xpPrefabSmall, 10, 
                        Game.m.xpPrefabMedium, 5, 
                        Game.m.xpPrefabBig, 1), 
                    monsterPosition.SetY(-2.75f) + this.Random(-1f, 1f) * Vector3.left), 
            .1f);
        
    }
}
