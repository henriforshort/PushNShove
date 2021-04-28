using UnityEngine;

public class Projectile : MonoBehaviour {
    [Header("Balancing")]
    public float speed;
    public MedievalCombat hitSound;
    public float range;
    public bool shakeOnHit;
    
    [Header("State")]
    public float critChance;
    public float damage;
    public float strength;

    public void Init(Unit unit) { //data is set when it is shot, not when it hits
        critChance = unit.data.critChance;
        damage = unit.data.damage;
        strength = unit.data.strength;
    }

    public void Init(float critChance, float damage, float strength) { //data is set when it is shot, not when it hits
        this.critChance = critChance;
        this.damage = damage;
        this.strength = strength;
    }
    
    //if equal or further left than an enemy, bump them
    public void Update() {
        Move();
        Hit();
    }

    public void Move() {
        if (Battle.m.gameState == Battle.State.PAUSE) return;
        
        transform.Translate(speed * Time.deltaTime, 0, 0);
        range -= speed * Time.deltaTime;
        if (range < 0) Destroy(gameObject);
        if (this.GetX() > range) Destroy(gameObject);
    }

    public void Hit() {
        Unit leftMostEnemy = Unit.monsterUnits.WithLowest(m => m.GetX());
        if (leftMostEnemy == null) return;
        if (leftMostEnemy.status != Unit.Status.ALIVE) return;
        if (this.GetX() + .5f < leftMostEnemy.GetX()) return;
        
        if (shakeOnHit) Battle.m.cameraManager.Shake(.2f);
        Game.m.PlaySound(hitSound);
        leftMostEnemy.GetBumpedBy(critChance, damage, strength);
        Destroy(gameObject);
    }
}
