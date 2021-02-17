using UnityEngine;

public class Arrow : MonoBehaviour {
    public Unit owner;
    
    public void Update() {
        //if equal or farther left than an enemy, bump them
        Unit leftMostEnemy = Unit.monsterUnits.WithLowest(m => m.GetX());
        if (owner != null && leftMostEnemy != null && this.GetX() + .5f >= leftMostEnemy.GetX()) {
            Game.m.PlaySound(MedievalCombat.STAB_7);
            leftMostEnemy.GetBumpedBy(owner);
            Destroy(gameObject);
        }
    }
}
