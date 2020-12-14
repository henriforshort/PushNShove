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
    public float attackAnimDuration;
    public float size;
    
    [Header("State")]
    public float currentSpeed;
    public Status status;
    public Anim anim;
    public float currentHealth;
    public float lastAttack;//last date I triggered my hit animation
    public bool lastWindupIsOne;
    public float critCollisionDate;
    public bool isOnFreezeFrame;
    public Unit attackTarget;

    [Header("Self References")]
    public Hero hero;
    public Slider healthBar;
    public Slider tmpHealthBar;
    public Animator animator;
    public Rigidbody rigidbodee;
    public List<HpLossUi> hpLossUis;

    private static List<Unit> _heroUnits;
    public static List<Unit> heroUnits => _heroUnits ?? (_heroUnits = new List<Unit>());

    private static List<Unit> _monsterUnits;
    public static List<Unit> monsterUnits => _monsterUnits ?? (_monsterUnits = new List<Unit>());

    public List<Unit> allies => side == Side.HERO ? heroUnits : monsterUnits;
    public List<Unit>  enemies => side == Side.MONSTER ? heroUnits : monsterUnits;

    public bool isRunning => currentSpeed.isAbout(maxSpeed);
    public bool isWalking => currentSpeed > 0;
    public bool isAttacking => Time.time - lastAttack < attackAnimDuration;
    public bool attackIsOnCooldown => Time.time - lastAttack < 0.5f;
    
    public enum Status { ALIVE, FALLING, DYING }
    public enum Anim { WALK, WINDUP, HIT, DEFEND, BUMPED }
    public enum Side { HERO = 1, MONSTER = -1 }
    
    
    // ====================
    // INIT
    // ====================

    public void Start() {
        status = Status.ALIVE;
        SetAnim(Anim.WALK);
        
        currentSpeed = maxSpeed;
        
        if (side != Side.HERO) SetHealth(maxHealth);
        tmpHealthBar.value = currentHealth;
        if (side == Side.HERO) healthBar.fillRect.GetComponent<Image>().color = G.m.yellow;
        if (side == Side.HERO) tmpHealthBar.fillRect.GetComponent<Image>().color = G.m.white;
        
        if (side == Side.HERO) heroUnits.Add(this);
        if (side == Side.MONSTER) monsterUnits.Add(this);
    }

    public void Update() {
        UpdateVisuals();
        UpdateSpeed();
        
        Move();
        CheckCollision();
    }
    
    
    // ====================
    // VISUALS
    // ====================

    public void UpdateVisuals() {
        animator.enabled = (B.m.gameState != B.State.PAUSE);

        if (!isAttacking && transform.position.z.isAbout(-1)) this.SetZ(0.5f);
        if (currentSpeed > 0 && transform.position.z.isAbout(-0.5f)) this.SetZ(0f);
        
        if (tmpHealthBar.value.isClearlyNot(currentHealth)) tmpHealthBar.LerpTo(healthBar.value, 3f);
        
        if (anim == Anim.HIT && !isAttacking) {
            SetAnim(Anim.DEFEND);
        }

        if (currentSpeed > 0 && status == Status.DYING) Die();
        
        if (currentSpeed > 0 && (anim == Anim.BUMPED || anim == Anim.DEFEND)) {
            status = Status.ALIVE;
            SetAnim(Anim.WALK);
            B.m.SpawnFX(R.m.bumpDustFxPrefab,
                new Vector3(this.GetX() - (int)side, -2, -2),
                side == Side.MONSTER);
        }
    }

    public void FreezeFrame() {
        isOnFreezeFrame = true;
        this.Wait(R.m.freezeFrameDuration, () => isOnFreezeFrame = false);
    }

    public void SetAnim(Anim a) {
        if (anim == a) return;
        anim = a;
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
        if (!CanUpdateSpeed()) return;
        
        currentSpeed = currentSpeed.LerpTo(maxSpeed, R.m.bumpRecoverySpeed);
        if (currentSpeed.isAboutOrHigherThan(maxSpeed)) currentSpeed = maxSpeed;
    }

    public bool CanUpdateSpeed() {
        if (isOnFreezeFrame) return false;
        if (B.m.gameState == B.State.PAUSE) return false;
        
        if (status == Status.FALLING) return false;
        if (currentSpeed.isAboutOrHigherThan(maxSpeed)) return false;

        return true;
    }

    public void Move() {
        if (!CanMove()) return;
        
        transform.position += currentSpeed * Time.deltaTime * (int)side * Vector3.right;
    }

    public bool CanMove() {
        if (isOnFreezeFrame) return false;
        if (B.m.gameState == B.State.PAUSE) return false;
        
        if (B.m.gameState == B.State.PLAYING) return true;
        if (status == Status.FALLING) return true; //During Game Over
        if (currentSpeed < 0) return true;
        
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
        
        ally.currentSpeed = currentSpeed;
        ally.SetAnim(Anim.BUMPED);
        ally.TakeCollisionDamage(-currentSpeed/5, true);
        ally.critCollisionDate = critCollisionDate;
        
        B.m.cameraManager.Shake(0.2f);
    }
    
    public void OnTriggerStay(Collider other) {
        if (status != Status.FALLING 
            && B.m.deathZones.Contains(other.gameObject)
            && currentSpeed.isAboutOrLowerThan(0)) 
            DeathByFall();
    }
    
    
    // ====================
    // COMBAT
    // ====================

    public void CheckCollision() { //Called by both sides
        Unit nearbyEnemy = NearbyEnemy();
        if (nearbyEnemy == null) return;
        
        if (isWalking) Attack();
    }
    
    public Unit NearbyEnemy () => enemies
            .Where (e => DistanceToMe(e) < G.m.collideDistance)
            .OrderBy(DistanceToMe)
            .FirstOrDefault();

    public void Attack() {//Called by both sides. Prepare attack. In 0.1 sec, will hit whoever is in range.
        this.SetZ(-1);
        FreezeFrame();
        SetAnim(Anim.WINDUP);
        this.Wait(0.1f, () => {
            //Set Hit anim
            Unit target = NearbyEnemy();
            SetAnim(Anim.HIT);
            if (target != null && target.status == Status.ALIVE) {
                //Inflict damage to any unit in range
                if (R.m.enableCheats && side == Side.MONSTER && Input.GetKey(KeyCode.W)) DeathByHp();
                if (R.m.enableCheats && side == Side.HERO && Input.GetKey(KeyCode.L)) DeathByHp();
                if (AttackLandsOn(target)) target.GetBumpedBy(this);
                else target.DefendFrom(this);
            }

            lastAttack = Time.time;
        });
    }

    public void GetBumpedBy(Unit other) {
        SetAnim(Anim.BUMPED);
        if (other.shakeOnHit) B.m.cameraManager.Shake(0.2f);

        critCollisionDate = critChance.Chance() ? Time.time : -1;
        TakeCollisionDamage(other.damage, critCollisionDate > 0);
        currentSpeed = R.m.bumpSpeed * other.strength * (1 - prot) - (critCollisionDate > 0 ? 5 : 0);
    }

    public void DefendFrom(Unit other) {
        status = Status.ALIVE;
        currentSpeed = R.m.defendSpeed * other.strength * (1 - prot);
    }

    public float DistanceToMe(Unit other) => (this.GetX() - other.GetX()).Abs();
    public bool AttackLandsOn(Unit other) {
        if (!CanAttack()) return false;
        if (!other.CanAttack()) return true;
        
        float momentum = (2 * weight * currentSpeed / maxSpeed).AtLeast(0);
        return Random.value < momentum / (momentum + other.weight);
    }
    public bool CanAttack() => isWalking && !attackIsOnCooldown;


    // ====================
    // HEALTH
    // ====================

    public void TakeCollisionDamage(float amount, bool isCrit = false) {
        amount = amount.MoreOrLessPercent(0.5f).Round();
        if (amount.isAbout(0)) {
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
            number.color = G.m.red;
            amount = 3 * amount;
            number.text = amount + "!";
            B.m.cameraManager.Shake(0.2f);
        } else {
            number.text = amount.ToString();
            
        }
        
        AddHealth(-amount);
    }

    public void AddHealth(float amount) => SetHealth(currentHealth + amount);
    public void SetHealth(float amount) {
        currentHealth = Mathf.Clamp(amount, 0, maxHealth);
        healthBar.value = currentHealth / maxHealth;
        if (hero != null) hero.icon.SetHealth(currentHealth/maxHealth);
        if (currentHealth <= 0) DeathByHp();
    }
    
    
    // ====================
    // DEATH
    // ====================

    public void DeathByHp() {
        Deactivate();
        status = Status.DYING;
        healthBar.transform.parent.gameObject.SetActive(false);
        currentSpeed = R.m.bumpSpeed * (1 - prot);
    }

    public void DeathByFall() {
        Deactivate();
        status = Status.FALLING;
        rigidbodee.useGravity = true;
        this.Wait(0.5f, Die);
    }

    public void Deactivate() {
        SetAnim(Anim.BUMPED);
        
    }

    public void Die() {
        if (B.m == null || B.m.gameState != B.State.PLAYING) return;
        if (size >= 2 || side == Side.HERO) B.m.cameraManager.Shake(0.2f);
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
