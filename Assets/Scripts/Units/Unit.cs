using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    [Header("Base Stats")]
    public float baseMaxHealth;
    [Range(0,1)] public float baseProt;
    public float baseSkill;
    public float baseDamage;
    public float baseStrength;
    public float baseResistance;
    [Range(0,1)] public float baseCritChance;
    
    [Header("Balancing")]
    public float size;
    public Human bumpedSound;
    public Animals bumpedSoundAnimal;
    public Human deathSound;
    public Animals deathSoundAnimal;
    public SoundManager.Pitch pitch;

    [Header("State")]
    public float speedLastFrame;
    public float currentSpeed;
    public Status status;
    public Anim anim;
    public bool lastPrepareIsTwo;
    public float critCollisionDate;
    public bool isOnFreezeFrame;
    public bool isInvincible;
    public bool lockZOrder;
    public bool lockAnim;
    public bool lockPosition;
    public int index;
    public float tempHealth;
    
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
    public UnitMelee melee => behavior as UnitMelee;

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
        WALK, PREPARE, HIT, DEFEND, BUMPED,
        ULT_BRUISER, ULT_STRONGMAN, ULT_ARCHER_AIM, ULT_ARCHER_SHOOT, ULT_JESTER
    }

    public bool isHero => unitSide is UnitHero;
    public bool isMonster => !isHero;
    
    
    // ====================
    // INIT
    // ====================

    public void Awake() { //Called before loading
        tmpHealthBar.value = data.currentHealth;
        if (behavior != null) behavior.unit = this;
        speedLastFrame = -1;
    }
    
    
    // ====================
    // UPDATE
    // ====================

    public void Update() {
        if (status == Status.DEAD) return;
        
        UpdateSpeed();
        UpdatePosition();
        UpdateVisuals();
        UpdateCollisions();
    }
    
    
    // ====================
    // VISUALS
    // ====================
    
    public void UpdateVisuals() {
        hanimator.enabled = (Battle.m.gameState != Battle.State.PAUSE);
        if (lockZOrder) this.SetZ(-5);
        if (isWalking && anim == Anim.BUMPED) SetAnim(Anim.WALK);
    }

    public void SetAnim(Anim a) {
        if (lockAnim) return;
        if (anim == a && hanimator.playing) return;
        if (!hanimator.enabled) return;
        
        anim = a;
        if (hanimator.gameObject.activeInHierarchy) hanimator.Play(GetAnim());
    }

    //Get current anim
    //Pick one at random if it's PREPARE
    //Get appropriate one if it's HIT (HIT if PREPARE, HIT2 if PREPARE2)
    public string GetAnim() {
        if (!hanimator.anims
            .Select(a => a.name + "2")
            .Contains(anim.ToString())) return anim.ToString();
        
        bool playAnimTwo = false;
        if (anim == Anim.PREPARE) {
            playAnimTwo = this.CoinFlip();
            lastPrepareIsTwo = playAnimTwo;
        }
        if (anim == Anim.HIT) playAnimTwo = lastPrepareIsTwo;
        return anim + (playAnimTwo ? "2" : "");
    }

    public void FreezeFrame() {
        isOnFreezeFrame = true;
        this.Wait(Game.m.freezeFrameDuration, () => isOnFreezeFrame = false);
    }

    
    // ====================
    // XP
    // ====================

    [HideInInspector] public UnityEvent OnLevelUp;

    public void AddXp(float amount) {
        if (data.isOnDoubleXp) amount*=2;
        SetXp(data.xp + amount);
    }

    public void SetXp(float amount) {
        data.xp = amount;
        if (data.xp >= data.xpToNextLevel) LevelUp();
        hero.icon.xpBar.value = data.xp/data.xpToNextLevel;
        hero.icon.levelNumber.text = data.level.ToString();
    }

    public void LevelUp() {
        data.xp -= data.xpToNextLevel;
        data.level++;
        data.xpToNextLevel *= Game.m.levelUpXpNeededIncrease;

        data.maxHealth.AddModifier(Game.m.levelUpStatBonus, StatModifier.Type.MULTIPLY, StatModifier.Scope.GAME, -10);
        data.damage.AddModifier(Game.m.levelUpStatBonus, StatModifier.Type.MULTIPLY, StatModifier.Scope.GAME, -10);
        data.strength.AddModifier(Game.m.levelUpStatBonus, StatModifier.Type.MULTIPLY, StatModifier.Scope.GAME, -10);
        data.resistance.AddModifier(Game.m.levelUpStatBonus, StatModifier.Type.MULTIPLY, StatModifier.Scope.GAME, -10);
        data.skill.AddModifier(Game.m.levelUpStatBonus, StatModifier.Type.MULTIPLY, StatModifier.Scope.GAME, -10);
        
        OnLevelUp.Invoke();
    }

    
    // ====================
    // MOVEMENT
    // ====================

    public void UpdateSpeed() {
        if (!CanUpdateSpeed()) return;

        speedLastFrame = currentSpeed;
        currentSpeed = currentSpeed
            .LerpTo(Game.m.unitMaxSpeed, Game.m.bumpRecoverySpeed)
            .AtMost(0);
        if (speedLastFrame.isClearlyNegative() && currentSpeed.isAboutOrHigherThan(0)) StartWalking();
    }

    public void StartWalking() {
        SetAnim(Anim.WALK);
        this.SetZ(0f);
        critCollisionDate = -1;
        if (status == Status.DYING) DieDuringBattle();
        else Game.m.SpawnFX(Run.m.bumpDustFxPrefab, 
            new Vector3(this.GetX() - 0.5f.ReverseIf(isMonster), -2, -2), isHero, 0.5f);
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

    public void UpdateCollisions() {
        if (CanDieByFall()) DeathByFall();
        FriendlyCollide(GetCollidedAlly());
    }

    public bool CanDieByFall() {
        if (status == Status.FALLING) return false;
        if (this.GetX().Abs() < Game.m.boardSize) return false;

        return true;
    }

    public Unit GetCollidedAlly() { //Called by unit in front
        Unit closestAlly = allies.Except(this)?.WithLowest(DistanceToMe);
        
        if (critCollisionDate.isAboutOrLowerThan(0)) return null;
        if (closestAlly == null) return null;
        if (closestAlly.critCollisionDate.isAbout(critCollisionDate)) return null;
        if (DistanceToMe(closestAlly) > 1f) return null;
        
        return closestAlly;
    }

    public void FriendlyCollide(Unit ally) { //Called by unit in front
        if (ally == null) return;
        
        ally.SetAnim(Anim.BUMPED);
        ally.TakeCollisionDamage(-currentSpeed/5, true);
        ally.critCollisionDate = critCollisionDate;
        ally.currentSpeed = currentSpeed;
        
        Game.m.PlaySound(MedievalCombat.BODY_FALL);
        Battle.m.cameraManager.Shake(0.2f);
    }
    
    public float DistanceToMe(Unit other) => (this.GetX() - other.GetX()).Abs();
    
    
    // ====================
    // COMBAT
    // ====================

    public void GetBumpedBy(Unit other) => 
        GetBumpedBy(other.data.critChance, other.data.damage, other.data.strength);
    public void GetBumpedBy(float critChance, float damage, float strength) {
        //crit
        if (critChance.Chance()) { 
            SetAnim(Anim.BUMPED);
            currentSpeed = (Game.m.bumpSpeed * strength / data.resistance - 5)
                .Clamp(Game.m.maxBumpSpeed, currentSpeed - 1);
            TakeCollisionDamage(damage, true);
            critCollisionDate = Time.time;
        }
        //block
        else if (data.prot.value.Chance()) { 
            SetAnim(Anim.DEFEND);
            currentSpeed = (Game.m.absorbSpeed * strength / data.resistance)
                .Clamp(Game.m.maxBumpSpeed, currentSpeed - 1);
            AddHealth(0, "Block", Game.m.darkGrey);
            Game.m.PlaySound(MedievalCombat.METAL_WEAPON_HIT_METAL_1);
        }
        //regular hit
        else {
            SetAnim(Anim.BUMPED);
            currentSpeed = (Game.m.bumpSpeed * strength / data.resistance)
                .Clamp(Game.m.maxBumpSpeed, currentSpeed - 1);
            TakeCollisionDamage(damage);
        }
    }

    public void SlightKnockbackFrom(Unit unit) => SlightKnockbackFrom(unit.data.strength);
    public void SlightKnockbackFrom(float strength) {
        if (isInvincible) currentSpeed = 0;
        else currentSpeed = Game.m.defendSpeed * strength / data.resistance;
    }


    // ====================
    // HEALTH
    // ====================

    [HideInInspector] public UnityEvent OnTakeCollisionDamage;

    public void TakeCollisionDamage(float amount, bool isCrit = false) {
        amount = (amount * this.Random(.5f, 1.5f)).Round();
        if (amount.isAbout(0)) {
            critCollisionDate = -1;
            return;
        }
        
        OnTakeCollisionDamage.Invoke();
        if (isHero || 0.5f.Chance()) {
            Game.m.PlaySound(bumpedSound, .5f, -1, pitch);
            Game.m.PlaySound(bumpedSoundAnimal, .5f, -1, pitch);
        }
        if (isCrit) {
            amount = 3 * amount;
            AddHealth(-amount, amount + "!", Game.m.red);
            Battle.m.cameraManager.Shake(0.2f);
        } else AddHealth(-amount, amount.ToString());
    }

    public void AddHealth(float amount, string uiText = null, Color uiColor = default, bool temp = false) {
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

        healthBar.enabled = false;
        this.Wait(0.1f, () => healthBar.enabled = true);
        
        if (temp) SetHealth(data.currentHealth, (tempHealth + amount).AtLeast(0));
        else if (amount.isClearlyNegative()) SetHealth(data.currentHealth + (tempHealth + amount).AtMost(0),
                (tempHealth + amount).AtLeast(0));
        else SetHealth(data.currentHealth + amount);
    }

    public void SetHealth(float amount, float tempAmount = -1) {
        data.currentHealth = Mathf.Clamp(amount, 0, data.maxHealth);
        healthBar.value = data.currentHealth / data.maxHealth;

        if (tempAmount.isClearlyNegative()) tempAmount = tempHealth;
        tempHealth = tempAmount;
        if (data.maxHealth.value.isClearlyGreaterThan(tempHealth + data.currentHealth))
            tmpHealthBar.value = (tempHealth + data.currentHealth) / data.maxHealth;
        else healthBar.value = data.currentHealth / (tempHealth + data.maxHealth);

        if (data.currentHealth <= 0 && status == Status.ALIVE) DeathByHp();
        
        if (hero != null) hero.icon.SetHealth(healthBar.value, tmpHealthBar.value);
    }
    
    
    // ====================
    // DEATH
    // ====================

    [HideInInspector] public UnityEvent onDeactivate;
    [HideInInspector] public UnityEvent onDeath;
    [HideInInspector] public UnityEvent onDefeat;

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
        onDeactivate.Invoke();
    }

    public void DieDuringBattle() {
        SetHealth(0);
        if (size >= 2 || isHero) Battle.m.cameraManager.Shake(0.2f);
        Instantiate(Run.m.deathCloudFxPrefab, transform.position + 1f*Vector3.up, Quaternion.identity);
        Game.m.PlaySound(MedievalCombat.BODY_FALL, 0.5f, 4);
        Game.m.PlaySound(deathSound, .5f, -1, pitch);
        Game.m.PlaySound(deathSoundAnimal);
        onDeath.Invoke();
    }

    public void OnDestroy() {
        if (Battle.m == null) return;
        if (Battle.m.gameState == Battle.State.GAME_OVER) return;
        if (Battle.m.gameState == Battle.State.RESTARTING) return;

        status = Status.DEAD;
        hpLossUis
            .Where(ui => ui != null)
            .ToList()
            .ForEach(ui => ui.transform.SetParent(Run.m.transform));
        allies.Remove(this);

        if (allies.Count == 0) onDefeat.Invoke();
    }
}