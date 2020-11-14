﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    [Header("Balancing")]
    public PI playerIndex;
    public float maxSpeed;
    public float mass;
    public float maxHealth;
    public float damage;
    public float attackSpeed; //Delay between attacks
    
    [Header("State")]
    public float currentSpeed;
    public S status;
    public float currentHealth;
    public float lastAttack;
    
    [Header("References")]
    public Slider healthBar;
    public Animator animator;
    public Rigidbody rigidbody;

    private static List<Unit> _player1Units;
    public static List<Unit> player1Units => _player1Units ?? (_player1Units = new List<Unit>());

    private static List<Unit> _player2Units;
    public static List<Unit> player2Units => _player2Units ?? (_player2Units = new List<Unit>());

    public List<Unit> allies => playerIndex == PI.PLAYER_ONE ? player1Units : player2Units;
    public List<Unit> enemies => playerIndex == PI.PLAYER_ONE ? player2Units : player1Units;
    
    public enum S { MOVING, BUMPED, FALLING }

    public void Start() {
        currentSpeed = maxSpeed;
        AddHealth(maxHealth);
        if (playerIndex == PI.PLAYER_ONE) player1Units.Add(this);
        if (playerIndex == PI.PLAYER_TWO) player2Units.Add(this);
    }

    public void Update() {
        AdjustSpeed();
        Move();
    }

    public void AdjustSpeed() {
        if (currentSpeed >= maxSpeed) return;
        if (G.m.gameState != G.S.PLAYING) return;
        if (status == S.FALLING) return;
        
        currentSpeed += Time.deltaTime * maxSpeed * G.m.bumpRecoverySpeed;
        if (currentSpeed > 0) {
            status = S.MOVING;
            animator.SetTrigger("walk");
        }
        
        if (currentSpeed >= maxSpeed) {
            currentSpeed = maxSpeed;
        };
    }

    public void Move() {
        if (G.m.gameState != G.S.PLAYING && status != S.FALLING) return;
        
        transform.position += currentSpeed * Time.deltaTime * (int)playerIndex * Vector3.right;
    }

    public void OnTriggerStay(Collider other) {
        Unit otherUnit = other.GetComponent<Unit>();
        if (status != S.BUMPED && enemies.Select(e => e.gameObject).Contains(other.gameObject)) {
            Collide(otherUnit);
        }
        
        if (status != S.FALLING && other.GetComponent<DeadZone>()) DeathByFall();
    }

    public void Collide(Unit other) {
        if (CanAttack()) Attack(other);
        if (other.CanAttack()) other.Attack(this);
        
        List<float> newSpeeds =  SpeedAfterBump(currentSpeed, other.currentSpeed, mass, 
            other.mass);
        currentSpeed = newSpeeds[0];
        other.currentSpeed = newSpeeds[1];
        
        G.m.audioSource.PlayOneShot(G.m.damageSounds.Random());
        
        Bump();
        other.Bump();
    }

    public List<float> SpeedAfterBump(float speed1, float speed2, float mass1, float mass2) {
        // Basic concept : the total amount of speed is conserved (the sum of the two unit's speed stays the same)
        // This total speed is distributed proportionally to each unit's momentum (speed * mass)
        // A few deatils have been added for better gamefeel

        float totalSpeed = speed1 + speed2;
        
        // Speed cannot be lower than 0.5 so that immobile units offer some resistance (and no one has < 0 momentum)
        float momentum1 = mass1 * Mathf.Max(speed1, G.m.minMomentum); 
        float momentum2 = mass2 * Mathf.Max(speed2, G.m.minMomentum);
        float totalMomentum = momentum1 + momentum2;
        
        // If unit was moving forward, stop it completely before adding the speed from the collision
        float initialSpeed1 = Mathf.Min(0, speed1);
        float initialSpeed2 = Mathf.Min(0, speed2);
        
        //Add some momentum to the other unit to make the &éollision more powerful
        //Gain a fraction of total speed (backwards) equivalent to the fraction of total momentum the other unit has
        float newSpeed1 = initialSpeed1 - ((momentum2 + G.m.collisionForceIncrease) / totalMomentum) * totalSpeed;
        float newSpeed2 = initialSpeed2 - ((momentum1 + G.m.collisionForceIncrease) / totalMomentum) * totalSpeed;
        
        return new List<float> {newSpeed1, newSpeed2};
    }

    public void Bump() {
        status = S.BUMPED;
        
        if (currentSpeed < G.m.speedToBump) animator.SetTrigger("bump");
        else animator.SetTrigger("hit");
    }

    public bool CanAttack() {
        return status == S.MOVING && Time.time - lastAttack > attackSpeed;
    }

    public void Attack(Unit other) {
        lastAttack = Time.time;
        other.AddHealth(-damage);
    }

    public void AddHealth(float amount) {
        currentHealth = Mathf.Clamp(currentHealth+amount, 0, maxHealth);
        healthBar.value = currentHealth / maxHealth;
        if (currentHealth <= 0) DeathByHp();
    }

    public void DeathByHp() {
        Deactivate();
        Destroy();
    }

    public void DeathByFall() {
        Deactivate();
        status = S.FALLING;
        rigidbody.useGravity = true;
        this.Wait(0.5f, () => Destroy());
    }

    public void Deactivate() {
        allies.Remove(this);

        if (allies.Count == 0) {
            if (playerIndex == PI.PLAYER_ONE) G.m.Defeat();
            else if (playerIndex == PI.PLAYER_TWO) G.m.Victory();
        }
    }

    public void Destroy() {
        G.m.audioSource.PlayOneShot(G.m.deathSounds.Random());
        Instantiate(G.m.deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
