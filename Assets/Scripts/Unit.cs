using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
    [Header("Balancing")]
    public PI playerIndex;
    public float maxSpeed;
    public float mass;
    public float maxHealth;
    public float damage;
    public float attackAnimDuration;
    public float size;
    public bool attackAnimOnAllCollisions;
    public Vector3 hpLossPosition;
    public bool shakeOnHit;
    
    [Header("State")]
    public float currentSpeed;
    public Status status;
    public Anim anim;
    public float currentHealth;
    public float lastAttack;
    public List<Unit> unitToCollide;
    
    [Header("References")]
    public Slider healthBar;
    public Image healthBarFill;
    public Slider orangeHealthBar;
    public Animator animator;
    public Rigidbody rigidbodee;

    private static List<Unit> _player1Units;
    public static List<Unit> player1Units => _player1Units ?? (_player1Units = new List<Unit>());

    private static List<Unit> _player2Units;
    public static List<Unit> player2Units => _player2Units ?? (_player2Units = new List<Unit>());

    public List<Unit> allies => playerIndex == PI.PLAYER_ONE ? player1Units : player2Units;
    public List<Unit> enemies => playerIndex == PI.PLAYER_ONE ? player2Units : player1Units;
    
    public enum Status { WALK, BUMPED, FALLING } //Status
    public enum Anim { WALK, HIT, BUMPED } //Animation

    public void Start() {
        status = Status.WALK;
        SetAnim(Anim.WALK);
        
        currentSpeed = maxSpeed;
        
        if (playerIndex == PI.PLAYER_ONE) SetHealth(G.m.heroHp);
        else SetHealth(maxHealth);
        orangeHealthBar.value = maxHealth;
        
        if (playerIndex == PI.PLAYER_ONE) player1Units.Add(this);
        if (playerIndex == PI.PLAYER_TWO) player2Units.Add(this);
    }

    public void InitRun() { //Called at the end of the first frame of a run
        SetHealth(G.m.heroHp);
    }

    public void Update() {
        UpdateSpeed();
        UpdateVisuals();
        
        if (unitToCollide.Count == 0) {
            Move();
        } else {
            unitToCollide.ForEach(Collide);
            unitToCollide.Clear();
        }
    }

    public void SetAnim(Anim a) {
        anim = a;
        animator.SetInteger("anim", (int)a);
        if (shakeOnHit && a == Anim.HIT) B.m.Shake(0.1f);
    }

    public void UpdateSpeed() {
        if (currentSpeed >= maxSpeed) return;
        if (B.m.gameState != B.State.PLAYING) return;
        if (status == Status.FALLING) return;
        
        currentSpeed += Time.deltaTime * maxSpeed * G.m.bumpRecoverySpeed;
        if (currentSpeed >= maxSpeed) currentSpeed = maxSpeed;
    }

    public void UpdateVisuals() {
        if (!orangeHealthBar.value.isApprox(currentHealth))
            orangeHealthBar.value = orangeHealthBar.value.LerpTo(healthBar.value, 3);
        
        if (currentSpeed > 0 && anim == Anim.BUMPED) {
            status = Status.WALK;
            SetAnim(Anim.WALK);
            B.m.SpawnFX(G.m.bumpDustFxPrefab,
                new Vector3(this.GetX() - (int)playerIndex*size, -2, -2),
                playerIndex == PI.PLAYER_TWO);
        }
        
        if (anim == Anim.HIT && Time.time - lastAttack > attackAnimDuration) SetAnim(Anim.WALK);
    }

    public void Move() {
        if (B.m.gameState != B.State.PLAYING && status != Status.FALLING) return;
        
        transform.position += currentSpeed * Time.deltaTime * (int)playerIndex * Vector3.right;
    }

    public void OnTriggerStay(Collider other) {
        Unit otherUnit = player2Units.FirstOrDefault(u => u.gameObject == other.gameObject);
        if (playerIndex == PI.PLAYER_ONE && otherUnit != null) unitToCollide.Add(otherUnit);
        
        if (status != Status.FALLING && B.m.deathZones.Contains(other.gameObject)) DeathByFall();
    }

    public void Collide(Unit other) { //Only called by player character
        if (currentSpeed < 0 && other.currentSpeed < 0) return;
        
        if (G.m.enableCheats && Input.GetKey(KeyCode.W)) other.DeathByHp();
        if (G.m.enableCheats && Input.GetKey(KeyCode.L)) DeathByHp();
        
        List<float> newSpeeds =  SpeedAfterBump(currentSpeed, other.currentSpeed, mass, 
            other.mass);
        currentSpeed = newSpeeds[0];
        other.currentSpeed = newSpeeds[1];

        if (other.attackAnimOnAllCollisions) {
            other.lastAttack = Time.time;
            other.SetAnim(Anim.HIT);
        }
        
        if (CanAttack(other)) Attack(other);
        else if (other.CanAttack(this)) other.Attack(this);
    }

    public List<float> SpeedAfterBump(float speed1, float speed2, float mass1, float mass2) {
        // Basic concept : the total amount of speed is conserved (the sum of the two unit's speed stays the same)
        // This total speed is distributed proportionally to each unit's momentum (speed * mass)
        // I added a few tweaks for better gamefeel
        float totalSpeed = speed1 + speed2;
        
        // Speed cannot be lower than 0.5 so that immobile units offer some resistance (and no one has < 0 momentum)
        float momentum1 = mass1 * Mathf.Max(speed1, G.m.minMomentum); 
        float momentum2 = mass2 * Mathf.Max(speed2, G.m.minMomentum);
        float totalMomentum = momentum1 + momentum2;
        
        // If unit was moving forward, stop it completely before adding the speed from the collision
        float initialSpeed1 = Mathf.Min(0, speed1);
        float initialSpeed2 = Mathf.Min(0, speed2);
        
        //Add some momentum to the other unit to make the collision more powerful
        //Gain a fraction of total speed (backwards) equivalent to the fraction of total momentum the other unit has
        float newSpeed1 = initialSpeed1 - ((momentum2 + G.m.collisionForceIncrease) / totalMomentum) * totalSpeed;
        float newSpeed2 = initialSpeed2 - ((momentum1 + G.m.collisionForceIncrease) / totalMomentum) * totalSpeed;
        
        return new List<float> {newSpeed1, newSpeed2};
    }

    public bool CanAttack(Unit other) {
        return status == Status.WALK 
               && currentSpeed > other.currentSpeed 
               && other.currentSpeed < G.m.speedToBump;
    }

    public void Attack(Unit other) {
        lastAttack = Time.time;
        SetAnim(Anim.HIT);
        
        other.status = Status.BUMPED;
        other.SetAnim(Anim.BUMPED);
        other.TakeDamage(damage);
        
        B.m.audioSource.PlayOneShot(G.m.damageSounds.Random());
        B.m.SpawnFX(G.m.sparkFxPrefab, 
            transform.position + new Vector3(0.75f * (int)playerIndex, 
                Random.Range(-0.5f, 0.5f), -2),
            playerIndex == PI.PLAYER_ONE);
    }

    public void TakeDamage(float amount) {
        amount = Random.Range((int)(0.8f * amount), (int)(1.2f * amount));
        SetHealth(currentHealth-amount);
        // healthBarFill.color = G.m.orange;
        // this.Wait(0.1f, () => healthBarFill.color = G.m.red);

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
            if (playerIndex == PI.PLAYER_ONE) B.m.Defeat();
            else if (playerIndex == PI.PLAYER_TWO) B.m.Victory();
        }
    }

    public void Destroy() {
        B.m.audioSource.PlayOneShot(G.m.deathSounds.Random());
        Instantiate(G.m.deathCloudFxPrefab, transform.position + 0.5f*Vector3.up, Quaternion.identity);
        Destroy(gameObject);
    }
}
