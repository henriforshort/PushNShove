using UnityEngine;

public class UnitSide : MonoBehaviour {
    private Unit _unit;
    public Unit unit => _unit ?? (_unit = GetComponent<Unit>());
    
    
    public virtual void Die() { }
    public virtual void GetDefeated() { }
}