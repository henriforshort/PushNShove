using UnityEngine;

public class Arrow : MonoBehaviour {
    public Unit owner;
    
    //if equal or farther left than an enemy, bump them
    public void Update() {
        Unit leftMostEnemy = Unit.monsterUnits.WithLowest(m => m.GetX());
        if (owner == null) return;
        if (leftMostEnemy == null) return;
        if (leftMostEnemy.status != Unit.Status.ALIVE) return;
        if (this.GetX() + .5f < leftMostEnemy.GetX()) return;
        
        Game.m.PlaySound(MedievalCombat.STAB_7);
        leftMostEnemy.GetBumpedBy(owner);
        Destroy(gameObject);
    }
}
