using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Unit : MonoBehaviour {
    [Header("Balancing")]
    public Side side;
    public float maxSpeed;
    public float maxHealth; //Resistance
    [Range (0,1)] public float prot; //How much collision force is reduced (whether I got hit or not)
    public float weight; //Chance to hit & not get hit
    public float damage; //How much damage I deal when i hit
    public float strength; //How far I push people I collide with (wether I hit them or not)
    [Range(0, 1)] public float critChance;
    public bool shakeOnHit;
    
    [Header("State")]
    public float currentSpeed;
    public Status status;
    public Anim anim;
    public float currentHealth;
    public float lastAttack;//last date I triggered my hit animation
    public float lastBump;//last date I hit someone
    public bool lastWindupIsOne;
    public float critCollisionDate;
    
    [Header("References")]
    public float size;
    public float attackAnimDuration;
    public Slider healthBar;
    public Slider orangeHealthBar;
    public Animator animator;
    public Rigidbody rigidbodee;
    public List<HpLossUi> hpLossUis;

    private static List<Unit> _heroUnits;
    public static List<Unit> heroUnits => _heroUnits ?? (_heroUnits = new List<Unit>());

    private static List<Unit> _monsterUnits;
    public static List<Unit> monsterUnits => _monsterUnits ?? (_monsterUnits = new List<Unit>());

    public List<Unit> allies => side == Side.HERO ? heroUnits : monsterUnits;
    public List<Unit> enemies => side == Side.MONSTER ? heroUnits : monsterUnits;

    public bool isRunning => currentSpeed.isApprox(maxSpeed);
    public bool isAttacking => Time.time - lastAttack < attackAnimDuration;
    public bool isOnFreezeFrame => Time.time - lastBump < R.m.freezeFrameDuration;
    
    public enum Status { WALK, BUMPED, FALLING, DYING }
    public enum Anim { WALK, WINDUP, HIT, DEFEND, BUMPED }public enum Side { HERO = 1, MONSTER = -1 }
    
    
    // ====================
    // INIT
    // ====================

    public void Start() {
        status = Status.WALK;
        SetAnim(Anim.WALK);
        
        currentSpeed = maxSpeed;
        
        if (side != Side.HERO) SetHealth(maxHealth);
        orangeHealthBar.value = currentHealth;
        if (side == Side.HERO) healthBar.fillRect.GetComponent<Image>().color = R.m.yellow;
        if (side == Side.HERO) orangeHealthBar.fillRect.GetComponent<Image>().color = R.m.white;
        
        if (side == Side.HERO) heroUnits.Add(this);
        if (side == Side.MONSTER) monsterUnits.Add(this);
    }

    public void Update() {
        UpdateVisuals();
        UpdateSpeed();
        
        Move();
    }
    
    
    // ====================
    // VISUALS
    // ====================

    public void UpdateVisuals() {
        animator.enabled = (B.m.gameState != B.State.PAUSE);

        if (!isAttacking && transform.position.z.isApprox(-1)) this.SetZ(0.5f);
        if (currentSpeed > 0 && transform.position.z.isApprox(-0.5f)) this.SetZ(0f);
        
        if (orangeHealthBar.value.isNotApprox(currentHealth)) orangeHealthBar.LerpTo(healthBar.value, 0.5f);
        
        if (anim == Anim.HIT && !isAttacking) {
            if (status == Status.BUMPED) SetAnim(Anim.BUMPED);
            else SetAnim(Anim.DEFEND);
        }

        if (currentSpeed > 0 && status == Status.DYING) Die();
        
        if (currentSpeed > 0 && (anim == Anim.BUMPED || anim == Anim.DEFEND)) {
            status = Status.WALK;
            SetAnim(Anim.WALK);
            B.m.SpawnFX(R.m.bumpDustFxPrefab,
                new Vector3(this.GetX() - (int)side, -2, -2),
                side == Side.MONSTER);
        }
    }

    public void SetAnim(Anim a) {
        if (anim == a) return;
        
        anim = a;
        if (a == Anim.HIT) lastAttack = Time.time;
        
        PlayAnim();
    }

    //Play current anim
    //Pick one at random if it's WINDUP
    //Play appropriate one if it's HIT (HIT if WINDUP, HIT2 if WINDUP2)
    public void PlayAnim() {
        if (anim == Anim.WINDUP) {
            if (this.CoinFlip()) {
                lastWindupIsOne = true;
                animator.Play(anim.ToString());
            } else {
                lastWindupIsOne = false;
                animator.Play(anim + "2");
            }
        } else if (anim == Anim.HIT) {
            if (lastWindupIsOne) animator.Play(anim.ToString());
            else animator.Play(anim + "2");
        } else animator.Play(anim.ToString());
    }
    
    
    // ====================
    // MOVEMENT
    // ====================

    public void UpdateSpeed() {
        if (currentSpeed >= maxSpeed) return;
        if (currentSpeed.isApprox(maxSpeed)) return;
        if (B.m.gameState == B.State.PAUSE) return;
        if (status == Status.FALLING) return;
        if (isOnFreezeFrame) return;
        
        currentSpeed = currentSpeed.LerpTo(maxSpeed, R.m.bumpRecoverySpeed);
        
        if (currentSpeed >= maxSpeed || currentSpeed.isApprox(maxSpeed)) currentSpeed = maxSpeed;
    }

    public void Move() {
        if (!CanMove()) return;
        
        transform.position += currentSpeed * Time.deltaTime * (int)side * Vector3.right;
    }

    public bool CanMove() {
        if (isOnFreezeFrame) return false;
        if (B.m.gameState == B.State.PLAYING) return true;
        else if (status == Status.FALLING) return true;
        else if (currentSpeed < 0) return true;
        
        return false;
    }
    
    
    // ====================
    // LONG RANGE COLLIDE (WINDUP & FRIENDLY COLLIDE)
    // ====================

    public void LongRangeCollide(Collider other) { //Called by both sides
        Unit collidedEnemy = enemies.FirstOrDefault(u => u.gameObject == other.gameObject);
        if (collidedEnemy != null 
                && currentSpeed > 0 
                && collidedEnemy.currentSpeed > 0) 
            SetAnim(Anim.WINDUP);
        
        Unit collidedAlly = allies.FirstOrDefault(u => u.gameObject == other.gameObject);
        if (collidedAlly != null 
                && enemies.Count > 0
                && currentSpeed < 0
                && this.isCloserTo(enemies[0].transform.position, than:collidedAlly)
                && critCollisionDate > 0
                && critCollisionDate.isNotApprox(collidedAlly.critCollisionDate)) 
            FriendlyCollide(collidedAlly);
    }

    public void FriendlyCollide(Unit ally) { //Called by the unit in front
        currentSpeed = (currentSpeed + 2).AtMost(0); //slow down
        
        ally.status = Status.BUMPED;
        ally.currentSpeed = currentSpeed;
        ally.SetAnim(Anim.BUMPED);
        ally.TakeCollisionDamage(-currentSpeed/5, true);
        ally.critCollisionDate = critCollisionDate;
        
        B.m.Shake(0.2f);
    }

    
    
    // ====================
    // SORT RANGE COLLIDE (ATTACK & FALL)
    // ====================
    
    public void OnTriggerStay(Collider other) {
        Unit otherUnit = monsterUnits.FirstOrDefault(u => u.gameObject == other.gameObject);
        if (side == Side.HERO 
                && currentSpeed > 0 
                && otherUnit != null 
                && otherUnit.currentSpeed > 0)
            Collide(otherUnit);
        
        if (status != Status.FALLING 
                && B.m.deathZones.Contains(other.gameObject)) 
            DeathByFall();
    }

    public void Collide(Unit other) { //Only called by player character
        bool heroAttacks = CanAttack(other);
        bool monsterAttacks = other.CanAttack(this);
        this.SetZ(-1);
        other.SetZ(-1);
        other.lastBump = Time.time;
        lastBump = Time.time;

        if (heroAttacks || monsterAttacks) { //FIGHT!
            B.m.SpawnFX(R.m.sparkFxPrefab,
                transform.position + new Vector3(1.5f * (int) side, 0, -2),
                side == Side.HERO, null, 0.5f,
                Vector3.forward * Random.Range(0, 360));

            // B.m.audioSource.PlayOneShot(R.m.damageSounds.Random());
        }

        if (heroAttacks) other.GetBumped(this, monsterAttacks);
        else other.Defend(this, monsterAttacks);
        if (monsterAttacks) GetBumped(other, heroAttacks);
        else Defend(other, heroAttacks);
        
        if (R.m.enableCheats && Input.GetKey(KeyCode.W)) other.DeathByHp();
        if (R.m.enableCheats && Input.GetKey(KeyCode.L)) DeathByHp();

        //I won't get bumped by a dead man!
        if (status == Status.FALLING || status == Status.DYING) other.Defend(this, true);
        if (other.status == Status.FALLING || other.status == Status.DYING) Defend(other, true);
    }

    public bool CanAttack(Unit other) {
        if (currentSpeed < 0) return false;
        if (Time.time - lastAttack < 0.5f) return false;
        if (other.currentSpeed < 0) return true;
        if (Time.time - other.lastAttack < 0.5f) return true;
        
        float momentum = (weight * currentSpeed / maxSpeed).AtLeast(0);
        return Random.value < momentum / (momentum + other.weight);
    }

    public void Defend(Unit other, bool alsoAttack) {
        status = Status.WALK;
        currentSpeed = R.m.defendSpeed * other.strength * (1 - prot);
        //If I'm bigger, and we both defend, only the smaller unit attacks! the bigger unit doesnt move
        if (size > other.size && !alsoAttack) SetAnim(Anim.DEFEND); 
        else SetAnim(Anim.HIT);
    } 

    public void GetBumped(Unit other, bool alsoAttack) {
        status = Status.BUMPED;

        //Small units don't hit if both are bumped
        if (alsoAttack && size >= 1) SetAnim(Anim.HIT);
        else SetAnim(Anim.BUMPED);
        if (other.shakeOnHit) B.m.Shake(0.2f);

        bool isCrit = critChance.Chance();
        currentSpeed = R.m.bumpSpeed * other.strength * (1 - prot) - (isCrit ? 5 : 0);
        critCollisionDate = isCrit ? Time.time : -1;
        TakeCollisionDamage(other.damage, isCrit);
    }


    // ====================
    // HEALTH
    // ====================

    public void TakeCollisionDamage(float amount, bool isCrit = false) {
        amount = amount.MoreOrLessPercent(0.5f).Round();
        if (amount.isApprox(0)) {
            critCollisionDate = -1;
            return;
        }
        
        HpLossUi hpLossUi = B.m.SpawnFX(R.m.hpLossUIPrefab,
                transform.position + new Vector3(0.2f * (int) side, 2.25f*size - 0.6f, -5),
                false,
                transform,
                3)
            .GetComponent<HpLossUi>();
        hpLossUis.Add(hpLossUi);
        hpLossUi.unit = this;
        
        TMP_Text number = hpLossUi.number;
        if (isCrit) {
            number.color = R.m.red;
            amount = 3 * amount;
            number.text = amount + "!";
            B.m.Shake(0.2f);
        } else {
            number.text = amount.ToString();
            
        }
        
        AddHealth(-amount);
    }

    public void AddHealth(float amount) => SetHealth(currentHealth + amount);
    public void SetHealth(float amount) {
        currentHealth = Mathf.Clamp(amount, 0, maxHealth);
        healthBar.value = currentHealth / maxHealth;
        if (currentHealth <= 0) DeathByHp();
    }
    
    
    // ====================
    // DEATH
    // ====================

    public void DeathByHp() {
        status = Status.DYING;
        SetAnim(Anim.BUMPED);
        currentSpeed = R.m.bumpSpeed * (1 - prot);
    }

    public void DeathByFall() {
        status = Status.FALLING;
        SetAnim(Anim.BUMPED);
        rigidbodee.useGravity = true;
        this.Wait(0.5f, Die);
    }

    public void Die() {
        if (B.m == null || B.m.gameState != B.State.PLAYING) return;
        if (size >= 2 || side == Side.HERO) B.m.Shake(0.2f);
        // B.m.audioSource.PlayOneShot(R.m.deathSounds.Random());
        Instantiate(R.m.deathCloudFxPrefab, transform.position + 0.5f*Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }

    public void OnDestroy() {
        if (B.m == null || B.m.gameState != B.State.PLAYING) return;
        
        allies.Remove(this);
        if (allies.Count == 0) {
            if (side == Side.HERO) B.m.Defeat();
            else if (side == Side.MONSTER) B.m.Victory();
        }
        
        hpLossUis.ForEach(ui => ui.transform.SetParent(R.m.transform));
    }
}
