using System.Collections.Generic;
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

    private static List<Unit> _player1Units;
    public static List<Unit> player1Units => _player1Units ?? (_player1Units = new List<Unit>());

    private static List<Unit> _player2Units;
    public static List<Unit> player2Units => _player2Units ?? (_player2Units = new List<Unit>());

    public List<Unit> allies => playerIndex == PI.PLAYER_ONE ? player1Units : player2Units;
    public List<Unit> enemies => playerIndex == PI.PLAYER_ONE ? player2Units : player1Units;
    
    public enum S { MOVING, BUMPED }

    public void Start() {
        currentSpeed = maxSpeed;
        AddHealth(maxHealth);
        if (playerIndex == PI.PLAYER_ONE) player1Units.Add(this);
        if (playerIndex == PI.PLAYER_TWO) player2Units.Add(this);
    }

    public void Update() {
        if (G.m.gameState == G.S.PLAYING) {
            AdjustSpeed();
            Move();
        }
    }

    public void AdjustSpeed() {
        if (currentSpeed >= maxSpeed) return;
        
        currentSpeed += Time.deltaTime * maxSpeed * G.m.bumpRecoverySpeed;
        if (currentSpeed > 0) {
            status = S.MOVING;
        }
        
        if (currentSpeed >= maxSpeed) {
            currentSpeed = maxSpeed;
        };
    }

    public void Move() {
        transform.position += currentSpeed * Time.deltaTime * (int)playerIndex * Vector3.right;
    }

    public void OnTriggerStay(Collider other) {
        Unit otherUnit = other.GetComponent<Unit>();
        if (status != S.BUMPED && enemies.Select(e => e.gameObject).Contains(other.gameObject)) {
            Collide(otherUnit);
        }
        
        if (other.GetComponent<DeadZone>()) Death();
    }

    public void Collide(Unit other) {
        if (CanAttack()) Attack(other);
        if (other.CanAttack()) other.Attack(this);
        
        status = S.BUMPED;
        other.status = S.BUMPED;
        
        //Realistic physics
        // float newSpeed = ((mass - other.mass)*currentSpeed - 2*other.mass*other.currentSpeed) / (mass + other.mass);
        // other.currentSpeed = (-(other.mass - mass)*other.currentSpeed + 2*mass*currentSpeed) / (mass + other.mass);
        // currentSpeed = newSpeed;

        float momentum = mass * Mathf.Max(currentSpeed, 0);
        float otherMomentum = other.mass * Mathf.Max(other.currentSpeed, 0);
        float totalSpeed = (currentSpeed + other.currentSpeed);
        float newSpeed = Mathf.Min(0, currentSpeed) - otherMomentum / (momentum + otherMomentum) * totalSpeed - 0.5f;
        other.currentSpeed = Mathf.Min(0, other.currentSpeed) - momentum / (momentum + otherMomentum) * totalSpeed - 0.5f;
        currentSpeed = newSpeed;
        
        G.m.audioSource.PlayOneShot(G.m.damageSounds.Random());
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
        if (currentHealth <= 0) Death();
    }

    public void Death() {
        allies.Remove(this);

        if (allies.Count == 0) {
            if (playerIndex == PI.PLAYER_ONE) G.m.Defeat();
            else if (playerIndex == PI.PLAYER_TWO) G.m.Victory();
        }

        Destroy(gameObject);
    }
}
