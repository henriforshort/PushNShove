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
    public float weight;
    public float maxHealth;
    public float damage;
    public float attackAnimDuration;
    public float size;
    public Vector3 hpLossPosition;
    public bool shakeOnHit;
    public Vector3 visualsPosition;
    
    [Header("State")]
    public float currentSpeed;
    public Status status;
    public Anim anim;
    public float currentHealth;
    public float lastAttack;
    public float collideDate;
    
    [Header("References")]
    public Slider healthBar;
    public Slider orangeHealthBar;
    public Animator animator;
    public Rigidbody rigidbodee;

    private static List<Unit> _heroUnits;
    public static List<Unit> heroUnits => _heroUnits ?? (_heroUnits = new List<Unit>());

    private static List<Unit> _monsterUnits;
    public static List<Unit> monsterUnits => _monsterUnits ?? (_monsterUnits = new List<Unit>());

    public List<Unit> allies => side == Side.HERO ? heroUnits : monsterUnits;
    public List<Unit> enemies => side == Side.MONSTER ? heroUnits : monsterUnits;
    
    public enum Status { WALK, BUMPED, FALLING } //Status
    public enum Anim { WALK, WINDUP, HIT, DEFEND, BUMPED } //Animation
    
    
    // ====================
    // BASIC METHODS
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
        UpdateSpeed();
        UpdateVisuals();
        
        Move();
    }

    public void SetAnim(Anim a) {
        if (anim == a) return;
        
        anim = a;

        if (a == Anim.HIT) lastAttack = Time.time;
        
        if (a == Anim.HIT && this.CoinFlip()) animator.Play("HIT2");
        else animator.Play(a.ToString());
    }

    public void UpdateSpeed() {
        if (currentSpeed >= maxSpeed) return;
        if (currentSpeed.isApprox(maxSpeed)) return;
        if (B.m.gameState != B.State.PLAYING) return;
        if (status == Status.FALLING) return;
        
        if (Time.time - collideDate > G.m.bumpDurationBeforeRecovery)
            currentSpeed = currentSpeed.LerpTo(maxSpeed, G.m.bumpRecoverySpeed);
        
        if (currentSpeed >= maxSpeed || currentSpeed.isApprox(maxSpeed)) currentSpeed = maxSpeed;
    }

    public void UpdateVisuals() {
        animator.enabled = (B.m.gameState != B.State.PAUSE);
        
        if (!orangeHealthBar.value.isApprox(currentHealth))
            orangeHealthBar.value = orangeHealthBar.value.LerpTo(healthBar.value, 2.5f);

        if (anim == Anim.HIT && Time.time - lastAttack > attackAnimDuration) {
            if (status == Status.BUMPED) SetAnim(Anim.BUMPED);
            else SetAnim(Anim.DEFEND);
        }
        
        if (currentSpeed > 0 && (anim == Anim.BUMPED || anim == Anim.DEFEND)) {
            status = Status.WALK;
            SetAnim(Anim.WALK);
            B.m.SpawnFX(G.m.bumpDustFxPrefab,
                new Vector3(this.GetX() - (int)side*size, -2, -2),
                side == Side.MONSTER);
        }
    }

    public void Move() {
        if (B.m.gameState != B.State.PLAYING && status != Status.FALLING) return;
        
        transform.position += currentSpeed * Time.deltaTime * (int)side * Vector3.right;
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
        
        other.collideDate = Time.time;
        collideDate = Time.time;
        
        if (currentSpeed > G.m.speedToAttack) SetAnim(Anim.HIT);
        else SetAnim(Anim.DEFEND);
        
        if (other.currentSpeed > G.m.speedToAttack) other.SetAnim(Anim.HIT);
        else other.SetAnim(Anim.DEFEND);

        SetSpeedsAfterBump(other, 
            currentSpeed < 0, 
            other.currentSpeed < 0);
        
        if (CanAttack(other)) Attack(other);
        if (other.CanAttack(this)) other.Attack(this);
    }

    public void SetSpeedsAfterBump(Unit other, bool hasDisadvantage, bool otherHasDisadvantage) {
        // Basic concept : the total amount of speed is conserved (the sum of the two unit's speed stays the same)
        // This total speed is distributed proportionally to each unit's weight
        // I added a few tweaks for better gamefeel
        float totalSpeed = currentSpeed + other.currentSpeed;
        
        //If a unit is moving backwards, their weight is divided bu 10
        float collisionWeight = hasDisadvantage ? weight / 10 : weight;
        float otherCollisionWeight = otherHasDisadvantage ? other.weight / 10 : other.weight;
        float totalWeight = collisionWeight + otherCollisionWeight;
        
        // If the units were moving forward, stop them completely before adding the speed from the collision
        currentSpeed = currentSpeed.AtMost(0);
        other.currentSpeed = other.currentSpeed.AtMost(0);
        
        //Add some random
        float random = Random.Range(0, totalWeight);
        collisionWeight = (collisionWeight + random)/2;
        otherCollisionWeight = (otherCollisionWeight + totalWeight - random)/2;
        
        //Gain a fraction of total speed (backwards) depending on other unit's weight, compared to total weight
        //I increased that number a bit because it looks nicer
        currentSpeed -= (otherCollisionWeight / totalWeight) * totalSpeed * G.m.collisionSpeedMultiplier;
        other.currentSpeed -= (collisionWeight / totalWeight) * totalSpeed * G.m.collisionSpeedMultiplier;
        
        //Clamp speeds, so that we don't have either imperceptible or extreme movements
        currentSpeed = currentSpeed.AtMost(G.m.postCollisionMinSpeed).AtLeast(G.m.postCollisionMaxSpeed);
        other.currentSpeed = other.currentSpeed.AtMost(G.m.postCollisionMinSpeed).AtLeast(G.m.postCollisionMaxSpeed);
    }
    
    
    // ====================
    // ATTACK & DAMAGE
    // ====================

    public bool CanAttack(Unit other) {
        return other.currentSpeed < G.m.speedToBump;
    }

    public void Attack(Unit other) {
        other.status = Status.BUMPED;
        other.TakeDamage(damage);

        if (shakeOnHit) B.m.Shake(0.1f);

        // B.m.audioSource.PlayOneShot(G.m.damageSounds.Random());
        B.m.SpawnFX(G.m.sparkFxPrefab,
            transform.position + visualsPosition + new Vector3(1.5f * (int) side, 1, -2),
            side == Side.HERO,
            null,
            0.5f,
            Vector3.forward * Random.Range(0, 360));
    }

    public void TakeDamage(float amount) {
        amount = Random.Range((int)(0.8f * amount), (int)(1.2f * amount));
        SetHealth(currentHealth-amount);

        HpLossUi hpLossUi = B.m.SpawnFX(G.m.hpLossUIPrefab,
                transform.position + hpLossPosition,
                false,
                transform)
            .GetComponent<HpLossUi>();
        
        hpLossUi.number.text = amount.ToString();
    }

    public void SetHealth(float amount) {
        currentHealth = Mathf.Clamp(amount, 0, maxHealth);
        healthBar.value = currentHealth / maxHealth;
        // healthBar.gameObject.SetActive(!currentHealth.isApprox(maxHealth));
        // orangeHealthBar.gameObject.SetActive(!currentHealth.isApprox(maxHealth));
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
        allies.Remove(this);

        if (allies.Count == 0) {
            if (side == Side.HERO) B.m.Defeat();
            else if (side == Side.MONSTER) B.m.Victory();
        }
    }

    public void Destroy() {
        // B.m.audioSource.PlayOneShot(G.m.deathSounds.Random());
        Instantiate(G.m.deathCloudFxPrefab, transform.position + 0.5f*Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
}
