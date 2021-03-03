using UnityEngine;

public class Arrow : MonoBehaviour {
    [Header("State")]
    public float critChance;
    public float damage;
    public float strength;

    public void Init(Unit unit) {
        critChance = unit.data.critChance;
        damage = unit.data.damage;
        strength = unit.data.strength;
    }
    
    //if equal or further left than an enemy, bump them
    public void Update() {
        Unit leftMostEnemy = Unit.monsterUnits.WithLowest(m => m.GetX());
        if (leftMostEnemy == null) return;
        if (leftMostEnemy.status != Unit.Status.ALIVE) return;
        if (this.GetX() + .5f < leftMostEnemy.GetX()) return;
        
        Game.m.PlaySound(MedievalCombat.STAB_7);
        leftMostEnemy.GetBumpedBy(critChance, damage, strength);
        Destroy(gameObject);
    }
}
