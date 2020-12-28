using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour {
    [Header("Stats")]
    public Stat maxSpeed;
    public Stat maxHealth; //Resistance
    public Stat prot; //How much collision force is reduced (whether I got hit or not)
    public Stat weight; //Chance to hit & not get hit
    public Stat damage; //How much damage I deal when i hit
    public Stat strength; //How far I push people I collide with (wether I hit them or not)
    public Stat critChance;

    [Header("Base Stats")]
    public float baseMaxSpeed;
    public float baseMaxHealth;
    [Range(0,1)] public float baseProt;
    public float baseWeight;
    public float baseDamage;
    public float baseStrength;
    [Range(0,1)] public float baseCritChance;
    
    [Header("Balancing")]
    public Side side;
    public bool shakeOnHit;
    public float attackAnimDuration;
    public float size;
    public float attackSpeed;
    
    [Header("State")]
    public float currentSpeed;
    public Status status;
    public AttackStatus attackStatus;
    public Anim anim;
    public float currentHealth;
    public bool lastWindupIsTwo;
    public float critCollisionDate;
    public bool isOnFreezeFrame;
    public bool isInvincible;
    public bool lockZOrder;
    public bool lockAnim;
    public bool lockPosition;
    public int index;

    [Header("Self References")]
    public Hero hero;
    public Monster monster;
    public Slider healthBar;
    public Slider tmpHealthBar;
    public Animator animator;
    public Rigidbody rigidbodee;
    
    [Header("Self References (Assigned at runtime)")]
    public List<HpLossUi> hpLossUis;

    private static List<Unit> _allHeroUnits; //Even the dead ones
    public static List<Unit> allHeroUnits => _allHeroUnits ?? (_allHeroUnits = new List<Unit>());

    private static List<Unit> _heroUnits; //Only the ones that are still alive
    public static List<Unit> heroUnits => _heroUnits ?? (_heroUnits = new List<Unit>());

    private static List<Unit> _monsterUnits;
    public static List<Unit> monsterUnits => _monsterUnits ?? (_monsterUnits = new List<Unit>());

    public List<Unit> allies => side == Side.HERO ? heroUnits : monsterUnits;
    public List<Unit>  enemies => side == Side.MONSTER ? heroUnits : monsterUnits;

    public float speedPercent => currentSpeed / maxSpeed;
    public bool isWalking => currentSpeed > 0;
    
    public enum Status { ALIVE, FALLING, DYING, DEAD }
    public enum AttackStatus { NOT_PREPARED, PREPARING, ATTACKING, RECOVERING }
    public enum Anim {
        WALK, WINDUP, HIT, DEFEND, BUMPED,
        ULT_BRUISER, ULT_STRONGMAN
    }
    public enum Side { HERO = 1, MONSTER = -1 }
    
    
    // ====================
    // INIT
    // ====================

    public void InitGame() {
        maxSpeed.Init(baseMaxSpeed);
        maxHealth.Init(baseMaxHealth);
        prot.Init(baseProt);
        weight.Init(baseWeight);
        damage.Init(baseDamage);
        strength.Init(baseStrength);
        critChance.Init(baseCritChance);
    }

    public void InitBattle() {
        SetAnim(Anim.WALK);
        currentSpeed = maxSpeed;
        tmpHealthBar.value = currentHealth;
        
        if (side == Side.MONSTER) MonsterInit();
        if (side == Side.HERO) HeroInit();
    }

    public void MonsterInit() {
        if (!gameObject.activeInHierarchy) return;
            
        monsterUnits.Add(this);
        InitGame();
        SetHealth(maxHealth);
    }

    public void HeroInit() {
        index = allHeroUnits.Count;
        allHeroUnits.Add(this);
        heroUnits.Add(this);
    }
    
    
    // ====================
    // UPDATE
    // ====================

    public void Update() {
        if (status == Status.DEAD) return;
        
        UpdateSpeed();
        UpdatePosition();
        UpdateCombat();
        UpdateVisuals();
    }
    
    
    // ====================
    // VISUALS
    // ====================
    
    public void UpdateVisuals() {
        animator.enabled = (Battle.m.gameState != Battle.State.PAUSE);
        
        if (lockZOrder) this.SetZ(-5);
        else if (attackStatus != AttackStatus.ATTACKING && this.GetZ().isAbout(-1)) this.SetZ(-0.5f);
    }

    public void SetAnim(Anim a) {
        if (lockAnim) return;
        if (anim == a) return;
        
        anim = a;
        PlayAnim();
    }

    //Play current anim
    //Pick one at random if it's WINDUP
    //Play appropriate one if it's HIT (HIT if WINDUP, HIT2 if WINDUP2)
    public void PlayAnim() {
        bool playAnimTwo = false;
        if (anim == Anim.WINDUP) {
            playAnimTwo = this.CoinFlip();
            lastWindupIsTwo = playAnimTwo;
        }
        if (anim == Anim.HIT) playAnimTwo = lastWindupIsTwo;
        if (animator.gameObject.activeInHierarchy) animator.Play(anim + (playAnimTwo ? "2" : ""));
    }

    public void FreezeFrame() {
        isOnFreezeFrame = true;
        this.Wait(Run.m.freezeFrameDuration, () => isOnFreezeFrame = false);
    }

    public void StartWalking() {
        SetAnim(Anim.WALK);
        this.SetZ(0f);
        if (status == Status.DYING) DieDuringBattle();
        else Game.m.SpawnFX(Run.m.bumpDustFxPrefab,
                new Vector3(this.GetX() - (int)side, -2, -2),
                side == Side.MONSTER, Battle.m.transform);
    }

    
    // ====================
    // MOVEMENT
    // ====================

    public void UpdateSpeed() {
        if (!CanUpdateSpeed()) return;
        
        float newSpeed = currentSpeed.LerpTo(maxSpeed, Run.m.bumpRecoverySpeed);
        if (currentSpeed < 0 && newSpeed > 0) StartWalking();
        currentSpeed = newSpeed;
        if (currentSpeed.isAboutOrHigherThan(maxSpeed)) currentSpeed = maxSpeed;
    }

    public bool CanUpdateSpeed() {
        if (lockPosition) return false;
        if (isOnFreezeFrame) return false;
        if (Battle.m.gameState == Battle.State.PAUSE) return false;
        
        if (status == Status.FALLING) return false;
        if (currentSpeed.isAboutOrHigherThan(maxSpeed)) return false;

        return true;
    }

    public void UpdatePosition() {
        if (!CanMove()) return;
        
        transform.position += currentSpeed * Time.deltaTime * (int)side * Vector3.right;
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

    public void UpdateCombat() { //Called by both sides
        if (Battle.m.gameState != Battle.State.PLAYING) return;
        if (isWalking 
                && attackStatus == AttackStatus.NOT_PREPARED 
                && NearbyEnemy() != null) 
            PrepareAttack();

        Unit collidingEnemy = CollidingEnemy();
        if (isWalking
                && attackStatus != AttackStatus.PREPARING
                && collidingEnemy != null
                && collidingEnemy.isWalking
                && collidingEnemy.attackStatus != AttackStatus.PREPARING) {
            SetAnim(Anim.DEFEND);
            DefendFrom(collidingEnemy);
            collidingEnemy.SetAnim(Anim.DEFEND);
            collidingEnemy.DefendFrom(this);
        }
    }

    public void PrepareAttack() {//Called by both sides
        SetAnim(Anim.WINDUP);
        attackStatus = AttackStatus.PREPARING;
        this.SetZ(-1);
        FreezeFrame();
        this.Wait(0.1f, TryAttack);
    }

    public void TryAttack() {//Called by both sides
        if (attackStatus != AttackStatus.PREPARING) return;
        
        Unit target = NearbyEnemy();
        if (target == null) Attack();
        else ResolveCombat(this, target);
    }

    public void Attack() {
        SetAnim(Anim.HIT);
        attackStatus = AttackStatus.ATTACKING;
        this.Wait(attackAnimDuration, RecoverFromAttack);
    }

    public void ResolveCombat(Unit unit1, Unit unit2) { //Called by attacking side only
        if (Run.m.enableCheats && Input.GetKey(KeyCode.W)) (unit1.side == Side.HERO ? unit2 : unit1).DeathByHp();
        if (Run.m.enableCheats && Input.GetKey(KeyCode.L)) (unit1.side == Side.MONSTER ? unit2 : unit1).DeathByHp();

        Unit winner = GetAttackWinner(unit1, unit2);
        Unit loser = (winner == unit1 ? unit2 : unit1); 
        
        winner.Attack();
        loser.Attack();
        
        loser.GetBumpedBy(winner);
        winner.DefendFrom(loser);
        
        if (winner.shakeOnHit) Battle.m.cameraManager.Shake(0.2f);
        Game.m.SpawnFX(Run.m.sparkFxPrefab,
            transform.position + new Vector3(1.5f * (int) side, 1, -2),
            false, null, 0.5f,
            Vector3.forward * Random.Range(0, 360));
        
    }

    public Unit GetAttackWinner(Unit unit1, Unit unit2) {
        if (unit1.isInvincible) return unit1;
        if (unit2.isInvincible) return unit2;
        
        if (!unit1.CanAttack()) return unit2;
        if (!unit2.CanAttack()) return unit1;
        
        float momentum1 = unit1.weight * (2 * unit1.speedPercent).Clamp01(); //max momentum if > 50% speed
        float momentum2 = unit2.weight * (2 * unit2.speedPercent).Clamp01();
        return Random.value < momentum1 / (momentum1 + momentum2) ? unit1 : unit2;
    }

    public void RecoverFromAttack() {
        attackStatus = AttackStatus.RECOVERING;
        this.Wait(attackSpeed, () => attackStatus = AttackStatus.NOT_PREPARED);
    }

    public void GetBumpedBy(Unit other) {
        SetAnim(Anim.BUMPED);
        currentSpeed = Run.m.bumpSpeed * other.strength * (1 - prot);
        
        bool isCrit = ((float)other.critChance).Chance();
        TakeCollisionDamage(other.damage, isCrit);
        if (isCrit) {
            critCollisionDate = Time.time;
            currentSpeed -= 5;
        }
    }

    public void DefendFrom(Unit other) {
        currentSpeed = Run.m.defendSpeed * other.strength * (1 - prot);
    }

    public Unit NearbyEnemy() => enemies
        .WithLowest(DistanceToMe)
        .If(e => e != null && DistanceToMe(e) < Run.m.attackDistance);
    
    public Unit CollidingEnemy() => enemies
        .WithLowest(DistanceToMe)
        .If(e => e != null && DistanceToMe(e) < Run.m.collideDistance);

    public float DistanceToMe(Unit other) => (this.GetX() - other.GetX()).Abs();
    public bool CanAttack() => isWalking && attackStatus == AttackStatus.PREPARING;

    public virtual void Ult() { }
    public virtual void EndUlt() { }


    // ====================
    // HEALTH
    // ====================

    public void TakeCollisionDamage(float amount, bool isCrit = false) {
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
            HpLossUi hpLossUi = Game.m.SpawnFX(Run.m.hpLossUIPrefab,
                    transform.position + 
                        new Vector3(0.2f * (int) side + this.Random(-0.5f, 0.5f), 2.25f*size - 0.6f, -5),
                    false,
                    transform,
                    3)
                .GetComponent<HpLossUi>();
            hpLossUis.Add(hpLossUi);
            hpLossUi.unit = this;
            TMP_Text number = hpLossUi.number;
            number.color = uiColor;
            number.text = uiText;
        }
        
        SetHealth(currentHealth + amount);
    }

    public void SetHealth(float amount) {
        currentHealth = Mathf.Clamp(amount, 0, maxHealth);
        healthBar.value = currentHealth / maxHealth;
        tmpHealthBar.value = healthBar.value;
        healthBar.gameObject.SetActive(false);
        this.Wait(0.1f, () => healthBar.gameObject.SetActive(true));
        
        if (hero != null) {
            hero.icon.SetHealth(currentHealth/maxHealth);
            hero.icon.FlashHealth();
        }
        if (currentHealth <= 0) DeathByHp();
    }
    
    
    // ====================
    // DEATH
    // ====================

    public void DeathByHp() {
        Deactivate();
        status = Status.DYING;
        healthBar.transform.parent.gameObject.SetActive(false);
        currentSpeed = Run.m.bumpSpeed * (1 - prot);
    }

    public void DeathByFall() {
        Deactivate();
        status = Status.FALLING;
        rigidbodee.useGravity = true;
        this.Wait(0.5f, DieDuringBattle);
    }

    public void Deactivate() {
        allies.Remove(this);
        SetAnim(Anim.BUMPED);
        if (side == Side.HERO) hero.icon.Die();
    }

    public void DieDuringBattle() {
        if (size >= 2 || side == Side.HERO) Battle.m.cameraManager.Shake(0.2f);
        Instantiate(Run.m.deathCloudFxPrefab, transform.position + 1.5f*Vector3.up, Quaternion.identity);
        Die();
    }

    public void Die() {
        status = Status.DEAD;
        if (side == Side.HERO) HeroDeath();
        else MonsterDeath();
    }

    public void HeroDeath() {
        allies.Remove(this);
        animator.gameObject.SetActive(false);
        Run.m.save.heroes[index].Save();
        hero.icon.Die();
        hero.EndUlt();
        OnDestroy();
    }

    public void MonsterDeath() {
        if (monster.dropRate.Chance() && !Run.m.itemsDepleted)
            heroUnits.Where(u => u.hero.items.Count < Run.m.maxItemsPerHero).ToList()
                .Random()
                ?.hero
                ?.GetItemFromFight(Run.m.GetRandomItem(), this);
        Destroy(gameObject);
    }

    public void OnDestroy() {
        if (Battle.m == null || Battle.m.gameState != Battle.State.PLAYING) return;
        hpLossUis.ForEach(ui => ui.transform.SetParent(Run.m.transform));
        allies.Remove(this);

        if (allies.Count == 0) {
            if (side == Side.HERO) Battle.m.Defeat();
            else Battle.m.Victory();
        }
    }
}
