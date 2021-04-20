using System.Collections.Generic;
using UnityEngine;

public class UnitHero : UnitSide {
    [Header("Balancing")]
    public float ultCooldown;
    public float ultDuration;
    
    [Header("State")]
    public UltStatus ultStatus;
    public float predictiveXp;
    public float ultDurationLeft;
    
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
        unit.OnLevelUp.AddListener(LevelUp);
        Battle.m.OnBattleEnd.AddListener(TransferPredictiveXp);
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
        
        ReloadUlt();
        ProcessUlt();
    }

    public void ReloadUlt() {
        if (ultStatus != UltStatus.RELOADING) return;
        
        ultCooldownLeft -= Time.deltaTime;
        if (ultCooldownLeft < 0) ReadyUlt();
        
    }

    public void ProcessUlt() {
        if (ultStatus != UltStatus.ACTIVATED) return;
        
        ultDurationLeft -= Time.deltaTime;
        if (ultDurationLeft < 0) EndUlt();
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
        ultDurationLeft = ultDuration;
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
        Game.m.PlaySound(MedievalCombat.COINS, .5f, -1, SoundManager.Pitch.RANDOM);
        icon.GainItemFromFight(itemPrefab, monsterPosition.SetY(-2.75f));
    }

    public void GetItemAtStartup(Item itemPrefab) {
        icon.GetItemAtStartup(itemPrefab);
    }
    
    
    // ====================
    // XP
    // ====================

    public void GetXp(float amount, Vector3 monsterPosition) {
        if (unit.status != Unit.Status.ALIVE) return;
        
        if (.3f.Chance()) Game.m.PlaySound(MedievalCombat.COINS, .5f, -1, SoundManager.Pitch.RANDOM);
        icon.GainXpFromFight(
            this.WeightedRandom(
                Game.m.xpPrefabSmall, 20,
                Game.m.xpPrefabMedium, 4,
                Game.m.xpPrefabBig, 1),
            monsterPosition.SetY(-2.75f) + this.Random(-1f, 1f) * Vector3.left,
            amount);
    }

    public void LevelUp() {
        icon.levelUpText.SetAlpha(1);
        this.Wait(2, () => icon.levelUpText.TweenAlpha(0, Tween.Style.EASE_IN, 1));
        icon.levelUpText.gameObject.TweenPosition(10f * Vector3.up, Tween.Style.BOUNCE, .25f);
        icon.levelNumber.Bounce(.5f, .25f, 
            () => icon.levelNumber.transform.localScale = Vector3.one);
        
        // SetHealth(data.maxHealth);
        Battle.m.OnBattleEnd.AddListener(RestoreAllHp);
        this.While(
            () => unit.data.currentHealth.isClearlyLowerThan(unit.data.maxHealth) && 
                  unit.status == Unit.Status.ALIVE, 
            () => unit.AddHealth(unit.data.maxHealth * .005f),
            .01f,
            () => Battle.m.OnBattleEnd.RemoveListener(RestoreAllHp));
    }

    public void RestoreAllHp() => unit.SetHealth(unit.data.maxHealth);

    public void TransferPredictiveXp() {
        unit.AddXp(predictiveXp);
        predictiveXp = 0;
    }
}
