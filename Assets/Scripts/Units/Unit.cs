using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour {
    [Header("Base Stats")]
    public float baseMaxHealth;
    [Range(0,1)] public float baseProt;
    public float baseWeight;
    public float baseDamage;
    public float baseStrength;
    [Range(0,1)] public float baseCritChance;
    
    [Header("Balancing")]
    public bool shakeOnHit;
    public float size;
    public MedievalCombat attackSound;
    public Animals attackSoundAnimal;
    public Animals deathSoundAnimal;
    public Human deathSoundHuman;
    public SoundManager.Pitch pitch;
    
    [Header("State")]
    public float currentSpeed;
    public Status status;
    public Anim anim;
    public bool lastWindupIsTwo;
    public float critCollisionDate;
    public bool isOnFreezeFrame;
    public bool isInvincible;
    public bool lockZOrder;
    public bool lockAnim;
    public bool lockPosition;
    public int index;
    
    [Header("References (Assigned at runtime)")]
    public int prefabIndex;
    public List<GameObject> hpLossUis;

    [Header("References")]
    public UnitSide unitSide;
    public UnitBehavior behavior;
    public Slider healthBar;
    public Slider tmpHealthBar;
    public Rigidbody rigidbodee;
    public Hanimator hanimator;
    
    
    public UnitHero hero => unitSide as UnitHero;
    public UnitMonster monster => unitSide as UnitMonster;

    private static List<Unit> _allHeroUnits; //Even the dead ones
    public static List<Unit> allHeroUnits => _allHeroUnits ?? (_allHeroUnits = new List<Unit>());

    private static List<Unit> _heroUnits; //Only the ones that are still alive
    public static List<Unit> heroUnits => _heroUnits ?? (_heroUnits = new List<Unit>());

    private static List<Unit> _monsterUnits;
    public static List<Unit> monsterUnits => _monsterUnits ?? (_monsterUnits = new List<Unit>());

    public List<Unit> allies => isHero ? heroUnits : monsterUnits;
    public List<Unit>  enemies => isMonster ? heroUnits : monsterUnits;

    public UnitData data => isHero ? Game.m.save.heroes[prefabIndex].data : monster.data;
    public float speedPercent => currentSpeed / Game.m.unitMaxSpeed;
    public bool isWalking => currentSpeed.isClearlyPositive();
    
    public enum Status { ALIVE, FALLING, DYING, DEAD }
    public enum Anim {
        WALK, WINDUP, HIT, DEFEND, BUMPED,
        ULT_BRUISER, ULT_STRONGMAN
    }

    public bool isHero => unitSide is UnitHero;
    public bool isMonster => !isHero;
    
    
    // ====================
    // INIT
    // ====================

    public void Awake() { //Called before loading
        SetAnim(Anim.WALK);
        tmpHealthBar.value = data.currentHealth;
        if (unitSide != null) unitSide.unit = this;
        if (behavior != null) behavior.unit = this;
    }

    public void InitBattle() { //Called after loading
        if (data.currentHealth.isAboutOrLowerThan(0)) Die();
        else SetHealth(data.currentHealth);
    }
    
    
    // ====================
    // UPDATE
    // ====================

    public void Update() {
        if (status == Status.DEAD) return;
        
        UpdateSpeed();
        UpdatePosition();
        UpdateVisuals();
    }
    
    
    // ====================
    // VISUALS
    // ====================
    
    public void UpdateVisuals() {
        hanimator.enabled = (Battle.m.gameState != Battle.State.PAUSE);
        
        if (lockZOrder) this.SetZ(-5);
    }

    public void SetAnim(Anim a) {
        if (lockAnim) return;
        if (anim == a) return;
        
        anim = a;
        if (hanimator.gameObject.activeInHierarchy) hanimator.Play(GetAnim());
    }

    //Play current anim
    //Pick one at random if it's WINDUP
    //Play appropriate one if it's HIT (HIT if WINDUP, HIT2 if WINDUP2)
    public string GetAnim() {
        bool playAnimTwo = false;
        if (isHero) {
            if (anim == Anim.WINDUP) {
                playAnimTwo = this.CoinFlip();
                lastWindupIsTwo = playAnimTwo;
            }
            if (anim == Anim.HIT) playAnimTwo = lastWindupIsTwo;
        }
        return anim + (playAnimTwo ? "2" : "");
    }

    public void FreezeFrame() {
        isOnFreezeFrame = true;
        this.Wait(Game.m.freezeFrameDuration, () => isOnFreezeFrame = false);
    }

    
    // ====================
    // MOVEMENT
    // ====================

    public void UpdateSpeed() {
        if (!CanUpdateSpeed()) return;
        
        currentSpeed = currentSpeed
            .LerpTo(Game.m.unitMaxSpeed, Game.m.bumpRecoverySpeed)
            .AtMost(0);
        
        if (status == Status.DYING && currentSpeed.isAbout(0)) DieDuringBattle();
    }

    public bool CanUpdateSpeed() {
        if (lockPosition) return false;
        if (isOnFreezeFrame) return false;
        if (Battle.m.gameState == Battle.State.PAUSE) return false;
        if (status == Status.FALLING) return false;
        if (currentSpeed.isAboutOrHigherThan(0)) return false;

        return true;
    }

    public void UpdatePosition() {
        if (!CanMove()) return;
        
        transform.position += currentSpeed.ReverseIf(isMonster) * Time.deltaTime * Vector3.right;
    }

    public bool CanMove() {
        if (lockPosition) return false;
        if (isOnFreezeFrame) return false;
        if (Battle.m.gameState == Battle.State.PAUSE) return false;
        
        if (Battle.m.gameState == Battle.State.PLAYING) return true;
        if (status == Status.FALLING) return true; //During Game Over
        if (currentSpeed < 0) return true; //During Game Over
        
        return false;
    }
    
    
    // ====================
    // COLLISIONS
    // ====================

    public void LongRangeCollide(Collider other) { //Called by both sides
        Unit collidedAlly = allies.FirstOrDefault(u => u.gameObject == other.gameObject);
        if (collidedAlly != null 
                && enemies.Count > 0
                && currentSpeed < 0
                && this.isCloserTo(enemies[0].transform.position, than:collidedAlly)
                && critCollisionDate > 0
                && critCollisionDate.isClearlyNot(collidedAlly.critCollisionDate)) 
            FriendlyCollide(collidedAlly);
    }

    public void FriendlyCollide(Unit ally) { //Called by the unit in front
        currentSpeed = (currentSpeed + 2).AtMost(0); //slow down
        
        allies
            .Where(a => a.critCollisionDate.isAbout(critCollisionDate))
            .ToList()
            .ForEach(a => a.currentSpeed = currentSpeed);
        
        ally.SetAnim(Anim.BUMPED);
        ally.TakeCollisionDamage(-currentSpeed/5, true);
        ally.critCollisionDate = critCollisionDate;
        
        Game.m.PlaySound(MedievalCombat.BODY_FALL);
        Battle.m.cameraManager.Shake(0.2f);
    }
    
    public void OnTriggerStay(Collider other) {
        if (status != Status.FALLING 
            && Battle.m.deathZones.Contains(other.gameObject))
            DeathByFall();
    }
    
    
    // ====================
    // COMBAT
    // ====================

    public UnitMelee melee => behavior as UnitMelee;

    public void GetBumpedBy(Unit other) {
        SetAnim(Anim.BUMPED);
        currentSpeed = Game.m.bumpSpeed * other.data.strength * (1 - data.prot);
        
        bool isCrit = ((float)other.data.critChance).Chance();
        TakeCollisionDamage(other.data.damage, isCrit);
        if (isCrit) {
            critCollisionDate = Time.time;
            currentSpeed -= 5;
        }
    }


    // ====================
    // HEALTH
    // ====================

    public void TakeCollisionDamage(float amount, bool isCrit = false) {
        if (isHero) Game.m.PlaySound(deathSoundHuman, .5f, -1, pitch);
        amount = amount.MoreOrLessPercent(0.5f).Round();
        if (amount.isAbout(0)) {
            critCollisionDate = -1;
            return;
        }
        if (isCrit) {
            amount = 3 * amount;
            AddHealth(-amount, amount + "!", Game.m.red);
            Battle.m.cameraManager.Shake(0.2f);
        } else AddHealth(-amount, amount.ToString());
    }

    public void AddHealth(float amount, string uiText = null, Color uiColor = default) {
        if (uiText != null) {
            if (uiColor == default) uiColor = Game.m.black;
            GameObject hpLossUi = Game.m.SpawnFX(Run.m.hpLossUIPrefab,
                new Vector3(this.GetX(), healthBar.GetY() +0.25f, -5),
                false, 3, transform);
            hpLossUis.Add(hpLossUi);
            TMP_Text number = hpLossUi.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            number.color = uiColor;
            number.text = uiText;
        }
        
        SetHealth(data.currentHealth + amount);
    }

    public void SetHealth(float amount) {
        data.currentHealth = Mathf.Clamp(amount, 0, data.maxHealth);
        healthBar.value = data.currentHealth / data.maxHealth;
        tmpHealthBar.value = healthBar.value;
        healthBar.gameObject.SetActive(false);
        this.Wait(0.1f, () => healthBar.gameObject.SetActive(true));
        
        if (hero != null) {
            hero.icon.SetHealth(data.currentHealth/data.maxHealth);
            hero.icon.FlashHealth();
        }
        if (data.currentHealth <= 0 && status == Status.ALIVE) DeathByHp();
    }
    
    
    // ====================
    // DEATH
    // ====================

    public void DeathByHp() {
        Deactivate();
        status = Status.DYING;
        healthBar.transform.parent.gameObject.SetActive(false);
        currentSpeed = Game.m.bumpSpeed * (1 - data.prot);
    }

    public void DeathByFall() {
        Deactivate();
        status = Status.FALLING;
        rigidbodee.useGravity = true;
        this.Wait(0.5f, () => {
            Game.m.PlaySound(MedievalCombat.STAB_1);
            DieDuringBattle();
        });
    }

    public void Deactivate() {
        allies.Remove(this);
        SetAnim(Anim.BUMPED);
        if (isHero) hero.icon.Die();
    }

    public void DieDuringBattle() {
        SetHealth(0);
        if (size >= 2 || isHero) Battle.m.cameraManager.Shake(0.2f);
        Instantiate(Run.m.deathCloudFxPrefab, transform.position + 1f*Vector3.up, Quaternion.identity);
        Game.m.PlaySound(MedievalCombat.BODY_FALL, 0.5f, 4);
        Game.m.PlaySound(deathSoundHuman, .5f, -1, pitch);
        Game.m.PlaySound(deathSoundAnimal);
        Die();
    }

    public void Die() {
        status = Status.DEAD;
        unitSide.Die();
    }

    public void OnDestroy() {
        if (Battle.m == null) return;
        if (Battle.m.gameState == Battle.State.GAME_OVER) return;
        if (Battle.m.gameState == Battle.State.RESTARTING) return;
        

        hpLossUis
            .Where(ui => ui != null)
            .ToList()
            .ForEach(ui => ui.transform.SetParent(Run.m.transform));
        allies.Remove(this);

        if (allies.Count == 0) unitSide.GetDefeated();
    }
}
