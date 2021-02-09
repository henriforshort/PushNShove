using UnityEngine;

public class UnitSide : MonoBehaviour {
    [HideInInspector] public Unit unit;
    
    
    public virtual void Die() { }
    public virtual void GetDefeated() { }
}