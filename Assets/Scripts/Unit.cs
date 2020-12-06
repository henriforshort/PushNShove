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
    public bool attackAnimOnAllCollisions;
    public Vector3 hpLossPosition;
    public bool shakeOnHit;
    public bool delayHit;
    
    [Header("State")]
    public float currentSpeed;
    public Status status;
    public Anim anim;
    public float currentHealth;
    public float lastAttack;
    public List<Unit> unitToCollide;
    public List<Unit> unitsColliding;
    
    [Header("References")]
    public Slider healthBar;
    public Slider orangeHealthBar;
    public Animator animator;
    public Rigidbody rigidbodee;

    private static List<Unit> _heroUnits;
    public static List<Unit> heroUnits => _heroUnits ?? (_heroUnits = new List<Unit>());

    private static List<Unit> _enemyUnits;
    public static List<Unit> enemyUnits => _enemyUnits ?? (_enemyUnits = new List<Unit>());

    public List<Unit> allies => side == Side.HERO ? heroUnits : enemyUnits;
    
    public enum Status { WALK, BUMPED, FALLING } //Status
    public enum Anim { WALK, HIT, DEFEND, BUMPED } //Animation

    public void Start() {
        status = Status.WALK;
        SetAnim(Anim.WALK);
        
        currentSpeed = maxSpeed;
        
        if (side != Side.HERO) SetHealth(maxHealth);
        orangeHealthBar.value = maxHealth;
        
        if (side == Side.HERO) heroUnits.Add(this);
        if (side == Side.ENEMY) enemyUnits.Add(this);
    }

    public void Update() {
        UpdateSpeed();
        UpdateVisuals();
        
        if (unitToCollide.Count == 0) {
            Move();
        } else {
            Collide(unitToCollide[0]);
            unitToCollide.RemoveAt(0);
        }
    }

    public void SetAnim(Anim a) {
        if (anim == a) return;
        
        anim = a;
        
        if (a == Anim.HIT && this.CoinFlip()) animator.Play("HIT2");
        else animator.Play(a.ToString());
    }

    public void UpdateSpeed() {
        if (currentSpeed >= maxSpeed) return;
        if (B.m.gameState != B.State.PLAYING) return;
        if (status == Status.FALLING) return;
        
        currentSpeed += Time.deltaTime * maxSpeed * G.m.bumpRecoverySpeed;
        if (currentSpeed >= maxSpeed) currentSpeed = maxSpeed;
    }

    public void UpdateVisuals() {
        animator.enabled = (B.m.gameState != B.State.PAUSE);
        
        if (!orangeHealthBar.value.isApprox(currentHealth))
            orangeHealthBar.value = orangeHealthBar.value.LerpTo(healthBar.value, 2.5f);
        
        if (currentSpeed > 0 && (anim == Anim.BUMPED || anim == Anim.DEFEND)) {
            status = Status.WALK;
            SetAnim(Anim.WALK);
            B.m.SpawnFX(G.m.bumpDustFxPrefab,
                new Vector3(this.GetX() - (int)side*size, -2, -2),
                side == Side.ENEMY);
        }
        
        if (anim == Anim.HIT && Time.time - lastAttack > attackAnimDuration) SetAnim(Anim.WALK);
    }

    public void Move() {
        if (B.m.gameState != B.State.PLAYING && status != Status.FALLING) return;
        
        transform.position += currentSpeed * Time.deltaTime * (int)side * Vector3.right;
    }

    public void OnTriggerStay(Collider other) {
        Unit otherUnit = enemyUnits.FirstOrDefault(u => u.gameObject == other.gameObject);
        if (side == Side.HERO && otherUnit != null) unitToCollide.Add(otherUnit);
        
        if (status != Status.FALLING && B.m.deathZones.Contains(other.gameObject)) DeathByFall();
    }

    public void Collide(Unit other) { //Only called by player character
        if (currentSpeed < 0 && other.currentSpeed < 0) return;
        if (unitsColliding.Contains(other)) return;
        
        if (G.m.enableCheats && Input.GetKey(KeyCode.W)) other.DeathByHp();
        if (G.m.enableCheats && Input.GetKey(KeyCode.L)) DeathByHp();
        
        SetAnim(Anim.DEFEND);
        other.SetAnim(Anim.DEFEND);

        unitsColliding.Add(other);
        List<float> newSpeeds =  SpeedAfterBump(currentSpeed, other.currentSpeed, weight, 
            other.weight);
        this.Wait(delayHit ? 0.05f : 0, () => {
            unitsColliding.Remove(other);
            currentSpeed = newSpeeds[0];
            other.currentSpeed = newSpeeds[1];
        });
        
        if (other.attackAnimOnAllCollisions) {
            other.lastAttack = Time.time;
            other.SetAnim(Anim.HIT); 
        }
        
        if (CanAttack(newSpeeds[0], newSpeeds[1])) Attack(other);
        else if (other.CanAttack(newSpeeds[1], newSpeeds[0])) other.Attack(this);
    }

    public List<float> SpeedAfterBump(float speed1, float speed2, float weight1, float weight2) {
        // Basic concept : the total amount of speed is conserved (the sum of the two unit's speed stays the same)
        // This total speed is distributed proportionally to each unit's momentum (speed * weight)
        // I added a few tweaks for better gamefeel
        float totalSpeed = speed1.AtLeast(0) + speed2.AtLeast(0);
        
        // Speed cannot be lower than 0 so that immobile units offer some resistance (and no one has < 0 momentum)
        float momentum1 = weight1 * speed1.AtLeast(0); 
        float momentum2 = weight2 * speed2.AtLeast(0);
        float totalMomentum = momentum1 + momentum2;
        
        // If unit was moving forward, stop it completely before adding the speed from the collision
        float initialSpeed1 = speed1.AtMost(0);
        float initialSpeed2 = speed2.AtMost(0);
        
        //Gain a fraction of total speed (backwards) equivalent to the fraction of total momentum the other unit has
        float newSpeed1 = (initialSpeed1 - (momentum2/totalMomentum)*totalSpeed).AtMost(G.m.postCollisionMinSpeed);
        float newSpeed2 = (initialSpeed2 - (momentum1/totalMomentum)*totalSpeed).AtMost(G.m.postCollisionMinSpeed);
        
        return new List<float> {newSpeed1, newSpeed2};
    }

    public bool CanAttack(float newSweed, float otherNewSpeed) {
        return status == Status.WALK 
               && newSweed > otherNewSpeed 
               && otherNewSpeed < G.m.speedToBump;
    }

    public void Attack(Unit other) {
        lastAttack = Time.time;
        SetAnim(Anim.HIT);

        this.Wait(delayHit ? 0.05f : 0, () => {
            if (other == null) return;
            
            other.status = Status.BUMPED;
            other.SetAnim(Anim.BUMPED);
            other.TakeDamage(damage);

            if (shakeOnHit) B.m.Shake(0.1f);

            // B.m.audioSource.PlayOneShot(G.m.damageSounds.Random());
            B.m.SpawnFX(G.m.sparkFxPrefab,
                transform.position + new Vector3(0.75f * (int) side,
                    Random.Range(-0.5f, 0.5f), -2),
                side == Side.HERO);
        });
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
        healthBar.gameObject.SetActive(!currentHealth.isApprox(maxHealth));
        orangeHealthBar.gameObject.SetActive(!currentHealth.isApprox(maxHealth));
        if (currentHealth <= 0) DeathByHp();
    }

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
            else if (side == Side.ENEMY) B.m.Victory();
        }
    }

    public void Destroy() {
        // B.m.audioSource.PlayOneShot(G.m.deathSounds.Random());
        Instantiate(G.m.deathCloudFxPrefab, transform.position + 0.5f*Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
}
