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
    public bool shakeOnHit;
    
    [Header("State")]
    public float currentSpeed;
    public Status status;
    public Anim anim;
    public float currentHealth;
    public float lastAttack;
    public bool lastWindupIsOne;
    public bool isDeactivated;
    
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
    
    public enum Status { WALK, BUMPED, FALLING }
    public enum Anim { WALK, WINDUP, HIT, DEFEND, BUMPED }
    
    
    // ====================
    // INIT
    // ====================

    public void Start() {
        status = Status.WALK;
        SetAnim(Anim.WALK);
        
        currentSpeed = maxSpeed;
        
        if (side != Side.HERO) SetHealth(maxHealth);
        orangeHealthBar.value = currentHealth;
        
        if (side == Side.HERO) heroUnits.Add(this);
        if (side == Side.MONSTER) monsterUnits.Add(this);
    }

    public void Update() {
        UpdateVisuals();
        UpdateSpeed();
        
        if (CanMove()) Move();
    }
    
    
    // ====================
    // VISUALS
    // ====================

    public void UpdateVisuals() {
        animator.enabled = (B.m.gameState != B.State.PAUSE);

        if (!isAttacking && transform.position.z.isApprox(-1)) this.SetZ(0.5f);
        if (currentSpeed > 0 && transform.position.z.isApprox(-0.5f)) this.SetZ(0f);
        
        if (orangeHealthBar.value.isNotApprox(currentHealth)) orangeHealthBar.LerpTo(healthBar.value, 2.5f);
        
        if (anim == Anim.HIT && !isAttacking) {
            if (status == Status.BUMPED) SetAnim(Anim.BUMPED);
            else SetAnim(Anim.DEFEND);
        }
        
        if (currentSpeed > 0 && (anim == Anim.BUMPED || anim == Anim.DEFEND)) {
            status = Status.WALK;
            SetAnim(Anim.WALK);
            B.m.SpawnFX(G.m.bumpDustFxPrefab,
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
        
        currentSpeed = currentSpeed.LerpTo(maxSpeed, G.m.bumpRecoverySpeed);
        
        if (currentSpeed >= maxSpeed || currentSpeed.isApprox(maxSpeed)) currentSpeed = maxSpeed;
    }

    public void Move() {
        transform.position += currentSpeed * Time.deltaTime * (int)side * Vector3.right;
    }

    public bool CanMove() {
        if (B.m.gameState == B.State.PLAYING) return true;
        else if (status == Status.FALLING) return true;
        else if (currentSpeed < 0) return true;
        else return false;
    }
    
    
    // ====================
    // COLLISIONS
    // ====================

    public void LongRangeCollide(Collider other) { //Called by both sides
        Unit otherUnit = enemies.FirstOrDefault(u => u.gameObject == other.gameObject);
        if (otherUnit == null) return;
        
        if (currentSpeed > 0 && otherUnit.currentSpeed > 0) SetAnim(Anim.WINDUP);
    }

    public void OnTriggerStay(Collider other) {
        Unit otherUnit = monsterUnits.FirstOrDefault(u => u.gameObject == other.gameObject);
        if (side == Side.HERO && otherUnit != null) Collide(otherUnit);
        
        if (status != Status.FALLING && B.m.deathZones.Contains(other.gameObject)) DeathByFall();
    }

    public void Collide(Unit other) { //Only called by player character
        if (currentSpeed < 0 && other.currentSpeed < 0) return;
        
        if (G.m.enableCheats && Input.GetKey(KeyCode.W)) other.DeathByHp();
        if (G.m.enableCheats && Input.GetKey(KeyCode.L)) DeathByHp();
        
        ProcessAttack(other);
    }
    
    
    // ====================
    // ATTACK
    // ====================

    public void ProcessAttack(Unit other) {
        bool heroAttacks = CanAttack(other);
        bool monsterAttacks = other.CanAttack(this);
        // heroAttacks = false;
        // monsterAttacks = false;

        this.SetZ(-1);
        other.SetZ(-1);

        if (heroAttacks || monsterAttacks) { //FIGHT!
            B.m.SpawnFX(G.m.sparkFxPrefab,
                transform.position + new Vector3(1.5f * (int) side, 0, -2),
                side == Side.HERO, null, 0.5f,
                Vector3.forward * Random.Range(0, 360));

            // B.m.audioSource.PlayOneShot(G.m.damageSounds.Random());
        }

        if (heroAttacks) other.GetBumped(this, monsterAttacks);
        else other.Defend(this, monsterAttacks);

        if (monsterAttacks) GetBumped(other, heroAttacks);
        else Defend(other, heroAttacks);

        if (isDeactivated) other.Defend(this, true);
        if (other.isDeactivated) Defend(other, true);
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
        currentSpeed = G.m.defendSpeed * other.strength * (1 - prot);
        //If I'm bigger, and we both defend, only the smaller unit attacks! the bigger unit doesnt move
        if (size > other.size && !alsoAttack) SetAnim(Anim.DEFEND); 
        else SetAnim(Anim.HIT);
    } 

    public void GetBumped(Unit other, bool alsoAttack) {
        status = Status.BUMPED;
        currentSpeed = G.m.bumpSpeed * other.strength * (1 - prot);
        if (alsoAttack || size.isApprox(0.5f)) SetAnim(Anim.HIT);
        else SetAnim(Anim.BUMPED);
        
        TakeCollisionDamage(other.damage);
        if (other.shakeOnHit) B.m.Shake(0.2f);
    }


    // ====================
    // HEALTH
    // ====================

    public void TakeCollisionDamage(float amount) {
        HpLossUi hpLossUi = B.m.SpawnFX(G.m.hpLossUIPrefab,
                transform.position + new Vector3(0.2f * (int) side, 2.25f*size - 0.6f, -5),
                false,
                transform,
                3)
            .GetComponent<HpLossUi>();
        TMP_Text number = hpLossUi.number;

        hpLossUis.Add(hpLossUi);
        hpLossUi.unit = this;
        
        amount = amount.MoreOrLessPercent(0.5f).Round();
        if (10.PercentChance()) { //Crit!
            number.color = G.m.red;
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
        Deactivate();
        Destroy();
    }

    public void DeathByFall() {
        Deactivate();
        status = Status.FALLING;
        SetAnim(Anim.BUMPED);
        rigidbodee.useGravity = true;
        this.Wait(0.5f, Destroy);
    }

    public void Deactivate() {
        isDeactivated = true;
        hpLossUis.ForEach(ui => ui.transform.SetParent(G.m.transform));
        allies.Remove(this);

        if (allies.Count == 0) {
            if (side == Side.HERO) B.m.Defeat();
            else if (side == Side.MONSTER) B.m.Victory();
        }
    }

    public void Destroy() {
        if (size >= 1) B.m.Shake(0.2f);
        // B.m.audioSource.PlayOneShot(G.m.deathSounds.Random());
        Instantiate(G.m.deathCloudFxPrefab, transform.position + 0.5f*Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
}
